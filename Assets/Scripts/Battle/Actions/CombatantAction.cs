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

        /*public virtual void Apply(GridSquare gridSquare)
        {
            _combatant.actionPoints.TryConsumePoints(CalculateCost(gridSquare));
        }*/

        public override bool CanApplyToSquare(GridSquare gridSquare)
        {
            return _combatant.actionPoints.CanConsumePoints(_cost);
        }

        public virtual int CalculateCost(GridSquare targetSquare)
        {
            return _cost;
        }
    }
}