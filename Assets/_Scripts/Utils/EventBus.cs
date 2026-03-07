using System.Collections.Generic;
using UnityEngine.Events;

public static class EventBus
{
    // Store events with no payload
    private static readonly Dictionary<string, UnityEvent> noPayloadEvents =
        new();

    // Store events with payload
    private static readonly Dictionary<string, object> payloadEvents = new();

    // Subscribe to a no-payload event
    public static void Subscribe(string key, UnityAction listener)
    {
        if (!noPayloadEvents.TryGetValue(key, out UnityEvent thisEvent))
        {
            thisEvent = new UnityEvent();
            noPayloadEvents[key] = thisEvent;
        }
        thisEvent.AddListener(listener);
    }

    // Publish a no-payload event
    public static void Publish(string key)
    {
        if (noPayloadEvents.TryGetValue(key, out UnityEvent thisEvent))
            thisEvent.Invoke();
    }

    // Subscribe to a payload event
    public static void Subscribe<T>(string key, UnityAction<T> listener)
    {
        if (!payloadEvents.TryGetValue(key, out object obj))
        {
            var newEvent = new UnityEvent<T>();
            payloadEvents[key] = newEvent;
            obj = newEvent;
        }
      ((UnityEvent<T>)obj).AddListener(listener);
    }

    // Publish a payload event
    public static void Publish<T>(string key, T payload)
    {
        if (payloadEvents.TryGetValue(key, out object obj))
            ((UnityEvent<T>)obj).Invoke(payload);
    }

    // Unsubscribe from a payload event
    public static void Unsubscribe<T>(string key, UnityAction<T> listener)
    {
        if (payloadEvents.TryGetValue(key, out object obj))
            ((UnityEvent<T>)obj).RemoveListener(listener);
    }

    // Unsubscribe from a no-payload event
    public static void Unsubscribe(string key, UnityAction listener)
    {
        if (noPayloadEvents.TryGetValue(key, out UnityEvent thisEvent))
            thisEvent.RemoveListener(listener);
    }
}
