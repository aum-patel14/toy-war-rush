// TOY WAR RUSH - EventBus.cs
// Global event bus for decoupled system communication.

using System;
using System.Collections.Generic;

public static class EventBus
{
    private static readonly Dictionary<string, Delegate> Events = new();

    public static void Subscribe<T>(string eventName, Action<T> handler)
    {
        if (Events.TryGetValue(eventName, out var existing))
            Events[eventName] = Delegate.Combine(existing, handler);
        else
            Events[eventName] = handler;
    }

    public static void Subscribe(string eventName, Action handler)
    {
        if (Events.TryGetValue(eventName, out var existing))
            Events[eventName] = Delegate.Combine(existing, handler);
        else
            Events[eventName] = handler;
    }

    public static void Unsubscribe<T>(string eventName, Action<T> handler)
    {
        if (Events.TryGetValue(eventName, out var existing))
        {
            var result = Delegate.Remove(existing, handler);
            if (result == null)
                Events.Remove(eventName);
            else
                Events[eventName] = result;
        }
    }

    public static void Unsubscribe(string eventName, Action handler)
    {
        if (Events.TryGetValue(eventName, out var existing))
        {
            var result = Delegate.Remove(existing, handler);
            if (result == null)
                Events.Remove(eventName);
            else
                Events[eventName] = result;
        }
    }

    public static void Publish<T>(string eventName, T data)
    {
        if (Events.TryGetValue(eventName, out var handler))
            (handler as Action<T>)?.Invoke(data);
    }

    public static void Publish(string eventName)
    {
        if (Events.TryGetValue(eventName, out var handler))
            (handler as Action)?.Invoke();
    }

    public static void Clear() => Events.Clear();
}

public static class GameEvents
{
    public const string LevelLoaded = "LevelLoaded";
    public const string FortressDestroyed = "FortressDestroyed";
    public const string GatePassed = "GatePassed";
    public const string ObstacleHit = "ObstacleHit";
    public const string UnitMerged = "UnitMerged";
    public const string CoinsChanged = "CoinsChanged";
}
