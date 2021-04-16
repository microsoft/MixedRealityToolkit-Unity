// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.Windows.Utilities;
using System;
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
        true,
        SupportedUnityXRPipelines.LegacyXR)]
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

            gazeSmoother = new EyeGazeSmoother();

            // Register for these events to forward along, in case code is still registering for the obsolete actions
            gazeSmoother.OnSaccade += GazeSmoother_OnSaccade;
            gazeSmoother.OnSaccadeX += GazeSmoother_OnSaccadeX;
            gazeSmoother.OnSaccadeY += GazeSmoother_OnSaccadeY;
        }

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

        private readonly bool eyesApiAvailable = false;

#if (UNITY_WSA && DOTNETWINRT_PRESENT) || WINDOWS_UWP
        private static bool askedForETAccessAlready = false; // To make sure that this is only triggered once.
#endif // (UNITY_WSA && DOTNETWINRT_PRESENT) || WINDOWS_UWP

        #region IMixedRealityCapabilityCheck Implementation

        /// <inheritdoc />
        public bool CheckCapability(MixedRealityCapability capability) => eyesApiAvailable && capability == MixedRealityCapability.EyeTracking;

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
            }

            ReadProfile();

            // Call the base after initialization to ensure any early exits do not
            // artificially declare the service as initialized.
            base.Initialize();
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

#if (UNITY_WSA && DOTNETWINRT_PRESENT) || WINDOWS_UWP
        private static readonly ProfilerMarker UpdatePerfMarker = new ProfilerMarker("[MRTK] WindowsMixedRealityEyeGazeDataProvider.Update");

        /// <inheritdoc />
        public override void Update()
        {
            using (UpdatePerfMarker.Auto())
            {
                if (!eyesApiAvailable || WindowsMixedRealityUtilities.SpatialCoordinateSystem == null)
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
                                newGaze = gazeSmoother.SmoothGaze(newGaze);
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
    }
}
