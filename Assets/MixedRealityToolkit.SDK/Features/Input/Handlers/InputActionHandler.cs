// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using UnityEngine.Events;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Script used to handle input action events. Invokes Unity events when the configured input action starts or ends. 
    /// </summary>
    public class InputActionHandler : BaseInputHandler, IMixedRealityInputActionHandler
    {
        [SerializeField]
        [Tooltip("Input Action to handle")]
        private MixedRealityInputAction InputAction = MixedRealityInputAction.None;

        [SerializeField]
        [Tooltip("Whether input events should be marked as used after handling so other handlers in the same game object ignore them")]
        private bool MarkEventsAsUsed = false;

        /// <summary>
        /// Unity event raised on action start, e.g. button pressed or gesture started. 
        /// Includes the input event that triggered the action.
        /// </summary>
        public InputActionUnityEvent OnInputActionStarted;

        /// <summary>
        /// Unity event raised on action end, e.g. button released or gesture completed.
        /// Includes the input event that triggered the action.
        /// </summary>
        public InputActionUnityEvent OnInputActionEnded;

        #region InputSystemGlobalHandlerListener Implementation

        /// <inheritdoc />
        protected override void RegisterHandlers()
        {
            InputSystem?.RegisterHandler<IMixedRealityInputActionHandler>(this);
        }

        /// <inheritdoc />
        protected override void UnregisterHandlers()
        {
            InputSystem?.UnregisterHandler<IMixedRealityInputActionHandler>(this);
        }

        #endregion InputSystemGlobalHandlerListener Implementation

        void IMixedRealityInputActionHandler.OnActionStarted(BaseInputEventData eventData)
        {
            if (eventData.MixedRealityInputAction == InputAction && !eventData.used)
            {
                OnInputActionStarted.Invoke(eventData);
                if (MarkEventsAsUsed)
                {
                    eventData.Use();
                }
            }
        }
        void IMixedRealityInputActionHandler.OnActionEnded(BaseInputEventData eventData)
        {
            if (eventData.MixedRealityInputAction == InputAction && !eventData.used)
            {
                OnInputActionEnded.Invoke(eventData);
                if (MarkEventsAsUsed)
                {
                    eventData.Use();
                }
            }
        }
    }
}