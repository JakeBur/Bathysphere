using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using Battle;

public class EncounterEntityPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        float margin = 25;
        position.x += margin;

        EditorGUI.BeginProperty(position, label, property);

        position.xMax = (position.xMax / 3) + margin;
        EditorGUI.PropertyField(position, property.FindPropertyRelative("position"), GUIContent.none);

        position.x += position.width + margin;
        EditorGUI.ObjectField(position, property.FindPropertyRelative("entityData"), GUIContent.none);

        position.x += position.width + margin;
        EditorGUI.PropertyField(position, property.FindPropertyRelative("entityData"), GUIContent.none);

        /*if (Event.current.type == EventType.MouseDown)
        {
            DragAndDrop.PrepareStartDrag();
            DragAndDrop.SetGenericData("SerializedProperty", property);
            DragAndDrop.StartDrag("Encounter Entity");
        }*/


        //if (Event.current.type != EventType.Repaint && Event.current.type != EventType.Layout) Debug.Log(Event.current.type);

        EditorGUI.EndProperty();
    }
}
