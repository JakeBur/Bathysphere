using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public class Enemy : Combatant
    {
        public override List<IBattleAction> GetPrimedActions()
        {
            return null;
        }

        public override void TakeDamage()
        {
            Debug.Log("Enemy took damage");
        }
    }
}

