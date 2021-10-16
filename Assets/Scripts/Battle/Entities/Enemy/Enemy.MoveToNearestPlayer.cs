using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Battle
{
    public partial class Enemy
    {
        public class MoveToNearestPlayer : EnemyAction
        {
            protected PlayerCharacter _target;

            public MoveToNearestPlayer(Combatant combatant, int cost, PlayerCharacter target) : base(combatant, cost)
            {
                _target = target;
            }

            public override int CalculateCost(GridSquare targetSquare)
            {
                List<GridSquare> path = BattleGridManager.Instance.Grid.CalculatePath(_enemy.Square, targetSquare);

                return base.CalculateCost(targetSquare) * Math.Min(path.Count - 1, _enemy.actionPoints.CurrentPoints);
            }

            public override void Apply(GridSquare targetSquare)
            {
                List<GridSquare> path = BattleGridManager.Instance.Grid.CalculatePath(_enemy.Square, _target.Square);

                path.RemoveAt(path.Count - 1);

                for (int i = _enemy.actionPoints.CurrentPoints; i < path.Count; i++)
                {
                    path.RemoveAt(path.Count - 1);
                }

                if(path.Count > 0)
                {
                    _enemy.Square = path[path.Count - 1];
                    _enemy.actionPoints.TryConsumePoints(path.Count);
                }
            }

            public override bool CanApplyToSquare(GridSquare targetSquare)
            {
                return targetSquare == null && _enemy._statusEffects.Find(effect => effect is StatusEffect.Pinned) == null;
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