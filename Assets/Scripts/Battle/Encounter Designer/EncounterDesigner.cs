using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEditor;
using System;
using System.Reflection;

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

        private void Start()
        {
            Instance = this;

            init = true;
            SceneView.duringSceneGui -= HandleSceneGUI;
            SceneView.duringSceneGui += HandleSceneGUI;

            GetComponentInChildren<BattleGridManager>().UpdateSize(encounter.gridSize);
        }

        private void HandleSceneGUI(SceneView view)
        {
            EncounterEntity draggedEntity = DragAndDrop.GetGenericData("EncounterEntity") as EncounterEntity;

            if(draggedEntity != null)
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Move;

                if (Event.current.type == EventType.MouseEnterWindow)
                {
                    Debug.Log("Dropping: " + draggedEntity.entityData.name);
                    DragAndDrop.AcceptDrag();
                }
            }
        }

        // From https://github.com/lordofduct/spacepuppy-unity-framework/blob/master/SpacepuppyBaseEditor/EditorHelper.cs
        private static object GetValue_Imp(object source, string name, int index)
        {
            var enumerable = GetValue_Imp(source, name) as System.Collections.IEnumerable;
            if (enumerable == null) return null;
            var enm = enumerable.GetEnumerator();

            for (int i = 0; i <= index; i++)
            {
                if (!enm.MoveNext()) return null;
            }
            return enm.Current;
        }

        // From https://github.com/lordofduct/spacepuppy-unity-framework/blob/master/SpacepuppyBaseEditor/EditorHelper.cs
        private static object GetValue_Imp(object source, string name)
        {
            if (source == null)
                return null;
            var type = source.GetType();

            while (type != null)
            {
                var f = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                if (f != null)
                    return f.GetValue(source);

                var p = type.GetProperty(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (p != null)
                    return p.GetValue(source, null);

                type = type.BaseType;
            }
            return null;
        }

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