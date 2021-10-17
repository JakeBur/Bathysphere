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
        protected abstract class KnightAction : PlayerAction
        {
            protected Knight _knight;

            public KnightAction(Knight knight, int cost) : base(knight, cost)
            {
                _knight = knight;
            }
        }
    }
}