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
        protected class Protect : KnightAction
        {
            private int _range;
            //public override bool IsInstant() => false;

            public Protect(Knight knight, int cost, int range) : base(knight, cost)
            {
                _range = range;
            }

            public override void Apply(GridSquare targetSquare)
            {
                Debug.Log("starting protect");
                targetSquare.Entities.Find(entity => entity is PlayerCharacter).AddActionInterceptor(new ProtectInterceptor(_knight));

                _knight.EndTurn();
            }

            public override bool CanApplyToSquare(GridSquare targetSquare)
            {
                return base.CanApplyToSquare(targetSquare) && CanTargetSquare(targetSquare);
            }

            public override bool CanTargetSquare(GridSquare targetSquare)
            {
                return GridSquare.Distance(_knight.Square, targetSquare) <= _range && targetSquare.Entities.Find(entity => entity is PlayerCharacter) != null;
            }

            public override void BeginPreview()
            {
                Highlighter.Instance.moveHighlights.Clear();
                Highlighter.Instance.moveHighlights.Highlight(this.FindTargetableSquares());
            }

            public override void UpdatePreview(GridSquare targetSquare)
            {
                Highlighter.Instance.playerBuffHighlights.Clear();
                if (CanApplyToSquare(targetSquare))
                {
                    Highlighter.Instance.playerBuffHighlights.Highlight(targetSquare);
                }
            }

            public override void EndPreview()
            {
                Highlighter.Instance.playerBuffHighlights.Clear();
                Highlighter.Instance.moveHighlights.Clear();
            }

            public override List<GridSquare> FindThreatenedSquaresAtTarget(GridSquare targetSquare)
            {
                return new List<GridSquare>();
            }
        }
    }
}