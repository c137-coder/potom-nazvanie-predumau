using System;
using System.Collections.Generic;

public static class EventBus
{
    private static readonly Dictionary<Type, Delegate> handlers = new();

    public static void Subscribe<T>(Action<T> handler)
    {
        Type type = typeof(T);
        handlers[type] = handlers.TryGetValue(type, out Delegate existing)
            ? Delegate.Combine(existing, handler)
            : handler;
    }

    public static void Unsubscribe<T>(Action<T> handler)
    {
        Type type = typeof(T);
        if (!handlers.TryGetValue(type, out Delegate existing))
        {
            return;
        }

        Delegate combined = Delegate.Remove(existing, handler);
        if (combined == null)
        {
            handlers.Remove(type);
        }
        else
        {
            handlers[type] = combined;
        }
    }

    public static void Publish<T>(T eventData)
    {
        if (handlers.TryGetValue(typeof(T), out Delegate existing))
        {
            ((Action<T>)existing).Invoke(eventData);
        }
    }
}
