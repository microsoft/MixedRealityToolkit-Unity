// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Script used to handle input action events. Invokes Unity events when the configured input action starts or ends. 
    /// </summary>
    [AddComponentMenu("Scripts/MRTK/SDK/InputActionHandler")]
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
            CoreServices.InputSystem?.RegisterHandler<IMixedRealityInputActionHandler>(this);
        }

        /// <inheritdoc />
        protected override void UnregisterHandlers()
        {
            CoreServices.InputSystem?.UnregisterHandler<IMixedRealityInputActionHandler>(this);
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