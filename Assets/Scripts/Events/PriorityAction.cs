using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class PriorityAction<T>
{
    public delegate void ActionHandler(T value, PriorityAction<T> context);
    
    private List<Entry> actionEntries;

    private bool _consumed;

    public PriorityAction()
    {
        actionEntries = new List<Entry>();
    }

    public void AddListener(ActionHandler value, int priority)
    {
        int targetIndex = Math.Max(actionEntries.Count - 1, 0);

        for(int i = 0; i < actionEntries.Count; i++)
        {
            if(priority > actionEntries[i].priority)
            {
                targetIndex = i + 1;
            }
        }

        actionEntries.Insert(targetIndex, new Entry(value, priority));
    }

    public void RemoveListener(ActionHandler toRemove)
    {
        Entry entryToRemove = actionEntries.Find(item => item.actionHandler == toRemove);
        actionEntries.Remove(entryToRemove);
    }

    private struct Entry : IEquatable<Entry>
    {
        public ActionHandler actionHandler;
        public int priority;

        public Entry(ActionHandler value, int priority)
        {
            this.actionHandler = value;
            this.priority = priority;
        }

        public bool Equals(Entry other)
        {
            return other.Equals(actionHandler);
        }
    }

    public static PriorityAction<T> operator +(PriorityAction<T> action, ActionHandler actionHandler)
    {
        //queue.actionHandlers.Add(actionHandler);
        action.AddListener(actionHandler, 0);

        return action;
    }

    /// <summary>
    /// Call from a subscribed action handler to prevent the calling of lower priority handlers after the current handler is finished.
    /// </summary>
    public void ConsumeEvent()
    {
        _consumed = true;
    }

    /// <summary>
    /// Calls all subscribed action handlers that share the same maximum priority
    /// </summary>
    public void Invoke(T value)
    {
        _consumed = false;

        int currentIndex = actionEntries.Count - 1;

        while(!_consumed && currentIndex >= 0)
        {
            actionEntries[currentIndex].actionHandler(value, this);

            currentIndex--;
        }
    }
}