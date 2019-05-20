// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using UnityEngine.Events;

namespace Microsoft.MixedReality.Toolkit.Input
{
    public class InputActionHandler : BaseInputHandler, IMixedRealityInputActionHandler
    {
        [SerializeField]
        [Tooltip("Input Action to handle")]
        private MixedRealityInputAction InputAction = MixedRealityInputAction.None;

        [SerializeField]
        [Tooltip("")]
        private bool MarkEventsAsUsed = false;

        [System.Serializable]
        public class InputActionUnityEvent : UnityEvent<BaseInputEventData> { }

        public InputActionUnityEvent OnInputActionStarted;
        public InputActionUnityEvent OnInputActionEnded;

        void IMixedRealityInputActionHandler.OnActionStarted(BaseInputEventData eventData)
        {
            if (eventData.MixedRealityInputAction == InputAction)
            {
                // TODO Remove
                Debug.Log("STARTED " + eventData);

                OnInputActionStarted.Invoke(eventData);
                if (MarkEventsAsUsed)
                {
                    eventData.Use();
                }
            }
        }
        void IMixedRealityInputActionHandler.OnActionEnded(BaseInputEventData eventData)
        {
            if (eventData.MixedRealityInputAction == InputAction)
            {
                // TODO Remove
                Debug.Log("ENDED " + eventData);

                OnInputActionEnded.Invoke(eventData);
                if (MarkEventsAsUsed)
                {
                    eventData.Use();
                }
            }
        }
    }
}