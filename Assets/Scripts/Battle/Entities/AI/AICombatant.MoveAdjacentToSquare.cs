using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Battle
{
    public partial class AICombatant
    {
        /// <summary>
        /// Moves the AI to any of the four adjacent squares to the given target square.
        /// </summary>
        public class MoveAdjacentToSquare : AICombatantAction
        {
            protected GridSquare TargetSquare
            {
                get
                {
                    return _targetSquare;
                }

                set
                {
                    if (_targetSquare != value)
                    {
                        _targetSquare = value;
                        _calculatedPath = BattleGridManager.Instance.Grid.CalculatePath(_aiCombatant.Square, _targetSquare);
                        _calculatedPath.RemoveAt(_calculatedPath.Count - 1);
                    }
                }
            }

            protected GridSquare _targetSquare;
            protected List<GridSquare> _calculatedPath;

            public MoveAdjacentToSquare(AICombatant aiCombatant, int cost) : base(aiCombatant, cost)
            {
                _satisfiablePredicates.Add(typeof(AIActionPredicate.SquareInRange));
            }

            public override int CalculateCost(GridSquare targetSquare)
            {
                TargetSquare = targetSquare;

                return base.CalculateCost(targetSquare) * Math.Min(_calculatedPath.Count, _aiCombatant.actionPoints.CurrentPoints);
            }

            public override void Apply(GridSquare targetSquare)
            {
                TargetSquare = targetSquare;

                List<GridSquare> path = new List<GridSquare>(_calculatedPath);
                int actionPoints = _aiCombatant.actionPoints.CurrentPoints;

                for (int i = path.Count - 1; i > actionPoints; i--)
                {
                    path.RemoveAt(i);
                }
                
                if (path.Count > 0)
                {
                    _aiCombatant.Square = path[path.Count - 1];
                    _aiCombatant.actionPoints.TryConsumePoints(path.Count);

                    Highlighter.Instance.pathHighlights.Highlight(path);
                }
            }

            public override bool CanApplyToSquare(GridSquare targetSquare)
            {
                TargetSquare = targetSquare;
                
                return _aiCombatant.actionPoints.CurrentPoints > 0 && _calculatedPath != null && _aiCombatant._statusEffects.Find(effect => effect is StatusEffect.Pinned) == null;
            }

            public override List<GridSquare> FindThreatenedSquaresAtTarget(GridSquare targetSquare)
            {
                return new List<GridSquare>();
            }

            public override bool CanTargetSquare(GridSquare targetSquare)
            {
                return true;
            }
        }
    }
}