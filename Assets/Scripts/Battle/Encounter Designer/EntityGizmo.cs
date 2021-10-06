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
        //SceneView.onSceneGUIDelegate += (SceneView.OnSceneFunc)Delegate.Combine(SceneView.onSceneGUIDelegate, new SceneView.OnSceneFunc(CustomOnSceneGUI));
        SceneView.duringSceneGui -= DrawHitbox;
        SceneView.duringSceneGui += DrawHitbox;
    }

    private void OnDisable()
    {
        SceneView.duringSceneGui -= DrawHitbox;
    }

    private void OnDestroy()
    {
        SceneView.duringSceneGui -= DrawHitbox;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DrawHitbox(SceneView sceneView)
    {
        if(Event.current.type == EventType.DragUpdated || Event.current.type == EventType.Repaint)
        {
            return;
        }

        Mesh mesh = AssetDatabase.LoadAssetAtPath("Assets/Editor/Gizmos/EntityGizmo.blend", typeof(Mesh)) as Mesh;

        EntityGizmo entityGizmo = this as EntityGizmo;

        //Handles.Button(entityGizmo.transform.position, Quaternion.identity, 1, 1, );
        //Handles.Label(entityGizmo.transform.position + Vector3.up * 3, "HI!");
        try
        {
            Handles.BeginGUI();
            {
                Vector2 position = HandleUtility.WorldToGUIPoint(entityGizmo.transform.position);
                GUIStyle hitboxStyle = new GUIStyle("box");

                GUILayout.BeginArea(new Rect(position.x - 50, position.y - 50, 100, 100), hitboxStyle);
                {

                    if (Event.current.type == EventType.MouseDown)
                    {
                        EncounterDesigner encounterDesigner = FindObjectOfType<EncounterDesigner>();

                        if (encounterDesigner)
                        {
                            EncounterEntity encounterEntity = encounterDesigner.worldObjects.Keys.ToList()
                                .Find(key => encounterDesigner.worldObjects[key] == entityGizmo.gameObject);

                            if (encounterEntity)
                            {
                                DragAndDrop.PrepareStartDrag();
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
        catch
        {
            Debug.Log("Error on: " + Event.current.type);
        }

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
