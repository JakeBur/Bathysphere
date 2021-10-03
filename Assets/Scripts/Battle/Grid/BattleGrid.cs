using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    /// <summary>
    /// A BattleGrid maintains a spatial relationship between a series of GridSquares using a 2D array. 
    /// </summary>
    public class BattleGrid
    {
        public GridSquare[,] squares;

        public BattleGrid(int width, int height)
        {
            squares = new GridSquare[width, height];
        }

        public GridSquare this[int x, int y]
        {
            get => squares[x, y];
            set
            {
                squares[x, y] = value;
                value.BindToGrid(this, x, y);
            }
        }
    }
}
