using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public class Entity : MonoBehaviour, ISelectable
    {
        public GridSquare Square
        {
            get
            {
                return _gridSquare;
            }

            set
            {
                if(_gridSquare) _gridSquare.RemoveEntity(this);
                value.AddEntity(this);
                _gridSquare = value;

                transform.position = _gridSquare.transform.position;
            }
        }

        private GridSquare _gridSquare;

        public void Select()
        {

        }

        public void Deselect()
        {

        }

        public GameObject GetGameObject()
        {
            return gameObject;
        }

        public virtual bool IsSelectable()
        {
            return true;
        }
    }
}
