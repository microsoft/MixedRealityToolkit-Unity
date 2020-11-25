// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine.XR;

#if WMR_ENABLED
using System;
using Unity.Profiling;
using Unity.XR.WindowsMR;
using UnityEngine;
#endif // WMR_ENABLED


namespace Microsoft.MixedReality.Toolkit.XRSDK.WindowsMixedReality
{
    [MixedRealityDataProvider(
        typeof(IMixedRealityInputSystem),
        SupportedPlatforms.WindowsUniversal,
        "XRSDK Windows Mixed Reality Eye Gaze Provider",
        "Profiles/DefaultMixedRealityEyeTrackingProfile.asset", "MixedRealityToolkit.SDK",
        true)]
    public class WindowsMixedRealityEyeGazeDataProvider : BaseEyeGazeDataProvider, IMixedRealityCapabilityCheck
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="inputSystem">The <see cref="Microsoft.MixedReality.Toolkit.Input.IMixedRealityInputSystem"/> instance that receives data from this provider.</param>
        /// <param name="name">Friendly name of the service.</param>
        /// <param name="priority">Service priority. Used to determine order of instantiation.</param>
        /// <param name="profile">The service's configuration profile.</param>
        public WindowsMixedRealityEyeGazeDataProvider(
            IMixedRealityInputSystem inputSystem,
            string name,
            uint priority,
            BaseMixedRealityProfile profile) : base(inputSystem, name, priority, profile) { }

        private InputDevice centerEye = default(InputDevice);

        #region IMixedRealityCapabilityCheck Implementation

        /// <inheritdoc />
        public bool CheckCapability(MixedRealityCapability capability) => centerEye.isValid && capability == MixedRealityCapability.EyeTracking;

        #endregion IMixedRealityCapabilityCheck Implementation

#if WMR_ENABLED
        private static readonly ProfilerMarker UpdatePerfMarker = new ProfilerMarker("[MRTK] WindowsMixedRealityEyeGazeDataProvider.Update");

        /// <inheritdoc />
        public override void Update()
        {
            using (UpdatePerfMarker.Auto())
            {
                if (!centerEye.isValid)
                {
                    centerEye = InputDevices.GetDeviceAtXRNode(XRNode.CenterEye);
                    if (!centerEye.isValid)
                    {
                        return;
                    }
                }

                bool gazeAvailable = false;
                if (centerEye.TryGetFeatureValue(WindowsMRUsages.EyeGazeAvailable, out gazeAvailable) && gazeAvailable)
                {
                    Service?.EyeGazeProvider?.UpdateEyeTrackingStatus(this, gazeAvailable);

                    Vector3 eyeGazePosition;
                    Quaternion eyeGazeRotation;
                    if (centerEye.TryGetFeatureValue(WindowsMRUsages.EyeGazePosition, out eyeGazePosition) && centerEye.TryGetFeatureValue(WindowsMRUsages.EyeGazeRotation, out eyeGazeRotation))
                    {
                        Vector3 worldPosition = CameraCache.Main.transform.TransformPoint(eyeGazePosition);
                        Vector3 worldRotation = CameraCache.Main.transform.TransformDirection(eyeGazeRotation * Vector3.forward);

                        Ray newGaze = new Ray(worldPosition, worldRotation);

                        if (SmoothEyeTracking)
                        {
                            newGaze = SmoothGaze(newGaze);
                        }

                        Service?.EyeGazeProvider?.UpdateEyeGaze(this, newGaze, DateTime.UtcNow);
                    }
                }
            }
        }
#endif // WMR_ENABLED
    }
}
