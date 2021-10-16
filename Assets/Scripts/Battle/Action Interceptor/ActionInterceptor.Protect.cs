using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public partial class Knight
    {
        /// <summary>
        /// Intercepts an enemy attack, blocks it, and does damage to the attacker.
        /// </summary>
        public class ProtectInterceptor : ActionInterceptor
        {
            protected Knight _knight;

            public ProtectInterceptor(Knight knight)
            {
                _knight = knight;

                _knight.AddStartTurnListener(HandleKnightStartTurn);

                BlocksAction = true;
            }

            public override bool CanIntercept(BattleAction battleAction, Combatant actingCombatant, GridSquare targetSquare)
            {
                return actingCombatant is Enemy;
            }

            public override void InterceptAction(BattleAction action, Combatant actingCombatant, GridSquare targetSquare)
            {
                new Counterattack(_knight, 0, 2).TryApply(actingCombatant.Square);
            }

            private void HandleKnightStartTurn()
            {
                _knight.RemoveActionInterceptor(this);
                _knight.RemoveStartTurnListener(HandleKnightStartTurn);
            }
        }
    }
}