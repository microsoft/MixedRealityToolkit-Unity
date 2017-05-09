// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using UnityEngine;

namespace HoloToolkit.Unity
{
    /// <summary>
    /// A scheduler that manages various timers.
    /// </summary>
    /// <example>
    /// <code>
    ///     private int myTimer = 0;
    ///     
    ///     private void MyTimerCallback()
    ///     {
    ///         // Do stuff
    ///     }
    ///     
    ///     private void StartMyTimer()
    ///     {
    ///         float durationInSec = 0.5f;
    ///         myTimer = TimerId.Start(durationInSec, MyTimerCallback);
    ///     }
    ///     
    ///     private void StopMyTimer()
    ///     {
    ///         myTimer.Stop();
    ///     }
    /// </code>
    /// </example>
    public sealed class TimerScheduler : Singleton<TimerScheduler>
    {
        private struct TimerData
        {
            public Callback Callback;
            public float Duration;
            public bool Loop;
            public int Id;

            public TimerData(float time, Callback callback, bool loop, int id)
            {
                Callback = callback;
                Loop = loop;
                Duration = time;
                Id = id;
            }
        }

        private struct TimerIdPair
        {
            public int Id;
            public int KeyTime;
        }

        public delegate void Callback();

        private PriorityQueue<int, TimerData> timers;
        private List<TimerData> deferredTimers;
        private List<TimerIdPair> activeTimers;
        private int nextTimerId;

        protected override void Awake()
        {
            base.Awake();

            timers = new PriorityQueue<int, TimerData>();
            deferredTimers = new List<TimerData>(10);
            activeTimers = new List<TimerIdPair>(20);

            nextTimerId = 1; // 0 is reserved
        }

        private int GetKeyTime(float time)
        {
            // key time is nearest millisecond
            return (int)(time * 1000);
        }

        private bool HasKeyTimePassed(int key)
        {
            return key <= GetKeyTime(Time.time);
        }

        private int AddTimer(TimerData timer, bool deferred = false)
        {
            if (deferred)
            {
                deferredTimers.Add(timer);
            }
            else
            {
                // calculate the key time for the evaluation point. Multiple timers can have this value.
                int keyTime = GetKeyTime(Time.time + timer.Duration);

                // re-add timer to queue
                timers.Push(keyTime, timer);

                // add to list of active timers
                activeTimers.Add(new TimerIdPair { Id = timer.Id, KeyTime = keyTime });
            }

            // make sure the scheduler is enabled now that we have a new timer.
            enabled = true;

            return timer.Id;
        }

        private int GetActiveTimerIndex(int timerId)
        {
            for (int i = 0; i < activeTimers.Count; ++i)
            {
                if (activeTimers[i].Id == timerId)
                {
                    return i;
                }
            }

            return -1;
        }

        private bool RemoveActiveTimer(int timerId)
        {
            int index = GetActiveTimerIndex(timerId);
            if (index > -1)
            {
                activeTimers.RemoveAt(index);
                return true;
            }

            return false;
        }

        private int GetTimerDeferredIndex(int timerId)
        {
            for (int i = 0; i < deferredTimers.Count; ++i)
            {
                if (deferredTimers[i].Id == timerId)
                {
                    return i;
                }
            }

            return -1;
        }

        private void AddDeferredTimers()
        {
            for (int i = 0; i < deferredTimers.Count; i++)
            {
                AddTimer(deferredTimers[i]);
            }

            deferredTimers.Clear();
        }

        private void Update()
        {
            // while waiting for an event to happen, we'll just early out
            while (timers.Count > 0 && HasKeyTimePassed(timers.Top.Key))
            {
                TimerData timer = timers.Top.Value;

                // remove from active timers
                RemoveActiveTimer(timer.Id);

                // remove from queue
                timers.Pop();

                // loop events by just reinserting them
                if (timer.Loop)
                {
                    // re-add timer
                    AddTimer(timer);
                }

                // activate the callback. call this after loop reinsertion, because
                // the callback could call StopTimer.
                timer.Callback();
            }

            AddDeferredTimers();

            // if there are no active timers there is no need to update every frame.
            if (timers.Count == 0)
            {
                enabled = false;
            }
        }

        /// <summary>
        /// Creates a new timer event which will be added next frame.
        /// </summary>
        /// <param name="timeSeconds"></param>
        /// <param name="callback"></param>
        /// <param name="loop"></param>
        /// <param name="deferred"> Deferred timers will be pushed to the priority queue during next update</param>
        public Timer StartTimer(float timeSeconds, Callback callback, bool loop = false, bool deferred = false)
        {
            int id = AddTimer(new TimerData(timeSeconds, callback, loop, nextTimerId++), deferred);

            // create a new id, and make a new timer with it
            return new Timer(id);
        }

        /// <summary>
        /// Disable an active timer. 
        /// </summary>
        /// <param name="timerId"></param>
        /// <returns></returns>
        public void StopTimer(Timer timerId)
        {
            if (timerId.Id != Timer.Invalid.Id)
            {
                int index = GetActiveTimerIndex(timerId.Id);
                if (index > -1)
                {
                    int priority = activeTimers[index].KeyTime;
                    activeTimers.RemoveAt(index);

                    // TODO: remove specific value
                    // allocation here is fine, since it's a temporary, and will get cleaned in gen 0 GC
                    int id = timerId.Id;
                    timers.RemoveAtPriority(priority, t => t.Id == id);
                }
                else
                {
                    int deferredIndex = GetTimerDeferredIndex(timerId.Id);
                    if (deferredIndex > -1)
                    {
                        deferredTimers.RemoveAt(deferredIndex);
                    }
                }
            }
        }

        public bool IsTimerActive(Timer timerId)
        {
            return GetActiveTimerIndex(timerId.Id) > -1 || GetTimerDeferredIndex(timerId.Id) > -1;
        }
    }
}