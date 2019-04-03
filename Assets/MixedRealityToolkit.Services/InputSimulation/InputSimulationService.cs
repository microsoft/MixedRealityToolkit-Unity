// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    [MixedRealityDataProvider(
        typeof(IMixedRealityInputSystem),
        SupportedPlatforms.WindowsEditor,
        "Input Simulation Service",
        "Profiles/DefaultMixedRealityInputSimulationProfile.asset", 
        "MixedRealityToolkit.SDK")]
    public class InputSimulationService : BaseInputDeviceManager
    {
        private ManualCameraControl cameraControl = null;
        private SimulatedHandDataProvider handDataProvider = null;

        /// <summary>
        /// Dictionary to capture all active hands detected
        /// </summary>
        private readonly Dictionary<Handedness, SimulatedHand> trackedHands = new Dictionary<Handedness, SimulatedHand>();

        #region BaseInputDeviceManager Implementation

        public InputSimulationService(
            IMixedRealityServiceRegistrar registrar, 
            IMixedRealityInputSystem inputSystem,
            MixedRealityInputSystemProfile inputSystemProfile,
            Transform playspace,
            string name, 
            uint priority, 
            BaseMixedRealityProfile profile) : base(registrar, inputSystem, inputSystemProfile, playspace, name, priority, profile)
        {
        }

        /// <inheritdoc />
        public override void Initialize()
        {
        }

        /// <inheritdoc />
        public override void Enable()
        {
        }

        /// <inheritdoc />
        public override void Disable()
        {
            DisableCameraControl();
            DisableHandSimulation();
        }

        /// <inheritdoc />
        public override void Update()
        {
            var profile = GetInputSimulationProfile();

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
                MixedRealityToolkit.InputSystem?.EyeGazeProvider?.UpdateEyeGaze(null, new Ray(CameraCache.Main.transform.position, CameraCache.Main.transform.forward), System.DateTime.UtcNow);
            }

            switch (profile.HandSimulationMode)
            {
                case HandSimulationMode.Disabled:
                    DisableHandSimulation();
                    break;

                case HandSimulationMode.Articulated:
                    EnableHandSimulation();
                    handDataProvider.Update();
                    break;

                case HandSimulationMode.Gestures:
                    EnableHandSimulation();
                    handDataProvider.Update();
                    break;
            }
        }

        #endregion BaseInputDeviceManager Implementation

        /// <summary>
        /// Return the service profile and ensure that the type is correct
        /// </summary>
        public MixedRealityInputSimulationProfile GetInputSimulationProfile()
        {
            var profile = ConfigurationProfile as MixedRealityInputSimulationProfile;
            if (!profile)
            {
                Debug.LogError("Profile for Input Simulation Service must be a MixedRealityInputSimulationProfile");
            }
            return profile;
        }

        private void EnableCameraControl()
        {
            if (cameraControl == null)
            {
                cameraControl = new ManualCameraControl(GetInputSimulationProfile());
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
                handDataProvider = new SimulatedHandDataProvider(GetInputSimulationProfile());
                handDataProvider.OnHandDataChanged += OnHandDataChanged;
            }
        }

        private void DisableHandSimulation()
        {
            RemoveAllHandDevices();

            if (handDataProvider != null)
            {
                handDataProvider.OnHandDataChanged -= OnHandDataChanged;
                handDataProvider = null;
            }
        }

        private void OnHandDataChanged()
        {
            UpdateHandInputSource(Handedness.Left, handDataProvider.CurrentFrameLeft);
            UpdateHandInputSource(Handedness.Right, handDataProvider.CurrentFrameRight);
        }

        // Register input sources for hands based on changes of the data provider
        private void UpdateHandInputSource(Handedness handedness, SimulatedHandData handData)
        {
            var profile = GetInputSimulationProfile();

            if (profile.HandSimulationMode == HandSimulationMode.Disabled)
            {
                RemoveAllHandDevices();
            }
            else
            {
                if (handData != null && handData.IsTracked)
                {
                    SimulatedHand controller = GetOrAddHandDevice(handedness, profile.HandSimulationMode);
                    controller.UpdateState(handData);
                }
                else
                {
                    RemoveHandDevice(handedness);
                }
            }
        }

        private SimulatedHand GetHandDevice(Handedness handedness)
        {
            if (trackedHands.TryGetValue(handedness, out SimulatedHand controller))
            {
                return controller;
            }
            return null;
        }

        private SimulatedHand GetOrAddHandDevice(Handedness handedness, HandSimulationMode simulationMode)
        {
            var controller = GetHandDevice(handedness);
            if (controller != null)
            {
                if (controller.SimulationMode == simulationMode)
                {
                    return controller;
                }
                else
                {
                    // Remove and recreate hand device if simulation mode doesn't match
                    RemoveHandDevice(handedness);
                }
            }

            SupportedControllerType st = simulationMode == HandSimulationMode.Gestures ? SupportedControllerType.GGVHand : SupportedControllerType.ArticulatedHand;
            IMixedRealityPointer[] pointers = RequestPointers(st, handedness);

            var inputSource = MixedRealityToolkit.InputSystem?.RequestNewGenericInputSource($"{handedness} Hand", pointers, InputSourceType.Hand);
            switch (simulationMode)
            {
                case HandSimulationMode.Articulated:
                    controller = new SimulatedArticulatedHand(TrackingState.Tracked, handedness, inputSource);
                    break;
                case HandSimulationMode.Gestures:
                    controller = new SimulatedGestureHand(TrackingState.Tracked, handedness, inputSource);
                    break;
                default:
                    controller = null;
                    break;
            }

            System.Type controllerType = simulationMode == HandSimulationMode.Gestures ? typeof(SimulatedGestureHand) : typeof(SimulatedArticulatedHand);
            if (controller == null)
            {
                Debug.LogError($"Failed to create {controllerType} controller");
                return null;
            }

            if (!controller.SetupConfiguration(controllerType, InputSourceType.Hand))
            {
                // Controller failed to be setup correctly.
                Debug.LogError($"Failed to Setup {controllerType} controller");
                // Return null so we don't raise the source detected.
                return null;
            }

            for (int i = 0; i < controller.InputSource?.Pointers?.Length; i++)
            {
                controller.InputSource.Pointers[i].Controller = controller;
            }

            MixedRealityToolkit.InputSystem?.RaiseSourceDetected(controller.InputSource, controller);

            trackedHands.Add(handedness, controller);

            return controller;
        }

        private void RemoveHandDevice(Handedness handedness)
        {
            var controller = GetHandDevice(handedness);
            if (controller != null)
            {
                MixedRealityToolkit.InputSystem?.RaiseSourceLost(controller.InputSource, controller);

                trackedHands.Remove(handedness);
            }
        }

        private void RemoveAllHandDevices()
        {
            foreach (var controller in trackedHands.Values)
            {
                MixedRealityToolkit.InputSystem?.RaiseSourceLost(controller.InputSource, controller);
            }
            trackedHands.Clear();
        }
    }
}