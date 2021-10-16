using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Battle;
using System.Linq;

[ExecuteAlways]
public class EntityGizmo : MonoBehaviour
{
    private void OnEnable()
    {
        SceneView.beforeSceneGui -= DrawHitbox;
        SceneView.beforeSceneGui += DrawHitbox;
    }

    private void OnDisable()
    {
        SceneView.beforeSceneGui -= DrawHitbox;
    }

    private void OnDestroy()
    {
        SceneView.beforeSceneGui -= DrawHitbox;
    }

    /// <summary>
    /// Creates the invisible interaction hitbox that allows the user to drag and drop EncounterEntities in the Encounter Editor.
    /// </summary>
    /// <param name="sceneView">The sceneView to draw the hitbox on.</param>
    public void DrawHitbox(SceneView sceneView)
    {
        if (Event.current.type == EventType.DragUpdated || Event.current.type == EventType.Repaint)
        {
            return;
        }

        if (DragAndDrop.GetGenericData("EncounterEntity") as EncounterEntity)
        {
            return;
        }

        Mesh mesh = AssetDatabase.LoadAssetAtPath("Assets/Editor/Gizmos/EntityGizmo.blend", typeof(Mesh)) as Mesh;

        EntityGizmo entityGizmo = this as EntityGizmo;

        Handles.BeginGUI();
        {
            Vector2 position = HandleUtility.WorldToGUIPoint(entityGizmo.transform.position);
            GUIStyle hitboxStyle = new GUIStyle("box");

            GUILayout.BeginArea(new Rect(position.x - 50, position.y - 50, 100, 100), hitboxStyle);
            {

                if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
                {
                    EncounterDesigner encounterDesigner = FindObjectOfType<EncounterDesigner>();

                    if (encounterDesigner && encounterDesigner.encounter != null)
                    {
                        EncounterEntity encounterEntity = encounterDesigner.worldObjects.Keys.ToList()
                            .Find(key => encounterDesigner.worldObjects[key] == entityGizmo.gameObject);

                        if (encounterEntity)
                        {
                            DragAndDrop.SetGenericData("EncounterEntity", encounterEntity);
                            DragAndDrop.StartDrag("Encounter Entity");
                            Event.current.Use();
                        }
                    }
                }
            }
            GUILayout.EndArea();
        }
        Handles.EndGUI();
    }

    private void OnDrawGizmos()
    {
        Mesh mesh = AssetDatabase.LoadAssetAtPath("Assets/Editor/Gizmos/EntityGizmo.blend", typeof(Mesh)) as Mesh;
        Gizmos.color = new Color(0, 1, 1, 0.5f);
        Gizmos.DrawMesh(mesh, transform.position, Quaternion.Euler(-90, 0, 0), Vector3.one * 0.5f);
        Gizmos.color = new Color(0, 1, 0, 0.2f);
        Gizmos.DrawWireMesh(mesh, transform.position, Quaternion.Euler(-90, 0, 0), Vector3.one * 0.5f);
    }
}
