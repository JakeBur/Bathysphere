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
        protected class Counterattack : KnightAction
        {
            private int _damage;

            public Counterattack(Knight knight, int cost, int damage) : base(knight, cost)
            {
                _damage = damage;
            }

            public override void Apply(GridSquare gridSquare)
            {
                Enemy enemy = gridSquare.Entities.Find(entity => entity is Enemy) as Enemy;
                
                enemy.TakeDamage(2);
                enemy.EndTurn();
            }

            public override bool CanApplyToSquare(GridSquare gridSquare)
            {
                return gridSquare.Entities.Find(entity => entity is Enemy);
            }

            public override bool CanTargetSquare(GridSquare gridSquare)
            {
                return false;
            }

            public override void BeginPreview()
            {
            }

            public override void UpdatePreview(GridSquare targetSquare)
            {
               
            }

            public override void EndPreview()
            {
                
            }

            public override List<GridSquare> FindThreatenedSquaresAtTarget(GridSquare gridSquare)
            {
                return new List<GridSquare>();
            }
        }
    }
}