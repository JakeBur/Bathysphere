using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public class KnightSword : Entity
    {
        private Knight _knight;

        public void BindToKnight(Knight knight)
        {
            _knight = knight;
        }

        public override void Select()
        {

        }

        public override List<PlayerAction> GetAvailableMenuActions()
        {
            List<PlayerAction> actions = new List<PlayerAction>();
            actions.Add(new TestAction(_knight, 1));

            return actions;
        }

        public override void Deselect()
        {
        }

        public override List<PlayerAction> GetAvailableComboActions(Entity entity)
        {
            throw new System.NotImplementedException();
        }

        protected class TestAction : PlayerAction
        {
            public TestAction(PlayerCharacter player, int cost) : base(player, cost)
            {

            }

            public override void Apply(GridSquare gridSquare)
            {
                Debug.Log("applied action");
            }

            public override void BeginPreview()
            {
                throw new System.NotImplementedException();
            }

            public override bool CanTargetSquare(GridSquare gridSquare)
            {
                return gridSquare == null;
            }

            public override void EndPreview()
            {
                throw new System.NotImplementedException();
            }

            public override List<GridSquare> FindThreatenedSquaresAtTarget(GridSquare gridSquare)
            {
                List<GridSquare> squares = new List<GridSquare>();
                squares.Add(gridSquare);

                return squares;
            }

            public override void UpdatePreview(GridSquare gridSquare)
            {
                throw new System.NotImplementedException();
            }
        }

    }
}