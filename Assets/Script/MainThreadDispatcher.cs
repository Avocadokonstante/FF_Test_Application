using UnityEngine;
using System;
using System.Collections.Generic;

public class MainThreadDispatcher : MonoBehaviour
{
    private static Queue<Action> actionQueue = new Queue<Action>();

    // Update is called once per frame
    void Update()
    {
        // Execute queued actions on the main thread
        while (actionQueue.Count > 0)
        {
            Action action = actionQueue.Dequeue();
            action?.Invoke();
        }
    }

    // Enqueue an action to be executed on the main thread
    /// <summary>
    /// This method puts a code part in a queue that is to be dispatched from the main thread, like e.g. creating
    /// gameObjects
    /// <example>
    /// Can be used like the following:
    /// MainThreadDispatcher.Enqueue(() => {Code}); 
    /// <\example>
    /// The code must be able to run without context, e.g. can't use loop variables if they are not included in the
    /// dispatched action
    /// </summary>
    /// <param name="action"> Code that shall be dispatched on the main thread </param>
    public static void Enqueue(Action action)
    {
        actionQueue.Enqueue(action);
    }
}

