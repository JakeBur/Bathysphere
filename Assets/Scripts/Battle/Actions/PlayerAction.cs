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

        public abstract void BeginPreview();
        public abstract void UpdatePreview(GridSquare gridSquare);
        public abstract void EndPreview();

        public PlayerAction(PlayerCharacter player, int cost) : base(player, cost)
        {
            _player = player;
            IsInstant = false;
        }
    }
}