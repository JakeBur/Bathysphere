using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public abstract partial class ActionInterceptor
    {
        public bool Consumes { get; protected set; }

        /// <summary>
        /// Applies any modifications to the action before it is applied.
        /// </summary>
        public abstract void InterceptAction(BattleAction action, Combatant actingCombatant, GridSquare targetSquare);

        public abstract bool CanIntercept(BattleAction battleAction, Combatant actingCombatant, GridSquare targetSquare);
    }
}