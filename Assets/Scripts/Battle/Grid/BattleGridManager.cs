using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Battle
{
    /// <summary>
    /// A BattleGridManager constructs a BattleGrid and manages its interaction with other Battle elements.
    /// </summary>
    public class BattleGridManager : MonoBehaviour
    {
        public static BattleGridManager Instance;

        public GameObject squarePrefab;
        public List<Entity> entities;

        /// <summary>
        /// Invoked when any GridSquare on the BattleGrid is clicked.
        /// Supplies the clicked GridSquare as an argument.
        /// </summary>
        public PriorityEvent<GridSquare> OnSquareClicked;

        /// <summary>
        /// Invoked when any GridSquare on the BattleGrid is hovered over.
        /// Supplies the hovered GridSquare as an argument.
        /// </summary>
        public Action<GridSquare> OnSquareHovered;

        /// <summary>
        /// The BattleGrid this BattleGridManager uses to track entities and their spatial relationships.
        /// </summary>
        [HideInInspector]
        private BattleGrid _grid;

        private void Awake()
        {
            Instance = this;
            OnSquareClicked = new PriorityEvent<GridSquare>();

            _grid = new BattleGrid(5, 5);

            // populate the grid with squares
            for(int x = 0; x < _grid.squares.GetLength(0); x++)
            {
                for (int y = 0; y < _grid.squares.GetLength(0); y++)
                {
                    GameObject square = Instantiate(squarePrefab, transform);
                    square.transform.localPosition = new Vector3(x, 0, y);

                    GridSquare gridSquare = square.GetComponent<GridSquare>();

                    _grid[x, y] = square.GetComponent<GridSquare>();

                    gridSquare.OnClick += HandleClick;
                }
            }

            // TODO: temporary manual placement of entities for testing
            entities.ForEach(entity => entity.OnRemovedFromPlay += RemoveEntity);

            entities[0].Square = _grid.squares[0, 0];
            entities[1].Square = _grid.squares[4, 4];
            entities[2].Square = _grid.squares[1, 4];
        }

        

        private void HandleClick(GridSquare gridSquare)
        {
            OnSquareClicked?.Invoke(gridSquare);
        }

        /// <summary>
        /// Removes an Entity from the BattleGrid.
        /// </summary>
        /// <param name="entity">The Entity to remove.</param>
        private void RemoveEntity(Entity entity)
        {
            entities.Remove(entity);
            entity.Square = null;
        }
    }
}
