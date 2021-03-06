using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Battle;
using System.Linq;
using System;

[CustomEditor(typeof(EntityGizmo))]
public class EntityGizmoEditor : Editor
{
    private void OnEnable()
    {
        //SceneView.onSceneGUIDelegate += (SceneView.OnSceneFunc)Delegate.Combine(SceneView.onSceneGUIDelegate, new SceneView.OnSceneFunc(CustomOnSceneGUI));
        //SceneView.duringSceneGui -= DrawHitbox;
        //SceneView.duringSceneGui += DrawHitbox;
    }

    private void OnDestroy()
    {
        //SceneView.duringSceneGui -= DrawHitbox;
    }

    public void DrawHitbox(SceneView sceneView)
    {
        Mesh mesh = AssetDatabase.LoadAssetAtPath("Assets/Editor/Gizmos/EntityGizmo.blend", typeof(Mesh)) as Mesh;

        EntityGizmo entityGizmo = target as EntityGizmo;

        //Handles.Button(entityGizmo.transform.position, Quaternion.identity, 1, 1, );
        //Handles.Label(entityGizmo.transform.position + Vector3.up * 3, "HI!");

        Handles.BeginGUI();
        {
            Vector2 position = HandleUtility.WorldToGUIPoint(entityGizmo.transform.position);
            GUIStyle hitboxStyle = new GUIStyle("box");

            GUILayout.BeginArea(new Rect(position.x - 50, position.y - 50, 100, 100), hitboxStyle);
            {

                if(Event.current.type == EventType.MouseDown)
                {
                    EncounterDesigner encounterDesigner = FindObjectOfType<EncounterDesigner>();

                    if(encounterDesigner)
                    {
                        EncounterEntity encounterEntity = encounterDesigner.worldObjects.Keys.ToList()
                            .Find(key => encounterDesigner.worldObjects[key] == entityGizmo.gameObject);

                        if(encounterEntity)
                        {
                            DragAndDrop.visualMode = DragAndDropVisualMode.Generic;
                            DragAndDrop.PrepareStartDrag();
                            DragAndDrop.SetGenericData("EncounterEntity", encounterEntity);
                            DragAndDrop.StartDrag("Encounter Entity");
                        }
                    }
                }
            }
            GUILayout.EndArea();
        }
        Handles.EndGUI();

    }

            /*Gizmos.color = new Color(0, 1, 1, 0.5f);
            Gizmos.DrawMesh(mesh, entityGizmo.transform.position, Quaternion.identity, Vector3.one * 0.5f);
            Gizmos.color = new Color(0, 1, 0, 1f);
            Gizmos.DrawWireMesh(mesh, entityGizmo.transform.position, Quaternion.identity, Vector3.one * 0.5f);*/

            //Handles.DrawWireCube((target as EntityGizmo).transform.position, Vector3.one);
}
