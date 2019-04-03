// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using UnityEngine.Events;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// A game object with the "Target" script attached can be selected and may react to hover states.
    /// </summary>
    public class EyeTrackingTarget : BaseEyeFocusHandler, IMixedRealityPointerHandler, IMixedRealitySpeechHandler
    {
        [SerializeField]
        private MixedRealityInputAction selectAction = MixedRealityInputAction.None;

        [SerializeField]
        private MixedRealityInputAction[] voice_select = null;

        [Tooltip("If true, the game object is set to the center of the currently looked at target.")]
        public bool cursor_snapToCenter = false;

        [Tooltip("Event is triggered when the user starts to look at the target.")]
        [SerializeField]
        private UnityEvent OnLookAtStart = null;

        [Tooltip("Event is triggered when the user continues to look at the target.")]
        [SerializeField]
        private UnityEvent WhileLookingAtTarget = null;

        [Tooltip("Event to be triggered when the user is looking away from the target.")]
        [SerializeField]
        private UnityEvent OnLookAway = null;

        [Tooltip("Event to be triggered when the target has been looked at for a given predefined duration.")]
        [SerializeField]
        private UnityEvent OnDwell = null;

        [Tooltip("Event to be triggered when the looked at target is selected.")]
        [SerializeField]
        private UnityEvent OnSelected = null;

        #region BaseEyeFocusHandler

        protected override void OnEyeFocusStart()
        {
            OnLookAtStart.Invoke();
        }

        protected override void OnEyeFocusStay()
        {
            WhileLookingAtTarget.Invoke();
        }

        protected override void OnEyeFocusDwell()
        {
            OnDwell.Invoke();
        }

        protected override void OnEyeFocusStop()
        {
            OnLookAway.Invoke();
        }

        #endregion BaseEyeFocusHandler

        #region IMixedRealityPointerHandler

        void IMixedRealityPointerHandler.OnPointerUp(MixedRealityPointerEventData eventData) { }

        void IMixedRealityPointerHandler.OnPointerDown(MixedRealityPointerEventData eventData) { }

        void IMixedRealityPointerHandler.OnPointerClicked(MixedRealityPointerEventData eventData)
        {
            if (eventData.MixedRealityInputAction == selectAction && HasFocus)
            {
                OnSelected.Invoke();
            }
        }

        void IMixedRealitySpeechHandler.OnSpeechKeywordRecognized(SpeechEventData eventData)
        {
            for (int i = 0; i < voice_select.Length; i++)
            {
                if (eventData.MixedRealityInputAction == voice_select[i] && HasFocus)
                {
                    OnSelected.Invoke();
                }
            }
        }
        #endregion
    }
}