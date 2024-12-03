using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Battle;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Decides when the End Turn button should be enabled.
/// </summary>
public class EndTurnButton : MonoBehaviour
{
    public Button button;

    private void Start()
    {
        TurnOrder.Instance.OnTurnAdvance += OnTurnAdvance;
    }

    private void OnTurnAdvance(ITurnOrderEntry entry)
    {
        SetButtonEnabled(entry is PlayerCharacter);
    }

    protected void SetButtonEnabled(bool enabled)
    {
        button.gameObject.SetActive(enabled);
    }
}
