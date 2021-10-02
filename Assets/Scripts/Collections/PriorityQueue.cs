using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public struct PriorityQueueEntry<T> : IEquatable<T> where T : IEquatable<T>
{
    public T value;
    public int priority;

    public PriorityQueueEntry(T value, int priority)
    {
        this.value = value;
        this.priority = priority;
    }

    public bool Equals(T other)
    {
        return other.Equals(value);
    }
}

public class PriorityQueue<T> where T : IEquatable<T>
{
    private Queue<PriorityQueueEntry<T>> queue;

    public PriorityQueue()
    {
        queue = new Queue<PriorityQueueEntry<T>>();
    }

    public T Peek()
    {
        return queue.Peek().value;
    }

    public void Enqueue(T value, int priority)
    {
        queue.Enqueue(new PriorityQueueEntry<T>(value, priority));
    }
    public T Dequeue()
    {
        return queue.Dequeue().value; 
    }

    public void RemoveElement(T toRemove)
    {
        List<PriorityQueueEntry<T>> listForm = queue.ToList();

        PriorityQueueEntry<T> entryToRemove = listForm.Find(item => item.value.Equals(toRemove));
        listForm.Remove(entryToRemove);
    }
}
