using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Battle
{
    public partial class Knight
    {
        [Serializable]
        protected class RecoverSword : KnightAction
        {
            [SerializeField]
            public GameObject _swordPrefab;

            public RecoverSword(Knight knight, int cost) : base(knight, cost)
            {
                IsInstant = true;
            }

            public override void Apply(GridSquare gridSquare)
            {
                KnightSword sword = _knight._knightSword;
                _knight._knightSword = null;
                _knight.InitializeMenuActions();
                _knight.InitializeComboActions();

                sword.RemoveFromPlay();
                Destroy(sword.gameObject);
            }

            public override void BeginPreview()
            {

            }

            public override bool CanTargetSquare(GridSquare gridSquare)
            {
                return false;
            }

            public override bool CanApplyToSquare(GridSquare gridSquare)
            {
                return base.CanApplyToSquare(gridSquare) && gridSquare == null && GridSquare.Distance(_player.Square, (_player as Knight)._knightSword.Square) == 1;
            }

            public override void EndPreview()
            {
            }

            public override List<GridSquare> FindThreatenedSquaresAtTarget(GridSquare gridSquare)
            {
                return new List<GridSquare>();
            }

            public override void UpdatePreview(GridSquare gridSquare)
            {

            }
        }
    }
}