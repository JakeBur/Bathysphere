using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    /// <summary>
    /// An Enemy is an AI controled Combatant hostile to the player.
    /// </summary>
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

