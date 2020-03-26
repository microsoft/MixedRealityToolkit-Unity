// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.SpatialAwareness;
using UnityEngine;
using UnityEngine.UI;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos
{
    /// <summary>
    /// Script that demonstrates querying the Mixed Reality Toolkit for platform capabilities.
    /// </summary>
    [AddComponentMenu("Scripts/MRTK/Examples/MixedRealityCapabilityDemo")]
    public class MixedRealityCapabilityDemo : MonoBehaviour
    {
        [SerializeField]
        private Text articulatedHandResult = null;

        [SerializeField]
        private Text ggvHandResult = null;

        [SerializeField]
        private Text motionControllerResult = null;

        [SerializeField]
        private Text eyeTrackingResult = null;

        [SerializeField]
        private Text voiceCommandResult = null;

        [SerializeField]
        private Text voiceDictationResult = null;

        [SerializeField]
        private Text spatialMeshResult = null;

        [SerializeField]
        private Text spatialPlaneResult = null;

        [SerializeField]
        private Text spatialPointResult = null;

        private void Start()
        {
            IMixedRealityCapabilityCheck capabilityChecker = CoreServices.InputSystem as IMixedRealityCapabilityCheck;
            if (capabilityChecker != null)
            {
                bool isSupported = capabilityChecker.CheckCapability(MixedRealityCapability.ArticulatedHand);
                articulatedHandResult.text = isSupported ? "Yes" : "No";
                articulatedHandResult.color = isSupported ? Color.green : Color.white;

                isSupported = capabilityChecker.CheckCapability(MixedRealityCapability.GGVHand);
                ggvHandResult.text = isSupported ? "Yes" : "No";
                ggvHandResult.color = isSupported ? Color.green : Color.white;

                isSupported = capabilityChecker.CheckCapability(MixedRealityCapability.MotionController);
                motionControllerResult.text = isSupported ? "Yes" : "No";
                motionControllerResult.color = isSupported ? Color.green : Color.white;

                isSupported = capabilityChecker.CheckCapability(MixedRealityCapability.EyeTracking);
                eyeTrackingResult.text = isSupported ? "Yes" : "No";
                eyeTrackingResult.color = isSupported ? Color.green : Color.white;

                isSupported = capabilityChecker.CheckCapability(MixedRealityCapability.VoiceCommand);
                voiceCommandResult.text = isSupported ? "Yes" : "No";
                voiceCommandResult.color = isSupported ? Color.green : Color.white;

                isSupported = capabilityChecker.CheckCapability(MixedRealityCapability.VoiceDictation);
                voiceDictationResult.text = isSupported ? "Yes" : "No";
                voiceDictationResult.color = isSupported ? Color.green : Color.white;
            }

            capabilityChecker = CoreServices.SpatialAwarenessSystem as IMixedRealityCapabilityCheck;
            if (capabilityChecker != null)
            {
                bool isSupported = capabilityChecker.CheckCapability(MixedRealityCapability.SpatialAwarenessMesh);
                spatialMeshResult.text = isSupported ? "Yes" : "No";
                spatialMeshResult.color = isSupported ? Color.green : Color.white;

                isSupported = capabilityChecker.CheckCapability(MixedRealityCapability.SpatialAwarenessPlane);
                spatialPlaneResult.text = isSupported ? "Yes" : "No";
                spatialPlaneResult.color = isSupported ? Color.green : Color.white;

                isSupported = capabilityChecker.CheckCapability(MixedRealityCapability.SpatialAwarenessPoint);
                spatialPointResult.text = isSupported ? "Yes" : "No";
                spatialPointResult.color = isSupported ? Color.green : Color.white;
            }
        }
    }
}
