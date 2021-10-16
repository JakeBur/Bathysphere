using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public abstract partial class StatusEffect
    {
        protected Combatant _target;
        protected int _lifetime;

        protected Action OnEndTurn;
        protected Action<ITurnOrderEntry> OnRemovedFromPlay;

        protected bool isIndefinite;

        public StatusEffect(Combatant target, int lifetime)
        {
            if(!TargetValid(target))
            {
                return;
            }

            _target = target;
            _lifetime = lifetime;

            _target.AddStatusEffect(this);
        }

        public StatusEffect(Combatant target)
        {
            if (!TargetValid(target))
            {
                return;
            }

            _target = target;
            isIndefinite = true;

            _target.AddStatusEffect(this);
        }

        public void Update()
        {
            if (!isIndefinite && _lifetime <= 0)
            {
                _target.RemoveStatusEffect(this);
                return;
            }

            ApplyEffect();
            _lifetime--;
        }

        public void Clear()
        {
            _target.RemoveStatusEffect(this);
        }

        protected abstract void ApplyEffect();
        protected abstract bool TargetValid(Combatant target);
    }
}