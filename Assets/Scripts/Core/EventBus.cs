using System;
using System.Collections.Generic;
using UnityEngine;

public interface IEvent
{
    
}

public abstract class BaseEvent : IEvent
{
    public DateTime Timestamp { get; private set; }

    protected BaseEvent()
    {
        Timestamp = DateTime.Now;
    }
}

public class EventBus : Singleton<EventBus>
{
    private readonly Dictionary<Type, List<object>> _eventSubscriptions = new Dictionary<Type, List<object>>();

    protected override void Awake()
    {
        _destroyOnLoad = true;
        base.Awake();
    }

    public void Subscribe<T>(Action<T> handler) where T : IEvent
    {
        var eventType = typeof(T);

        if (!_eventSubscriptions.ContainsKey(eventType))
        {
            _eventSubscriptions[eventType] = new List<object>();
        }

        _eventSubscriptions[eventType].Add(handler);
    }

    public void Unsubscribe<T>(Action<T> handler) where T : IEvent
    {
        var eventType = typeof(T);

        if (_eventSubscriptions.ContainsKey(eventType))
        {
            _eventSubscriptions[eventType].Remove(handler);

            if (_eventSubscriptions[eventType].Count == 0)
            {
                _eventSubscriptions.Remove(eventType);
            }
        }
    }

    public void Publish<T>(T eventData) where T : IEvent
    {
        var eventType = typeof(T);

        if (_eventSubscriptions.ContainsKey(eventType))
        {
            var handlers = new List<object>(_eventSubscriptions[eventType]);

            foreach (var handler in handlers)
            {
                try
                {
                    ((Action<T>)handler).Invoke(eventData);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"[EventBus] Error handling event {eventType.Name}: {ex.Message}");
                }
            }
        }
    }

    public void Clear()
    {
        _eventSubscriptions.Clear();
    }

    public void Clear<T>() where T : IEvent
    {
        var eventType = typeof(T);
        if (_eventSubscriptions.ContainsKey(eventType))
        {
            _eventSubscriptions.Remove(eventType);
        }
    }

    protected override void OnDestroy()
    {
        Clear();
        base.OnDestroy();
    }
}