using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using System.Linq;
using UnityEditor.UIElements;
using Battle;

/// <summary>
/// Editor window that modifies an Encounter.
/// </summary>
public class EncounterDesignerWindow : EditorWindow
{
    /// <summary>
    /// The currently selected Encounter asset.
    /// </summary>
    private static Encounter selectedEncounter;

    /// <summary>
    /// Reference to the ListView UI showing all EntityData assets in the project.
    /// </summary>
    private static ListView entityList;

    /// <summary>
    /// Reference to the ListView UI showing all Encounter assets in the project.
    /// </summary>
    private static ListView encounterList;

    /// <summary>
    /// The list of all Encounter assets in the project.
    /// </summary>
    private static Encounter[] encounters;

    /// <summary>
    /// Instantiates the window.
    /// </summary>
    [MenuItem("Tools/Encounter Designer")]
    public static void ShowWindow()
    {
        EncounterDesignerWindow window = GetWindow<EncounterDesignerWindow>();
        if(window != null) window.Close();
        window = GetWindow<EncounterDesignerWindow>();
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

    /// <summary>
    /// Finds all Encounter assets in the project and stores them in the given array.
    /// </summary>
    /// <param name="encounters">The array to fill with all Encounter assets in the project.</param>
    public static void FindAllEncounters(out Encounter[] encounters)
    {
        string[] guids = AssetDatabase.FindAssets("t:Encounter");

        encounters = new Encounter[guids.Length];

        for(int i = 0; i < guids.Length; i++)
        {
            encounters[i] = AssetDatabase.LoadAssetAtPath<Encounter>(AssetDatabase.GUIDToAssetPath(guids[i]));
        }
    }

    /// <summary>
    /// Finds all EntityData assets in the project and stores them in the given array.
    /// </summary>
    /// <param name="entityData">The array to fill with all EntityData assets in the project.</param>
    public static void FindAllEntityData(out EntityData[] entityData)
    {
        string[] guids = AssetDatabase.FindAssets("t:EntityData");

        entityData = new EntityData[guids.Length];

        for (int i = 0; i < guids.Length; i++)
        {
            entityData[i] = AssetDatabase.LoadAssetAtPath<EntityData>(AssetDatabase.GUIDToAssetPath(guids[i]));
        }
    }

    /// <summary>
    /// Updates the contents of the EncounterList to reflect the current state of the project.
    /// </summary>
    public static void RefreshEncounterList()
    {
        FindAllEncounters(out encounters);
        encounterList.itemsSource = encounters;
        encounterList.Refresh();
    }

    /// <summary>
    /// Builds the UI that allows the user to select Encounters.
    /// </summary>
    private void BuildEncounterListView()
    {
        FindAllEncounters(out encounters);

        Button addEncounterButton = rootVisualElement.Query<Button>("add-encounter-button").First();
        addEncounterButton.clicked += () => EncounterCreatorWizard.ShowWindow();

        encounterList = rootVisualElement.Query<ListView>("encounter-list").First();
        encounterList.makeItem = () =>
        {
            VisualElement container = new VisualElement();
            container.focusable = true;
            container.pickingMode = PickingMode.Position;
            
            Label label = new Label();
            label.name = "encounter-label";
            container.Add(label);

            TextField renameField = new TextField();
            renameField.name = "rename-field";
            renameField.visible = false;
            container.Add(renameField);

            return container;
        };

        encounterList.bindItem = (element, i) => 
        {
            Label label = element.Query<Label>("encounter-label");
            TextField renameField = element.Query<TextField>("rename-field");

            element.AddManipulator(new ContextualMenuManipulator(
                (ContextualMenuPopulateEvent populateEvent) =>
                {
                    populateEvent.menu.AppendAction("Rename",
                        (DropdownMenuAction action) =>
                        {
                            label.style.display = DisplayStyle.None;

                            renameField.visible = true;
                            renameField.value = label.text;
                        });
                }));

            renameField.RegisterCallback<KeyDownEvent>((keyDownEvent) =>
            {
                if (keyDownEvent.keyCode == KeyCode.Return)
                {
                    encounters[i].Rename(renameField.value);
                    
                    label.style.display = DisplayStyle.Flex;
                    label.text = renameField.value;
                    renameField.visible = false;
                }
                else if(keyDownEvent.keyCode == KeyCode.Escape)
                {
                    label.style.display = DisplayStyle.Flex;
                    label.text = renameField.value;
                    renameField.visible = false;
                }
            });

            element.Query<Label>("encounter-label").First().text = encounters[i].name;
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

    /// <summary>
    /// Builds the UI that allows the user to select EntityData and add it to the Encounter.
    /// </summary>
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

    /// <summary>
    /// Builds the UI that allows the user to modify Encounters.
    /// </summary>
    /// <param name="encounter"></param>
    private void BuildEncounterInfoBox(Encounter encounter)
    {
        EncounterDesigner encounterDesigner = EncounterDesigner.FindEncounterDesigner();//

        VisualElement encounterInfoBox = rootVisualElement.Query<VisualElement>("encounter-info").First();

        selectedEncounter = encounter;

        // Grid size setting
        SerializedObject serializedEncounter = new SerializedObject(encounter);
        VisualElement gridSizeBox = rootVisualElement.Query<VisualElement>("grid-size-field").First();
        PropertyField gridSizeField = new PropertyField(serializedEncounter.FindProperty("gridSize"));
        gridSizeField.Bind(serializedEncounter);

        gridSizeField.RegisterValueChangeCallback(
            (SerializedPropertyChangeEvent callback) =>
            {
                encounter.gridSize = new Vector2Int(Mathf.Max(encounter.gridSize.x, 0), Mathf.Max(encounter.gridSize.y, 0));
                if(encounterDesigner) encounterDesigner.Initialize();
            });

        gridSizeBox.Clear();
        gridSizeBox.Add(gridSizeField);


        // Entity list
        entityList = rootVisualElement.Query<ListView>("entity-list").First();
        entityList.makeItem = BuildEntityListing;
        entityList.bindItem = (element, i) =>
        {
            if(i < encounter.Entities.Count)
            {
                BindEntityListing(element, encounter.Entities[i]);
            }
        };

        entityList.itemsSource = encounter.Entities;
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
        EncounterDesigner.FindEncounterDesigner(true).SetEncounter(selectedEncounter);
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

    /// <summary>
    /// Builds a listing for a given EncounterEntity in the Encounter info box.
    /// </summary>
    /// <returns>A reference to the created VisualElement.</returns>
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

    /// <summary>
    /// Binds the given EntityListing to a specific EncounterEntity and its properties.
    /// </summary>
    /// <param name="element">The entity listing to bind.</param>
    /// <param name="entity">The entity to bind the entity listing to.</param>
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
