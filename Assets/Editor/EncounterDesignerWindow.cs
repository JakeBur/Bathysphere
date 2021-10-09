using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using System.Linq;
using UnityEditor.UIElements;
using Battle;

public class EncounterDesignerWindow : EditorWindow
{
    private static Encounter selectedEncounter;
    private static ListView entityList;
    private static ListView encounterList;

    private static Encounter[] encounters;

    [MenuItem("Tools/Encounter Designer")]
    public static void ShowWindow()
    {
        EncounterDesignerWindow window = GetWindow<EncounterDesignerWindow>();
        window.titleContent = new GUIContent("Encounter Designer");
        window.minSize = new Vector2(200, 200);
    }

    private void OnEnable()
    {
        VisualTreeAsset original = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/EncounterDesignerWindow.uxml");
        TemplateContainer treeAsset = original.CloneTree();
        rootVisualElement.Add(treeAsset);

        BuildEncounterListView();
        BuildEntityDataListView();
    }

    public static void FindAllEncounters(out Encounter[] encounters)
    {
        string[] guids = AssetDatabase.FindAssets("t:Encounter");

        encounters = new Encounter[guids.Length];

        for(int i = 0; i < guids.Length; i++)
        {
            encounters[i] = AssetDatabase.LoadAssetAtPath<Encounter>(AssetDatabase.GUIDToAssetPath(guids[i]));
        }
    }

    public static void FindAllEntityData(out EntityData[] entityData)
    {
        string[] guids = AssetDatabase.FindAssets("t:EntityData");

        entityData = new EntityData[guids.Length];

        for (int i = 0; i < guids.Length; i++)
        {
            entityData[i] = AssetDatabase.LoadAssetAtPath<EntityData>(AssetDatabase.GUIDToAssetPath(guids[i]));
        }
    }

    public static void RefreshEncounterList()
    {
        FindAllEncounters(out encounters);
        encounterList.itemsSource = encounters;
        encounterList.Refresh();
    }

    private void BuildEncounterListView()
    {
        FindAllEncounters(out encounters);

        Button addEncounterButton = rootVisualElement.Query<Button>("add-encounter-button").First();
        //here's where we can launch an add encounter window
        addEncounterButton.clicked += () => EncounterCreatorWizard.ShowWindow();

        encounterList = rootVisualElement.Query<ListView>("encounter-list").First();
        encounterList.makeItem = () => new Label();
        encounterList.bindItem = (element, i) => 
        {
            (element as Label).text = encounters[i].name; 
        };

        encounterList.itemsSource = encounters;
        encounterList.itemHeight = 16;
        encounterList.selectionType = SelectionType.Single;

        encounterList.onSelectionChange += (enumerable) =>
        {
            foreach(Object it in enumerable)
            {
                BuildEncounterInfoBox(it as Encounter);
            }
        };

        // allow the user to delete encounters
        encounterList.RegisterCallback<KeyDownEvent>(
            (keyDownEvent) =>
            {
                if (keyDownEvent.keyCode == KeyCode.Delete)
                {
                    bool deleted = EditorUtility.DisplayDialog(
                        "Delete Encounter",
                        $"Are you sure you want to delete the encounter {(encounterList.selectedItem as Encounter).name}?",
                            "Delete",
                            "Cancel");
                    
                    if(deleted)
                    {
                        (encounterList.selectedItem as Encounter).Delete();
                        RefreshEncounterList();
                    }
                }
            });

        encounterList.Refresh();
    }

    private void BuildEntityDataListView()
    {
        FindAllEntityData(out EntityData[] entityData);

        ListView entityDataList = rootVisualElement.Query<ListView>("entityData-list").First();
        entityDataList.makeItem = () => new Label();
        entityDataList.bindItem = (element, i) =>
        {
            element.RegisterCallback<MouseDownEvent>((MouseDownEvent mouseDownEvent) => StartDragEntityData(mouseDownEvent, entityData[i]));
            (element as Label).text = entityData[i].name;
        };

        entityDataList.itemsSource = entityData;
        entityDataList.itemHeight = 16;
        entityDataList.selectionType = SelectionType.Single;

        entityDataList.Refresh();
    }

    private void BuildEncounterInfoBox(Encounter encounter)
    {
        VisualElement encounterInfoBox = rootVisualElement.Query<VisualElement>("encounter-info").First();

        selectedEncounter = encounter;

        // Grid size setting
        SerializedObject serializedEncounter = new SerializedObject(encounter);
        VisualElement gridSizeBox = rootVisualElement.Query<VisualElement>("grid-size-field").First();
        PropertyField gridSizeField = new PropertyField(serializedEncounter.FindProperty("gridSize"));
        gridSizeField.Bind(serializedEncounter);
        gridSizeBox.Clear();
        gridSizeBox.Add(gridSizeField);

        // Entity list
        entityList = rootVisualElement.Query<ListView>("entity-list").First();
        entityList.makeItem = BuildEntityListing;
        entityList.bindItem = (element, i) =>
        {
            if(i < encounter.entities.Count)
            {
                BindEntityListing(element, encounter.entities[i]);
            }
        };

        entityList.itemsSource = encounter.entities;
        entityList.itemHeight = 32;
        entityList.selectionType = SelectionType.Single;

        entityList.Refresh();

        // refresh the list whenever the encounter's elements change
        encounter.OnContentsUpdated -= entityList.Refresh;// make sure we only subscribe once
        encounter.OnContentsUpdated += entityList.Refresh;

        // Open encounter button
        Button openEncounterButton = rootVisualElement.Query<Button>("encounter-designer-button").First();

        openEncounterButton.clicked -= HandleEncounterButtonClick;
        openEncounterButton.clicked += HandleEncounterButtonClick;

        encounterInfoBox.Add(openEncounterButton);

        // Drag and drop
        encounterInfoBox.RegisterCallback<DragUpdatedEvent>(HandleDragUpdate);
        encounterInfoBox.RegisterCallback<DragPerformEvent>(HandleDragPerform);
    }

    private void HandleEncounterButtonClick()
    {
        GameObject.Find("Encounter Designer").GetComponent<EncounterDesigner>().SetEncounter(selectedEncounter);
    }

    private void HandleDragUpdate(DragUpdatedEvent dragUpdatedEvent)
    {
        EncounterEntity encounterEntity = DragAndDrop.GetGenericData("EncounterEntity") as EncounterEntity;
        if (encounterEntity)
        {
            DragAndDrop.visualMode = DragAndDropVisualMode.Generic;
            return;
        }

        // recieve EntityData from our own drag start
        EntityData entityData = DragAndDrop.GetGenericData("EntityData") as EntityData;
        if (entityData)
        {
            DragAndDrop.visualMode = DragAndDropVisualMode.Generic;
            return;
        }

        // receive EntityData from Unity file system
        if (DragAndDrop.objectReferences.Length == 0) return;
        entityData = DragAndDrop.objectReferences.First() as EntityData;

        if (entityData != null)
        {
            DragAndDrop.visualMode = DragAndDropVisualMode.Generic;
        }
    }

    private void HandleDragPerform(DragPerformEvent dragPerformEvent)
    {
        EncounterEntity encounterEntity = DragAndDrop.GetGenericData("EncounterEntity") as EncounterEntity;
        if (encounterEntity)
        {
            encounterEntity.Position = new Vector2Int(-1, -1);
            return;
        }

        // recieve EntityData from our own drag start
        EntityData entityData = DragAndDrop.GetGenericData("EntityData") as EntityData;
        if (entityData)
        {
            selectedEncounter.AddEntity(entityData);
            return;
        }

        // receive EntityData from Unity file system
        entityData = DragAndDrop.objectReferences.First() as EntityData;

        if (entityData != null)
        {
            selectedEncounter.AddEntity(entityData);
        }
    }

    private VisualElement BuildEntityListing()
    {
        VisualElement entityBox = new VisualElement();
        entityBox.style.flexGrow = 1;
        entityBox.style.flexShrink = 1;

        Label label = new Label("DEFAULTLABEL");
        label.name = "entity-label";
        entityBox.Add(label);

        VisualElement columnContainer = new VisualElement();
        columnContainer.style.flexDirection = FlexDirection.Row;
        columnContainer.name = "column-container";
        entityBox.Add(columnContainer);

        Label position = new Label("POSITIONLABEL");
        position.name = "position";
        columnContainer.Add(position);

        Button deleteButton = new Button();
        deleteButton.name = "delete-button";
        deleteButton.text = "Delete";
        columnContainer.Add(deleteButton);
        deleteButton.style.flexDirection = FlexDirection.RowReverse;

        return entityBox;
    }

    private void BindEntityListing(VisualElement element, EncounterEntity entity)
    {
        element.Query<Label>("entity-label").First().text = entity.EntityData.name;

        Button deleteButton = element.Query<Button>("delete-button").First();
        deleteButton.clicked += () => selectedEncounter.RemoveEntity(entity);

        Label position = element.Query<Label>("position").First();
        position.text = $"X: {entity.Position.x} Y: {entity.Position.y}";
        entity.OnContentsUpdated += () => position.text = $"X: {entity.Position.x} Y: {entity.Position.y}";

        element.RegisterCallback<MouseDownEvent>((MouseDownEvent mouseDownEvent) => StartDragEncounterEntity(mouseDownEvent, entity));
    }

    private void StartDragEncounterEntity(MouseDownEvent mouseDownEvent, EncounterEntity encounterEntity)
    {
        DragAndDrop.visualMode = DragAndDropVisualMode.Generic;
        DragAndDrop.PrepareStartDrag();
        DragAndDrop.SetGenericData("EncounterEntity", encounterEntity);
        DragAndDrop.StartDrag("Encounter Entity");
    }

    private void StartDragEntityData(MouseDownEvent mouseDownEvent, EntityData entityData)
    {
        DragAndDrop.visualMode = DragAndDropVisualMode.Generic;
        DragAndDrop.PrepareStartDrag();
        DragAndDrop.SetGenericData("EntityData", entityData);
        DragAndDrop.StartDrag("EntityData");
    }

    
}
