using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    /// <summary>
    /// A Combatant is an Entity that has health, can take damage, and die. A Combatant is a IBattleActor with abilities and attacks.
    /// </summary>
    public abstract class Combatant : Entity, IBattleActor, IDamageable, ITurnOrderEntry
    {
        /// <summary>
        /// Tracker for the current health of this Combatant.
        /// </summary>
        public int health;

        /// <summary>
        /// Should be invoked when this Combatant's turn is up.
        /// </summary>
        protected Action OnTurnEnd;

        public void AddEndTurnListener(Action action)
        {
            OnTurnEnd += action;
        }

        public void AddRemovedFromPlayListener(Action<ITurnOrderEntry> action)
        {
            OnRemovedFromPlay += (Entity entity) => action?.Invoke(entity as ITurnOrderEntry);
        }

        public void RemoveEndTurnListener(Action action)
        {
            OnTurnEnd -= action;
        }

        public virtual void TakeDamage(int damage)
        {
            health -= damage;

            if(health <= 0)
            {
                Die();
            }
        }

        /// <summary>
        /// Call to remove this Combatant from play.
        /// </summary>
        private void Die()
        {
            RemoveFromPlay();
            Destroy(gameObject);
        }

        public abstract List<IBattleAction> GetPrimedActions();
        public abstract void StartTurn();
    }
}

