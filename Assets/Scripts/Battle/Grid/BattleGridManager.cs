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
        private List<Entity> _entities;

        /// <summary>
        /// Invoked when any GridSquare on the BattleGrid is clicked.
        /// Supplies the clicked GridSquare as an argument.
        /// </summary>
        public PriorityEvent<GridSquare> OnSquareClicked;

        public void UpdateSize(Vector2Int size)
        {
            UpdateSize(size.x, size.y);
        }

        public void UpdateSize(int x, int y)
        {
            if(Grid != null) Grid.DestroyImmediate();
            foreach(GridSquare gridSquare in GetComponentsInChildren<GridSquare>())
            {
                DestroyImmediate(gridSquare.gameObject);
            }
            InstantiateGrid(x, y);
        }

        /// <summary>
        /// Invoked when any GridSquare on the BattleGrid is hovered over.
        /// Supplies the hovered GridSquare as an argument.
        /// </summary>
        public Action<GridSquare> OnSquareHovered;

        /// <summary>
        /// The BattleGrid this BattleGridManager uses to track entities and their spatial relationships.
        /// </summary>
        public BattleGrid Grid { get; private set; }

        private void Awake()
        {
            Instance = this;
            _entities = new List<Entity>();
            OnSquareClicked = new PriorityEvent<GridSquare>();
        }

        public void InstantiateGrid(Vector2Int size)
        {
            InstantiateGrid(size.x, size.y);
        }

        public void InstantiateGrid(int width, int height)
        {
            Grid = new BattleGrid(width, height);

            // populate the grid with squares
            for (int x = 0; x < Grid.squares.GetLength(0); x++)
            {
                for (int y = 0; y < Grid.squares.GetLength(1); y++)
                {
                    GameObject square = Instantiate(squarePrefab, transform);
                    square.transform.localPosition = new Vector3(x, 0, y);

                    GridSquare gridSquare = square.GetComponent<GridSquare>();

                    Grid[x, y] = square.GetComponent<GridSquare>();

                    gridSquare.OnClick += HandleClick;
                }
            }
        }

        private void HandleClick(GridSquare gridSquare)
        {
            OnSquareClicked?.Invoke(gridSquare);
        }

        /// <summary>
        /// Adds the given entity to the BattleGrid at the given location.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        /// <param name="x">X position the Entity should be placed at.</param>
        /// <param name="y">Y position the Entity should be placed at.</param>
        public void AddEntity(Entity entity, int x, int y)
        {
            _entities.Add(entity);
            entity.OnRemovedFromPlay += RemoveEntity;

            entity.Square = Grid[x, y];
        }

        /// <summary>
        /// Removes an Entity from the BattleGrid.
        /// </summary>
        /// <param name="entity">The Entity to remove.</param>
        private void RemoveEntity(Entity entity)
        {
            _entities.Remove(entity);
            entity.Square = null;
        }
    }
}
