using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scheduler : Singleton<Scheduler>
{
    public delegate void Schedulable();
    public delegate void Schedulable<T>(T value);

    private struct ScheduleQueueEntry
    {
        public Schedulable schedulable;
        public float delay;

        public ScheduleQueueEntry(Schedulable schedulable, float delay)
        {
            this.schedulable = schedulable;
            this.delay = delay;
        }
    }

    private static List<ScheduleQueueEntry> _toSchedule;

    protected new void Awake()
    {
        base.Awake();

        _toSchedule = new List<ScheduleQueueEntry>();
    }

    protected void Update()
    {
        if(_toSchedule.Count > 0)
        {
            foreach (ScheduleQueueEntry entry in _toSchedule)
            {
                Schedule(entry.schedulable, entry.delay);
            }

            _toSchedule.Clear();
        }
    }

    public static void Schedule(Schedulable schedulable, float delay)
    {
        Instance.StartCoroutine(WaitAndCall(schedulable, delay));
    }

    public static void ScheduleWithoutMainThread(Schedulable schedulable, float delay)
    {
        _toSchedule.Add(new ScheduleQueueEntry(schedulable, delay));
    }

    public static void Schedule<T>(Schedulable<T> schedulable, T value, float delay)
    {
        Instance.StartCoroutine(WaitAndCall(schedulable, value, delay));
    }

    public static void ScheduleForEndOfFrame(Schedulable schedulable)
    {
        Instance.StartCoroutine(WaitForEndFrameAndCall(schedulable));
    }

    public static void ScheduleForEndOfFrame<T>(Schedulable<T> schedulable, T value)
    {
        Instance.StartCoroutine(WaitForEndFrameAndCall(schedulable, value));
    }

    public static void ScheduleRealTime(Schedulable schedulable, float delay)
    {
        Instance.StartCoroutine(WaitAndCallRealTime(schedulable, delay));
    }
    
    public static void ScheduleRealTime<T>(Schedulable<T> schedulable, T value, float delay)
    {
        Instance.StartCoroutine(WaitAndCallRealTime(schedulable, value, delay));
    }

    private static IEnumerator WaitForEndFrameAndCall(Schedulable schedulable)
    {
        yield return new WaitForEndOfFrame();
        schedulable();
    }

    private static IEnumerator WaitForEndFrameAndCall<T>(Schedulable<T> schedulable, T value)
    {
        yield return new WaitForEndOfFrame();
        schedulable(value);
    }

    private static IEnumerator WaitAndCall(Schedulable schedulable, float delay)
    {
        yield return new WaitForSeconds(delay);
        schedulable();
    }

    private static IEnumerator WaitAndCall<T>(Schedulable<T> schedulable, T value, float delay)
    {
        yield return new WaitForSeconds(delay);
        schedulable(value);
    }

    private static IEnumerator WaitAndCallRealTime(Schedulable schedulable, float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        schedulable();
    }

    private static IEnumerator WaitAndCallRealTime<T>(Schedulable<T> schedulable, T value, float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        schedulable(value);
    }
}
