using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public class TurnIndicator : MonoBehaviour
    {
        private void Start()
        {
            TurnOrder.Instance.OnTurnAdvance += HandleTurnAdvance;
            HandleTurnAdvance(TurnOrder.CurrentEntry);
        }

        private void Update()
        {
            HandleTurnAdvance(TurnOrder.CurrentEntry);
        }

        private void HandleTurnAdvance(ITurnOrderEntry turnOrderEntry)
        {
            MonoBehaviour turnOrderEntryGameObject = TurnOrder.CurrentEntry as MonoBehaviour;
            if(turnOrderEntryGameObject != null)
            {
                transform.position = turnOrderEntryGameObject.transform.position;
            }
            else
            {
                Debug.Log("Turn order entry not a game object");
            }
        }
    }
}