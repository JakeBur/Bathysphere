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

        public StatusEffect(Combatant target, int lifetime)
        {
            if(!TargetValid(target))
            {
                return;
            }

            _target = target;
            _lifetime = lifetime;

            _target.AddStartTurnListener(Update);
        }

        ~StatusEffect()
        {
            Debug.Log($"{this} with target {_target} destroyed with lifetime {_lifetime} left");
        }

        private void Update()
        {
            if (_lifetime <= 0)
            {
                _target.RemoveEndTurnListener(Update);
                return;
            }

            ApplyEffect();
            _lifetime--;
        }

        protected abstract void ApplyEffect();
        protected abstract bool TargetValid(Combatant target);
    }
}