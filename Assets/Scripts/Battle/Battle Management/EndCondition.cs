using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    /// <summary>
    /// Parent class for victory/defeat conditions for battles.
    /// </summary>
    public abstract class EndCondition
    {
        /// <summary>
        /// Checks whether the conditions described by this EndCondition are met.
        /// </summary>
        /// <returns>True if the conditions are met, false otherwise.</returns>
        public abstract bool Met(BattleManager battleManager);
    }
}