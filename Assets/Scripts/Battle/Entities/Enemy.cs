using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public class Enemy : Combatant
    {
        public override List<IBattleAction> GetPrimedActions()
        {
            return new List<IBattleAction>();
        }

        public override void StartTurn()
        {
            Debug.Log("Starting enemy turn");
            Scheduler.Schedule(() => OnTurnEnd?.Invoke(), 2f);
        }
    }
}

