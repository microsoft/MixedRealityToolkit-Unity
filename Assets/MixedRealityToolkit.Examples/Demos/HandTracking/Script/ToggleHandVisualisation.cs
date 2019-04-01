// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos
{
    public class ToggleHandVisualisation : MonoBehaviour
    {
        public bool isHandMeshVisible = false;

        void updateHandMeshVisibility()
        {
            MixedRealityHandTrackingProfile handTrackingProfile = MixedRealityToolkit.Instance.ActiveProfile.InputSystemProfile.HandTrackingProfile;
            if (handTrackingProfile != null)
            { 
                handTrackingProfile.EnableHandMeshVisualization = isHandMeshVisible;
            }
        }

        /// <summary>
        /// Initial setting of hand mesh visualization - default is disabled
        /// </summary>
        void Start()
        {
            updateHandMeshVisibility();
        }


        /// <summary>
        /// Toggles hand mesh visualization
        /// </summary>
        public void OnToggleHandMesh()
        {
            isHandMeshVisible = !isHandMeshVisible;
            updateHandMeshVisibility();
        }

    }
}