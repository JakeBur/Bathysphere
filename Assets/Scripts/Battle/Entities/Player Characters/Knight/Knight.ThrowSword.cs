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
            protected int _range;

            [SerializeField]
            public GameObject _swordPrefab;

            public ThrowSword(Knight knight, int cost, int range, GameObject swordPrefab) : base(knight, cost)
            {
                _swordPrefab = swordPrefab;
                _range = range;
            }

            //public override bool IsInstant() => true;

            public override void Apply(GridSquare gridSquare)
            {
                _knight._knightSword = BattleManager.InstantiateEntity(_swordPrefab, gridSquare.Position) as KnightSword;
                _knight.InitializeMenuActions();
                _knight.InitializeComboActions();

                // deal 2 damage to enemy at center
                Enemy enemy = gridSquare.Entities.Find(entity => entity is Enemy) as Enemy;
                if (enemy)
                {
                    enemy.TakeDamage(2);
                }

                for (int i = 0; i < 4; i++)
                {
                    enemy = gridSquare.GetAdjacent(GridDirection.North + i)?.Entities.Find(entity => entity is Enemy) as Enemy;

                    if(enemy)
                    {
                        enemy.TakeDamage(1);
                    }
                }
            }

            public override void BeginPreview()
            {
                Highlighter.Instance.moveHighlights.Clear();
                Highlighter.Instance.moveHighlights.Highlight(this.FindTargetableSquares());
            }

            public override bool CanTargetSquare(GridSquare gridSquare)
            {
                bool validTarget = gridSquare.Entities.Count == 0 || gridSquare.Entities.Find(entity => entity is Enemy) as Enemy != null;

                return validTarget && GridSquare.Distance(_player.Square, gridSquare) <= _range;
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
                for (int i = 0; i < 4; i++)
                {
                    GridSquare square = gridSquare.GetAdjacent(GridDirection.North + i);
                    if(square) squares.Add(square);
                }

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