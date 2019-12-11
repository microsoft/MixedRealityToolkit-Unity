// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI
{
    public class ToggleHandVisualisation : MonoBehaviour
    {
        public bool isHandMeshVisible = false;
        public bool isHandJointVisible = false;

        /// <summary>
        /// Initial setting of hand mesh visualization - default is disabled
        /// </summary>
        private void Start()
        {
            UpdateHandVisibility();
        }

        /// <summary>
        /// Updates the hand tracking profile with the current local visualization settings
        /// </summary>
        private void UpdateHandVisibility()
        {
            MixedRealityHandTrackingProfile handTrackingProfile = CoreServices.InputSystem?.InputSystemProfile?.HandTrackingProfile;
            if (handTrackingProfile != null)
            {
                handTrackingProfile.EnableHandMeshVisualization = isHandMeshVisible;
                handTrackingProfile.EnableHandJointVisualization = isHandJointVisible;
            }
        }

        /// <summary>
        /// Toggles hand mesh visualization
        /// </summary>
        public void OnToggleHandMesh()
        {
            isHandMeshVisible = !isHandMeshVisible;
            UpdateHandVisibility();
        }

        /// <summary>
        /// Toggles hand joint visualization
        /// </summary>
        public void OnToggleHandJoint()
        {
            isHandJointVisible = !isHandJointVisible;
            UpdateHandVisibility();
        }
    }
}
