using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public class TurnManager : MonoBehaviour
    {
        public static TurnManager Instance;

        public List<GameObject> turnOrderEntries;

        private List<ITurnOrderEntry> _turnOrder;

        private void Awake()
        {
            Instance = this;

            _turnOrder = new List<ITurnOrderEntry>();
            turnOrderEntries.ForEach(entry =>
            {
                entry.GetComponent<ITurnOrderEntry>().AddEndTurnListener(AdvanceTurn);
                _turnOrder.Add(entry.GetComponent<ITurnOrderEntry>());
            });

            _turnOrder[0].StartTurn();
        }

        public bool TurnOrderEntryIsActive(ITurnOrderEntry entry)
        {
            return _turnOrder[0] == entry;
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
        }
    }
}
