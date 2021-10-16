using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Battle
{
    /// <summary>
    /// Battle end condition that requires all player characters to be defeated.
    /// </summary>
    public class AllPlayersDefeated : EndCondition
    {
        public override bool Met(BattleManager battleManager)
        {
            return battleManager.entities.Where(entity => entity is PlayerCharacter).ToList().Count == 0;
        }
    }
}