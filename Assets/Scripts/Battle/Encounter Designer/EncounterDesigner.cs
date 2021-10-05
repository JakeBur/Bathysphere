using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEditor;
using System;
using System.Linq;

namespace Battle
{

    [ExecuteInEditMode]
    public class EncounterDesigner : MonoBehaviour
    {
        public static SerializedProperty dragDropItem;

        public static EncounterDesigner Instance;

        public Encounter encounter;

        public GameObject battleSystems;

        private List<Entity> _unplacedEntities;

        private Dictionary<EncounterEntity, GameObject> worldObjects;

        public bool initialize;

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
        }

        private void Initialize()
        {
            Instance = this;

            SceneView.duringSceneGui -= HandleSceneGUI;
            SceneView.duringSceneGui += HandleSceneGUI;

            BattleGridManager battleGridManager = GetComponentInChildren<BattleGridManager>();
            battleGridManager.UpdateSize(encounter.gridSize);

            FindObjectsOfType<Entity>().ToList().ForEach(entity => DestroyImmediate(entity.gameObject));

            worldObjects = new Dictionary<EncounterEntity, GameObject>();

            encounter.entities.ForEach(entity =>
            {
                GameObject worldEntity = Instantiate(entity.entityData.prefab);
                if(battleGridManager.Grid.HasPosition(entity.position))
                {
                    worldEntity.transform.position = battleGridManager.Grid.squares[entity.position.x, entity.position.y].transform.position;
                }

                worldObjects.Add(entity, worldEntity);
            });
        }

        private void HandleSceneGUI(SceneView view)
        {
            EncounterEntity draggedEntity = DragAndDrop.GetGenericData("EncounterEntity") as EncounterEntity;

            if(draggedEntity != null && encounter.entities.Contains(draggedEntity))
            {
                EditorUtility.SetDirty(draggedEntity);

                Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
                Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity);
                GridSquare targetSquare = null;

                if (hitInfo.collider != null)
                {
                    targetSquare = hitInfo.collider.gameObject.GetComponent<GridSquare>();

                    if (targetSquare)
                    {
                        DragAndDrop.visualMode = DragAndDropVisualMode.Move;
                        if (worldObjects.ContainsKey(draggedEntity))
                        {
                            worldObjects[draggedEntity].SetActive(true);
                        }
                        else
                        {
                            worldObjects[draggedEntity] = Instantiate(draggedEntity.entityData.prefab);
                        }

                        worldObjects[draggedEntity].transform.position = targetSquare.transform.position;
                        draggedEntity.position = targetSquare.Position;
                    }
                    else
                    {
                        if (worldObjects.ContainsKey(draggedEntity)) worldObjects[draggedEntity].SetActive(false);
                    }
                }
                else
                {
                    if (worldObjects.ContainsKey(draggedEntity)) worldObjects[draggedEntity].SetActive(false);
                }

                if (Event.current.type == EventType.MouseEnterWindow)
                {
                    if(targetSquare)
                    {
                        draggedEntity.position = targetSquare.Position;
                    }
                    else
                    {
                        draggedEntity.position = new Vector2Int(-1, -1);
                    }

                    DragAndDrop.AcceptDrag();
                    DragAndDrop.PrepareStartDrag();
                }
            }
        }

        public void SetEncounter(Encounter encounter)
        {
            this.encounter = encounter;
            Initialize();
        }

        private void HandleClick(InputAction.CallbackContext context)
        {
            PutDownEntity();
        }

        public static void PickUpEntity(SerializedProperty entity)
        {
            Debug.Log("picking up: " + entity);
            dragDropItem = entity;
        }
        
        public static void PutDownEntity()
        {
            if(dragDropItem != null)
            {
                Debug.Log("putting down: " + dragDropItem);
                dragDropItem = null;
            }
        }

        public void UpdateGridSize(int x, int y)
        {
            BattleGridManager gridManager = battleSystems.GetComponent<BattleGridManager>();

            List<Entity> entities = gridManager.Grid.FindEntities();

            gridManager.UpdateSize(x, y);
            entities.ForEach(entity =>
            {
                if(gridManager.Grid.HasPosition(entity.Square.Position))
                {
                    entity.Square = gridManager.Grid[entity.Square.Position.x, entity.Square.Position.y];
                }
                else
                {
                    entity.Square = null;
                    _unplacedEntities.Add(entity);
                }
            });
        }

        public Vector2Int GetGridSize()
        {
            return battleSystems.GetComponent<BattleGridManager>().Grid.Size;
        }
    }
}