using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Battle
{
    /// <summary>
    /// A GridSquare a GameObject that contains Entities and has a position on the BattleGrid.
    /// </summary>
    public class GridSquare : MonoBehaviour, IClickable
    {
        /// <summary>
        /// Invoked when this GridSquare is clicked on.
        /// Supplies this GridSquare as an argument.
        /// </summary>
        public Action<GridSquare> OnClick;

        /// <summary>
        /// Reference to the BattleGrid that contains this GridSquare.
        /// </summary>
        public BattleGrid BattleGrid { get; private set; }

        /// <summary>
        /// List of Entities currently contained in this GridSquare.
        /// </summary>
        public List<Entity> Entities { get; private set; }

        /// <summary>
        /// The position on the BattleGrid of this GridSquare.
        /// </summary>
        [HideInInspector]
        public Vector2Int Position { get; private set; }

        public int X { get => Position.x; }
        public int Y { get => Position.y; }

        private void Awake()
        {
            Entities = new List<Entity>();
        }

        public void Click()
        {
            OnClick?.Invoke(this);
        }

        /// <summary>
        /// Adds an Entity to this GridSquare.
        /// </summary>
        /// <param name="entity">The Entity to add.</param>
        public void AddEntity(Entity entity)
        {
            Entities.Add(entity);
        }

        /// <summary>
        /// Removes an Entity from this GridSquare.
        /// </summary>
        /// <param name="entity">The Entity to remove.</param>
        public void RemoveEntity(Entity entity)
        {
            if(Entities.Contains(entity))
            {
                Entities.Remove(entity);
            }
            else
            {
                Debug.LogError($"Unable to remove entity: {entity}, it is already not in the list.");
            }
        }

        /// <summary>
        /// Binds this GridSquare to a specific position on the given grid.
        /// </summary>
        /// <param name="grid">The BattleGrid to bind to.</param>
        /// <param name="x">Horizontal position.</param>
        /// <param name="y">Vertical position.</param>
        public void BindToGrid(BattleGrid grid, int x, int y)
        {
            this.BattleGrid = grid;
            Position = new Vector2Int(x, y);
        }
    }
}