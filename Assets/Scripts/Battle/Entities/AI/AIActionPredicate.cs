using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    /// <summary>
    /// An AIActionPredicate represents the preconditions for an AI action.
    /// </summary>
    public abstract partial class AIActionPredicate
    {
        /// <summary>
        /// Determine whether this predicate is satisfied for the given target.
        /// </summary>
        /// <param name="targetSquare">The square that would be targeted by the action.</param>
        /// <returns>True if the predicate is satsified, false otherwise.</returns>
        public abstract bool Satisfied(AICombatant actor, GridSquare targetSquare);
    }
}