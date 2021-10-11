using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Battle
{
    public partial class Knight
    {
        [Serializable]
        protected class ThrowSword : KnightAction
        {
            [SerializeField]
            private int _range;

            [SerializeField]
            public GameObject _swordPrefab;

            public ThrowSword(Knight knight, int cost, int range) : base(knight, cost)
            {
                _range = range;
            }

            //public override bool IsInstant() => true;

            public override void Apply(GridSquare gridSquare)
            {
                (_player as Knight)._knightSword = BattleManager.InstantiateEntity(_swordPrefab, gridSquare.Position) as KnightSword;
                (_player as Knight).InitializeMenuActions();
                (_player as Knight).InitializeComboActions();
            }

            public override void BeginPreview()
            {
                Highlighter.Instance.moveHighlights.Clear();
                Highlighter.Instance.moveHighlights.Highlight(this.FindTargetableSquares());
            }

            public override bool CanTargetSquare(GridSquare gridSquare)
            {
                return gridSquare.Entities.Count == 0 && GridSquare.Distance(_player.Square, gridSquare) <= _range;
            }

            public override void EndPreview()
            {
                Highlighter.Instance.playerAttackHighlights.Clear();
                Highlighter.Instance.moveHighlights.Clear();
            }

            public override List<GridSquare> FindThreatenedSquaresAtTarget(GridSquare gridSquare)
            {
                //throw new NotImplementedException();
                List<GridSquare> squares = new List<GridSquare>();
                squares.Add(gridSquare);
                return squares;
            }

            public override void UpdatePreview(GridSquare gridSquare)
            {
                Highlighter.Instance.playerAttackHighlights.Clear();

                if(CanTargetSquare(gridSquare))
                {
                    Highlighter.Instance.playerAttackHighlights.Highlight(FindThreatenedSquaresAtTarget(gridSquare));
                }
            }
        }
    }
}