using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public partial class AIActionPredicate
    { 
        public class SquareInRange : AIActionPredicate
        {
            protected int _range;

            public SquareInRange(int range)
            {
                _range = range;
            }

            public override bool Satisfied(AICombatant actor, GridSquare targetSquare)
            {
                return GridSquare.Distance(actor.Square, targetSquare) <= _range;
            }
        }
    }
}