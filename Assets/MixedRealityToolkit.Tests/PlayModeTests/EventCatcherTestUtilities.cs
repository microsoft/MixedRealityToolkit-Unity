// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#if !WINDOWS_UWP
// When the .NET scripting backend is enabled and C# projects are built
// The assembly that this file is part of is still built for the player,
// even though the assembly itself is marked as a test assembly (this is not
// expected because test assemblies should not be included in player builds).
// Because the .NET backend is deprecated in 2018 and removed in 2019 and this
// issue will likely persist for 2018, this issue is worked around by wrapping all
// play mode tests in this check.

using Microsoft.MixedReality.Toolkit.Input;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace Microsoft.MixedReality.Toolkit.Tests
{
    /// <summary>
    /// Base class for counting events raised on the focused object.
    /// </summary>
    public abstract class FocusedObjectEventCatcher<T> : MonoBehaviour, IDisposable where T : MonoBehaviour
    {
        protected int eventsStarted = 0;
        public int EventsStarted => eventsStarted;

        protected int eventsCompleted = 0;
        public int EventsCompleted => eventsCompleted;

        public static T Create(GameObject gameObject)
        {
            return gameObject.AddComponent<T>();
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Destroy(this);
        }
    }

    /// <summary>
    /// Base class for counting global events.
    /// </summary>
    public abstract class GlobalEventCatcher<T> : InputSystemGlobalHandlerListener, IDisposable where T : MonoBehaviour
    {
        protected int eventsStarted = 0;
        public int EventsStarted => eventsStarted;

        protected int eventsCompleted = 0;
        public int EventsCompleted => eventsCompleted;

        public static T Create()
        {
            GameObject go = new GameObject("GlobalEventCatcher");
            return go.AddComponent<T>();
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Destroy(this.gameObject);
        }
    }

    /// <summary>
    /// Utility for counting touch events.
    /// </summary>
    /// <remarks>
    /// Touching an object does not imply getting focus, so use a global event handler to be independent from focus.
    /// </remarks>
    public class TouchEventCatcher : FocusedObjectEventCatcher<TouchEventCatcher>, IMixedRealityTouchHandler
    {
        public readonly UnityEvent OnTouchStartedEvent = new UnityEvent();
        public readonly UnityEvent OnTouchCompletedEvent = new UnityEvent();

        /// <inheritdoc />
        public void OnTouchCompleted(HandTrackingInputEventData eventData)
        {
            ++eventsCompleted;

            OnTouchCompletedEvent.Invoke();
        }

        /// <inheritdoc />
        public void OnTouchStarted(HandTrackingInputEventData eventData)
        {
            ++eventsStarted;

            OnTouchStartedEvent.Invoke();
        }

        /// <inheritdoc />
        public void OnTouchUpdated(HandTrackingInputEventData eventData)
        {
        }
    }
}
#endif