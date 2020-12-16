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
using UnityEngine.XR.OpenXR.Features.Interactions;
#endif // UNITY_OPENXR

namespace Microsoft.MixedReality.Toolkit.XRSDK.OpenXR
{
    [MixedRealityDataProvider(
        typeof(IMixedRealityInputSystem),
        (SupportedPlatforms)(-1),
        "OpenXR XRSDK Eye Gaze Provider",
        "Profiles/DefaultMixedRealityEyeTrackingProfile.asset", "MixedRealityToolkit.SDK",
        true)]
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
            BaseMixedRealityProfile profile) : base(inputSystem, name, priority, profile) { }

        /// <inheritdoc />
        public bool SmoothEyeTracking { get; set; } = false;

        /// <inheritdoc />
        public IMixedRealityEyeSaccadeProvider SaccadeProvider => this;

        /// <inheritdoc />
        public event Action OnSaccade;

        /// <inheritdoc />
        public event Action OnSaccadeX;

        /// <inheritdoc />
        public event Action OnSaccadeY;

        private readonly float smoothFactorNormalized = 0.96f;
        private readonly float saccadeThreshInDegree = 2.5f; // in degrees (not radians)

        private Ray? oldGaze;
        private int confidenceOfSaccade = 0;
        private int confidenceOfSaccadeThreshold = 6; // TODO(https://github.com/Microsoft/MixedRealityToolkit-Unity/issues/3767): This value should be adjusted based on the FPS of the ET system
        private Ray saccade_initialGazePoint;
        private readonly List<Ray> saccade_newGazeCluster = new List<Ray>();

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

        private void ReadProfile()
        {
            if (ConfigurationProfile == null)
            {
                Debug.LogError("OpenXR Eye Tracking Provider requires a configuration profile to run properly.");
                return;
            }

            MixedRealityEyeTrackingProfile profile = ConfigurationProfile as MixedRealityEyeTrackingProfile;
            if (profile == null)
            {
                Debug.LogError("OpenXR Eye Tracking Provider's configuration profile must be a MixedRealityEyeTrackingProfile.");
                return;
            }

            SmoothEyeTracking = profile.SmoothEyeTracking;

#if !UNITY_OPENXR
            Debug.LogWarning("OpenXR Eye Tracking Provider requires Unity's OpenXR Plugin to be installed.");
#endif // !UNITY_OPENXR
        }

        private static readonly ProfilerMarker UpdatePerfMarker = new ProfilerMarker("[MRTK] OpenXREyeGazeDataProvider.Update");

        /// <inheritdoc />
        public override void Update()
        {
            using (UpdatePerfMarker.Auto())
            {
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

#if UNITY_OPENXR
                if (eyeTrackingDevice.TryGetFeatureValue(CommonUsages.isTracked, out bool gazeAvailable))
                {
                    Service?.EyeGazeProvider?.UpdateEyeTrackingStatus(this, gazeAvailable);

                    if (gazeAvailable &&
                        eyeTrackingDevice.TryGetFeatureValue(EyeTrackingUsages.gazePosition, out Vector3 eyeGazePosition) &&
                        eyeTrackingDevice.TryGetFeatureValue(EyeTrackingUsages.gazeRotation, out Quaternion eyeGazeRotation))
                    {
                        Vector3 worldPosition = MixedRealityPlayspace.TransformPoint(eyeGazePosition);
                        Vector3 worldRotation = MixedRealityPlayspace.TransformDirection(eyeGazeRotation * Vector3.forward);

                        Ray newGaze = new Ray(worldPosition, worldRotation);

                        if (SmoothEyeTracking)
                        {
                            newGaze = SmoothGaze(newGaze);
                        }

                        Service?.EyeGazeProvider?.UpdateEyeGaze(this, newGaze, DateTime.UtcNow);
                    }
                }
                else
                {
                    Service?.EyeGazeProvider?.UpdateEyeTrackingStatus(this, false);
                }
#endif // UNITY_OPENXR
            }
        }

        private static readonly ProfilerMarker SmoothGazePerfMarker = new ProfilerMarker("[MRTK] OpenXREyeGazeDataProvider.SmoothGaze");

        /// <summary>
        /// Smooths eye gaze by detecting saccades and tracking gaze clusters.
        /// </summary>
        /// <param name="newGaze">The ray to smooth.</param>
        /// <returns>The smoothed ray.</returns>
        private Ray SmoothGaze(Ray? newGaze)
        {
            using (SmoothGazePerfMarker.Auto())
            {
                if (!oldGaze.HasValue)
                {
                    oldGaze = newGaze;
                    return newGaze.Value;
                }

                Ray smoothedGaze = new Ray();
                bool isSaccading = false;

                // Handle saccades vs. outliers: Instead of simply checking that two successive gaze points are sufficiently 
                // apart, we check for clusters of gaze points instead.
                // 1. If the user's gaze points are far enough apart, this may be a saccade, but also could be an outlier.
                //    So, let's mark it as a potential saccade.
                if ((IsSaccading(oldGaze.Value, newGaze.Value) && (confidenceOfSaccade == 0)))
                {
                    confidenceOfSaccade++;
                    saccade_initialGazePoint = oldGaze.Value;
                    saccade_newGazeCluster.Clear();
                    saccade_newGazeCluster.Add(newGaze.Value);
                }
                // 2. If we have a potential saccade marked, let's check if the new points are within the proximity of 
                //    the initial saccade point.
                else if ((confidenceOfSaccade > 0) && (confidenceOfSaccade < confidenceOfSaccadeThreshold))
                {
                    confidenceOfSaccade++;

                    // First, let's check that we don't just have a bunch of random outliers
                    // The assumption is that after a person saccades, they fixate for a certain 
                    // amount of time resulting in a cluster of gaze points.
                    for (int i = 0; i < saccade_newGazeCluster.Count; i++)
                    {
                        if (IsSaccading(saccade_newGazeCluster[i], newGaze.Value))
                        {
                            confidenceOfSaccade = 0;
                        }

                        // Meanwhile we want to make sure that we are still looking sufficiently far away from our 
                        // original gaze point before saccading.
                        if (!IsSaccading(saccade_initialGazePoint, newGaze.Value))
                        {
                            confidenceOfSaccade = 0;
                        }
                    }
                    saccade_newGazeCluster.Add(newGaze.Value);
                }
                else if (confidenceOfSaccade == confidenceOfSaccadeThreshold)
                {
                    isSaccading = true;
                }

                // Saccade-dependent local smoothing
                if (isSaccading)
                {
                    smoothedGaze.direction = newGaze.Value.direction;
                    smoothedGaze.origin = newGaze.Value.origin;
                    confidenceOfSaccade = 0;
                }
                else
                {
                    smoothedGaze.direction = oldGaze.Value.direction * smoothFactorNormalized + newGaze.Value.direction * (1 - smoothFactorNormalized);
                    smoothedGaze.origin = oldGaze.Value.origin * smoothFactorNormalized + newGaze.Value.origin * (1 - smoothFactorNormalized);
                }

                oldGaze = smoothedGaze;
                return smoothedGaze;
            }
        }

        private static readonly ProfilerMarker IsSaccadingPerfMarker = new ProfilerMarker("[MRTK] BaseWindowsMixedRealityEyeGazeDataProvider.IsSaccading");

        private bool IsSaccading(Ray rayOld, Ray rayNew)
        {
            using (IsSaccadingPerfMarker.Auto())
            {
                Vector3 v1 = rayOld.origin + rayOld.direction;
                Vector3 v2 = rayNew.origin + rayNew.direction;

                if (Vector3.Angle(v1, v2) > saccadeThreshInDegree)
                {
                    Vector2 hv1 = new Vector2(v1.x, 0);
                    Vector2 hv2 = new Vector2(v2.x, 0);
                    if (Vector2.Angle(hv1, hv2) > saccadeThreshInDegree)
                    {
                        PostOnSaccadeHorizontally();
                    }

                    Vector2 vv1 = new Vector2(0, v1.y);
                    Vector2 vv2 = new Vector2(0, v2.y);
                    if (Vector2.Angle(vv1, vv2) > saccadeThreshInDegree)
                    {
                        PostOnSaccadeVertically();
                    }

                    PostOnSaccade();

                    return true;
                }
                return false;
            }
        }

        private void PostOnSaccade() => OnSaccade?.Invoke();
        private void PostOnSaccadeHorizontally() => OnSaccadeX?.Invoke();
        private void PostOnSaccadeVertically() => OnSaccadeY?.Invoke();
    }
}
