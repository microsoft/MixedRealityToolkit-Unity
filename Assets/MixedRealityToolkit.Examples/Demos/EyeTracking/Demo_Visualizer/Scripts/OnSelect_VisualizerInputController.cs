// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine;
using UnityEngine.Events;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos.EyeTracking
{
    /// <summary>
    /// When the button is selected, it triggers starting the specified scene.
    /// </summary>
    [RequireComponent(typeof(EyeTrackingTarget))]
    public class OnSelect_VisualizerInputController : BaseEyeFocusHandler, IMixedRealityPointerHandler
    {
        [SerializeField]
        public UnityEvent EventToTrigger;

        [SerializeField]
        [Tooltip("Optional AudioClip which is played when the button is selected.")]
        private AudioClip audio_OnSelect;

        private void OnTargetSelected()
        {
            Debug.LogFormat(">> [{0}] Selected! -- {1} -- {2}", name, ToString(), name);
        }

        void IMixedRealityPointerHandler.OnPointerClicked(MixedRealityPointerEventData eventData)
        {
            if (HasFocus)
            {
                OnTargetSelected();
            }
        }

        void IMixedRealityPointerHandler.OnPointerDown(MixedRealityPointerEventData eventData) { }

        void IMixedRealityPointerHandler.OnPointerUp(MixedRealityPointerEventData eventData) { }
    }
}
