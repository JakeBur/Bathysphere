using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Battle
{
    public abstract class EnemyAction : CombatantAction
    {
        protected Enemy _enemy;

        public EnemyAction(Combatant combatant, int cost) : base(combatant, cost)
        {
            _enemy = combatant as Enemy;
        }


    }
}