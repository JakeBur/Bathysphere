using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
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
                value.battleGrid = this;
                value.SetPosition(x, y);
            }
        }
    }
}
