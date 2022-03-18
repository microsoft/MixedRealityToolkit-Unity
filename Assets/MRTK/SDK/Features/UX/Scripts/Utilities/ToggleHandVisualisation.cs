// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI
{
    [AddComponentMenu("Scripts/MRTK/SDK/ToggleHandVisualisation")]
    public class ToggleHandVisualisation : MonoBehaviour
    {
        /// <summary>
        /// Toggles hand mesh visualization
        /// </summary>
        public void OnToggleHandMesh()
        {
            MixedRealityInputSystemProfile inputSystemProfile = CoreServices.InputSystem?.InputSystemProfile;
            if (inputSystemProfile == null)
            {
                return;
            }

            MixedRealityHandTrackingProfile handTrackingProfile = inputSystemProfile.HandTrackingProfile;
            if (handTrackingProfile != null)
            {
                handTrackingProfile.EnableHandMeshVisualization = !handTrackingProfile.EnableHandMeshVisualization;
            }
        }

        /// <summary>
        /// Toggles hand joint visualization
        /// </summary>
        public void OnToggleHandJoint()
        {
            MixedRealityHandTrackingProfile handTrackingProfile = null;

            if (CoreServices.InputSystem?.InputSystemProfile != null)
            {
                handTrackingProfile = CoreServices.InputSystem.InputSystemProfile.HandTrackingProfile;
            }

            if (handTrackingProfile != null)
            {
                handTrackingProfile.EnableHandJointVisualization = !handTrackingProfile.EnableHandJointVisualization;
            }
        }
    }
}
