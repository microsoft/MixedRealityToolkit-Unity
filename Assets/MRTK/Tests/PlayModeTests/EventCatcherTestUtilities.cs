// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

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
using UnityEngine.UI;

namespace Microsoft.MixedReality.Toolkit.Tests
{
    /// <summary>
    /// Base class for counting events raised on the focused object.
    /// </summary>
    public abstract class FocusedObjectEventCatcher<T> : MonoBehaviour, IDisposable where T : MonoBehaviour
    {
        public int EventsStarted { get; protected set; } = 0;
        public int EventsCompleted { get; protected set; } = 0;

        public static T Create(GameObject gameObject)
        {
            return gameObject.AddComponent<T>();
        }

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
        public int EventsStarted { get; protected set; } = 0;
        public int EventsCompleted { get; protected set; } = 0;

        public static T Create()
        {
            GameObject go = new GameObject("GlobalEventCatcher");
            return go.AddComponent<T>();
        }

        public void Dispose()
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Utility for counting pointer events.
    /// </summary>
    internal class PointerEventCatcher : FocusedObjectEventCatcher<PointerEventCatcher>, IMixedRealityPointerHandler
    {
        public readonly UnityEvent OnPointerDownEvent = new UnityEvent();
        public readonly UnityEvent OnPointerUpEvent = new UnityEvent();
        public readonly UnityEvent OnPointerDraggedEvent = new UnityEvent();
        public readonly UnityEvent OnPointerClickedEvent = new UnityEvent();

        public int DragEventCount = 0;

        /// <inheritdoc />
        public void OnPointerClicked(MixedRealityPointerEventData eventData)
        {
            OnPointerClickedEvent.Invoke();
        }

        /// <inheritdoc />
        public void OnPointerDown(MixedRealityPointerEventData eventData)
        {
            ++EventsStarted;
            OnPointerDownEvent.Invoke();
        }

        /// <inheritdoc />
        public void OnPointerDragged(MixedRealityPointerEventData eventData)
        {
            ++DragEventCount;
            OnPointerDraggedEvent.Invoke();
        }

        /// <inheritdoc />
        public void OnPointerUp(MixedRealityPointerEventData eventData)
        {
            ++EventsCompleted;
            OnPointerUpEvent.Invoke();
        }


    }

    /// <summary>
    /// Utility for counting touch events.
    /// </summary>
    public class TouchEventCatcher : FocusedObjectEventCatcher<TouchEventCatcher>, IMixedRealityTouchHandler
    {
        public readonly UnityEvent OnTouchStartedEvent = new UnityEvent();
        public readonly UnityEvent OnTouchCompletedEvent = new UnityEvent();

        /// <inheritdoc />
        public void OnTouchCompleted(HandTrackingInputEventData eventData)
        {
            ++EventsCompleted;

            OnTouchCompletedEvent.Invoke();
        }

        /// <inheritdoc />
        public void OnTouchStarted(HandTrackingInputEventData eventData)
        {
            ++EventsStarted;

            OnTouchStartedEvent.Invoke();
        }

        /// <inheritdoc />
        public void OnTouchUpdated(HandTrackingInputEventData eventData)
        {
        }
    }

    /// <summary>
    /// Base class for counting Unity button events.
    /// </summary>
    public class UnityButtonEventCatcher : IDisposable
    {
        public int Click { get; protected set; } = 0;

        private Button button;

        public UnityButtonEventCatcher(Button button)
        {
            this.button = button;
            button.onClick.AddListener(OnClick);
        }

        public void Dispose()
        {
            button.onClick.RemoveListener(OnClick);
        }

        private void OnClick()
        {
            ++Click;
        }
    }

    /// <summary>
    /// Base class for counting Unity button events.
    /// </summary>
    public class UnityToggleEventCatcher : IDisposable
    {
        public int Changed { get; protected set; } = 0;
        public bool IsOn { get; protected set; }

        private Toggle toggle;

        public UnityToggleEventCatcher(Toggle toggle)
        {
            this.toggle = toggle;
            this.IsOn = toggle.isOn;
            toggle.onValueChanged.AddListener(OnValueChanged);
        }

        public void Dispose()
        {
            toggle.onValueChanged.RemoveListener(OnValueChanged);
        }

        private void OnValueChanged(bool value)
        {
            ++Changed;
            IsOn = toggle.isOn;
        }
    }
}
#endif