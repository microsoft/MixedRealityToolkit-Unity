// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit
{
    /// <summary>
    /// Attribute to enable an inspector checkbox for
    /// toggling the state at edit-time. Mostly used for Toggle state.
    /// </summary>
    public class EditableTimedFlagAttribute : PropertyAttribute { }

    /// <summary>
    /// Represents a flag that can be activated or deactivated, and whose active duration is tracked and 
    /// obtained via the <see cref="GetTimeElapsedSinceSetActive"/> function. Instances of this object
    /// will also raise <see cref="OnEntered"/> and <see cref "OnExited"/> events when their activate state is altered.
    /// </summary>
    [System.Serializable]
    public class TimedFlag
    {
        // Convenience operator overload.
        public static implicit operator bool(TimedFlag flag) => flag.Active;

        [SerializeField, HideInInspector]
        private bool active = false;

        /// <summary>
        /// Whether or not the state is currently active.
        /// </summary>
        public bool Active
        {
            get => active;
            internal set
            {
                if (value != active)
                {
                    active = value;

                    if (active)
                    {
                        startTime = Time.time;
                        OnEntered.Invoke(startTime);
                    }
                    else
                    {
                        endTime = Time.time;
                        OnExited.Invoke(endTime);
                    }
                }
            }
        }

        /// <summary>
        /// Initialize the state to a particular value, without firing
        /// any events or listeners.
        /// </summary>
        /// <param name="value">The value to initialize the state to.</param>
        public void Initialize(bool value)
        {
            active = value;
        }

        private float startTime;

        /// <summary>
        /// The time this state was set to active
        /// </summary>
        public float StartTime
        {
            get => startTime;
        }

        private float endTime;

        /// <summary>
        /// The time this state was set to inactive (active = false)
        /// </summary>
        public float EndTime
        {
            get => endTime;
        }

        [SerializeField]
        private StatefulTimerEvent onEntered = new StatefulTimerEvent();

        /// <summary>
        /// Fired when the state is set to active. Float argument is the
        /// time at which the event occurred.
        /// </summary>
        public StatefulTimerEvent OnEntered
        {
            get => onEntered;
        }

        [SerializeField]
        private StatefulTimerEvent onExited = new StatefulTimerEvent();

        /// <summary>
        /// Fired when the state is set to active. Float argument is the
        /// time at which the event occurred.
        /// </summary>
        public StatefulTimerEvent OnExited
        {
            get => onExited;
        }

        /// <summary>
        /// Get the amount of time that has passed since this state was set to active.
        /// </summary>
        /// <returns>Time elapsed in seconds</returns>
        public float GetTimeElapsedSinceSetActive()
        {
            if (Active)
            {
                return Time.time - StartTime;
            }
            else
            {
                return 0;
            }
        }

        public override string ToString()
        {
            return Active.ToString();
        }
    }
}
