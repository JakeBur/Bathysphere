using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public abstract class Combatant : Entity, IBattleActor, IDamageable
    {
        public abstract List<IBattleAction> GetPrimedActions();

        public abstract void TakeDamage();
    }
}

