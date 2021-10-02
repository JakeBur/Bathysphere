using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public abstract class Combatant : Entity, IBattleActor, IDamageable, ITurnOrderEntry
    {
        public int health;

        protected Action OnTurnEnd;

        ~Combatant()
        {

        }

        public void AddEndTurnListener(Action action)
        {
            OnTurnEnd += action;
        }

        public void AddRemovedFromPlayListener(Action<ITurnOrderEntry> action)
        {
            OnRemovedFromPlay += (Entity entity) => action?.Invoke(entity as ITurnOrderEntry);
        }

        public abstract List<IBattleAction> GetPrimedActions();

        public void RemoveEndTurnListener(Action action)
        {
            OnTurnEnd -= action;
        }

        public abstract void StartTurn();

        public virtual void TakeDamage(int damage)
        {
            health -= damage;

            if(health <= 0)
            {
                Die();
            }
        }

        private void Die()
        {
            RemoveFromPlay();
            Destroy(gameObject);
        }
    }
}

