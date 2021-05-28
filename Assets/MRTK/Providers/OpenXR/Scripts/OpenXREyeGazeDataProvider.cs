// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Collections.Generic;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.XR;

#if UNITY_OPENXR
using UnityEngine.XR.OpenXR;
using UnityEngine.XR.OpenXR.Features.Interactions;
#endif // UNITY_OPENXR

namespace Microsoft.MixedReality.Toolkit.XRSDK.OpenXR
{
    [MixedRealityDataProvider(
        typeof(IMixedRealityInputSystem),
        (SupportedPlatforms)(-1),
        "OpenXR XRSDK Eye Gaze Provider",
        "Profiles/DefaultMixedRealityEyeTrackingProfile.asset", "MixedRealityToolkit.SDK",
        true,
        SupportedUnityXRPipelines.XRSDK)]
    public class OpenXREyeGazeDataProvider : BaseInputDeviceManager, IMixedRealityEyeGazeDataProvider, IMixedRealityEyeSaccadeProvider, IMixedRealityCapabilityCheck
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="inputSystem">The <see cref="Microsoft.MixedReality.Toolkit.Input.IMixedRealityInputSystem"/> instance that receives data from this provider.</param>
        /// <param name="name">Friendly name of the service.</param>
        /// <param name="priority">Service priority. Used to determine order of instantiation.</param>
        /// <param name="profile">The service's configuration profile.</param>
        public OpenXREyeGazeDataProvider(
            IMixedRealityInputSystem inputSystem,
            string name,
            uint priority,
            BaseMixedRealityProfile profile) : base(inputSystem, name, priority, profile)
        {
            gazeSmoother = new EyeGazeSmoother();

            // Register for these events to forward along, in case code is still registering for the obsolete actions
            gazeSmoother.OnSaccade += GazeSmoother_OnSaccade;
            gazeSmoother.OnSaccadeX += GazeSmoother_OnSaccadeX;
            gazeSmoother.OnSaccadeY += GazeSmoother_OnSaccadeY;
        }

        private bool? IsActiveLoader =>
#if UNITY_OPENXR
            LoaderHelpers.IsLoaderActive<OpenXRLoaderBase>();
#else
            false;
#endif // UNITY_OPENXR

        /// <inheritdoc />
        public bool SmoothEyeTracking { get; set; } = false;

        /// <inheritdoc />
        public IMixedRealityEyeSaccadeProvider SaccadeProvider => gazeSmoother;
        private readonly EyeGazeSmoother gazeSmoother;

        /// <inheritdoc />
        [Obsolete("Register for this provider's SaccadeProvider's actions instead")]
        public event Action OnSaccade;
        private void GazeSmoother_OnSaccade() => OnSaccade?.Invoke();

        /// <inheritdoc />
        [Obsolete("Register for this provider's SaccadeProvider's actions instead")]
        public event Action OnSaccadeX;
        private void GazeSmoother_OnSaccadeX() => OnSaccadeX?.Invoke();

        /// <inheritdoc />
        [Obsolete("Register for this provider's SaccadeProvider's actions instead")]
        public event Action OnSaccadeY;
        private void GazeSmoother_OnSaccadeY() => OnSaccadeY?.Invoke();

        private static readonly List<InputDevice> InputDeviceList = new List<InputDevice>();
        private InputDevice eyeTrackingDevice = default(InputDevice);

        #region IMixedRealityCapabilityCheck Implementation

        /// <inheritdoc />
        public bool CheckCapability(MixedRealityCapability capability) => eyeTrackingDevice.isValid && capability == MixedRealityCapability.EyeTracking;

        #endregion IMixedRealityCapabilityCheck Implementation

        /// <inheritdoc />
        public override void Initialize()
        {
            if (Application.isPlaying)
            {
                ReadProfile();
            }

            base.Initialize();
        }

        /// <inheritdoc />
        public override void Enable()
        {
            if (!IsActiveLoader.HasValue)
            {
                IsEnabled = false;
                EnableIfLoaderBecomesActive();
                return;
            }
            else if (!IsActiveLoader.Value)
            {
                IsEnabled = false;
                return;
            }

            base.Enable();
        }

        private async void EnableIfLoaderBecomesActive()
        {
            await new WaitUntil(() => IsActiveLoader.HasValue);
            if (IsActiveLoader.Value)
            {
                Enable();
            }
        }

        private void ReadProfile()
        {
            if (ConfigurationProfile == null)
            {
                Debug.LogError($"{Name} requires a configuration profile to run properly.");
                return;
            }

            MixedRealityEyeTrackingProfile profile = ConfigurationProfile as MixedRealityEyeTrackingProfile;
            if (profile == null)
            {
                Debug.LogError($"{Name}'s configuration profile must be a MixedRealityEyeTrackingProfile.");
                return;
            }

            SmoothEyeTracking = profile.SmoothEyeTracking;
        }

        private static readonly ProfilerMarker UpdatePerfMarker = new ProfilerMarker("[MRTK] OpenXREyeGazeDataProvider.Update");

        /// <inheritdoc />
        public override void Update()
        {
            using (UpdatePerfMarker.Auto())
            {
                if (!IsEnabled)
                {
                    return;
                }

                if (!eyeTrackingDevice.isValid)
                {
                    InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.EyeTracking, InputDeviceList);
                    if (InputDeviceList.Count > 0)
                    {
                        eyeTrackingDevice = InputDeviceList[0];
                    }

                    if (!eyeTrackingDevice.isValid)
                    {
                        Service?.EyeGazeProvider?.UpdateEyeTrackingStatus(this, false);
                        return;
                    }
                }

                Service?.EyeGazeProvider?.UpdateEyeTrackingStatus(this, true);

#if UNITY_OPENXR
                if (eyeTrackingDevice.TryGetFeatureValue(CommonUsages.isTracked, out bool gazeTracked)
                    && gazeTracked
                    && eyeTrackingDevice.TryGetFeatureValue(EyeTrackingUsages.gazePosition, out Vector3 eyeGazePosition)
                    && eyeTrackingDevice.TryGetFeatureValue(EyeTrackingUsages.gazeRotation, out Quaternion eyeGazeRotation))
                {
                    Vector3 worldPosition = MixedRealityPlayspace.TransformPoint(eyeGazePosition);
                    Vector3 worldRotation = MixedRealityPlayspace.TransformDirection(eyeGazeRotation * Vector3.forward);

                    Ray newGaze = new Ray(worldPosition, worldRotation);

                    if (SmoothEyeTracking)
                    {
                        newGaze = gazeSmoother.SmoothGaze(newGaze);
                    }

                    Service?.EyeGazeProvider?.UpdateEyeGaze(this, newGaze, DateTime.UtcNow);
                }
#endif // UNITY_OPENXR
            }
        }
    }
}
