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
        void TakeDamage(int damage);
    }
}
