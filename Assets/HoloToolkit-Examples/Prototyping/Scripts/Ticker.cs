// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using System.Collections;

namespace HoloToolkit.Examples.Prototyping
{
    /// <summary>
    /// Ticker is an advanced coroutine instance that is easy to run and manage.
    /// Supports: Start, Stop, Pause and Resume, with OnComplete and OnUpdate funtionality.
    /// </summary>
    public class Ticker
    {
        public enum TickerEventType { OnTick, OnComplete}
        /// <summary>
        /// Is the coroutine running?
        /// </summary>
        public bool IsRunning { get { return mBaseRoutine != null; } }

        /// <summary>
        /// Pause the Ticker
        /// </summary>
        public bool IsPaused { get; set; }

        /// <summary>
        /// Callback delegates
        /// </summary>
        /// <param name="ticker"></param>
        public delegate void TickerEvent(Ticker ticker, TickerEventType type);

        /// <summary>
        /// Fired when Ticker completes
        /// </summary>
        public event TickerEvent OnComplete;

        /// <summary>
        /// Fired on Ticker updates - could use for animation
        /// </summary>
        public event TickerEvent OnTick;

        /// <summary>
        /// Cached custom data for later use
        /// </summary>
        public object CustomData { get; set; }

        /// <summary>
        /// Host game object to attach the coroutine to
        /// </summary>
        public MonoBehaviour HostMonoBehavior { get; private set; }

        /// <summary>
        /// the current time of the coroutine, if running
        /// </summary>
        public float CurrentTime { get; private set; }

        /// <summary>
        /// Length of time from start to completion
        /// </summary>
        public float Duration { get; set; }

        /// <summary>
        /// Internal properties needed to run the coroutine
        /// </summary>
        private Coroutine mBaseRoutine;

        /// <summary>
        /// Initiate the Ticker
        /// </summary>
        /// <param name="gameObject">Host gameObject to attach the coroutine to</param>
        /// <param name="duration">Length of time from state to completion</param>
        /// <param name="data">Custom data object for caching information needed OnComplete or OnUpdate</param>
        public Ticker(MonoBehaviour monoBehavior, float duration, object customData = null)
        {
            CurrentTime = 0;
            HostMonoBehavior = monoBehavior;
            Duration = duration;
            CustomData = customData;
        }

        //TickerCallbackFunction callback, TickerCallbackFunction update = null
        
        /// <summary>
        /// Start the Ticker
        /// </summary>
        public void Start()
        {
            CurrentTime = 0;

            if (mBaseRoutine != null)
            {
                HostMonoBehavior.StopCoroutine(mBaseRoutine);
            }

            if (CurrentTime < Duration)
            {
                mBaseRoutine = HostMonoBehavior.StartCoroutine(CoroutineInstructions());
            }
            else
            {
                SendOnTick();
                SendOnComplete();
            }
        }

        /// <summary>
        /// Stops and resets the Ticker. Does not call OnComplete
        /// </summary>
        public void Stop()
        {
            CurrentTime = 0;

            if (mBaseRoutine != null)
            {
                HostMonoBehavior.StopCoroutine(mBaseRoutine);
            }

            mBaseRoutine = null;
        }

        /// <summary>
        /// Fires the OnTick event
        /// </summary>
        private void SendOnTick()
        {
            if (OnTick != null)
                OnTick(this, TickerEventType.OnTick);
        }

        /// <summary>
        /// Fires the OnComplete event
        /// </summary>
        private void SendOnComplete()
        {
            if (OnComplete != null)
                OnComplete(this, TickerEventType.OnComplete);
        }

        /// <summary>
        /// Handle the coroutine updates
        /// </summary>
        /// <param name="start">Start or resume time</param>
        /// <param name="duration">The length of time from start to completion</param>
        /// <returns></returns>
        private IEnumerator CoroutineInstructions()
        {
            while (CurrentTime < Duration)
            {
                yield return new WaitUntil(() => !IsPaused);

                CurrentTime += Time.deltaTime;
                CurrentTime = Mathf.Clamp(CurrentTime, 0f, Duration);

                if (CurrentTime >= Duration)
                {
                    SendOnComplete();
                }
                else
                {
                    SendOnTick();
                }
            }

            mBaseRoutine = null;
        }
    }
}
