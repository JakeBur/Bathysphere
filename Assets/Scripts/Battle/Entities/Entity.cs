using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Battle
{
    public class Entity : MonoBehaviour, ISelectable
    {
        public Action<Entity> OnRemovedFromPlay;

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

        private GridSquare _gridSquare;

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
