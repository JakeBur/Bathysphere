using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    /// <summary>
    /// Interface for any object that can take damage from attacks.
    /// </summary>
    public interface IDamageable
    {
        /// <summary>
        /// Takes the given amount of damage.
        /// </summary>
        /// <param name="damage">The damage to take.</param>
        void TakeDamage(int damage);
    }
}
