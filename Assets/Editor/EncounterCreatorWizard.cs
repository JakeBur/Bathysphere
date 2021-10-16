using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Battle;

/// <summary>
/// Window for the creation of a new Encounter.
/// </summary>
public class EncounterCreatorWizard : EditorWindow
{
    /// <summary>
    /// Instantiates the window.
    /// </summary>
    public static void ShowWindow()
    {
        EncounterCreatorWizard window = GetWindow<EncounterCreatorWizard>();
        window.titleContent = new GUIContent("Create an Encounter");
        window.minSize = new Vector2(300, 200);
        window.maxSize = new Vector2(300, 200);
    }

    private void OnEnable()
    {
        VisualTreeAsset original = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/EncounterCreatorWizard.uxml");
        TemplateContainer treeAsset = original.CloneTree();
        rootVisualElement.Add(treeAsset);

        BuildForm();
    }

    /// <summary>
    /// Builds the form that the user will fill out to create a new Encounter.
    /// </summary>
    private void BuildForm()
    {
        TextField nameField = rootVisualElement.Query<TextField>("name-field").First();
        EncounterDesignerWindow.FindAllEncounters(out Encounter[] encounters);
        nameField.value = $"Encounter {encounters.Length+1}";

        rootVisualElement.Query<Button>("create-encounter-button").First().clicked += () =>
        {
            AssetDatabase.CreateAsset(CreateInstance(typeof(Encounter)), $"Assets/Battle/Encounters/{nameField.value}.asset");
            EncounterDesignerWindow.RefreshEncounterList();
            Close();
        };
    }
}
