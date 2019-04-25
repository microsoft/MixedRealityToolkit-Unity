// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.Windows.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;

#if WINDOWS_UWP
using Windows.Perception;
using Windows.Perception.People;
using Windows.UI.Input.Spatial;
#endif

namespace Microsoft.MixedReality.Toolkit.WindowsMixedReality.Input
{
    [MixedRealityDataProvider(
        typeof(IMixedRealityInputSystem),
        SupportedPlatforms.WindowsUniversal,
        "Windows Mixed Reality Eye Gaze Provider",
        "Profiles/DefaultMixedRealityEyeTrackingProfile.asset", "MixedRealityToolkit.SDK")]
    public class WindowsMixedRealityEyeGazeDataProvider : BaseInputDeviceManager, IMixedRealityEyeGazeDataProvider, IMixedRealityEyeSaccadeProvider
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">Friendly name of the service.</param>
        /// <param name="priority">Service priority. Used to determine order of instantiation.</param>
        /// <param name="profile">The service's configuration profile.</param>
        public WindowsMixedRealityEyeGazeDataProvider(
            IMixedRealityServiceRegistrar registrar,
            IMixedRealityInputSystem inputSystem,
            MixedRealityInputSystemProfile inputSystemProfile,
            Transform playspace,
            string name,
            uint priority,
            BaseMixedRealityProfile profile) : base(registrar, inputSystem, inputSystemProfile, playspace, name, priority, profile) { }

        public bool SmoothEyeTracking { get; set; } = false;

        private readonly float smoothFactorNormalized = 0.96f;
        private readonly float saccadeThreshInDegree = 2.5f; // In degrees (not radians)

        private Ray? oldGaze;
        private int confidenceOfSaccade = 0;
        private int confidenceOfSaccadeThreshold = 6; // TODO(https://github.com/Microsoft/MixedRealityToolkit-Unity/issues/3767): This value should be adjusted based on the FPS of the ET system
        private Ray saccade_initialGazePoint;
        private List<Ray> saccade_newGazeCluster = new List<Ray>();

#if WINDOWS_UWP
        private static bool askedForETAccessAlready = false; // To make sure that this is only triggered once.
#endif

        public event Action OnSaccade;
        public event Action OnSaccadeX;
        public event Action OnSaccadeY;

        public override void Initialize()
        {
            if (Application.isPlaying && WindowsApiChecker.UniversalApiContractV8_IsAvailable)
            {
#if WINDOWS_UWP
                AskForETPermission();
#endif
                ReadProfile();
            }
        }

        public override void Update()
        {
#if WINDOWS_UWP
            if (WindowsMixedRealityUtilities.SpatialCoordinateSystem == null || !WindowsApiChecker.UniversalApiContractV8_IsAvailable)
            {
                return;
            }

            SpatialPointerPose pointerPose = SpatialPointerPose.TryGetAtTimestamp(WindowsMixedRealityUtilities.SpatialCoordinateSystem, PerceptionTimestampHelper.FromHistoricalTargetTime(DateTimeOffset.Now));
            if (pointerPose != null)
            {
                var eyes = pointerPose.Eyes;
                if ((eyes != null) && (eyes.Gaze.HasValue))
                {
                    Ray newGaze = new Ray(WindowsMixedRealityUtilities.SystemVector3ToUnity(eyes.Gaze.Value.Origin), WindowsMixedRealityUtilities.SystemVector3ToUnity(eyes.Gaze.Value.Direction));

                    if (SmoothEyeTracking)
                    {
                        newGaze = SmoothGaze(newGaze);
                    }

                    MixedRealityToolkit.InputSystem?.EyeGazeProvider?.UpdateEyeGaze(this, newGaze, eyes.UpdateTimestamp.TargetTime.UtcDateTime);
                }
            }
#endif // WINDOWS_UWP
        }

        private void ReadProfile()
        {
            if (ConfigurationProfile == null)
            {
                Debug.LogError("Windows Mixed Reality Eye Tracking Provider requires a configuration profile to run properly.");
                return;
            }

            MixedRealityEyeTrackingProfile profile = ConfigurationProfile as MixedRealityEyeTrackingProfile;
            if (profile == null)
            {
                Debug.LogError("Windows Mixed Reality Eye Tracking Provider's configuration profile must be a MixedRealityEyeTrackingProfile.");
                return;
            }

            SmoothEyeTracking = profile.SmoothEyeTracking;
        }

#if WINDOWS_UWP
        /// <summary>
        /// Triggers a prompt to let the user decide whether to permit using eye tracking 
        /// </summary>
        private async void AskForETPermission()
        {
            if (!askedForETAccessAlready)  // Making sure this is only triggered once
            {
                askedForETAccessAlready = true;
                await EyesPose.RequestAccessAsync();
            }
        }
#endif // WINDOWS_UWP

        private Ray SmoothGaze(Ray? newGaze)
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

            Vector3 v1 = oldGaze.Value.origin + oldGaze.Value.direction;
            Vector3 v2 = newGaze.Value.origin + newGaze.Value.direction;

            // Saccade-dependent local smoothing
            if (isSaccading)
            {
                smoothedGaze.direction = newGaze.Value.direction;
                smoothedGaze.origin = newGaze.Value.origin;
                confidenceOfSaccade = 0;
                isSaccading = false;
            }
            else
            {
                smoothedGaze.direction = oldGaze.Value.direction * smoothFactorNormalized + newGaze.Value.direction * (1 - smoothFactorNormalized);
                smoothedGaze.origin = oldGaze.Value.origin * smoothFactorNormalized + newGaze.Value.origin * (1 - smoothFactorNormalized);
            }

            oldGaze = smoothedGaze;
            return smoothedGaze;
        }

        private bool IsSaccading(Ray rayOld, Ray rayNew)
        {
            Vector3 v1 = rayOld.origin + rayOld.direction;
            Vector3 v2 = rayNew.origin + rayNew.direction;

            if (Vector3.Angle(v1, v2) > saccadeThreshInDegree)
            {
                Vector2 hv1 = new Vector2(v1.x, 0);
                Vector2 hv2 = new Vector2(v2.x, 0);
                if (Vector2.Angle(hv1, hv2) > saccadeThreshInDegree)
                {
                    Post_OnSaccadeHorizontally();
                }

                Vector2 vv1 = new Vector2(0, v1.y);
                Vector2 vv2 = new Vector2(0, v2.y);
                if (Vector2.Angle(vv1, vv2) > saccadeThreshInDegree)
                {
                    Post_OnSaccadeVertically();
                }

                Post_OnSaccade();

                return true;
            }
            return false;
        }

        private void Post_OnSaccade()
        {
            OnSaccade?.Invoke();
        }

        private void Post_OnSaccadeHorizontally()
        {
            OnSaccadeX?.Invoke();
        }

        private void Post_OnSaccadeVertically()
        {
            OnSaccadeY?.Invoke();
        }
    }
}