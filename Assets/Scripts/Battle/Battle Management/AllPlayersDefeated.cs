using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Battle
{
    public class AllPlayersDefeated : EndCondition
    {
        public override bool Met(BattleManager battleManager)
        {
            return battleManager.entities.Where(entity => entity is PlayerCharacter).ToList().Count == 0;
        }
    }
}