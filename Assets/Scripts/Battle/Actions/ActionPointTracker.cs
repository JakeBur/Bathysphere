using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Battle
{
    [Serializable]
    public class ActionPointTracker
    {
        [SerializeField]
        private int _maxPoints;

        public int CurrentPoints { get; private set; }
        public int MaxPoints { get => _maxPoints; }

        public void Reset()
        {
            CurrentPoints = MaxPoints;
        }

        /// <summary>
        /// Tries to use the given amount of points.
        /// If there are enough points, the points are used and this function returns true.
        /// Otherwise, no points are consumed and the function returns false.
        /// </summary>
        /// <param name="amount">The amount of points to consume.</param>
        /// <returns>True if there were enough points to consume, false otherwise.</returns>
        public bool TryConsumePoints(int amount)
        {
            if (CurrentPoints >= amount)
            {
                CurrentPoints -= amount;
                return true;
            }
            else
            { 
                return false;
            }
        }

        /// <summary>
        /// Determines whether there are enough remaining points to consume the given amount.
        /// </summary>
        /// <param name="amount">The amount to test.</param>
        /// <returns>True if there are enough points to consume, false otherwise.</returns>
        public bool CanConsumePoints(int amount)
        {
            //Debug.Log("checking if can consume: " + (CurrentPoints >= amount));

            return CurrentPoints >= amount;
        }
    }
}