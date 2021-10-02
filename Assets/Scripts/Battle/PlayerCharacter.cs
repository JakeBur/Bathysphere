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

            actions.Add(new Attack());
            actions.Add(new Move(this));

            return actions;
        }

        public override void TakeDamage()
        {
            Debug.Log("Player took damage");
        }

        private class Move : IBattleAction
        {
            private Entity entity;

            public Move(Entity entity)
            {
                this.entity = entity;
            }

            public void Apply(GridSquare gridSquare)
            {
                entity.Square = gridSquare;
            }

            public bool CanApplyToSquare(GridSquare gridSquare)
            {
                //TODO
                return true;
            }
        }

        private class Attack : IBattleAction
        {
            public void Apply(GridSquare gridSquare)
            {
                gridSquare.entities.Find(entity => entity is IDamageable).GetComponent<IDamageable>().TakeDamage();
            }

            public bool CanApplyToSquare(GridSquare gridSquare)
            {
                return gridSquare.entities.Find(entity => entity is IDamageable) != null;
            }
        }
    }
}



