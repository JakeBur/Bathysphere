using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEditor;
using System;

namespace Battle
{

    [ExecuteInEditMode]
    public class EncounterDesigner : MonoBehaviour
    {
        public static SerializedProperty dragDropItem;

        public static EncounterDesigner Instance;

        public Encounter encounter;

        public GameObject battleSystems;
        private InputActions _inputActions;

        private List<Entity> _unplacedEntities;

        public bool init = false;
        private Action<SceneView> subscriber;

        private void Update()
        {
            if (!init)
            {
                Instance = this;

                init = true;
                SceneView.beforeSceneGui -= subscriber;

                subscriber = (SceneView view) =>
                {
                    if (Event.current.type == EventType.MouseEnterWindow) PutDownEntity();
                };

                SceneView.duringSceneGui += subscriber;

                GetComponentInChildren<BattleGridManager>().UpdateSize(encounter.gridSize);
            }
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