using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Battle
{
    public class GridSquare : MonoBehaviour, IClickable
    {
        public Action<GridSquare> OnClick;
        public BattleGrid battleGrid;

        public List<Entity> entities;

        [HideInInspector]
        public Vector2Int position;

        public int X { get => position.x; }
        public int Y { get => position.y; }

        public void Click()
        {
            OnClick?.Invoke(this);
        }

        public void AddEntity(Entity entity)
        {
            entities.Add(entity);
        }

        public void RemoveEntity(Entity entity)
        {
            if(entities.Contains(entity))
            {
                entities.Remove(entity);
            }
            else
            {
                Debug.LogError($"Unable to remove entity: {entity}, it is already not in the list.");
            }
        }

        public void SetPosition(int x, int y)
        {
            position = new Vector2Int(x, y);
        }
    }
}