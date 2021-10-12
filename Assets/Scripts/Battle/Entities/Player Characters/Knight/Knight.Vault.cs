using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

namespace Battle
{
    public partial class Knight
    {
        [Serializable]
        protected class Vault : KnightAction
        {
            private int _range;
            //public override bool IsInstant() => false;

            public Vault(Knight knight, int cost, int range) : base(knight, cost)
            {
                _range = range;
            }

            public override void Apply(GridSquare gridSquare)
            {
                Enemy enemyOnSwordSquare = _knight._knightSword.Square.Entities.Find(entity => entity is Enemy) as Enemy;

                if (enemyOnSwordSquare)
                {
                    enemyOnSwordSquare.TakeDamage(1);
                }

                (gridSquare.Entities.Find(entity => entity is Enemy) as Enemy).TakeDamage(2);

                GridDirection direction = BattleGridManager.Instance.Grid.GetDirectionFromTo(_knight.Square, _knight._knightSword.Square);

                _knight.Square = gridSquare.GetAdjacent(direction);
            }

            public override bool CanApplyToSquare(GridSquare gridSquare)
            {
                if(!(base.CanApplyToSquare(gridSquare) && CanTargetSquare(gridSquare)))
                {
                    return false;
                }

                // make sure the spot behind the enemy is available
                GridDirection direction = BattleGridManager.Instance.Grid.GetDirectionFromTo(_knight.Square, _knight._knightSword.Square);
                GridSquare landingSquare = gridSquare.GetAdjacent(direction);
                if (landingSquare == null || landingSquare.Entities.Count > 0)
                {
                    return false;
                }

                return gridSquare.Entities.Find(entity => entity is Enemy) != null;
            }

            public override bool CanTargetSquare(GridSquare gridSquare)
            {

                if(GridSquare.Distance(_knight.Square, _knight._knightSword.Square) > _range || GridSquare.Distance(_knight._knightSword.Square, gridSquare) > _range)
                {
                    return false;
                }

                if(gridSquare == _knight._knightSword.Square)
                {
                    return false;
                }

                if(BattleGridManager.Instance.Grid.GetDirectionFromTo(_knight.Square, _knight._knightSword.Square) !=
                    BattleGridManager.Instance.Grid.GetDirectionFromTo(_knight._knightSword.Square, gridSquare))
                {
                    return false;
                }

                return gridSquare != _knight.Square;
            }

            public override void BeginPreview()
            {
                //highlight all targetable squares
                Highlighter.Instance.moveHighlights.Highlight(this.FindTargetableSquares());
            }

            public override void UpdatePreview(GridSquare targetSquare)
            {
                Highlighter.Instance.playerAttackHighlights.Clear();
                Highlighter.Instance.playerAttackGreyoutHighlights.Clear();

                if (CanTargetSquare(targetSquare))
                {
                    if (CanApplyToSquare(targetSquare))
                    {
                        Highlighter.Instance.playerAttackHighlights.Highlight(FindThreatenedSquaresAtTarget(targetSquare));
                    }
                    else
                    {
                        Highlighter.Instance.playerAttackGreyoutHighlights.Highlight(FindThreatenedSquaresAtTarget(targetSquare));
                    }
                }
            }

            public override void EndPreview()
            {
                //de-highlight all threatened squares
                Highlighter.Instance.playerAttackGreyoutHighlights.Clear();
                Highlighter.Instance.playerAttackHighlights.Clear();
                Highlighter.Instance.moveHighlights.Clear();
            }

            public override List<GridSquare> FindThreatenedSquaresAtTarget(GridSquare gridSquare)
            {
                List<GridSquare> squares = new List<GridSquare>();

                squares.Add(gridSquare);

                return squares;
            }
        }
    }
}