using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public class TurnIndicator : MonoBehaviour
    {
        private void Start()
        {
            TurnManager.Instance.OnTurnAdvance += HandleTurnAdvance;
        }

        private void HandleTurnAdvance(ITurnOrderEntry turnOrderEntry)
        {
            transform.position = (turnOrderEntry as MonoBehaviour).transform.position;
        }
    }
}