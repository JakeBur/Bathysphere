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
                Enemy enemy = gridSquare.Entities.Find(entity => entity is Enemy) as Enemy;

                if (enemy)
                {
                    new StatusEffect.Staggered(enemy, 1);
                    enemy.TakeDamage(1);
                }
            }

            public override bool CanApplyToSquare(GridSquare gridSquare)
            {
                return base.CanApplyToSquare(gridSquare) && CanTargetSquare(gridSquare) && gridSquare.Entities.Find(entity => entity is Enemy) as Enemy != null;
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
                        Highlighter.Instance.playerAttackHighlights.Highlight(targetSquare);
                    }
                    else
                    {
                        Highlighter.Instance.playerAttackGreyoutHighlights.Highlight(targetSquare);
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