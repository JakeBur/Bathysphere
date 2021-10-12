using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

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

        private GridSquare _selectedSquare;
        private int _selectionIndex = 0;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            BattleGridManager.Instance.OnSquareClicked.AddListener(HandleClick, 0);
        }

        public void Select(ISelectable selectable)
        {
            if (selected != null) selected.Deselect();
            selected = selectable;
            selected.Select();

            OnSelect?.Invoke(selected);
        }

        /// <summary>
        /// Handler for click events from the BattleGrid.
        /// </summary>
        /// <param name="square">The clicked square.</param>
        /// <param name="context">The calling PriorityEvent</param>
        private void HandleClick(GridSquare square, PriorityEvent<GridSquare> context)
        {
            List<Entity> selectableEntities = square.Entities.Where(entity => entity.IsSelectable()).ToList();

            if(selectableEntities.Count > 0)
            {
                // cycle selection
                if(square == _selectedSquare)
                {
                    _selectionIndex = (_selectionIndex + 1) % selectableEntities.Count;
                }
                else
                {
                    _selectedSquare = square;
                    _selectionIndex = 0;
                }

                int ignoreCounter = _selectionIndex;

                // Find the first selectable entity at the given index and select it
                foreach(Entity entity in selectableEntities)
                {
                    if(entity.IsSelectable())
                    {
                        if(ignoreCounter == 0)
                        {
                            Select(entity);

                            // only select one entity
                            break;
                        }
                        else
                        {
                            ignoreCounter--;
                        }
                    }
                }
            }
        }
    }

}