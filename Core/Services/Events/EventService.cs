using System;
using System.Collections.Generic;

internal static class EventService
{
    private static readonly Dictionary<Type, Action<GameEvent>> Events = new Dictionary<Type, Action<GameEvent>>();

    private static readonly Dictionary<Delegate, Action<GameEvent>> EventLookups =
        new Dictionary<Delegate, Action<GameEvent>>();

    internal static void Subscribe<T>(Action<T> evt) where T : GameEvent
    {
        if (!EventLookups.ContainsKey(evt))
        {
            void NewAction(GameEvent e) => evt((T) e);
                
            EventLookups[evt] = NewAction;

            if (Events.ContainsKey(typeof(T)))
                Events[typeof(T)] += NewAction;
            else
                Events[typeof(T)] = NewAction;
        }
    }

    internal static void UnSubscribe<T>(Action<T> evt) where T : GameEvent
    {
        if (EventLookups.TryGetValue(evt, out var action))
        {
            if (Events.TryGetValue(typeof(T), out var tempAction))
            {
                tempAction -= action;
                if (tempAction == null)
                    Events.Remove(typeof(T));
                else
                    Events[typeof(T)] = tempAction;
            }

            EventLookups.Remove(evt);
        }
    }

    internal static void Broadcast(GameEvent evt)
    {
        if (Events.TryGetValue(evt.GetType(), out var action))
            action.Invoke(evt);
    }

    internal static void Clear()
    {
        Events.Clear();
        EventLookups.Clear();
    }
}