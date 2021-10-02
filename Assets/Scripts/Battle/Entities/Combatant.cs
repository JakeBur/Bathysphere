using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public abstract class Combatant : Entity, IBattleActor, IDamageable, ITurnOrderEntry
    {
        protected Action OnTurnEnd;

        public void AddEndTurnListener(Action action)
        {
            OnTurnEnd += action;
        }

        public abstract List<IBattleAction> GetPrimedActions();

        public void RemoveEndTurnListener(Action action)
        {
            OnTurnEnd -= action;
        }

        public abstract void StartTurn();

        public abstract void TakeDamage();
    }
}

