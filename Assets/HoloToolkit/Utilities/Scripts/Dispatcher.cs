// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoloToolkit.Unity
{
    /// <summary>
    /// Simple dispatcher class that queue up actions to be executed on the UI thread.
    /// </summary>
    public class Dispatcher : Singleton<Dispatcher>
    {
        private System.Object queueLock = new System.Object();
        private readonly Queue<Action> updateActionQueue = new Queue<Action>();

        private void Update()
        {
            while (updateActionQueue.Count > 0)
            {
                Action action;
                lock (queueLock)
                {
                    action = updateActionQueue.Dequeue();
                }
                action();
            }
        }

        /// <summary>
        /// Executes the specified action on the UI thread.
        /// </summary>
        /// <param name="action"></param>
        public void Execute(Action action)
        {
            lock (queueLock)
            {
                updateActionQueue.Enqueue(action);
            }
        }

        /// <summary>
        /// Executes the specified action after the a delay.
        /// </summary>
        /// <param name="action">Action to execute.</param>
        /// <param name="delay">Delay in seconds before the action should be executed.</param>
        public void DelayedExecute(Action action, float delay)
        {
            lock (queueLock)
            {
                updateActionQueue.Enqueue(() =>
                {
                    StartCoroutine(DelayedActionRunner(action, delay));
                });
            }
        }

        private IEnumerator DelayedActionRunner(Action action, float delay)
        {
            yield return new WaitForSeconds(delay);
            action();
        }
    }
}
