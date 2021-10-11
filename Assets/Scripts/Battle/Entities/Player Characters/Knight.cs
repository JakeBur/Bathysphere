using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Battle
{
    public class Knight : PlayerCharacter
    {
        public override List<IBattleAction> GetAvailableMenuActions()
        {
            throw new System.NotImplementedException();
        }

        protected override void InitializeMenuActions()
        {
            menuActions.Clear();

            menuActions.Add(new Slash(this, 2));
        }

        protected class Slash : PlayerAction
        {
            private Knight knight;

            public Slash(Knight knight, int cost) : base(knight, cost)
            {
                this.knight = knight;
            }

            public override void Apply(GridSquare gridSquare)
            {
                foreach(GridSquare affectedSquare in FindAffectedSquares(gridSquare))
                {
                    affectedSquare.Entities.Where(entity => entity is Enemy).ToList().ForEach(enemy => enemy.GetComponent<IDamageable>().TakeDamage(2));
                }
            }

            public override bool CanApplyToSquare(GridSquare gridSquare)
            {
                bool atLeastOneEnemy = false;

                foreach (GridSquare affectedSquare in FindAffectedSquares(gridSquare))
                {
                    if(affectedSquare.Entities.Find(entity => entity is Enemy))
                    {
                        atLeastOneEnemy = true;
                        break;
                    }
                }

                return base.CanApplyToSquare(gridSquare) && CanTargetSquare(gridSquare) && atLeastOneEnemy;
            }

            public override bool CanTargetSquare(GridSquare gridSquare)
            {
                return GridSquare.Distance(knight.Square, gridSquare) <= 2 && gridSquare != knight.Square;
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
                    if(CanApplyToSquare(targetSquare))
                    {
                        Highlighter.Instance.playerAttackHighlights.Highlight(FindAffectedSquares(targetSquare));
                    }
                    else
                    {
                        Highlighter.Instance.playerAttackGreyoutHighlights.Highlight(FindAffectedSquares(targetSquare));
                    }
                }
            }

            private List<GridSquare> FindAffectedSquares(GridSquare targetSquare)
            {
                List<GridSquare> squares = new List<GridSquare>();

                GridDirection forwardDirection = targetSquare.Grid.GetDirectionFromTo(knight.Square, targetSquare, GridDirection.East);

                squares.Add(targetSquare);

                GridSquare adjacentSquare = targetSquare.GetAdjacent(forwardDirection.RotateClockwise());
                if(adjacentSquare) squares.Add(adjacentSquare);

                adjacentSquare = targetSquare.GetAdjacent(forwardDirection.RotateCounterclockwise());
                if (adjacentSquare) squares.Add(adjacentSquare);

                return squares;
            }

            public override void EndPreview()
            {
                //de-highlight all threatened squares
                Highlighter.Instance.playerAttackGreyoutHighlights.Clear();
                Highlighter.Instance.playerAttackHighlights.Clear();
                Highlighter.Instance.moveHighlights.Clear();
            }

            public override List<GridSquare> FindThreatenedSquares()
            {
                return new List<GridSquare>();
            }
        }
    }
}