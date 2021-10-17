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

        /// <summary>
        /// Tracker for currently available action points.
        /// </summary>
        public ActionPointTracker actionPoints;

        /// <summary>
        /// List of currenty active status effects on this Combatant.
        /// </summary>
        protected List<StatusEffect> _statusEffects;

        /// <summary>
        /// Called when this Combatant begins its turn.
        /// Subscribe to using the ITurnOrderEntry's AddStartTurnListner.
        /// </summary>
        private Action OnStartTurn;

        /// <summary>
        /// Called when this Combatant's turn is up.
        /// </summary>
        private Action OnEndTurn;

        protected new void Awake()
        {
            base.Awake();
            _statusEffects = new List<StatusEffect>();
        }

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

        /// <summary>
        /// Adds a new StatusEffect to this Combatant.
        /// </summary>
        /// <param name="statusEffect">The StatusEffect to add.</param>
        public void AddStatusEffect(StatusEffect statusEffect)
        {
            _statusEffects.Add(statusEffect);
        }

        /// <summary>
        /// Removes a StatusEffect from this Combatant.
        /// </summary>
        /// <param name="statusEffect">The StatusEffect to remove.</param>
        public void RemoveStatusEffect(StatusEffect statusEffect)
        {
            _statusEffects.Remove(statusEffect);
        }

        public virtual void TakeDamage(int damage)
        {
            health -= damage;

            if(health <= 0)
            {
                Die();
            }
        }

        public void StartTurn()
        {
            actionPoints.Reset();
            _statusEffects.ForEach(effect => effect.Update());
            StartTurnBehavior();
            OnStartTurn?.Invoke();
        }

        /// <summary>
        /// Ends this Combatant's turn.
        /// </summary>
        public void EndTurn()
        {
            EndTurnBehavior();
            OnEndTurn?.Invoke();
        }

        /// <summary>
        /// Tries to apply the given action
        /// Will not apply the action if either:
        /// 1) There are not enough action points to pay for it.
        /// 2) The action was blocked by an ActionInterceptor.
        /// </summary>
        /// <param name="combatantAction">The action to apply.</param>
        /// <param name="targetSquare">The square to target with the action.</param>
        /// <returns>True if the action was applied successfully</returns>
        public virtual bool TryApplyAction(CombatantAction combatantAction, GridSquare targetSquare)
        {
            int cost = combatantAction.CalculateCost(targetSquare);
            if(actionPoints.CanConsumePoints(cost))
            {
                bool actionWasSuccessful = combatantAction.TryApply(targetSquare);
                actionPoints.TryConsumePoints(cost);

                return actionWasSuccessful;
            }

            return false;
        }

        public void AddStartTurnListener(Action action)
        {
            OnStartTurn += action;
        }

        public void RemoveStartTurnListener(Action action)
        {
            OnStartTurn -= action;
        }

        /// <summary>
        /// Override to do setup at the beginning of this Combatant's turn.
        /// </summary>
        protected virtual void StartTurnBehavior() { }

        /// <summary>
        /// Override to do teardown at the end of this Combatant's turn.
        /// </summary>
        protected virtual void EndTurnBehavior() { }

        /// <summary>
        /// Executes the highest priority valid IBattleAction associated with the currently selected IBattleActor, if any.
        /// </summary>
        /// <param name="square">The target GridSquare for the action.</param>
        /// <param name="context">The priority event that called this action.</param>
        protected void TryExecuteAction(GridSquare square, BattleAction battleAction, PriorityEvent<GridSquare> context)
        {
            if (battleAction.CanApplyToSquare(square))
            {
                battleAction.TryApply(square);

                // if we could do something, consume the event
                context.ConsumeEvent();
            }
        }

        /// <summary>
        /// Removes this Combatant from play and destroys its GameObject.
        /// </summary>
        private void Die()
        {
            RemoveFromPlay();
            Destroy(gameObject);
        }
    }
}

