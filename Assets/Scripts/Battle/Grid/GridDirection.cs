using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public enum GridDirection { North, East, South, West };// arranged in clockwise order

    public static class GridDirectionExtensions
    {
        public static GridDirection RotateClockwise(this GridDirection direction)
        {
            return (GridDirection)(((int)(direction + 1)) % 4);
        }

        public static GridDirection RotateCounterclockwise(this GridDirection direction)
        {
            return (GridDirection)((4 + (int)(direction - 1)) % 4);
        }
    }
}