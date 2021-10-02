using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public class PlayerCharacter : Combatant
    {
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
                return gridSquare.entities.Count == 0;
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
                gridSquare.entities.Find(entity => entity is IDamageable).GetComponent<IDamageable>().TakeDamage(1);
                player.OnTurnEnd?.Invoke();
            }

            public bool CanApplyToSquare(GridSquare gridSquare)
            {
                Entity targetEntity = gridSquare.entities.Find(entity => entity is IDamageable);

                return targetEntity is Enemy;
            }
        }
    }
}



