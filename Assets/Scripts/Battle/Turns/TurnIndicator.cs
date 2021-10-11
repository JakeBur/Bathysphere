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
            HandleTurnAdvance(TurnManager.Instance.CurrentEntry);
        }

        private void Update()
        {
            HandleTurnAdvance(TurnManager.Instance.CurrentEntry);
        }

        private void HandleTurnAdvance(ITurnOrderEntry turnOrderEntry)
        {
            MonoBehaviour turnOrderEntryGameObject = TurnManager.Instance.CurrentEntry as MonoBehaviour;
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