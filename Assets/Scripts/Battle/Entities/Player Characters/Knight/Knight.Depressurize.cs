using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Battle
{
    public partial class Knight
    {
        [Serializable]
        protected class Depressurize : KnightAction
        {
            [SerializeField]
            public GameObject _swordPrefab;

            private int _damage;

            public Depressurize(Knight knight, int cost, int damage) : base(knight, cost)
            {
                _damage = damage;
                IsInstant = true;
            }

            public override void Apply(GridSquare gridSquare)
            {
                KnightSword sword = _knight._knightSword;

                (sword.Square.Entities.Find(entity => entity is Enemy) as Enemy).TakeDamage(_damage);

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
                if(!base.CanApplyToSquare(gridSquare))
                {
                    return false;
                }

                if(_knight._knightSword.Square.Entities.Find(entity => entity is Enemy) == null)
                {
                    return false;
                }

                return gridSquare == null && GridSquare.Distance(_knight.Square, _knight._knightSword.Square) == 1;
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