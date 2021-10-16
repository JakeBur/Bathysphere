using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Battle
{
    public abstract partial class StatusEffect
    {
        public class Staggered : StatusEffect
        {
            public Staggered(Combatant combatant, int lifetime) : base(combatant, lifetime)
            {
            }

            protected override void ApplyEffect()
            {
                _target.actionPoints.TryConsumePoints(Math.Min(2, _target.actionPoints.CurrentPoints));
            }

            protected override bool TargetValid(Combatant target)
            {
                return true;
            }
        }
    }
}