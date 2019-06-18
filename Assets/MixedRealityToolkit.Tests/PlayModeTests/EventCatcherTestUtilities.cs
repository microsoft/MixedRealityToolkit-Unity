// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#if !WINDOWS_UWP
// When the .NET scripting backend is enabled and C# projects are built
// Unity doesn't include the required assemblies (i.e. the ones below).
// Given that the .NET backend is deprecated by Unity at this point it's we have
// to work around this on our end.
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities;
using NUnit.Framework;
using System;
using System.Collections;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

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
    public abstract class GlobalEventCatcher<T> : InputSystemGlobalListener, IDisposable where T : MonoBehaviour
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
    public class TouchEventCatcher : GlobalEventCatcher<TouchEventCatcher>, IMixedRealityTouchHandler
    {
        /// <inheritdoc />
        public void OnTouchCompleted(HandTrackingInputEventData eventData)
        {
            ++eventsCompleted;
        }

        /// <inheritdoc />
        public void OnTouchStarted(HandTrackingInputEventData eventData)
        {
            ++eventsStarted;
        }

        /// <inheritdoc />
        public void OnTouchUpdated(HandTrackingInputEventData eventData)
        {
        }
    }
}
#endif