using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public partial class Enemy
    {
        public class Attack : EnemyAction
        {
            public Attack(AICombatant aiCombatant, int cost) : base(aiCombatant, cost)
            {
                _predicates.Add(new AIActionPredicate.SquareInRange(1));
            }

            public override void Apply(GridSquare targetSquare)
            {
                Scheduler.Schedule(() =>
                {
                    (targetSquare.Entities.Find(entity => entity is PlayerCharacter) as PlayerCharacter).TakeDamage(1);
                    _enemy.EndTurn();
                }, 1f);
            }

            public override bool CanApplyToSquare(GridSquare targetSquare)
            {
                return base.CanApplyToSquare(targetSquare) && targetSquare.Entities.Find(entity => entity is PlayerCharacter);
            }

            public override bool CanTargetSquare(GridSquare targetSquare)
            {
                return targetSquare.Entities.Find(entity => entity is PlayerCharacter) != null;
            }

            public override List<GridSquare> FindThreatenedSquaresAtTarget(GridSquare targetSquare)
            {
                return new List<GridSquare>();
            }
        }
    }
}