using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Battle
{
    /// <summary>
    /// Interface for any object that can appear in the turn order, control the game state for a bit, and then return it to the TurnManager again.
    /// </summary>
    /// <remarks>
    /// Player characters and enemies are good examples, but this can also be a weather effect, a timer ticking down, or even a story event.
    /// 
    /// A turn order entry should have an end turn event that can be subscribed to. This should be invoked when the entry is finished doing
    /// what it wanted to do for its turn.
    /// 
    /// A turn order entry should also have an event that is invoked when it is removed from play (like when a Combatent dies) 
    /// so that we don't try to call a defunct turn order entry's methods.
    /// </remarks>
    public interface ITurnOrderEntry
    {
        /// <summary>
        /// Adds a listener to the start turn event, which will be called when this TurnOrderEntry begins its turn, after its initialization function is called.
        /// </summary>
        /// <param name="action">The action to subscribe.</param>
        void AddStartTurnListener(Action action);

        /// <summary>
        /// Removes a listener to the start turn event.
        /// </summary>
        /// <param name="action">The action to unsubscribe.</param>
        void RemoveStartTurnListener(Action action);

        /// <summary>
        /// Adds a listener to the end turn event, which will be called when this TurnOrderEntry is done with its behavior.
        /// </summary>
        /// <param name="action">The action to subscribe.</param>
        void AddEndTurnListener(Action action);
        
        /// <summary>
        /// Removes a listener to the end turn event.
        /// </summary>
        /// <param name="action">The action to unsubscribe.</param>
        void RemoveEndTurnListener(Action action);

        /// <summary>
        /// Adds a listener to the event that will be called when the turn order entry is removed from play.
        /// </summary>
        /// <param name="action">The action to subscribe.</param>
        void AddRemovedFromPlayListener(Action<ITurnOrderEntry> action);

        /// <summary>
        /// Call to yield control of the game state to this turn order entry.
        /// </summary>
        void StartTurn();
    }

    public static class TurnOrderEntryExtentions
    {
        /// <summary>
        /// Checks whether the TurnManager is currently yielding control of the game state to this specific TurnOrderEntry.
        /// </summary>
        /// <returns>True if this entry is being executed, false otherwise.</returns>
        public static bool TurnActive(this ITurnOrderEntry entry)
        {
            return TurnOrder.Instance.TurnOrderEntryIsActive(entry);
        }
    }

}
