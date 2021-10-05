using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using Battle;

[CustomPropertyDrawer(typeof(EntityData))]
public class EncounterDataPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        float margin = 25;

        position.x += margin + 300;

        Rect iconPosition = position;
        iconPosition.width = iconPosition.height;
        position.xMin += position.height + margin;

        EditorGUI.BeginProperty(position, label, property);

        SerializedProperty icon = property.FindPropertyRelative("icon");
        Debug.Log(property.FindPropertyRelative("icon"));

        EditorGUI.EndProperty();
    }
}
