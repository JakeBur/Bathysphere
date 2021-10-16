using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Battle
{
    /// <summary>
    /// An action point tracker manages available action points and enforces action point usage patterns.
    /// </summary>
    [Serializable]
    public class ActionPointTracker
    {
        /// <summary>
        /// The amount of points this tracker returns to when refreshed.
        /// </summary>
        [SerializeField]
        private int _maxPoints;

        /// <summary>
        /// The current action point count.
        /// </summary>
        public int CurrentPoints { get; private set; }

        /// <summary>
        /// Accessor property for the maximum point value.
        /// </summary>
        public int MaxPoints { get => _maxPoints; }

        /// <summary>
        /// Resets action point count on this tracker to full.
        /// TODO: this is where we can manage buffs and debuffs for action points.
        /// </summary>
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
            return CurrentPoints >= amount;
        }
    }
}