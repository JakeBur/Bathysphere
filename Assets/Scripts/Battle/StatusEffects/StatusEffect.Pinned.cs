using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Battle
{
    public abstract partial class StatusEffect
    {
        public class Pinned : StatusEffect
        {
            public Pinned(Combatant combatant) : base(combatant)
            {

            }

            protected override void ApplyEffect()
            {

            }

            protected override bool TargetValid(Combatant target)
            {
                return true;
            }
        }
    }
}