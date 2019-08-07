// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine;
using UnityEngine.Events;

namespace Microsoft.MixedReality.Toolkit.UI
{
    /// <summary>
    /// Utility component to hook up Unity events to the OnFocusEnter and OnFocusExit events.
    /// </summary>
    public class FocusHandlerTrigger : MonoBehaviour, IMixedRealityFocusHandler
    {
        /// <summary>
        /// Event which is triggered when focus begins.
        /// </summary>
        [Header("Events")]
        public UnityEvent OnFocusEnterEvent;

        /// <summary>
        /// Event which is triggered when focus ends.
        /// </summary>
        public UnityEvent OnFocusExitEvent;

        /// <inheritdoc />
        public void OnFocusEnter(FocusEventData eventData)
        {
            OnFocusEnterEvent.Invoke();
        }

        /// <inheritdoc />
        public void OnFocusExit(FocusEventData eventData)
        {
            OnFocusExitEvent.Invoke();
        }
    }
}
