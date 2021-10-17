using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Battle
{
    public abstract partial class AICombatant : Combatant
    {
        /// <summary>
        /// Goal actions are actions that the AICombatant will actively seek to complete, and will prioritize 
        /// </summary>
        protected List<AICombatantAction> _goalActions;
        protected List<AICombatantAction> _availableActions;

        public AICombatant()
        {
            _goalActions = new List<AICombatantAction>();
            _availableActions = new List<AICombatantAction>();

            InitializeGoalActions();
            InitializeSecondaryActions();
            _availableActions.AddRange(_goalActions);
        }

        protected abstract void InitializeGoalActions();
        protected abstract void InitializeSecondaryActions();

        protected override void StartTurnBehavior()
        {
            int actionCount = 0;//using this to guard against infinate loops

            while(SelectAndApplyAction())
            {
                actionCount++;
                if (actionCount > 20)
                {
                    Debug.LogError($"{this} exceeded action count.", this);
                    EndTurn();
                    return;
                }
            }

            EndTurn();
        }

        protected bool SelectAndApplyAction()
        {
            int bestCost = 1000;
            GridSquare bestTarget = null;
            AICombatantAction bestAction = null;

            foreach(AICombatantAction action in _goalActions)
            {
                List<GridSquare> targetableSquares = BattleGridManager.Instance.Grid.GetSquareList().Where(square => action.CanTargetSquare(square)).ToList();

                foreach (GridSquare square in targetableSquares)
                {
                    int cost = action.CalculateCost(square);

                    if (action.CanApplyToSquare(square) && cost < bestCost)
                    {
                        bestCost = cost;
                        bestTarget = square;
                        bestAction = action;
                        continue;
                    }

                    cost = action.CalculateCostWithPredicates(square, _availableActions, out AICombatantAction satisfier);
                    if (cost < bestCost)
                    {
                        bestCost = cost;
                        bestTarget = square;
                        bestAction = satisfier;
                    }
                }
            }

            if (bestAction != null)
            {
                if (bestAction.CanApplyToSquare(bestTarget))
                {
                    if (TryApplyAction(bestAction, bestTarget))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

    }
}