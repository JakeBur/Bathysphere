using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Battle
{
    /// <summary>
    /// Manager that allows a variety of ITurnOrderEntries to control the game state sequentially.
    /// </summary>
    public class TurnOrder : MonoBehaviour
    {
        public static TurnOrder Instance;

        /// <summary>
        /// Invoked when the turn is advanced.
        /// Supplies the new ITurnOrderEntry as an argument.
        /// </summary>
        public Action<ITurnOrderEntry> OnTurnAdvance;

        /// <summary>
        /// Tracker for the ordering of ITurnOrderEntries.
        /// Is used like a cyclical queue, with the current turn stored in the 0th element.
        /// </summary>
        private List<ITurnOrderEntry> _turnOrder;

        /// <summary>
        /// True when a new turn should be started on the next update.
        /// </summary>
        protected bool _advanceTurn;

        public static ITurnOrderEntry CurrentEntry
        {
            get
            {
                if (Instance._turnOrder != null && Instance._turnOrder.Count > 0) return Instance._turnOrder[0];
                else return null;
            }
        }

        private void Awake()
        {
            Instance = this;
            _turnOrder = new List<ITurnOrderEntry>();
        }

        private void Update()
        {
            if(_advanceTurn)
            {
                _advanceTurn = false;

                _turnOrder[0].StartTurn();
                OnTurnAdvance?.Invoke(_turnOrder[0]);
            }
        }

        /// <summary>
        /// Starts the turn of the fist ITurnOrderEntry in the order.
        /// </summary>
        public static void StartTurnOrder()
        {
            Instance._turnOrder[0].StartTurn();
        }

        /// <summary>
        /// Adds the given ITurnOrderEntry to the end of the turn order.
        /// </summary>
        /// <param name="entry">The ITurnOrderEntry to add.</param>
        public static void Add(ITurnOrderEntry entry)
        {
            if (entry == null) return;

            // listen for when the entry yields game state control back to us
            entry.AddEndTurnListener(Instance.AdvanceTurn);

            // listen for when the entry is removed from play so that we don't continue to try to execute it.
            entry.AddRemovedFromPlayListener(Instance.Remove);

            Instance._turnOrder.Add(entry);
        }

        /// <summary>
        /// Inserts the turn order entry before the given index. Values for index less than 1 are treated as 1.
        /// </summary>
        /// <param name="index">The index to insert at.</param>
        /// <param name="entry">The ITurnOrderEntry to add.</param>
        public static void Insert(int index, ITurnOrderEntry entry)
        {
            if (entry == null) return;

            // listen for when the entry yields game state control back to us
            entry.AddEndTurnListener(Instance.AdvanceTurn);

            // listen for when the entry is removed from play so that we don't continue to try to execute it.
            entry.AddRemovedFromPlayListener(Instance.Remove);

            if(index < Instance._turnOrder.Count)
            {
                Instance._turnOrder.Insert(Math.Min(index, 1), entry);
            }
            else
            {
                Instance._turnOrder.Add(entry);
            }
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
        private void Remove(ITurnOrderEntry entry)
        {
            _turnOrder.Remove(entry);
        }

        /// <summary>
        /// Advances the to the next ITurnOrderEntry, yielding game state control to it.
        /// </summary>
        private void AdvanceTurn()
        {
            ITurnOrderEntry currentTurn = _turnOrder[0];

            // move the current entry to the end of the queue
            _turnOrder.RemoveAt(0);
            _turnOrder.Add(currentTurn);

            _advanceTurn = true;
        }
    }
}
