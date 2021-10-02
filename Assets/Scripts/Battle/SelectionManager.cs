using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Battle
{
    public class SelectionManager : MonoBehaviour
    {
        public static SelectionManager Instance;

        public Action<ISelectable> OnSelect;

        public ISelectable selected;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            BattleGridManager.Instance.OnSquareClicked.AddListener(HandleClick, 0);
        }

        private void HandleClick(GridSquare square, PriorityAction<GridSquare> context)
        {
            if(square.entities.Count > 0)
            {
                // Find the first selectable entity in the list and select it
                foreach(Entity entity in square.entities)
                {
                    if(entity.IsSelectable())
                    {
                        if(selected != null) selected.Deselect();
                        selected = entity;
                        selected.Select();

                        OnSelect?.Invoke(selected);

                        // only select one entity
                        break;
                    }
                }
            }
        }
    }

}