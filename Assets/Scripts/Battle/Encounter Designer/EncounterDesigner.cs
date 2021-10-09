using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEditor;
using System;
using System.Linq;
using UnityEditor.SceneManagement;

namespace Battle
{
    //[ExecuteInEditMode]
    [ExecuteAlways]
    public class EncounterDesigner : MonoBehaviour
    {
        public static SerializedProperty dragDropItem;

        public Encounter encounter;

        public GameObject battleSystems;

        public Dictionary<EncounterEntity, GameObject> worldObjects;

        // instantiated preview object for an uncommitted addition of an entityData
        private GameObject entityDataPreview;

        public GameObject battleSystemsPrefab;

        public bool initialize;

        private void Awake()
        {
            if(Application.isPlaying)
            {
                CleanScene();
                GameObject battleSystems = Instantiate(battleSystemsPrefab);
                battleSystems.GetComponent<BattleManager>().StartEncounter(encounter);
            }
            else
            {
                Initialize();
            }
            
        }

        private void Start()
        {
            Initialize();
        }

        private void Update()
        {
            if(!initialize)
            {
                initialize = true;
                Initialize();
            }
        }

        private void OnDestroy()
        {
            SceneView.duringSceneGui -= HandleSceneGUI;
            if(encounter != null) encounter.OnContentsUpdated -= InitializeEntities;
        }

        public static void PickUpEntity(SerializedProperty entity)
        {
            Debug.Log("picking up: " + entity);
            dragDropItem = entity;
        }

        public static void PutDownEntity()
        {
            if (dragDropItem != null)
            {
                Debug.Log("putting down: " + dragDropItem);
                dragDropItem = null;
            }
        }

        public Vector2Int GetGridSize()
        {
            return battleSystems.GetComponent<BattleGridManager>().Grid.Size;
        }

        public void Initialize()
        {
            BattleGridManager battleGridManager = GetComponentInChildren<BattleGridManager>();

            if (encounter == null)
            {
                CleanScene();
                battleGridManager.UpdateSize(0, 0);
                return;
            }

            SceneView.duringSceneGui -= HandleSceneGUI;
            SceneView.duringSceneGui += HandleSceneGUI;

            battleGridManager.UpdateSize(encounter.gridSize);

            InitializeEntities();

            encounter.OnContentsUpdated -= InitializeEntities;
            encounter.OnContentsUpdated += InitializeEntities;
        }

        private void CleanScene()
        {
            FindObjectsOfType<Entity>().ToList().ForEach(entity => DestroyImmediate(entity.gameObject));
            if(Application.isPlaying) Destroy(FindObjectOfType<BattleManager>().gameObject);
        }

        private void InitializeEntities()
        {
            BattleGridManager battleGridManager = GetComponentInChildren<BattleGridManager>();

            FindObjectsOfType<Entity>().ToList().ForEach(entity => DestroyImmediate(entity.gameObject));
            
            worldObjects = new Dictionary<EncounterEntity, GameObject>();

            encounter.Entities.ForEach(entity =>
            {
                GameObject worldEntity = Instantiate(entity.EntityData.prefab);
                worldEntity.AddComponent<EntityGizmo>();

                if (battleGridManager.Grid.HasPosition(entity.Position))
                {
                    worldEntity.transform.position = battleGridManager.Grid.squares[entity.Position.x, entity.Position.y].transform.position;
                }
                else
                {
                    worldEntity.SetActive(false);
                }

                worldObjects.Add(entity, worldEntity);
            });
        }

        private void HandleSceneGUI(SceneView view)
        {
            EncounterEntity encounterEntity = DragAndDrop.GetGenericData("EncounterEntity") as EncounterEntity;
            if(encounterEntity)
            {
                HandleDragDrop(encounterEntity);
                return;
            }

            // recieve EntityData from our own drag start
            EntityData entityData = DragAndDrop.GetGenericData("EntityData") as EntityData;
            if (entityData)
            {
                HandleDragDrop(entityData);
                return;
            }

            // receive EntityData from Unity file system
            if (DragAndDrop.objectReferences.Length == 0) return;
            entityData = DragAndDrop.objectReferences.First() as EntityData;
            if (entityData)
            {
                HandleDragDrop(entityData);
                return;
            }
        }

        private void HandleDragDrop(EncounterEntity entity)
        {
            if (encounter.Entities.Contains(entity))
            {

                Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
                Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity);
                GridSquare targetSquare = null;

                if (hitInfo.collider != null)
                {
                    targetSquare = hitInfo.collider.gameObject.GetComponent<GridSquare>();

                    if (targetSquare)
                    {
                        DragAndDrop.visualMode = DragAndDropVisualMode.Move;
                        if (worldObjects.ContainsKey(entity))
                        {
                            worldObjects[entity].SetActive(true);
                        }
                        else
                        {
                            worldObjects[entity] = Instantiate(entity.EntityData.prefab);
                        }

                        worldObjects[entity].transform.position = targetSquare.transform.position;
                        entity.Position = targetSquare.Position;
                    }
                    else
                    {
                        if (worldObjects.ContainsKey(entity)) worldObjects[entity].SetActive(false);
                    }
                }
                else
                {
                    if (worldObjects.ContainsKey(entity)) worldObjects[entity].SetActive(false);
                }


                if (Event.current.type == EventType.MouseEnterWindow)
                {
                    if (targetSquare)
                    {
                        entity.Position = targetSquare.Position;
                    }
                    else
                    {
                        entity.Position = new Vector2Int(-1, -1);
                    }

                    DragAndDrop.AcceptDrag();
                    DragAndDrop.PrepareStartDrag();
                }
            }
        }

        private void HandleDragDrop(EntityData entity)
        {
            Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity);
            GridSquare targetSquare = null;

            if (hitInfo.collider != null)
            {
                targetSquare = hitInfo.collider.gameObject.GetComponent<GridSquare>();

                if (targetSquare)
                {
                    DragAndDrop.visualMode = DragAndDropVisualMode.Generic;
                    if (!(Event.current.type == EventType.DragPerform || Event.current.type == EventType.DragUpdated)) return;

                    if (entityDataPreview)
                    {
                        entityDataPreview.SetActive(true);
                    }
                    else
                    {
                        entityDataPreview = Instantiate(entity.prefab);
                    }

                    entityDataPreview.transform.position = targetSquare.transform.position;
                }
                else
                {
                    if (entityDataPreview) entityDataPreview.SetActive(false);
                }
            }
            else
            {
                if (entityDataPreview) entityDataPreview.SetActive(false);
            }

            if (Event.current.type == EventType.DragPerform || Event.current.type == EventType.DragExited)
            {
                Vector2Int? position = null;
                if(targetSquare)
                {
                    position = targetSquare.Position;
                }

                EncounterEntity encounterEntity = encounter.AddEntity(entity, position);
                DestroyImmediate(entityDataPreview);

                Event.current.Use();
                DragAndDrop.AcceptDrag();
            }

            /*if(Event.current.type == EventType.DragExited)
            {
                DestroyImmediate(entityDataPreview);
            }*/
        }

        public static EncounterDesigner FindEncounterDesigner(bool promptIfOutsideScene = false)
        {
            EncounterDesigner encounterDesigner = FindObjectOfType<EncounterDesigner>();


            if (!encounterDesigner && promptIfOutsideScene)
            {
                bool openScene = EditorUtility.DisplayDialog("Open Encounter Designer?",
                    "Would you like to open the Encounter Designer Scene to make changes to this Encounter?",
                    "Open Scene", "Cancel");

                if (openScene)
                {

                    EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
                    EditorSceneManager.OpenScene("Assets/Encounter Designer/EncounterDesigner.unity");
                    
                    encounterDesigner = FindObjectOfType<EncounterDesigner>();
                }
            }

            return encounterDesigner;
        }

        public void SetEncounter(Encounter encounter)
        {
            if(this.encounter != null) this.encounter.OnContentsUpdated -= InitializeEntities;

            this.encounter = encounter;
            Initialize();
        }

        private void HandleClick(InputAction.CallbackContext context)
        {
            PutDownEntity();
        }

        
    }
}