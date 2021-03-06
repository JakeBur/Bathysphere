using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Battle
{
    /// <summary>
    /// Battle end condition that requires all enemies to be defeated.
    /// </summary>
    public class AllEnemiesDefeated : EndCondition
    {
        public override bool Met(BattleManager battleManager)
        {
            return battleManager.entities.Where(entity => entity is Enemy).ToList().Count == 0;
        }
    }
}