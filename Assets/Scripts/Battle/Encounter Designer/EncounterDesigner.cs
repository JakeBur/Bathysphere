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
    /// <summary>
    /// Manages the state of a scene to allow the user to modify an Encounter.
    /// </summary>
    [ExecuteAlways]
    public class EncounterDesigner : MonoBehaviour
    {
        public Encounter encounter;

        public GameObject battleSystems;

        public Dictionary<EncounterEntity, GameObject> worldObjects;

        /// <summary>
        /// Instantiated preview object for an uncommitted addition of an entityData.
        /// </summary>
        private GameObject entityDataPreview;

        /// <summary>
        /// Reference to the prefab that should be instantiated on play so that the Encounter can be tested.
        /// </summary>
        public GameObject battleSystemsPrefab;

        private void Awake()
        {

            if (Application.isPlaying)
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

        private void OnDestroy()
        {
            SceneView.duringSceneGui -= HandleSceneGUI;
            if(encounter != null) encounter.OnContentsUpdated -= InitializeEntities;
        }

        /// <summary>
        /// Initializes the scene when in edit mode to reflect the state of the current Encounter.
        /// </summary>
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

        /// <summary>
        /// Gets the current EncounterDesigner.
        /// Optionally, if it doesn't exist, gives the user a prompt which will open the EncounterDesigner scene.
        /// </summary>
        /// <param name="promptIfOutsideScene">Whether to prompt the user to switch scenes or not.</param>
        /// <returns>The current EncounterDesigner, or null if one does not exist and the user decided not to open the correct scene.</returns>
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

        /// <summary>
        /// Sets the current encounter.
        /// Calls Initialization functionality to update the scene to the new Encounter.
        /// </summary>
        /// <param name="encounter">The Encounter to swap to.</param>
        public void SetEncounter(Encounter encounter)
        {
            if (this.encounter != null) this.encounter.OnContentsUpdated -= InitializeEntities;

            this.encounter = encounter;
            Initialize();
        }

        /// <summary>
        /// Clears the scene of Entities to have a clean slate.
        /// If the application is in play mode, it deletes the edit mode instance of the BattleManager as well.
        /// </summary>
        private void CleanScene()
        {
            FindObjectsOfType<Entity>().ToList().ForEach(entity => DestroyImmediate(entity.gameObject));
            if(Application.isPlaying) DestroyImmediate(FindObjectOfType<BattleManager>().gameObject);
        }

        /// <summary>
        /// Initializes all entities desribed by the current Encounter's Encounter Entities.
        /// </summary>
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

        /// <summary>
        /// Handler for the SceneView update loop. Used to hook into drag and drop events.
        /// </summary>
        /// <param name="view">The SceneView being updated.</param>
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

        /// <summary>
        /// Handles drag and drop functionality for EncounterEntities.
        /// </summary>
        /// <param name="entity">The EncounterEntity being dragged.</param>
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

        /// <summary>
        /// Handles drag and drop functionality for EntityData.
        /// </summary>
        /// <param name="entity">The EntityData being dragged.</param>
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

                DestroyImmediate(entityDataPreview);

                Event.current.Use();
                DragAndDrop.AcceptDrag();
            }
        }
    }
}