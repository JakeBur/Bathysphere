using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public class Knight : PlayerCharacter
    {
        

        

        protected override void InitializeMenuActions()
        {
            menuActions.Clear();

            menuActions.Add(new Slash(this));
        }

        protected class Slash : PlayerAction
        {
            private Knight player;

            public Slash(Knight player)
            {
                this.player = player;
            }

            public override void Apply(GridSquare gridSquare)
            {
                gridSquare.Entities.Find(entity => entity is IDamageable).GetComponent<IDamageable>().TakeDamage(1);
                player.OnTurnEnd?.Invoke();
            }

            public override bool CanApplyToSquare(GridSquare gridSquare)
            {
                Entity targetEntity = gridSquare.Entities.Find(entity => entity is IDamageable);

                return targetEntity is Enemy;
            }

            public override void BeginPreview()
            {
                //highlight all threatened squares
            }
            
            public override void UpdatePreview(GridSquare targetSquare)
            {
                if(CanApplyToSquare(targetSquare))
                {
                    // highlight aoe
                }
            }
            
            public override void EndPreview()
            {
                //he-highlight all threatened squares
            }

            public override List<GridSquare> FindThreatenedSquares()
            {
                return new List<GridSquare>();
            }
        }
    }
}