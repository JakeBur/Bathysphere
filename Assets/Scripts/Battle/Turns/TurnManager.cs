using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Battle
{
    /// <summary>
    /// Manager that allows a variety of ITurnOrderEntries to control the game state sequentially.
    /// </summary>
    public class TurnManager : MonoBehaviour
    {
        public static TurnManager Instance;

        /// <summary>
        /// Invoked when the turn is advanced.
        /// Supplies the new ITurnOrderEntry as an argument.
        /// </summary>
        public Action<ITurnOrderEntry> OnTurnAdvance;

        //TODO: Temporary manually generated list for testing
        public List<GameObject> turnOrderEntries;

        /// <summary>
        /// Tracker for the ordering of ITurnOrderEntries.
        /// Is used like a cyclical queue, with the current turn stored in the 0th element.
        /// </summary>
        private List<ITurnOrderEntry> _turnOrder;

        private void Awake()
        {
            Instance = this;

            _turnOrder = new List<ITurnOrderEntry>();
            turnOrderEntries.ForEach(entryObject =>
            {
                ITurnOrderEntry entry = entryObject.GetComponent<ITurnOrderEntry>();

                // listen for when the entry yields game state control back to us
                entry.AddEndTurnListener(AdvanceTurn);

                // listen for when the entry is removed from play so that we don't continue to try to execute it.
                entry.AddRemovedFromPlayListener(RemoveTurnOrderEntry);

                _turnOrder.Add(entry);
            });

            _turnOrder[0].StartTurn();
        }

        /// <summary>
        /// Checks if the given ITurnOrderEntry is the currently active ITurnOrderEntry.
        /// </summary>
        /// <param name="entry">The ITurnOrderEntry to check.</param>
        /// <returns>True if the given ITurnOrderEntry is the currently active ITurnOrderEntry, false otherwise.</returns>
        public bool TurnOrderEntryIsActive(ITurnOrderEntry entry)
        {
            return _turnOrder[0] == entry;
        }

        /// <summary>
        /// Removes the given ITurnOrderEntry from the turn order.
        /// </summary>
        /// <param name="entry">The entry to remove.</param>
        private void RemoveTurnOrderEntry(ITurnOrderEntry entry)
        {
            _turnOrder.Remove(entry);
        }

        /// <summary>
        /// Advances the to the next ITurnOrderEntry, yielding game state control to it.
        /// </summary>
        private void AdvanceTurn()
        {
            Debug.Log("advancing turn");

            ITurnOrderEntry currentTurn = _turnOrder[0];

            // move the current entry to the end of the queue
            _turnOrder.RemoveAt(0);
            _turnOrder.Add(currentTurn);

            // start the turn
            _turnOrder[0].StartTurn();

            OnTurnAdvance?.Invoke(_turnOrder[0]);
        }
    }
}
