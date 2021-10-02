using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Battle
{
    public interface ITurnOrderEntry
    {
        void AddEndTurnListener(Action action);
        void RemoveEndTurnListener(Action action);

        void AddRemovedFromPlayListener(Action<ITurnOrderEntry> action);

        void StartTurn();
    }

    public static class TurnOrderEntryExtentions
    {
        public static bool TurnActive(this ITurnOrderEntry entry)
        {
            return TurnManager.Instance.TurnOrderEntryIsActive(entry);
        }
    }

}
