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

    [MenuItem("Tools/Encounter Designer")]
    public static void ShowWindow()
    {
        EncounterDesignerWindow window = GetWindow<EncounterDesignerWindow>();
        window.titleContent = new GUIContent("Encounter Designer");
        window.minSize = new Vector2(200, 200);
    }

    private void FindAllEncounters(out Encounter[] encounters)
    {
        string[] guids = AssetDatabase.FindAssets("t:Encounter");

        encounters = new Encounter[guids.Length];

        for(int i = 0; i < guids.Length; i++)
        {
            encounters[i] = AssetDatabase.LoadAssetAtPath<Encounter>(AssetDatabase.GUIDToAssetPath(guids[i]));
        }
    }

    private void BuildEncounterListView()
    {
        FindAllEncounters(out Encounter[] encounters);

        Button addEncounterButton = rootVisualElement.Query<Button>("add-encounter-button").First();
        //here's where we can launch an add encounter window
        addEncounterButton.clicked += () => Debug.Log("Add encounter button pressed");

        ListView encounterList = rootVisualElement.Query<ListView>("encounter-list").First();
        encounterList.makeItem = () => new Label();
        encounterList.bindItem = (element, i) => (element as Label).text = encounters[i].name;

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

        encounterList.Refresh();
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
        if (DragAndDrop.objectReferences.Length == 0) return;

        EntityData entityData = DragAndDrop.objectReferences.First() as EntityData;
        
        if (entityData != null)
        {
            DragAndDrop.visualMode = DragAndDropVisualMode.Generic;
        }
    }

    private void HandleDragPerform(DragPerformEvent dragPerformEvent)
    {
        EntityData entityData = DragAndDrop.objectReferences.First() as EntityData;

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

        Button deleteButton = new Button();
        deleteButton.name = "delete-button";
        deleteButton.text = "Delete";
        columnContainer.Add(deleteButton);

        return entityBox;
    }

    private void BindEntityListing(VisualElement element, EncounterEntity entity)
    {
        element.Query<Label>("entity-label").First().text = entity.entityData.name;

        SerializedObject serializedEntity = new SerializedObject(entity);

        PropertyField positionField = new PropertyField(serializedEntity.FindProperty("position"));
        element.Query<VisualElement>("column-container").First().Add(positionField);
        positionField.Bind(serializedEntity);
        positionField.style.flexGrow = 1;

        Button deleteButton = element.Query<Button>("delete-button").First();
        deleteButton.clicked += () => selectedEncounter.RemoveEntity(entity);

        element.RegisterCallback<MouseDownEvent>((MouseDownEvent mouseDownEvent) => StartDragEntity(mouseDownEvent, entity));
    }

    private void StartDragEntity(MouseDownEvent mouseDownEvent, EncounterEntity encounterEntity)
    {
        DragAndDrop.visualMode = DragAndDropVisualMode.Generic;
        DragAndDrop.PrepareStartDrag();
        DragAndDrop.SetGenericData("EncounterEntity", encounterEntity);
        DragAndDrop.StartDrag("Encounter Entity");
    }

    private void OnEnable()
    {
        VisualTreeAsset original = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/EncounterDesignerWindow.uxml");
        TemplateContainer treeAsset = original.CloneTree();
        rootVisualElement.Add(treeAsset);

        BuildEncounterListView();
    }
}
