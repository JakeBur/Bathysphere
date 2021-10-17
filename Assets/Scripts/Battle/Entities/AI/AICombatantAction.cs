using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Battle
{
    /// <summary>
    /// An AIAction is an action that has a concept both of predicates that must be satisfied to execute it as well as 
    /// the predicates that are partially or fully satisfied when it is executed
    /// </summary>
    public abstract class AICombatantAction : CombatantAction
    {
        /// <summary>
        /// List of predicates that must be satsifed before this AIAction can be executed.
        /// </summary>
        protected List<AIActionPredicate> _predicates;

        /// <summary>
        /// List of predicates that this AIAction can satsify or partially satisfy when executed.
        /// </summary>
        protected List<Type> _satisfiablePredicates;

        protected AICombatant _aiCombatant;

        public AICombatantAction(AICombatant aiCombatant, int cost) : base(aiCombatant, cost)
        {
            _aiCombatant = aiCombatant;
            _predicates = new List<AIActionPredicate>();
            _satisfiablePredicates = new List<Type>();
        }

        public override bool CanApplyToSquare(GridSquare targetSquare)
        {
            foreach(AIActionPredicate predicate in _predicates)
            {
                if(!predicate.Satisfied(_aiCombatant, targetSquare))
                {
                    return false;
                }
            }

            return true;
        }

        public int CalculateCostWithPredicates(GridSquare targetSquare, List<AICombatantAction> availableActions, out AICombatantAction satisfier)
        {
            int cost = CalculateCost(targetSquare);

            int bestNestedCost = 1000;

            satisfier = this;// base case for recursion

            foreach (AIActionPredicate predicate in _predicates)
            {
                if(predicate.Satisfied(_aiCombatant, targetSquare))
                {
                    continue;
                }

                AICombatantAction currentSatisfier = availableActions.Find(action => 
                    action._satisfiablePredicates.Find(type => type == predicate.GetType()) != null);

                if (currentSatisfier != null)
                {
                    int nestedCost = currentSatisfier.CalculateCostWithPredicates(targetSquare, availableActions, out AICombatantAction nestedSatisfier);
                    cost += nestedCost;

                    if(nestedCost < bestNestedCost)
                    {
                        satisfier = nestedSatisfier;
                    }
                }
            }

            return cost;
        }

        /*public AICombatantAction GetPredicateSatsifier(GridSquare targetSquare, List<AICombatantAction> availableActions)
        {
            foreach(AICombatantAction action in availableActions)
            {
                if(!action.SatisfiesPredicate())

                if(action.CanApplyToSquare(targetSquare))
                {
                    return action;
                }
                else
                {
                    AICombatantAction nestedPredicate = action.GetPredicateSatsifier(targetSquare, availableActions);

                    if (nestedPredicate != null)
                    {
                        return nestedPredicate;
                    }
                }
            }

            if()
            {

            }
        }*/

        public abstract bool CanTargetSquare(GridSquare targetSquare);

        public List<GridSquare> FindValidTargets()
        {
            List<GridSquare> validTargets = new List<GridSquare>();

            foreach (GridSquare square in BattleGridManager.Instance.Grid.squares)
            {
                if(CanApplyToSquare(square))
                {
                    validTargets.Add(square);
                }
            }

            return validTargets;
        }
    }
}