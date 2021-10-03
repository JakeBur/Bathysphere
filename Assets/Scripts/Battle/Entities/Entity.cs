using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Battle
{
    /// <summary>
    /// An Entity is a GameObject that can be placed on a BattleGrid at a specific GridSquare
    /// </summary>
    public class Entity : MonoBehaviour, ISelectable
    {
        /// <summary>
        /// Action to invoke when this Entity is removed from play.
        /// Supplies this Entity as an argument.
        /// </summary>
        public Action<Entity> OnRemovedFromPlay;

        /// <summary>
        /// The square that this Entity resides in.
        /// Set to move this Entity to a different square.
        /// </summary>
        public GridSquare Square
        {
            get
            {
                return _gridSquare;
            }

            set
            {
                if(_gridSquare) _gridSquare.RemoveEntity(this);
                _gridSquare = value;

                if(_gridSquare != null)
                {
                    _gridSquare.AddEntity(this);
                    transform.position = _gridSquare.transform.position;
                }
            }
        }

        /// <summary>
        /// Private reference to the GridSquare this Entity is occupying.
        /// </summary>
        private GridSquare _gridSquare;

        /// <summary>
        /// Call when this Entity is removed from play for any reason.
        /// (For instance, if a child class of Entity can Die, it should call RemoveFromPlay() as part of the process of dying).
        /// </summary>
        protected void RemoveFromPlay()
        {
            OnRemovedFromPlay?.Invoke(this);
        }

        public void Select()
        {

        }

        public void Deselect()
        {

        }

        public virtual bool IsSelectable()
        {
            return true;
        }
    }
}
