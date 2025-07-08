#region

using System;
using System.Collections.Generic;
using UnityEngine;

#endregion

public class GlobalEvents : MonoBehaviour
{
    private static GlobalEvents _instance;

    private static readonly Dictionary<Type, List<Delegate>> Subscribers = new();

    public static void Subscribe<T>(Action<T> handler) where T : EventBase
    {
        var type = typeof(T);
        if (!Subscribers.ContainsKey(type))
            Subscribers[type] = new List<Delegate>();
        Subscribers[type].Add(handler);

        /*if (EventHistory.TryGetValue(type, out var events))
            events.ToList().ForEach(it => handler.Invoke(it as T));*/
    }

    public static void Unsubscribe<T>(Action<T> handler) where T : EventBase
    {
        var type = typeof(T);
        if (!Subscribers.TryGetValue(type, out var subscriber))
            return;
        subscriber.Remove(handler);
    }

    public static void Raise<T>(T eventToPublish) where T : EventBase
    {
        var type = eventToPublish.GetType();
        if (!Subscribers.TryGetValue(type, out var handlers))
            return;

        foreach (var handler in handlers)
            handler.DynamicInvoke(eventToPublish);
    }

    public static async void RaiseAsync<T>(T eventToPublish) where T : EventBase
    {
        Raise(eventToPublish);
    }
}