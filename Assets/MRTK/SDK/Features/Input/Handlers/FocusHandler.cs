// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;
using UnityEngine.Events;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Utility component to hook up Unity events to the OnFocusEnter and OnFocusExit events.
    /// </summary>
    [AddComponentMenu("Scripts/MRTK/SDK/FocusHandler")]
    public class FocusHandler : MonoBehaviour, IMixedRealityFocusHandler
    {
        [SerializeField]
        [Tooltip("Whether input events should be marked as used after handling so other handlers in the same game object ignore them.")]
        private bool markEventsAsUsed = false;

        /// <summary>
        /// Whether input events should be marked as used after handling so other handlers in the same game object ignore them.
        /// </summary>
        public bool MarkEventsAsUsed
        {
            get { return markEventsAsUsed; }
            set { markEventsAsUsed = value; }
        }

        [SerializeField]
        [Tooltip("Event which is triggered when focus begins.")]
        private UnityEvent onFocusEnterEvent = new UnityEvent();

        /// <summary>
        /// Event which is triggered when focus begins.
        /// </summary>
        public UnityEvent OnFocusEnterEvent
        {
            get { return onFocusEnterEvent; }
            set { onFocusEnterEvent = value; }
        }

        [SerializeField]
        [Tooltip("Event which is triggered when focus ends.")]
        private UnityEvent onFocusExitEvent = new UnityEvent();

        /// <summary>
        /// Event which is triggered when focus ends.
        /// </summary>
        public UnityEvent OnFocusExitEvent
        {
            get { return onFocusExitEvent; }
            set { onFocusExitEvent = value; }
        }

        /// <inheritdoc />
        public void OnFocusEnter(FocusEventData eventData)
        {
            if (!eventData.used)
            {
                onFocusEnterEvent.Invoke();

                if (markEventsAsUsed)
                {
                    eventData.Use();
                }
            }
        }

        /// <inheritdoc />
        public void OnFocusExit(FocusEventData eventData)
        {
            if (!eventData.used)
            {
                onFocusExitEvent.Invoke();

                if (markEventsAsUsed)
                {
                    eventData.Use();
                }
            }
        }
    }
}
