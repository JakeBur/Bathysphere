using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Battle
{
    public abstract class CombatantAction : BattleAction
    {
        [SerializeField]
        public int _cost;

        protected Combatant _combatant;

        public CombatantAction(Combatant combatant, int cost)
        {
            _combatant = combatant;

            _cost = cost;
        }

        public override bool CanApplyToSquare(GridSquare targetSquare)
        {
            return _combatant.actionPoints.CanConsumePoints(_cost);
        }

        public virtual int CalculateCost(GridSquare targetSquare)
        {
            return _cost;
        }
    }
}