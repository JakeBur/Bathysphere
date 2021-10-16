using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    /// <summary>
    /// Parent class for any object that can, under certain conditions, apply effects to a given GridSquare.
    /// Use this to implement attacks and abilities that target a specific square.
    /// </summary>
    public abstract class BattleAction
    {
        /// <summary>
        /// Determines whether the conditions are met to apply this action.
        /// </summary>
        /// <param name="targetSquare">The square to target.</param>
        /// <returns>True if the conditions are met, false otherwise.</returns>
        public abstract bool CanApplyToSquare(GridSquare targetSquare);

        /// <summary>
        /// Applies the behavior of this action to the target square.
        /// </summary>
        /// <param name="targetSquare">The square to apply the action to.</param>
        public abstract void Apply(GridSquare targetSquare);

        /// <summary>
        /// Generates a list of squares that will feel the effects of this action if it is targeted at the given square.
        /// </summary>
        /// <param name="targetSquare">The square to target.</param>
        /// <returns>A list of squares that would be effected by this action if it were taken at the given square.</returns>
        public abstract List<GridSquare> FindThreatenedSquaresAtTarget(GridSquare targetSquare);

        /// <summary>
        /// Tries to apply this action to the given square.
        /// This may be blocked by an ActionInterceptor.
        /// </summary>
        /// <param name="targetSquare">The square to apply the effect to.</param>
        public virtual void TryApply(GridSquare targetSquare)
        {
            if (targetSquare != null)
            {
                List<Entity> entities = targetSquare.Entities;

                foreach (Entity entity in entities)
                {
                    // if the action was intercepted
                    if (entity.TryInterceptAction(this, null))
                    {
                        return;
                    }
                }
            }

            Apply(targetSquare);
        }
        
        /// <summary>
        /// Generates a list of all squares that might be effected by this action, given every possible valid target for application.
        /// </summary>
        /// <returns>The generated list.</returns>
        public List<GridSquare> FindThreatenedSquares()
        {
            List<GridSquare> threatenedSquares = new List<GridSquare>();

            foreach (GridSquare gridSquare in BattleGridManager.Instance.Grid.squares)
            {
                if (CanApplyToSquare(gridSquare))
                {
                    threatenedSquares.AddRange(FindThreatenedSquaresAtTarget(gridSquare));
                }
            }

            return threatenedSquares;
        }
    }
}


