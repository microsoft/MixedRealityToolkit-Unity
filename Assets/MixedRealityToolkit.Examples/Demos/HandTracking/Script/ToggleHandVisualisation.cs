// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos
{
    public class ToggleHandVisualisation : MonoBehaviour
    {
        public bool isHandMeshVisible = false;
        public bool isHandJointVisible = false;

        private IMixedRealityInputSystem inputSystem = null;
        protected IMixedRealityInputSystem InputSystem
        {
            get
            {
                if (inputSystem == null)
                {
                    MixedRealityServiceRegistry.TryGetService<IMixedRealityInputSystem>(out inputSystem);
                }
                return inputSystem;
            }
        }

        void updateHandVisibility()
        {
            MixedRealityHandTrackingProfile handTrackingProfile = InputSystem?.InputSystemProfile?.HandTrackingProfile;
            if (handTrackingProfile != null)
            { 
                handTrackingProfile.EnableHandMeshVisualization = isHandMeshVisible;
                handTrackingProfile.EnableHandJointVisualization = isHandJointVisible;
            }
        }

        /// <summary>
        /// Initial setting of hand mesh visualization - default is disabled
        /// </summary>
        void Start()
        {
            updateHandVisibility();
        }


        /// <summary>
        /// Toggles hand mesh visualization
        /// </summary>
        public void OnToggleHandMesh()
        {
            isHandMeshVisible = !isHandMeshVisible;
            updateHandVisibility();
        }

        /// <summary>
        /// Toggles hand joint visualization
        /// </summary>
        public void OnToggleHandJoint()
        {
            isHandJointVisible = !isHandJointVisible;
            updateHandVisibility();
        }

    }
}
