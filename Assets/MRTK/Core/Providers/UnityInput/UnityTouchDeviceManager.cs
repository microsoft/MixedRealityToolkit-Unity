﻿// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections.Generic;
using Unity.Profiling;
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
        [System.Obsolete("This constructor is obsolete (registrar parameter is no longer required) and will be removed in a future version of the Microsoft Mixed Reality Toolkit.")]
        public UnityTouchDeviceManager(
            IMixedRealityServiceRegistrar registrar,
            IMixedRealityInputSystem inputSystem,
            string name = null,
            uint priority = DefaultPriority,
            BaseMixedRealityProfile profile = null) : this(inputSystem, name, priority, profile)
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
        public UnityTouchDeviceManager(
            IMixedRealityInputSystem inputSystem,
            string name = null,
            uint priority = DefaultPriority,
            BaseMixedRealityProfile profile = null) : base(inputSystem, name, priority, profile) { }

        private static readonly Dictionary<int, UnityTouchController> ActiveTouches = new Dictionary<int, UnityTouchController>();

        private List<UnityTouchController> touchesToRemove = new List<UnityTouchController>();

        private static readonly ProfilerMarker UpdatePerfMarker = new ProfilerMarker("[MRTK] UnityTouchDeviceManager.Update");

        /// <inheritdoc />
        public override void Update()
        {
            using (UpdatePerfMarker.Auto())
            {
                if (!IsEnabled)
                {
                    return;
                }
                
                base.Update();

                // Ensure that touch up and source lost events are at least one frame apart.
                for (int i = 0; i < touchesToRemove.Count; i++)
                {
                    IMixedRealityController controller = touchesToRemove[i];
                    Service?.RaiseSourceLost(controller.InputSource, controller);
                }
                touchesToRemove.Clear();

                int touchCount = UInput.touchCount;
                for (int i = 0; i < touchCount; i++)
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
            }
        }

        /// <inheritdoc />
        public override void Disable()
        {
            base.Disable();

            foreach (var controller in ActiveTouches)
            {
                if (controller.Value == null || Service == null) { continue; }

                foreach (var inputSource in Service.DetectedInputSources)
                {
                    if (inputSource.SourceId == controller.Value.InputSource.SourceId)
                    {
                        Service.RaiseSourceLost(controller.Value.InputSource, controller.Value);
                    }
                }
            }

            ActiveTouches.Clear();
        }

        private static readonly ProfilerMarker AddTouchControllerPerfMarker = new ProfilerMarker("[MRTK] UnityTouchDeviceManager.AddTouchController");

        private void AddTouchController(Touch touch, Ray ray)
        {
            using (AddTouchControllerPerfMarker.Auto())
            {
                UnityTouchController controller;

                if (!ActiveTouches.TryGetValue(touch.fingerId, out controller))
                {
                    IMixedRealityInputSource inputSource = null;

                    if (Service != null)
                    {
                        var pointers = RequestPointers(SupportedControllerType.TouchScreen, Handedness.Any);
                        inputSource = Service.RequestNewGenericInputSource($"Touch {touch.fingerId}", pointers);
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

                    ActiveTouches.Add(touch.fingerId, controller);
                }

                Service?.RaiseSourceDetected(controller.InputSource, controller);

                controller.TouchData = touch;
                controller.StartTouch();
            }
        }

        private static readonly ProfilerMarker UpdateTouchDataPerfMarker = new ProfilerMarker("[MRTK] UnityTouchDeviceManager.UpdateTouchData");

        private void UpdateTouchData(Touch touch, Ray ray)
        {
            using (UpdateTouchDataPerfMarker.Auto())
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
        }

        private static readonly ProfilerMarker RemoveTouchControllerPerfMarker = new ProfilerMarker("[MRTK] UnityTouchDeviceManager.RemoveTouchController");

        private void RemoveTouchController(Touch touch)
        {
            using (RemoveTouchControllerPerfMarker.Auto())
            {
                UnityTouchController controller;

                if (!ActiveTouches.TryGetValue(touch.fingerId, out controller))
                {
                    return;
                }

                RecyclePointers(controller.InputSource);

                controller.TouchData = touch;
                controller.EndTouch();
                // Schedule the source lost event.
                touchesToRemove.Add(controller);
                // Remove from the active collection
                ActiveTouches.Remove(touch.fingerId);
            }
        }
    }
}
