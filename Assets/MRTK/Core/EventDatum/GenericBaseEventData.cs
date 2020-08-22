// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using UnityEngine;
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
        /// The BaseEventData.selectedObject is explicitly hidden because access to it
        /// (either via get or set) throws a NullReferenceException in typical usage within
        /// the MRTK. Prefer using the subclasses own fields to access information about
        /// the event instead of fields on BaseEventData.
        /// </summary>
        /// <remarks>
        /// BaseEventData is only used because it's part of Unity's EventSystem dispatching,
        /// so this code must subclass it in order to leverage EventSystem.ExecuteEvents
        /// </remarks>
        public new GameObject selectedObject { get; protected set; }

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