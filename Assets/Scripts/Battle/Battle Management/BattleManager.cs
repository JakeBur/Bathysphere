using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public class BattleManager : MonoBehaviour
    {
        public static BattleManager Instance;

        public Encounter encounter;

        public List<Entity> entities;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            /*
            BattleGridManager.Instance.AddEntity(entities[0], 0, 0);
            BattleGridManager.Instance.AddEntity(entities[1], 4, 4);
            BattleGridManager.Instance.AddEntity(entities[2], 1, 4);

            TurnManager.Instance.AddTurnOrderEntry(entities[0] as ITurnOrderEntry);
            TurnManager.Instance.AddTurnOrderEntry(entities[1] as ITurnOrderEntry);
            TurnManager.Instance.AddTurnOrderEntry(entities[2] as ITurnOrderEntry);
            TurnManager.Instance.StartTurnOrder();*/
            BattleGridManager.Instance.InstantiateGrid(encounter.gridSize);

            foreach(EncounterEntity encounterEntity in encounter.entities)
            {
                GameObject createdEntityObject = Instantiate(encounterEntity.prefab);
                Entity entity = createdEntityObject.GetComponent<Entity>();
                entities.Add(entity);
                entity.OnRemovedFromPlay += HandleEntityRemovedFromPlay;
                entity.Square = BattleGridManager.Instance.Grid[encounterEntity.position.x, encounterEntity.position.y];
                TurnManager.Instance.AddTurnOrderEntry(entity as ITurnOrderEntry);
            }

            TurnManager.Instance.StartTurnOrder();
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