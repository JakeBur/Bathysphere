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

    private void CreateEncounterListView()
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
            foreach (Object it in enumerable)
            {
                VisualElement encounterInfoBox = rootVisualElement.Query<VisualElement>("encounter-info").First();

                Encounter encounter = it as Encounter;

                SerializedObject serializedEncounter = new SerializedObject(encounter);

                VisualElement gridSizeBox = rootVisualElement.Query<VisualElement>("grid-size-field").First();
                PropertyField gridSizeField = new PropertyField(serializedEncounter.FindProperty("gridSize"));
                gridSizeField.Bind(serializedEncounter);
                gridSizeBox.Clear();
                gridSizeBox.Add(gridSizeField);

                Button openEncounterButton = rootVisualElement.Query<Button>("encounter-designer-button").First();
                openEncounterButton.clicked += () =>
                {
                    Debug.Log("boom");
                    GameObject.Find("Encounter Designer").GetComponent<EncounterDesigner>().SetEncounter(encounter);
                };
                encounterInfoBox.Add(openEncounterButton);

                ListView entityList = rootVisualElement.Query<ListView>("entity-list").First();
                entityList.makeItem = BuildEntityListing;
                entityList.bindItem = (element, i) => BindEntityListing(element, encounter.entities[i]);

                entityList.itemsSource = encounter.entities;
                //entityList.itemHeight = 16;
                entityList.itemHeight = 32;
                entityList.selectionType = SelectionType.Single;

                entityList.Refresh();
            }
        };

        encounterList.Refresh();
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

        


        /*label = new Label("Position");
        label.style.flexGrow = 1;
        columnContainer.Add(label);*/




        //entityBox.HandleEvent(new MouseDownEvent());


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

        element.RegisterCallback<MouseDownEvent>((MouseDownEvent mouseDownEvent) => StartDragEntity(mouseDownEvent, entity));
    }


    private VisualElement BuildEntityDetails()
    {
        return null;
    }

    private void StartDragEntity(MouseDownEvent mouseDownEvent, EncounterEntity encounterEntity)
    {
        Debug.Log("trying to start drag");
        DragAndDrop.visualMode = DragAndDropVisualMode.Generic;
        DragAndDrop.PrepareStartDrag();
        DragAndDrop.SetGenericData("EncounterEntity", encounterEntity);
        DragAndDrop.StartDrag("Encounter Entity");
    }

    /*private VisualElement CreateEntityListing(Encounter.EncounterEntity entity)
    {
        VisualElement visualElement = new VisualElement();

        Image image = new Image();
        image.image = entity.entity.gameObject;

        visualElement.Add();
    }*/


    private void OnEnable()
    {
        VisualTreeAsset original = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/EncounterDesignerWindow.uxml");
        TemplateContainer treeAsset = original.CloneTree();
        rootVisualElement.Add(treeAsset);

        CreateEncounterListView();
    }
}
