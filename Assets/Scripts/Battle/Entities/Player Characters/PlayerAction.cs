using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public abstract class PlayerAction : CombatantAction
    {
        protected PlayerCharacter _player;
        public bool IsInstant { get; protected set; }

        public PlayerAction(PlayerCharacter player, int cost) : base(player, cost)
        {
            _player = player;
            IsInstant = false;
        }

        public abstract bool CanTargetSquare(GridSquare targetSquare);
        public abstract void BeginPreview();
        public abstract void UpdatePreview(GridSquare gridSquare);
        public abstract void EndPreview();

        public List<GridSquare> FindTargetableSquares()
        {
            List<GridSquare> targetableSquares = new List<GridSquare>();

            foreach (GridSquare gridSquare in BattleGridManager.Instance.Grid.squares)
            {
                if (CanTargetSquare(gridSquare))
                {
                    targetableSquares.Add(gridSquare);

                }
            }

            return targetableSquares;
        }
    }
}