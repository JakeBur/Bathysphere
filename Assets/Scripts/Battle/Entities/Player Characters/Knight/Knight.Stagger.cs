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
        protected class Stagger : PlayerAction
        {
            private int _range;

            private Knight knight;

            public Stagger(Knight knight, int cost, int range) : base(knight, cost)
            {
                this.knight = knight;
                _range = range;
            }

            public override void Apply(GridSquare gridSquare)
            {
                foreach (GridSquare affectedSquare in FindThreatenedSquaresAtTarget(gridSquare))
                {
                    affectedSquare.Entities.Where(entity => entity is Enemy).ToList().ForEach(enemy => enemy.GetComponent<IDamageable>().TakeDamage(2));
                }
            }

            public override bool CanApplyToSquare(GridSquare gridSquare)
            {
                bool atLeastOneEnemy = false;

                foreach (GridSquare affectedSquare in FindThreatenedSquaresAtTarget(gridSquare))
                {
                    if (affectedSquare.Entities.Find(entity => entity is Enemy))
                    {
                        atLeastOneEnemy = true;
                        break;
                    }
                }

                return base.CanApplyToSquare(gridSquare) && CanTargetSquare(gridSquare) && atLeastOneEnemy;
            }

            public override bool CanTargetSquare(GridSquare gridSquare)
            {
                return GridSquare.Distance(knight.Square, gridSquare) <= _range && gridSquare != knight.Square;
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

                GridDirection forwardDirection = gridSquare.Grid.GetDirectionFromTo(knight.Square, gridSquare, GridDirection.East);

                squares.Add(gridSquare);

                GridSquare adjacentSquare = gridSquare.GetAdjacent(forwardDirection.RotateClockwise());
                if (adjacentSquare) squares.Add(adjacentSquare);

                adjacentSquare = gridSquare.GetAdjacent(forwardDirection.RotateCounterclockwise());
                if (adjacentSquare) squares.Add(adjacentSquare);

                return squares;
            }
        }
    }
}