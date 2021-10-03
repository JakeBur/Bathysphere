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
        public List<IBattleAction> menuActions;

        private void Awake()
        {
            menuActions = new List<IBattleAction>();

            menuActions.Add(new Attack(this));
            menuActions.Add(new Move(this));
        }

        public override List<IBattleAction> GetPrimedActions()
        {
            List<IBattleAction> actions = new List<IBattleAction>();
            if(this.TurnActive())
            {
                actions.Add(new Attack(this));
                actions.Add(new Move(this));
            }

            return actions;
        }

        public override void StartTurn()
        {
            Debug.Log("Starting turn for player: " + gameObject);
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

        private class Attack : IBattleAction
        {
            private PlayerCharacter player;

            public Attack(PlayerCharacter player)
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



