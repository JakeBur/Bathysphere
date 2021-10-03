using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public class BattleManager : MonoBehaviour
    {
        public List<Entity> entities;

        private void Start()
        {
            BattleGridManager.Instance.AddEntity(entities[0], 0, 0);
            BattleGridManager.Instance.AddEntity(entities[1], 4, 4);
            BattleGridManager.Instance.AddEntity(entities[2], 1, 4);

            entities.ForEach(entity => entity.OnRemovedFromPlay += HandleEntityRemovedFromPlay);
        }

        private void HandleEntityRemovedFromPlay(Entity entity)
        {
            entities.Remove(entity);

            if(new AllPlayersDefeated().Met(this))
            {
                Debug.Log("Defeat!");
            }

            if (new AllEnemiesDefeated().Met(this))
            {
                Debug.Log("Victory!");
            }
        }
    }
}