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
        public abstract bool CanApplyToSquare(GridSquare gridSquare);
        public abstract bool CanTargetSquare(GridSquare gridSquare);
        public abstract void Apply(GridSquare gridSquare);

        public abstract List<GridSquare> FindThreatenedSquaresAtTarget(GridSquare gridSquare);

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


