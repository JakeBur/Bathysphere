using System;
using System.Collections;
using System.Collections.Generic;
using Battle;
using UnityEngine;

public class TestTurn : MonoBehaviour, Battle.ITurnOrderEntry
{
    private Action OnTurnEnd;

    public bool endTurn = false;

    private bool _turnActive = false;

    public void AddEndTurnListener(Action action)
    {
        OnTurnEnd += action;
    }

    public void RemoveEndTurnListener(Action action)
    {
        OnTurnEnd -= action;
    }

    public void StartTurn()
    {
        Debug.Log("Starting turn for " + gameObject);
        _turnActive = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(_turnActive)
        {
            if(endTurn)
            {
                endTurn = false;
                OnTurnEnd?.Invoke();
            }
        }
    }

    public void AddRemovedFromPlayListener(Action<ITurnOrderEntry> action)
    {
        throw new NotImplementedException();
    }

    public void RemoveRemovedFromPlayListener(Action<ITurnOrderEntry> action)
    {
        throw new NotImplementedException();
    }

    public void AddStartTurnListener(Action action)
    {
        throw new NotImplementedException();
    }

    public void RemoveStartTurnListener(Action action)
    {
        throw new NotImplementedException();
    }
}
