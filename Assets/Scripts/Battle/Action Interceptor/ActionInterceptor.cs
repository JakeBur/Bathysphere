using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    /// <summary>
    /// An action interceptor does something in response to an action being applied to the square an Entity resides on.
    /// If BlocksAction is true, the action interceptor blocks the action from taking place entirely while still executing its own response.
    /// </summary>
    public abstract partial class ActionInterceptor
    {
        /// <summary>
        /// Determines whether this ActionInterceptor blocks the execution of the action it is intercepting.
        /// </summary>
        public bool BlocksAction { get; protected set; }

        /// <summary>
        /// Responds to the given action by executing additional behavior.
        /// </summary>
        /// <param name="action">The action to respond to.</param>
        /// <param name="actingCombatant">The combatant, if any, that took the action.</param>
        /// <param name="targetSquare">The GridSquare being targeted by the action.</param>
        public abstract void InterceptAction(BattleAction action, Combatant actingCombatant, GridSquare targetSquare);

        /// <summary>
        /// Checks whether this ActionInterceptor can/should act on the given Action.
        /// </summary>
        /// <param name="action">The action to respond to.</param>
        /// <param name="actingCombatant">The combatant, if any, that took the action.</param>
        /// <param name="targetSquare">The GridSquare being targeted by the action.</param>
        /// <returns>True if the action should be intercepted by this interceptor, false otherwise.</returns>
        public abstract bool CanIntercept(BattleAction action, Combatant actingCombatant, GridSquare targetSquare);
    }
}