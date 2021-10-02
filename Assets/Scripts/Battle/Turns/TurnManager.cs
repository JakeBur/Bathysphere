using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Battle
{
    public class TurnManager : MonoBehaviour
    {
        public static TurnManager Instance;

        public Action<ITurnOrderEntry> OnTurnAdvance;

        public List<GameObject> turnOrderEntries;

        private List<ITurnOrderEntry> _turnOrder;

        private void Awake()
        {
            Instance = this;

            _turnOrder = new List<ITurnOrderEntry>();
            turnOrderEntries.ForEach(entryObject =>
            {
                ITurnOrderEntry entry = entryObject.GetComponent<ITurnOrderEntry>();
                entry.AddEndTurnListener(AdvanceTurn);
                entry.AddRemovedFromPlayListener(RemoveTurnOrderEntry);
                _turnOrder.Add(entry);
            });

            _turnOrder[0].StartTurn();
        }

        public bool TurnOrderEntryIsActive(ITurnOrderEntry entry)
        {
            return _turnOrder[0] == entry;
        }

        private void RemoveTurnOrderEntry(ITurnOrderEntry entry)
        {
            _turnOrder.Remove(entry);
        }

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
