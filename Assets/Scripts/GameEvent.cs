using System;
using System.Collections.Generic;
using UnityEngine;

public class GameEvent
{
    private event Action GameAction;

    public void Publish()
    {
        try
        {
            GameAction?.Invoke();
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error invoking event: {ex.Message}");
        }
    }

    public void Subscribe(Action subscriber)
    {
        GameAction += subscriber;
    }

    public void Unsubscribe(Action subscriber)
    {
        GameAction -= subscriber;
    }

    public void DebugSubscribers()
    {
        foreach (var subscriber in GameAction.GetInvocationList())
        {
            Debug.Log($"Subscriber: {subscriber.Method.Name} Target: {subscriber.Target}");
        }
    }
}

public class GameEvent<T>
{
    private event Action<T> GameAction = delegate { };

    public void Publish(T param)
    {
        try
        {
            GameAction?.Invoke(param);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error invoking event: {ex.Message}");
        }
    }

    public void Subscribe(Action<T> subscriber)
    {
        GameAction += subscriber;
    }

    public void Unsubscribe(Action<T> subscriber)
    {
        GameAction -= subscriber;
    }

    public void DebugSubscribers()
    {
        foreach (var subscriber in GameAction.GetInvocationList())
        {
            Debug.Log($"Subscriber: {subscriber.Method.Name} Target: {subscriber.Target}");
        }
    }
}

public class GameEvent<S, T>
{
    private event Action<S, T> GameAction = delegate { };

    public void Publish(S param1, T param2)
    {
        try
        {
            GameAction?.Invoke(param1, param2);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error invoking event: {ex.Message}");
        }
    }

    public void Subscribe(Action<S, T> subscriber)
    {
        GameAction += subscriber;
    }

    public void Unsubscribe(Action<S, T> subscriber)
    {
        GameAction -= subscriber;
    }

    public void DebugSubscribers()
    {
        foreach (var subscriber in GameAction.GetInvocationList())
        {
            Debug.Log($"Subscriber: {subscriber.Method.Name} Target: {subscriber.Target}");
        }
    }
}

public static class Events
{
    public static readonly GameEvent AudioStart = new();
    public static readonly GameEvent AudioStop = new();
    public static readonly GameEvent AudioSkip = new();
    public static readonly GameEvent<string> DisplaySubtitles = new();
    public static readonly GameEvent SubtitlesSkip = new();
}
