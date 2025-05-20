using System;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class UnityMainThreadDispatcher : MonoBehaviour
{
    private static readonly Queue<Action> _queue = new Queue<Action>();
    private static UnityMainThreadDispatcher _instance;

    public static void Enqueue(Action action)
    {
        if (_instance == null)
        {
            Debug.LogError("UnityMainThreadDispatcher not initialized. Add it to a GameObject in the scene.");
            return;
        }

        lock (_queue)
        {
            _queue.Enqueue(action);
        }
    }

    public static T EnqueueSync<T>(Func<T> func)
    {
        if (_instance == null)
        {
            Debug.LogError("UnityMainThreadDispatcher not initialized. Add it to a GameObject in the scene.");
            return default;
        }

        T result = default;
        ManualResetEvent doneEvent = new ManualResetEvent(false);

        Enqueue(() =>
        {
            result = func();
            doneEvent.Set(); // Signal the waiting thread
        });

        doneEvent.WaitOne(); // Wait here until main thread runs the function
        return result;
    }

    private void Update()
    {
        lock (_queue)
        {
            while (_queue.Count > 0)
            {
                _queue.Dequeue()?.Invoke();
            }
        }
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
    }
}