using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    /// <summary>
    /// A Combatant is an Entity that has health, can take damage, and die. A Combatant is a IBattleActor with abilities and attacks.
    /// </summary>
    public abstract class Combatant : Entity, IDamageable, ITurnOrderEntry
    {
        /// <summary>
        /// Tracker for the current health of this Combatant.
        /// </summary>
        public int health;

        public ActionPointTracker actionPoints;

        protected void Start()
        {
            // try to execute actions when a square on the grid is clicked
        }

        /// <summary>
        /// Executes the highest priority valid IBattleAction associated with the currently selected IBattleActor, if any.
        /// </summary>
        /// <param name="square">The target GridSquare for the action.</param>
        /// <param name="context">The priority event that called this action.</param>
        protected void TryExecuteAction(GridSquare square, IBattleAction battleAction, PriorityEvent<GridSquare> context)
        {
            if (battleAction.CanApplyToSquare(square))
            {
                battleAction.Apply(square);

                // if we could do something, consume the event
                context.ConsumeEvent();
            }
        }

        /// <summary>
        /// Should be invoked when this Combatant's turn is up.
        /// </summary>
        protected Action OnEndTurn;

        public void AddEndTurnListener(Action action)
        {
            OnEndTurn += action;
        }

        public void AddRemovedFromPlayListener(Action<ITurnOrderEntry> action)
        {
            OnRemovedFromPlay += (Entity entity) => action?.Invoke(entity as ITurnOrderEntry);
        }

        public void RemoveEndTurnListener(Action action)
        {
            OnEndTurn -= action;
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

        public virtual void StartTurn()
        {
            actionPoints.Reset();
        }
        
        public virtual void EndTurn()
        {
            OnEndTurn?.Invoke();
        }

        public virtual bool ApplyAction(CombatantAction combatantAction, GridSquare targetSquare)
        {
            int cost = combatantAction.CalculateCost(targetSquare);
            if(actionPoints.TryConsumePoints(cost))
            {
                combatantAction.Apply(targetSquare);
                return true;
            }

            return false;
        }
    }
}

