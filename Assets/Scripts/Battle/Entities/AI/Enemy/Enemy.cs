using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Battle
{
    /// <summary>
    /// An Enemy is an AI controled Combatant hostile to the player.
    /// </summary>
    public partial class Enemy : AICombatant
    {
        public List<PlayerCharacter> targets;

        public GameObject pathMarkerPrefab;
        public GameObject targetMarkerPrefab;

        protected new void Awake()
        {
            base.Awake();

            
        }

        private void Start()
        {
            targets = GameObject.FindObjectsOfType<PlayerCharacter>().ToList();
        }

        public override void Deselect()
        {
        }

        public override void Select()
        {
        }

        public override List<PlayerAction> GetAvailableMenuActions()
        {
            return new List<PlayerAction>();
        }

        public override List<PlayerAction> GetAvailableComboActions(Entity entity)
        {
            throw new System.NotImplementedException();
        }

        protected override void InitializeGoalActions()
        {
            _goalActions.Add(new Attack(this, 2));
        }

        protected override void InitializeSecondaryActions()
        {
            _availableActions.Add(new MoveAdjacentToSquare(this, 1));
        }
    }
}

