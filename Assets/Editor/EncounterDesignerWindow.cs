using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using Battle;
using System.Linq;
using UnityEditor.UIElements;

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
                encounterInfoBox.Clear();

                Encounter encounter = it as Encounter;

                SerializedObject serializedEncounter = new SerializedObject(encounter);
                
                /*SerializedObject serializedEncounter = new SerializedObject(encounter);
                SerializedProperty encounterProperty = serializedEncounter.GetIterator();
                encounterProperty.Next(true);

                while (encounterProperty.NextVisible(false))
                {
                    PropertyField propertyField = new PropertyField(encounterProperty);

                    propertyField.SetEnabled(encounterProperty.name != "n_Script");
                    propertyField.Bind(serializedEncounter);
                    encounterInfoBox.Add(propertyField);
                }*/

                //SerializedObject serializedEncounter = new SerializedObject(encounter.entities);
                //SerializedProperty entityProperty = serializedEncounter.FindProperty("");

                PropertyField gridSizeField = new PropertyField(serializedEncounter.FindProperty("gridSize"));
                gridSizeField.Bind(serializedEncounter);
                encounterInfoBox.Add(gridSizeField);

                Button openEncounterButton = new Button();
                openEncounterButton.text = "Open in Encounter Designer";
                openEncounterButton.clicked += () => EncounterDesigner.Instance.encounter = encounter;
                encounterInfoBox.Add(openEncounterButton);

                PropertyField entitiesField = new PropertyField(serializedEncounter.FindProperty("entities"));
                entitiesField.Bind(serializedEncounter);
                entitiesField.style.flexGrow = 1;
                encounterInfoBox.Add(entitiesField);
                /*TextField entitiesField = new TextField();
                encounter.entities[0]
                gridSizeField.Bind(serializedEncounter);
                entitiesField.style.flexGrow = 1;
                encounterInfoBox.Add(entitiesField);*/


                /*ListView entityList = new ListView();
                entityList.makeItem = () =>
                {
                    return new PropertyField();
                };

                //entityList.bindItem = (element, i) => (element as PropertyField) = encounter.entities[i].prefab.name;
                
                entityList.bindItem = (element, i) =>
                {
                    Debug.Log("ey");
                    Debug.Log((element as PropertyField));
                    (element as PropertyField).Bind(new SerializedObject(encounter.entities[i]));
                        
                };

                entityList.itemsSource = encounter.entities;
                entityList.itemHeight = 64;
                entityList.selectionType = SelectionType.Single;
                entityList.name = "entitylist";
                entityList.style.flexGrow = 1;

                encounterInfoBox.Add(entityList);
                entityList.Refresh();*/
            }
        };

        encounterList.Refresh();
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
