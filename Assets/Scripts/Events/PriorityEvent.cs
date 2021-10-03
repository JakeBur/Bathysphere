using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

/// <summary>
/// A priority event is a dynamic list of delegates with priorities.
/// When the priority event is invoked, it calls all subscribed delegates in priority order.
/// Any subscribed delegate can consume the event, preventing it from calling any lower priority events.
/// </summary>
/// <typeparam name="T">The data that should be supplied by the event as a parameter when it is invoked.</typeparam>
public class PriorityEvent<T>
{
    /// <summary>
    /// Any subscribers must match this delegate.
    /// </summary>
    /// <param name="value">The value that will be supplied by invocations of this event.</param>
    /// <param name="context">The event itself, used by callers to potentially consume the event.</param>
    public delegate void ActionHandler(T value, PriorityEvent<T> context);
    
    /// <summary>
    /// List of prioritized entries. Maintained in order of rising priority.
    /// </summary>
    private List<Entry> _actionEntries;

    /// <summary>
    /// Tracker for if a subscriber has consumed the current invocation of this event.
    /// </summary>
    private bool _consumed;

    public PriorityEvent()
    {
        _actionEntries = new List<Entry>();
    }

    /// <summary>
    /// Adds a listener to the event with the given priority.
    /// </summary>
    /// <param name="listener">The listener.</param>
    /// <param name="priority">The listener's priority. Higher values are executed first.</param>
    public void AddListener(ActionHandler listener, int priority)
    {
        int targetIndex = Math.Max(_actionEntries.Count - 1, 0);

        for(int i = 0; i < _actionEntries.Count; i++)
        {
            if(priority > _actionEntries[i].priority)
            {
                targetIndex = i + 1;
            }
        }

        _actionEntries.Insert(targetIndex, new Entry(listener, priority));
    }

    /// <summary>
    /// Removes the given subscriber from the list of entries.
    /// </summary>
    /// <param name="toRemove">The subscriber to remove.</param>
    public void RemoveListener(ActionHandler toRemove)
    {
        Entry entryToRemove = _actionEntries.Find(item => item.actionHandler == toRemove);
        _actionEntries.Remove(entryToRemove);
    }

    /// <summary>
    /// An Entry is holds an ActionHandler and a integer priority, to be tracked by the PriorityEvent.
    /// </summary>
    private struct Entry : IEquatable<Entry>
    {
        /// <summary>
        /// The delegate to be called for this entry.
        /// </summary>
        public ActionHandler actionHandler;

        /// <summary>
        /// The priority of this entry. Higher values are executed first.
        /// </summary>
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

    public static PriorityEvent<T> operator +(PriorityEvent<T> action, ActionHandler actionHandler)
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
    /// Calls all subscribed action handlers in order from highest priority to lowest priority.
    /// Interrupted by a call to ConsumeEvent();
    /// </summary>
    public void Invoke(T value)
    {
        _consumed = false;

        int currentIndex = _actionEntries.Count - 1;

        while(!_consumed && currentIndex >= 0)
        {
            _actionEntries[currentIndex].actionHandler(value, this);

            currentIndex--;
        }
    }
}