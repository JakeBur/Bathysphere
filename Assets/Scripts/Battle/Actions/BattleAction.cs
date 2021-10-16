using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    /// <summary>
    /// Interface for any object that can, under certain conditions, apply effects to a given GridSquare.
    /// Use this to implement attacks and abilities that target a specific square.
    /// </summary>
    public abstract class BattleAction
    {
        public Combatant ActingCombatant { get; protected set; }

        public abstract bool CanApplyToSquare(GridSquare targetSquare);
        public abstract bool CanTargetSquare(GridSquare targetSquare);
        public abstract void Apply(GridSquare targetSquare);

        public abstract List<GridSquare> FindThreatenedSquaresAtTarget(GridSquare targetSquare);

        public void TryApply(GridSquare targetSquare)
        {
            if(!ActingCombatant)
            {
                Apply(targetSquare);
                return;
            }

            if(targetSquare != null)
            {
                List<Entity> entities = targetSquare.Entities;

                foreach (Entity entity in entities)
                {
                    // if the action was intercepted
                    if (entity.TryInterceptAction(this, ActingCombatant))
                    {
                        return;
                    }
                }
            }

            Apply(targetSquare);
        }

        public List<GridSquare> FindTargetableSquares()
        {
            List<GridSquare> targetableSquares = new List<GridSquare>();

            foreach (GridSquare gridSquare in BattleGridManager.Instance.Grid.squares)
            {
                if (CanTargetSquare(gridSquare))
                {
                    targetableSquares.Add(gridSquare);

                }
            }

            return targetableSquares;
        }

        public List<GridSquare> FindThreatenedSquares()
        {
            List<GridSquare> threatenedSquares = new List<GridSquare>();

            foreach (GridSquare gridSquare in BattleGridManager.Instance.Grid.squares)
            {
                if (CanTargetSquare(gridSquare))
                {
                    threatenedSquares.AddRange(FindThreatenedSquaresAtTarget(gridSquare));
                }
            }

            return threatenedSquares;
        }
    }
}


