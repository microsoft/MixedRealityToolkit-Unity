// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections.Generic;
using UnityEngine;
using UInput = UnityEngine.Input;

namespace Microsoft.MixedReality.Toolkit.Input.UnityInput
{
    /// <summary>
    /// Manages Touch devices using unity input system.
    /// </summary>
    [MixedRealityDataProvider(
        typeof(IMixedRealityInputSystem),
        (SupportedPlatforms)(-1),  // All platforms supported by Unity
        "Unity Touch Device Manager")]
    public class UnityTouchDeviceManager : BaseInputDeviceManager
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="registrar">The <see cref="IMixedRealityServiceRegistrar"/> instance that loaded the data provider.</param>
        /// <param name="inputSystem">The <see cref="Microsoft.MixedReality.Toolkit.Input.IMixedRealityInputSystem"/> instance that receives data from this provider.</param>
        /// <param name="name">Friendly name of the service.</param>
        /// <param name="priority">Service priority. Used to determine order of instantiation.</param>
        /// <param name="profile">The service's configuration profile.</param>
        public UnityTouchDeviceManager(
            IMixedRealityServiceRegistrar registrar,
            IMixedRealityInputSystem inputSystem,
            string name = null,
            uint priority = DefaultPriority,
            BaseMixedRealityProfile profile = null) : base(registrar, inputSystem, name, priority, profile) { }

        private static readonly Dictionary<int, UnityTouchController> ActiveTouches = new Dictionary<int, UnityTouchController>();

        /// <inheritdoc />
        public override void Update()
        {
            int touchCount = UInput.touchCount;
            for (var i = 0; i < touchCount; i++)
            {
                Touch touch = UInput.touches[i];

                // Construct a ray from the current touch coordinates
                Ray ray = CameraCache.Main.ScreenPointToRay(touch.position);

                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        AddTouchController(touch, ray);
                        break;
                    case TouchPhase.Moved:
                    case TouchPhase.Stationary:
                        UpdateTouchData(touch, ray);
                        break;
                    case TouchPhase.Ended:
                    case TouchPhase.Canceled:
                        RemoveTouchController(touch);
                        break;
                }
            }

            foreach (var controller in ActiveTouches)
            {
                controller.Value?.Update();
            }
        }

        /// <inheritdoc />
        public override void Disable()
        {
            IMixedRealityInputSystem inputSystem = Service as IMixedRealityInputSystem;
            
            foreach (var controller in ActiveTouches)
            {
                if (controller.Value == null || inputSystem == null) { continue; }

                foreach (var inputSource in inputSystem.DetectedInputSources)
                {
                    if (inputSource.SourceId == controller.Value.InputSource.SourceId)
                    {
                        inputSystem.RaiseSourceLost(controller.Value.InputSource, controller.Value);
                    }
                }
            }

            ActiveTouches.Clear();
        }

        private void AddTouchController(Touch touch, Ray ray)
        {
            UnityTouchController controller;
            IMixedRealityInputSystem inputSystem = Service as IMixedRealityInputSystem;

            if (!ActiveTouches.TryGetValue(touch.fingerId, out controller))
            {
                IMixedRealityInputSource inputSource = null;

                if (inputSystem != null)
                {
                    var pointers = RequestPointers(SupportedControllerType.TouchScreen, Handedness.Any);
                    inputSource = inputSystem.RequestNewGenericInputSource($"Touch {touch.fingerId}", pointers);
                }

                controller = new UnityTouchController(TrackingState.NotApplicable, Handedness.Any, inputSource);

                if (inputSource != null)
                {
                    for (int i = 0; i < inputSource.Pointers.Length; i++)
                    {
                        inputSource.Pointers[i].Controller = controller;
                        var touchPointer = (IMixedRealityTouchPointer)inputSource.Pointers[i];
                        touchPointer.TouchRay = ray;
                        touchPointer.FingerId = touch.fingerId;
                    }
                }

                controller.SetupConfiguration(typeof(UnityTouchController));
                ActiveTouches.Add(touch.fingerId, controller);
            }

            inputSystem?.RaiseSourceDetected(controller.InputSource, controller);
            controller.StartTouch();
            UpdateTouchData(touch, ray);
        }

        private void UpdateTouchData(Touch touch, Ray ray)
        {
            UnityTouchController controller;

            if (!ActiveTouches.TryGetValue(touch.fingerId, out controller))
            {
                return;
            }

            controller.TouchData = touch;
            var pointer = (IMixedRealityTouchPointer)controller.InputSource.Pointers[0];
            controller.ScreenPointRay = pointer.TouchRay = ray;
            controller.Update();
        }

        private void RemoveTouchController(Touch touch)
        {
            UnityTouchController controller;

            if (!ActiveTouches.TryGetValue(touch.fingerId, out controller))
            {
                return;
            }

            controller.EndTouch();
            IMixedRealityInputSystem inputSystem = Service as IMixedRealityInputSystem;
            inputSystem?.RaiseSourceLost(controller.InputSource, controller);
        }
    }
}
