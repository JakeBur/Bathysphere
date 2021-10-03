using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    /// <summary>
    /// A PlayerCharacter is a player controled combatant.
    /// </summary>
    public class PlayerCharacter : Combatant
    {
        /// <summary>
        /// List of actions that should appear in the contextual action menu when this PlayerCharacter is selected.
        /// </summary>
        public List<IBattleAction> menuActions;

        /// <summary>
        /// The currently primed action, to be supplied when GetPrimedActions() is called.
        /// </summary>
        private IBattleAction _primedAction;

        private void Awake()
        {
            menuActions = new List<IBattleAction>();

            menuActions.Add(new TestAttack(this));
        }

        private void Start()
        {
            InputManager.Instance.OnCancelPressed += Cancel;
        }

        public override List<IBattleAction> GetPrimedActions()
        {
            List<IBattleAction> actions = new List<IBattleAction>();
            if(this.TurnActive())
            {
                if(_primedAction != null) actions.Add(_primedAction);
                actions.Add(new Move(this));
            }

            return actions;
        }

        public override void StartTurn()
        {
            Debug.Log("Starting turn for player: " + gameObject);
        }

        /// <summary>
        /// Primes the given IBattleAction if its contained in the character's menu actions.
        /// </summary>
        /// <param name="action"></param>
        public void PrimeMenuAction(IBattleAction action)
        {
            if(menuActions.Contains(action))
            {
                _primedAction = action;
            }
        }

        /// <summary>
        /// Cancels the currently primed action, if any.
        /// </summary>
        private void Cancel()
        {
            _primedAction = null;
        }

        private class Move : IBattleAction
        {
            private PlayerCharacter player;

            public Move(PlayerCharacter player)
            {
                this.player = player;
            }

            public void Apply(GridSquare gridSquare)
            {
                player.Square = gridSquare;
                player.OnTurnEnd?.Invoke();
            }

            public bool CanApplyToSquare(GridSquare gridSquare)
            {
                return gridSquare.Entities.Count == 0;
            }
        }

        private class TestAttack : IBattleAction
        {
            private PlayerCharacter player;

            public TestAttack(PlayerCharacter player)
            {
                this.player = player;
            }

            public void Apply(GridSquare gridSquare)
            {
                gridSquare.Entities.Find(entity => entity is IDamageable).GetComponent<IDamageable>().TakeDamage(1);
                player.OnTurnEnd?.Invoke();
            }

            public bool CanApplyToSquare(GridSquare gridSquare)
            {
                Entity targetEntity = gridSquare.Entities.Find(entity => entity is IDamageable);

                return targetEntity is Enemy;
            }
        }
    }
}



