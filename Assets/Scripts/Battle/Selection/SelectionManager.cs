using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Battle
{
    /// <summary>
    /// Manager for the player's selections.
    /// </summary>
    public class SelectionManager : MonoBehaviour
    {
        public static SelectionManager Instance;

        /// <summary>
        /// Invoked when a new ISelectable is selected.
        /// Supplies the newly selected ISelectable as an argument.
        /// </summary>
        public Action<ISelectable> OnSelect;

        /// <summary>
        /// The currently selected ISelectable.
        /// null if nothing is currently selected.
        /// </summary>
        public ISelectable selected;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            BattleGridManager.Instance.OnSquareClicked.AddListener(HandleClick, 0);
        }

        /// <summary>
        /// Handler for click events from the BattleGrid.
        /// </summary>
        /// <param name="square">The clicked square.</param>
        /// <param name="context">The calling PriorityEvent</param>
        private void HandleClick(GridSquare square, PriorityEvent<GridSquare> context)
        {
            if(square.Entities.Count > 0)
            {
                // Find the first selectable entity in the list and select it
                foreach(Entity entity in square.Entities)
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