// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;
using UInput = UnityEngine.Input;
using Microsoft.MixedReality.Toolkit.Input;

namespace Microsoft.MixedReality.Toolkit.Input.UnityInput
{
    /// <summary>
    /// Manages the mouse using unity input system.
    /// </summary>
    [MixedRealityController(SupportedControllerType.Mouse, new[] { Handedness.Any })]
    public class MouseController : OrdinaryMouseController
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="trackingState"></param>
        /// <param name="controllerHandedness"></param>
        /// <param name="inputSource"></param>
        /// <param name="interactions"></param>
        public MouseController(TrackingState trackingState, Handedness controllerHandedness, IMixedRealityInputSource inputSource = null, MixedRealityInteractionMapping[] interactions = null)
            : base(trackingState, controllerHandedness, inputSource, interactions)
        {
#if UNITY_EDITOR
            if (UnityEditor.EditorWindow.focusedWindow != null)
            {
                UnityEditor.EditorWindow.focusedWindow.ShowNotification(new GUIContent("Press \"ESC\" to regain mouse control"));
            }
#endif

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}
