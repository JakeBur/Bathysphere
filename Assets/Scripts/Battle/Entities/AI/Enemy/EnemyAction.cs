using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Battle
{
    public abstract class EnemyAction : AICombatantAction
    {
        protected Enemy _enemy;

        public EnemyAction(AICombatant aiCombatant, int cost) : base(aiCombatant, cost)
        {
            _enemy = aiCombatant as Enemy;
        }
    }
}