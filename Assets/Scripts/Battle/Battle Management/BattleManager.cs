using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Battle
{
    /// <summary>
    /// Creates and manages the overall state of the battle, including the management of Entities and victory/defeat conditions.
    /// </summary>
    public class BattleManager : MonoBehaviour
    {
        /// <summary>
        /// Static reference to self.
        /// </summary>
        public static BattleManager Instance;

        /// <summary>
        /// Reference to the encounter that this battle started using.
        /// </summary>
        private Encounter _encounter;

        /// <summary>
        /// List of all entities in this battle.
        /// </summary>
        public List<Entity> entities;

        private void Awake()
        {
            Instance = this;
        }

        /// <summary>
        /// Starts the battle using the given Encounter as a guide for grid layout and entities.
        /// </summary>
        /// <param name="encounter">The encounter to use in the generation of this battle.</param>
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

            TurnOrder.StartTurnOrder();
        }

        /// <summary>
        /// Instantiates a new Entity at the given position using the given prefab.
        /// </summary>
        /// <param name="prefab">The prefab to instantiate.</param>
        /// <param name="position">The position to create the Entity at.</param>
        /// <returns>The created entity, or null if the instantiation process failed.</returns>
        public static Entity InstantiateEntity(GameObject prefab, Vector2Int position)
        {
            return InstantiateEntity(prefab, position.x, position.y);
        }

        /// <summary>
        /// Instantiates a new Entity at the given position using the given prefab.
        /// </summary>
        /// <param name="prefab">The prefab to instantiate.</param>
        /// <param name="x">The X position to create the Entity at.</param>
        /// <param name="y">The Y position to create the Entity at.</param>
        /// <returns>The created entity, or null if the instantiation process failed.</returns>
        public static Entity InstantiateEntity(GameObject prefab, int x, int y)
        {
            if(!BattleGridManager.Instance.Grid.HasPosition(x, y))
            {
                Debug.LogError("InstantiateEntity Error: provided position is not on the grid. Aborting instantiation");
                return null;
            }

            GameObject createdEntityObject = Instantiate(prefab);

            Entity entity = createdEntityObject.GetComponent<Entity>();

            if(entity == null)
            {
                Destroy(createdEntityObject);
                Debug.LogError("InstantiateEntity Error: provided prefab is not an Entity. Aborting instantiation");
                return null;
            }

            Instance.entities.Add(entity);
            entity.OnRemovedFromPlay += Instance.HandleEntityRemovedFromPlay;

            entity.Square = BattleGridManager.Instance.Grid[x, y];
            TurnOrder.Add(entity as ITurnOrderEntry);

            entity.Initialize();

            return entity;
        }

        /// <summary>
        /// Instantiates an Entity using the information contained in the given EncounterEntity.
        /// </summary>
        /// <param name="encounterEntity">The EncounterEntity that will provide the prefab and position for the new Entity.</param>
        /// <returns>The created entity, or null if the instantiation process failed.</returns>
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