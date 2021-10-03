using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    /// <summary>
    /// Interface for any object that can produce a set of available IBattleActions to potentially be used in battle.
    /// </summary>
    public interface IBattleActor
    {
        List<IBattleAction> GetPrimedActions();
    }
}


