// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine;
using UnityEngine.UI;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos
{
    /// <summary>
    /// Script that demonstrates querying the Mixed Reality Toolkit input system for platform capabilities.
    /// </summary>
    public class InputCapabilitiesDemo : MonoBehaviour
    {
        [SerializeField]
        private Text articulatedHandResult = null;

        [SerializeField]
        private Text ggvHandResult = null;

        [SerializeField]
        private Text motionControllerResult = null;

        [SerializeField]
        private Text eyeTrackingResult = null;

        private IMixedRealityInputSystem inputSystem = null;
        private IMixedRealityInputSystem InputSystem
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

        private async void Start()
        {
            await new WaitUntil(() => InputSystem != null);

            bool isSupported = InputSystem.CheckCapability(MixedRealityInputCapabilities.ArticulatedHand);
            articulatedHandResult.text = isSupported ? "Yes" : "No";
            articulatedHandResult.color = isSupported ? Color.green : Color.white;

            isSupported = InputSystem.CheckCapability(MixedRealityInputCapabilities.GGVHand);
            ggvHandResult.text = isSupported ? "Yes" : "No";
            ggvHandResult.color = isSupported ? Color.green : Color.white;

            isSupported = InputSystem.CheckCapability(MixedRealityInputCapabilities.MotionController);
            motionControllerResult.text = isSupported ? "Yes" : "No";
            motionControllerResult.color = isSupported ? Color.green : Color.white;

            isSupported = InputSystem.CheckCapability(MixedRealityInputCapabilities.EyeTracking);
            eyeTrackingResult.text = isSupported ? "Yes" : "No";
            eyeTrackingResult.color = isSupported ? Color.green : Color.white;
        }
    }
}
