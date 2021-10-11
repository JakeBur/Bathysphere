using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Battle
{
    public class BattleManager : MonoBehaviour
    {
        public static BattleManager Instance;

        private Encounter _encounter;

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
            
        }

        public void StartEncounter(Encounter encounter)
        {
            if(encounter == null)
            {
                Debug.LogError("Ecounter Error: chosen encounter was null");
                return;
            }

            _encounter = encounter;

            BattleGridManager.Instance.InstantiateGrid(encounter.gridSize);

            foreach (EncounterEntity encounterEntity in encounter.Entities)//.Select(container => container.encounterEntity)
            {
                InstantiateEntity(encounterEntity);
            }

            TurnManager.Instance.StartTurnOrder();
        }

        public static Entity InstantiateEntity(GameObject prefab, Vector2Int position)
        {
            return InstantiateEntity(prefab, position.x, position.y);
        }

        public static Entity InstantiateEntity(GameObject prefab, int x, int y)
        {
            GameObject createdEntityObject = Instantiate(prefab);

            Entity entity = createdEntityObject.GetComponent<Entity>();

            if(entity == null)
            {
                Destroy(createdEntityObject);
                Debug.Log("InstantiateEntity Error: provided prefab is not an Entity. Aborting instantiation");
                return null;
            }

            Instance.entities.Add(entity);
            entity.OnRemovedFromPlay += Instance.HandleEntityRemovedFromPlay;

            entity.Square = BattleGridManager.Instance.Grid[x, y];
            TurnManager.Instance.AddTurnOrderEntry(entity as ITurnOrderEntry);

            return entity;
        }

        public static Entity InstantiateEntity(EncounterEntity encounterEntity)
        {
            return InstantiateEntity(encounterEntity.EntityData.prefab, encounterEntity.Position);
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