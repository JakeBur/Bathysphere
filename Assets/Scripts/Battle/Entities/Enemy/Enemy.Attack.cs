using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public partial class Enemy
    {
        public class Attack : EnemyAction
        {
            public Attack(Combatant combatant, int cost) : base(combatant, cost)
            {

            }

            public override void Apply(GridSquare targetSquare)
            {
                Scheduler.Schedule(() =>
                {
                    (targetSquare.Entities.Find(entity => entity is PlayerCharacter) as PlayerCharacter).TakeDamage(1);
                    _enemy.EndTurn();

                }, 3f);
            }

            public override bool CanApplyToSquare(GridSquare targetSquare)
            {
                return base.CanApplyToSquare(targetSquare) && targetSquare.Entities.Find(entity => entity is PlayerCharacter) && GridSquare.Distance(_enemy.Square, targetSquare) == 1;
            }

            public override bool CanTargetSquare(GridSquare targetSquare)
            {
                return true;
            }

            public override List<GridSquare> FindThreatenedSquaresAtTarget(GridSquare targetSquare)
            {
                return new List<GridSquare>();
            }
        }
    }
}