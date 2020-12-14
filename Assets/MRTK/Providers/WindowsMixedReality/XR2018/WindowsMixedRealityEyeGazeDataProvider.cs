// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.Windows.Utilities;
using System;
using System.Collections.Generic;
using Unity.Profiling;
using UnityEngine;

#if WINDOWS_UWP
using Windows.Perception;
using Windows.Perception.People;
using Windows.UI.Input.Spatial;
#elif UNITY_WSA && DOTNETWINRT_PRESENT
using Microsoft.Windows.Perception;
using Microsoft.Windows.Perception.People;
using Microsoft.Windows.UI.Input.Spatial;
#endif

namespace Microsoft.MixedReality.Toolkit.WindowsMixedReality.Input
{
    [MixedRealityDataProvider(
        typeof(IMixedRealityInputSystem),
        SupportedPlatforms.WindowsUniversal,
        "Windows Mixed Reality Eye Gaze Provider",
        "Profiles/DefaultMixedRealityEyeTrackingProfile.asset", "MixedRealityToolkit.SDK",
        true)]
    public class WindowsMixedRealityEyeGazeDataProvider : BaseInputDeviceManager, IMixedRealityEyeGazeDataProvider, IMixedRealityEyeSaccadeProvider, IMixedRealityCapabilityCheck
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="registrar">The <see cref="IMixedRealityServiceRegistrar"/> instance that loaded the data provider.</param>
        /// <param name="inputSystem">The <see cref="Microsoft.MixedReality.Toolkit.Input.IMixedRealityInputSystem"/> instance that receives data from this provider.</param>
        /// <param name="name">Friendly name of the service.</param>
        /// <param name="priority">Service priority. Used to determine order of instantiation.</param>
        /// <param name="profile">The service's configuration profile.</param>
        [Obsolete("This constructor is obsolete (registrar parameter is no longer required) and will be removed in a future version of the Microsoft Mixed Reality Toolkit.")]
        public WindowsMixedRealityEyeGazeDataProvider(
            IMixedRealityServiceRegistrar registrar,
            IMixedRealityInputSystem inputSystem,
            string name,
            uint priority,
            BaseMixedRealityProfile profile) : this(inputSystem, name, priority, profile)
        {
            Registrar = registrar;
        }

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
            BaseMixedRealityProfile profile) : base(inputSystem, name, priority, profile)
        {
            eyesApiAvailable = WindowsApiChecker.IsPropertyAvailable(
                "Windows.UI.Input.Spatial",
                "SpatialPointerPose",
                "Eyes");

#if (UNITY_WSA && DOTNETWINRT_PRESENT) || WINDOWS_UWP
            if (eyesApiAvailable)
            {
                eyesApiAvailable &= EyesPose.IsSupported();
            }
#endif // (UNITY_WSA && DOTNETWINRT_PRESENT) || WINDOWS_UWP
        }

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

        private readonly bool eyesApiAvailable = false;
        private readonly float smoothFactorNormalized = 0.96f;
        private readonly float saccadeThreshInDegree = 2.5f; // In degrees (not radians)

        private Ray? oldGaze;
        private int confidenceOfSaccade = 0;
        private int confidenceOfSaccadeThreshold = 6; // TODO(https://github.com/Microsoft/MixedRealityToolkit-Unity/issues/3767): This value should be adjusted based on the FPS of the ET system
        private Ray saccade_initialGazePoint;
        private readonly List<Ray> saccade_newGazeCluster = new List<Ray>();

#if (UNITY_WSA && DOTNETWINRT_PRESENT) || WINDOWS_UWP
        private static bool askedForETAccessAlready = false; // To make sure that this is only triggered once.
#endif // (UNITY_WSA && DOTNETWINRT_PRESENT) || WINDOWS_UWP

        #region IMixedRealityCapabilityCheck Implementation

        /// <inheritdoc />
        public bool CheckCapability(MixedRealityCapability capability)
        {
            if (eyesApiAvailable)
            {
#if (UNITY_WSA && DOTNETWINRT_PRESENT) || WINDOWS_UWP
                return (capability == MixedRealityCapability.EyeTracking);
#endif // (UNITY_WSA && DOTNETWINRT_PRESENT) || WINDOWS_UWP
            }

            return false;
        }

        #endregion IMixedRealityCapabilityCheck Implementation

        /// <inheritdoc />
        public override void Initialize()
        {
#if UNITY_EDITOR && UNITY_WSA && UNITY_2019_3_OR_NEWER
            Utilities.Editor.UWPCapabilityUtility.RequireCapability(
                    UnityEditor.PlayerSettings.WSACapability.GazeInput,
                    GetType());
#endif // UNITY_EDITOR && UNITY_WSA && UNITY_2019_3_OR_NEWER

            if (Application.isPlaying && eyesApiAvailable)
            {
#if (UNITY_WSA && DOTNETWINRT_PRESENT) || WINDOWS_UWP
                AskForETPermission();
#endif // (UNITY_WSA && DOTNETWINRT_PRESENT) || WINDOWS_UWP
                ReadProfile();

                // Call the base after initialization to ensure any early exits do not
                // artificially declare the service as initialized.
                base.Initialize();
            }
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

#if (UNITY_WSA && DOTNETWINRT_PRESENT) || WINDOWS_UWP
        private static readonly ProfilerMarker UpdatePerfMarker = new ProfilerMarker("[MRTK] WindowsMixedRealityEyeGazeDataProvider.Update");

        /// <inheritdoc />
        public override void Update()
        {
            using (UpdatePerfMarker.Auto())
            {
                if (WindowsMixedRealityUtilities.SpatialCoordinateSystem == null || !eyesApiAvailable)
                {
                    return;
                }

                base.Update();

                SpatialPointerPose pointerPose = SpatialPointerPose.TryGetAtTimestamp(WindowsMixedRealityUtilities.SpatialCoordinateSystem, PerceptionTimestampHelper.FromHistoricalTargetTime(DateTimeOffset.Now));
                if (pointerPose != null)
                {
                    var eyes = pointerPose.Eyes;
                    if (eyes != null)
                    {
                        Service?.EyeGazeProvider?.UpdateEyeTrackingStatus(this, eyes.IsCalibrationValid);

                        if (eyes.Gaze.HasValue)
                        {
                            Vector3 origin = MixedRealityPlayspace.TransformPoint(eyes.Gaze.Value.Origin.ToUnityVector3());
                            Vector3 direction = MixedRealityPlayspace.TransformDirection(eyes.Gaze.Value.Direction.ToUnityVector3());

                            Ray newGaze = new Ray(origin, direction);

                            if (SmoothEyeTracking)
                            {
                                newGaze = SmoothGaze(newGaze);
                            }

                            Service?.EyeGazeProvider?.UpdateEyeGaze(this, newGaze, eyes.UpdateTimestamp.TargetTime.UtcDateTime);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Triggers a prompt to let the user decide whether to permit using eye tracking 
        /// </summary>
        private async void AskForETPermission()
        {
            if (!askedForETAccessAlready) // Making sure this is only triggered once
            {
                askedForETAccessAlready = true;
                await EyesPose.RequestAccessAsync();
            }
        }
#endif // (UNITY_WSA && DOTNETWINRT_PRESENT) || WINDOWS_UWP

        private static readonly ProfilerMarker SmoothGazePerfMarker = new ProfilerMarker("[MRTK] WindowsMixedRealityEyeGazeDataProvider.SmoothGaze");

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

        private static readonly ProfilerMarker IsSaccadingPerfMarker = new ProfilerMarker("[MRTK] WindowsMixedRealityEyeGazeDataProvider.IsSaccading");

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
