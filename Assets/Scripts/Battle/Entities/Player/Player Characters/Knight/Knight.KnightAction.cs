using System.Collections;
using System.Collections.Generic;
using Battle;
using UnityEngine;

namespace Battle
{
    public partial class Knight
    {
        public abstract class KnightAction : PlayerAction
        {
            protected Knight _knight;

            public KnightAction(Knight knight, int cost) : base(knight, cost)
            {
                _knight = knight;
            }
        }
    }
}


