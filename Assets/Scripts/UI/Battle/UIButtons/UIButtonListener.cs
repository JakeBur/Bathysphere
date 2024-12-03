using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;


/// <summary>
/// Passthrough for unity buttons.
/// </summary>
public class UIButtonListener : MonoBehaviour
{
    public static UIButtonListener Instance;

    private void Awake()
    {
        Assert.IsNull(Instance, "More than one instance of UIButtonListener has been created.");
        Instance = this;
    }

    public Action OnEndTurn;

    public void EndTurn()
    {
        OnEndTurn?.Invoke();
    }
}
