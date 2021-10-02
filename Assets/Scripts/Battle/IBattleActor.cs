using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public interface IBattleActor
    {
        List<IBattleAction> GetPrimedActions();
    }
}


