// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine.EventSystems;

namespace Microsoft.MixedReality.Toolkit
{
    /// <summary>
    /// Generic Base Event Data for Sending Events through the Event System.
    /// </summary>
    public class GenericBaseEventData : BaseEventData
    {
        /// <summary>
        /// The Event Source that the event originates from.
        /// </summary>
        public IMixedRealityEventSource EventSource { get; private set; }

        /// <summary>
        /// The UTC time at which the event occurred.
        /// </summary>
        public DateTime EventTime { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="eventSystem">Usually <see href="https://docs.unity3d.com/ScriptReference/EventSystems.EventSystem-current.html">EventSystems.EventSystem.current</see></param>
        public GenericBaseEventData(EventSystem eventSystem) : base(eventSystem) { }

        /// <summary>
        /// Used to initialize/reset the event and populate the data.
        /// </summary>
        /// <param name="eventSource">The source of the event.</param>
        protected void BaseInitialize(IMixedRealityEventSource eventSource)
        {
            Reset();
            EventTime = DateTime.UtcNow;
            EventSource = eventSource;
        }
    }
}