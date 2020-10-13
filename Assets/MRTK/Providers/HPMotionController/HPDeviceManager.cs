// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Collections.Generic;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.XR;
using Microsoft.MixedReality.Input;

namespace Microsoft.MixedReality.Toolkit.HP.Input
{
    public class MotionControllerState
    {
        public MotionControllerState(MotionController mc)
        {
            this.MotionController = mc;
        }
        public void Update(DateTime currentTime)
        {
            this.CurrentReading = MotionController.TryGetReadingAtTime(currentTime);
        }
        public MotionController MotionController { get; private set; }
        public MotionControllerReading CurrentReading { get; private set; }

    }

    /// <summary>
    /// Manages XR SDK devices.
    /// </summary>
    [MixedRealityDataProvider(
        typeof(IMixedRealityInputSystem),
        SupportedPlatforms.WindowsStandalone | SupportedPlatforms.MacStandalone | SupportedPlatforms.LinuxStandalone | SupportedPlatforms.WindowsUniversal,
        "XRSDK Device Manager")]
    public class HPDeviceManager : BaseInputDeviceManager, IMixedRealityCapabilityCheck
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="inputSystem">The <see cref="Microsoft.MixedReality.Toolkit.Input.IMixedRealityInputSystem"/> instance that receives data from this provider.</param>
        /// <param name="name">Friendly name of the service.</param>
        /// <param name="priority">Service priority. Used to determine order of instantiation.</param>
        /// <param name="profile">The service's configuration profile.</param>
        public HPDeviceManager(
            IMixedRealityInputSystem inputSystem,
            string name = null,
            uint priority = DefaultPriority,
            BaseMixedRealityProfile profile = null) : base(inputSystem, name, priority, profile) { }

        /// <inheritdoc />
        public virtual bool CheckCapability(MixedRealityCapability capability)
        {
            // The XR SDK platform supports motion controllers.
            return (capability == MixedRealityCapability.MotionController);
        }

        private MotionControllerWatcher motionControllerWatcher;

        /// <summary>
        /// Dictionary to capture all active controllers detected
        /// </summary>
        private readonly Dictionary<uint, HPMotionController> activeControllers = new Dictionary<uint, HPMotionController>();
        private readonly Dictionary<uint, MotionControllerState> trackedControllerStates = new Dictionary<uint, MotionControllerState>();

        //private readonly List<InputDevice> inputDevices = new List<InputDevice>();
        //private readonly List<InputDevice> inputDevicesSubset = new List<InputDevice>();
        //private readonly List<InputDevice> lastInputDevices = new List<InputDevice>();

        private List<InputDeviceCharacteristics> GenericDesiredInputCharacteristics = new List<InputDeviceCharacteristics>()
        {
            InputDeviceCharacteristics.Controller
        };
        protected virtual List<InputDeviceCharacteristics> DesiredInputCharacteristics
        {
            get { return GenericDesiredInputCharacteristics; }
            set { GenericDesiredInputCharacteristics = value; }
        }

        private static readonly ProfilerMarker UpdatePerfMarker = new ProfilerMarker("[MRTK] HPDeviceManager.Update");

        /// <inheritdoc/>
        public override void Enable()
        {
            if (!Application.isPlaying) { return; }

            motionControllerWatcher = new MotionControllerWatcher();
            motionControllerWatcher.MotionControllerAdded += AddController;
            motionControllerWatcher.MotionControllerRemoved += RemoveController;
            var nowait = motionControllerWatcher.StartAsync();
        }

        private uint GetKey(uint vid, uint pid, uint version, uint handedness)
        {
            return (vid << 48) + (pid << 32) + (version << 16) + handedness;
        }
        private uint GetKey(MotionController mc)
        {
            var handedness = ((uint)(mc.Handedness == MixedReality.Input.Handedness.Right ? 2 : (mc.Handedness == MixedReality.Input.Handedness.Left ? 1 : 0)));
            return GetKey(mc.VendorId, mc.ProductId, mc.Version, handedness);
        }

        private void AddController(object sender, MotionController e)
        {
            lock (trackedControllerStates)
            {
                Utilities.Handedness controllingHand;
                switch (e.Handedness)
                {
                    default:
                        controllingHand = Utilities.Handedness.None;
                        break;
                    case MixedReality.Input.Handedness.Left:
                        controllingHand = Utilities.Handedness.Left;
                        break;
                    case MixedReality.Input.Handedness.Right:
                        controllingHand = Utilities.Handedness.Right;
                        break;
                }

                IMixedRealityPointer[] pointers = RequestPointers(SupportedControllerType.HPMotionController, controllingHand);
                InputSourceType inputSourceType = InputSourceType.Controller;

                string nameModifier = controllingHand == Utilities.Handedness.None ? inputSourceType.ToString() : controllingHand.ToString();
                var inputSource = Service?.RequestNewGenericInputSource($"MP Motion Controller {nameModifier}", pointers, inputSourceType);

                HPMotionController detectedController;
                detectedController = new HPMotionController(TrackingState.NotTracked, controllingHand, inputSource);
                if (!detectedController.Enabled)
                {
                    // Controller failed to be setup correctly.
                    // Exit early so we don't start tracking the controller in our data structures
                    return;
                }

                for (int i = 0; i < detectedController.InputSource?.Pointers?.Length; i++)
                {
                    detectedController.InputSource.Pointers[i].Controller = detectedController;
                }

                uint controllerId = GetKey(e);

                activeControllers.Add(controllerId, detectedController);
                trackedControllerStates[controllerId] = new MotionControllerState(e);
            }
        }

        private void RemoveController(object sender, MotionController e)
        {
            lock (trackedControllerStates)
            {
                uint controllerId = GetKey(e);

                if(!(activeControllers.ContainsKey(controllerId) && trackedControllerStates.ContainsKey(controllerId)))
                {
                    // for some reason this controller was never tracked in the first place, ignore it
                    return;
                }

                var controller = activeControllers[controllerId];

                RecyclePointers(controller.InputSource);

                var visualizer = controller.Visualizer;

                if (visualizer != null && !visualizer.Equals(null) &&
                    visualizer.GameObjectProxy != null)
                {
                    visualizer.GameObjectProxy.SetActive(false);
                }

                trackedControllerStates.Remove(controllerId);
                activeControllers.Remove(controllerId);
            }
        }


        /// <inheritdoc/>
        public override void Update()
        {
            using (UpdatePerfMarker.Auto())
            {
                base.Update();

                foreach (MotionControllerState controllerState in trackedControllerStates.Values)
                {
                    uint controllerId = GetKey(controllerState.MotionController);
                    var controller = activeControllers[controllerId];

                    controllerState.Update(DateTime.Now);
                    controller.UpdateController(controllerState);
                }
            }
        }
    }
}
