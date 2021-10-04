using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;

namespace Battle.Editor
{
    [CustomPropertyDrawer(typeof(EncounterEntity))]
    public class EncounterEntityPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            float margin = 25;
            position.x += margin;

            Rect imageRect = position;
            imageRect.width = imageRect.height;
            position.xMin += position.height + margin;
            EditorGUI.BeginProperty(position, label, property);
            //position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
            //GUILayout.Box(property.FindPropertyRelative("icon").objectReferenceValue as Texture);
            //imageRect.width = imageRect.width * 2;
            //imageRect.height = imageRect.width * 2;
            //GUI.Button(imageRect, GUIContent.none);
            GUI.DrawTexture(imageRect, property.FindPropertyRelative("icon").objectReferenceValue as Texture);
            

            position.xMax = (position.xMax / 3) + margin;
            EditorGUI.ObjectField(position, property.FindPropertyRelative("icon"), GUIContent.none);

            position.x += position.width + margin;
            EditorGUI.PropertyField(position, property.FindPropertyRelative("position"), GUIContent.none);

            if (Event.current.type == EventType.MouseDown)
            {
                EncounterDesigner.PickUpEntity(property);
            }

            //if (Event.current.type != EventType.Repaint && Event.current.type != EventType.Layout) Debug.Log(Event.current.type);

            EditorGUI.EndProperty();
        }
    }
}