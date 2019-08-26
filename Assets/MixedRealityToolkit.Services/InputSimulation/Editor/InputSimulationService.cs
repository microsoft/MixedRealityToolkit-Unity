// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    [MixedRealityDataProvider(
        typeof(IMixedRealityInputSystem),
        SupportedPlatforms.WindowsEditor | SupportedPlatforms.MacEditor | SupportedPlatforms.LinuxEditor,
        "Input Simulation Service",
        "Profiles/DefaultMixedRealityInputSimulationProfile.asset",
        "MixedRealityToolkit.SDK")]
    [HelpURL("https://microsoft.github.io/MixedRealityToolkit-Unity/Documentation/InputSimulation/InputSimulationService.html")]
    public class InputSimulationService :
        BaseInputSimulationService,
        IInputSimulationService,
        IMixedRealityEyeGazeDataProvider,
        IMixedRealityCapabilityCheck
    {
        private ManualCameraControl cameraControl = null;
        private SimulatedHandDataProvider handDataProvider = null;

        /// <inheritdoc />
        public SimulatedHandData HandDataLeft { get; } = new SimulatedHandData();
        /// <inheritdoc />
        public SimulatedHandData HandDataRight { get; } = new SimulatedHandData();

        /// <inheritdoc />
        public bool IsSimulatingHandLeft => (handDataProvider != null ? handDataProvider.IsSimulatingLeft : false);
        /// <inheritdoc />
        public bool IsSimulatingHandRight => (handDataProvider != null ? handDataProvider.IsSimulatingRight : false);

        /// <inheritdoc />
        public bool IsAlwaysVisibleHandLeft
        {
            get => handDataProvider != null ? handDataProvider.IsAlwaysVisibleLeft : false;
            set
            {
                if (handDataProvider != null)
                {
                    handDataProvider.IsAlwaysVisibleLeft = value;
                }
            }
        }
        /// <inheritdoc />
        public bool IsAlwaysVisibleHandRight
        {
            get => handDataProvider != null ? handDataProvider.IsAlwaysVisibleRight : false;
            set
            {
                if (handDataProvider != null)
                {
                    handDataProvider.IsAlwaysVisibleRight = value;
                }
            }
        }

        /// <inheritdoc />
        public void ResetHandLeft()
        {
            if (handDataProvider != null)
            {
                handDataProvider.ResetHand(Handedness.Left);
            }
        }
        /// <inheritdoc />
        public void ResetHandRight()
        {
            if (handDataProvider != null)
            {
                handDataProvider.ResetHand(Handedness.Right);
            }
        }

        /// <summary>
        /// If true then keyboard and mouse input are used to simulate hands.
        /// </summary>
        public bool UserInputEnabled { get; set; } = true;

        /// <summary>
        /// Timestamp of the last hand device update
        /// </summary>
        private long lastHandUpdateTimestamp = 0;

        /// <summary>
        /// Indicators to show input simulation state in the viewport.
        /// </summary>
        private GameObject indicators;

        #region BaseInputDeviceManager Implementation

        public InputSimulationService(
            IMixedRealityServiceRegistrar registrar,
            IMixedRealityInputSystem inputSystem,
            string name,
            uint priority,
            BaseMixedRealityProfile profile) : base(registrar, inputSystem, name, priority, profile) { }

        /// <inheritdoc />
        public bool CheckCapability(MixedRealityCapability capability)
        {
            switch (capability)
            {
                case MixedRealityCapability.ArticulatedHand:
                    return (InputSimulationProfile.HandSimulationMode == HandSimulationMode.Articulated);

                case MixedRealityCapability.GGVHand:
                    // If any hand simulation is enabled, GGV interactions are supported.
                    return (InputSimulationProfile.HandSimulationMode != HandSimulationMode.Disabled);

                case MixedRealityCapability.EyeTracking:
                    return InputSimulationProfile.SimulateEyePosition;
            }

            return false;
        }

        /// <inheritdoc />
        public override void Initialize()
        {
            ArticulatedHandPose.LoadGesturePoses();
        }

        /// <inheritdoc />
        public override void Destroy()
        {
            ArticulatedHandPose.ResetGesturePoses();
        }

        /// <inheritdoc />
        public override void Enable()
        {
            var profile = InputSimulationProfile;

            if (indicators == null && profile.IndicatorsPrefab)
            {
                indicators = GameObject.Instantiate(profile.IndicatorsPrefab);
            }
        }

        /// <inheritdoc />
        public override void Disable()
        {
            if (indicators)
            {
                GameObject.Destroy(indicators);
            }

            DisableCameraControl();
            DisableHandSimulation();
        }

        /// <inheritdoc />
        public override void Update()
        {
            var profile = InputSimulationProfile;

            if (profile.IsCameraControlEnabled)
            {
                EnableCameraControl();
                if (CameraCache.Main)
                {
                    cameraControl.UpdateTransform(CameraCache.Main.transform);
                }
            }
            else
            {
                DisableCameraControl();
            }

            if (profile.SimulateEyePosition)
            {
                // In the simulated eye gaze condition, let's set the eye tracking calibration status automatically to true
                InputSystem?.EyeGazeProvider?.UpdateEyeTrackingStatus(this, true);

                // Update the simulated eye gaze with the current camera position and forward vector
                InputSystem?.EyeGazeProvider?.UpdateEyeGaze(this, new Ray(CameraCache.Main.transform.position, CameraCache.Main.transform.forward), DateTime.UtcNow);
            }

            switch (profile.HandSimulationMode)
            {
                case HandSimulationMode.Disabled:
                    DisableHandSimulation();
                    break;

                case HandSimulationMode.Articulated:
                case HandSimulationMode.Gestures:
                    EnableHandSimulation();

                    if (UserInputEnabled)
                    {
                        handDataProvider.UpdateHandData(HandDataLeft, HandDataRight);
                    }
                    break;
            }
        }

        /// <inheritdoc />
        public override void LateUpdate()
        {
            var profile = InputSimulationProfile;

            // Apply hand data in LateUpdate to ensure external changes are applied.
            // HandDataLeft/Right can be modified after the services Update() call.
            if (profile.HandSimulationMode == HandSimulationMode.Disabled)
            {
                RemoveAllHandDevices();
            }
            else
            {
                DateTime currentTime = DateTime.UtcNow;
                double msSinceLastHandUpdate = currentTime.Subtract(new DateTime(lastHandUpdateTimestamp)).TotalMilliseconds;
                // TODO implement custom hand device update frequency here, use 1000/fps instead of 0
                if (msSinceLastHandUpdate > 0)
                {
                    if (HandDataLeft.Timestamp > lastHandUpdateTimestamp)
                    {
                        UpdateHandDevice(profile.HandSimulationMode, Handedness.Left, HandDataLeft);
                    }
                    if (HandDataRight.Timestamp > lastHandUpdateTimestamp)
                    {
                        UpdateHandDevice(profile.HandSimulationMode, Handedness.Right, HandDataRight);
                    }

                    lastHandUpdateTimestamp = currentTime.Ticks;
                }
            }
        }

        #endregion BaseInputDeviceManager Implementation

        private MixedRealityInputSimulationProfile inputSimulationProfile = null;

        /// <inheritdoc/>
        public MixedRealityInputSimulationProfile InputSimulationProfile
        {
            get
            {
                if (inputSimulationProfile == null)
                {
                    inputSimulationProfile = ConfigurationProfile as MixedRealityInputSimulationProfile;
                }
                return inputSimulationProfile;
            }
            set
            {
                inputSimulationProfile = value;
            }
        }

        /// <inheritdoc/>
        IMixedRealityEyeSaccadeProvider IMixedRealityEyeGazeDataProvider.SaccadeProvider => null;

        /// <inheritdoc/>
        bool IMixedRealityEyeGazeDataProvider.SmoothEyeTracking { get; set; }

        private void EnableCameraControl()
        {
            if (cameraControl == null)
            {
                cameraControl = new ManualCameraControl(InputSimulationProfile);
            }
        }

        private void DisableCameraControl()
        {
            if (cameraControl != null)
            {
                cameraControl = null;
            }
        }

        private void EnableHandSimulation()
        {
            if (handDataProvider == null)
            {
                handDataProvider = new SimulatedHandDataProvider(InputSimulationProfile);
            }
        }

        private void DisableHandSimulation()
        {
            RemoveAllHandDevices();

            if (handDataProvider != null)
            {
                handDataProvider = null;
            }
        }
    }
}
