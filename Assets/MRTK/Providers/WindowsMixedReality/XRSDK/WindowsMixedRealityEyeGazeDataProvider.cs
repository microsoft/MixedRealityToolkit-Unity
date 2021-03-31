// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine.XR;

#if WMR_2_7_OR_NEWER_ENABLED
using System;
using Unity.Profiling;
using Unity.XR.WindowsMR;
using UnityEngine;
#endif // WMR_2_7_OR_NEWER_ENABLED


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
        public bool CheckCapability(MixedRealityCapability capability) =>
#if WMR_2_7_OR_NEWER_ENABLED
                                                                          capability == MixedRealityCapability.EyeTracking
                                                                          && centerEye.isValid
                                                                          && centerEye.TryGetFeatureValue(WindowsMRUsages.EyeGazeAvailable, out bool gazeAvailable)
                                                                          && gazeAvailable;
#else
                                                                          false;
#endif // WMR_2_7_OR_NEWER_ENABLED

        #endregion IMixedRealityCapabilityCheck Implementation

#if WMR_2_7_OR_NEWER_ENABLED
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

                if (!centerEye.TryGetFeatureValue(WindowsMRUsages.EyeGazeAvailable, out bool gazeAvailable) || !gazeAvailable)
                {
                    Service?.EyeGazeProvider?.UpdateEyeTrackingStatus(this, false);
                    return;
                }

                Service?.EyeGazeProvider?.UpdateEyeTrackingStatus(this, true);

                if (centerEye.TryGetFeatureValue(WindowsMRUsages.EyeGazeTracked, out bool gazeTracked) && gazeTracked
                    && centerEye.TryGetFeatureValue(WindowsMRUsages.EyeGazePosition, out Vector3 eyeGazePosition)
                    && centerEye.TryGetFeatureValue(WindowsMRUsages.EyeGazeRotation, out Quaternion eyeGazeRotation))
                {
                    Ray newGaze = new Ray(eyeGazePosition, eyeGazeRotation * Vector3.forward);

                    if (SmoothEyeTracking)
                    {
                        newGaze = SmoothGaze(newGaze);
                    }

                    Service?.EyeGazeProvider?.UpdateEyeGaze(this, newGaze, DateTime.UtcNow);
                }
            }
        }
#endif // WMR_2_7_OR_NEWER_ENABLED
    }
}
