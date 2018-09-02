// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Definitions.Devices;
using Microsoft.MixedReality.Toolkit.Core.Definitions.Physics;
using Microsoft.MixedReality.Toolkit.Core.Definitions.Utilities;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.Devices;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.InputSystem;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.InputSystem.Handlers;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.Physics;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.TeleportSystem;
using Microsoft.MixedReality.Toolkit.Core.Managers;
using Microsoft.MixedReality.Toolkit.Core.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Devices.UnityInput
{
    /// <summary>
    /// Manages Touch devices using unity input system.
    /// </summary>
    public class UnityTouchDeviceManager : BaseDeviceManager
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="priority"></param>
        public UnityTouchDeviceManager(string name, uint priority) : base(name, priority) { }

        /// <summary>
        /// Internal Touch Pointer Implementation.
        /// </summary>
        private class TouchPointer : IMixedRealityPointer
        {
            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="pointerName"></param>
            public TouchPointer(string pointerName)
            {
                InputSystem = MixedRealityManager.Instance.GetManager<IMixedRealityInputSystem>();
                PointerId = InputSystem.FocusProvider.GenerateNewPointerId();
                PointerName = pointerName;
            }

            /// <inheritdoc />
            public IMixedRealityInputSystem InputSystem { get; }

            /// <inheritdoc />
            public virtual IMixedRealityController Controller
            {
                get { return controller; }
                set
                {
                    controller = value;
                    inputSourceParent = controller.InputSource;
                }
            }

            private IMixedRealityController controller;

            /// <inheritdoc />
            public uint PointerId { get; }

            /// <inheritdoc />
            public string PointerName { get; set; }

            /// <inheritdoc />
            public virtual IMixedRealityInputSource InputSourceParent
            {
                get { return inputSourceParent; }
                set { inputSourceParent = value; }
            }

            private IMixedRealityInputSource inputSourceParent;

            /// <inheritdoc />
            public IMixedRealityCursor BaseCursor { get; set; }

            /// <inheritdoc />
            public ICursorModifier CursorModifier { get; set; }

            /// <inheritdoc />
            public IMixedRealityTeleportHotSpot TeleportHotSpot { get; set; }

            /// <inheritdoc />
            public bool IsInteractionEnabled { get; set; }

            /// <inheritdoc />
            public bool IsFocusLocked { get; set; }

            /// <inheritdoc />
            public virtual float PointerExtent { get; set; } = 10f;

            /// <inheritdoc />
            public RayStep[] Rays { get; protected set; } = { new RayStep(Vector3.zero, Vector3.forward) };

            /// <inheritdoc />
            public LayerMask[] PrioritizedLayerMasksOverride { get; set; }

            /// <inheritdoc />
            public IMixedRealityFocusHandler FocusTarget { get; set; }

            /// <inheritdoc />
            public IPointerResult Result { get; set; }

            /// <inheritdoc />
            public IBaseRayStabilizer RayStabilizer { get; set; }

            /// <inheritdoc />
            public RaycastModeType RaycastMode { get; set; } = RaycastModeType.Simple;

            /// <inheritdoc />
            public float SphereCastRadius { get; set; }

            public float PointerOrientation { get; } = 0f;

            /// <inheritdoc />
            public virtual void OnPreRaycast()
            {
                Ray pointingRay;
                if (TryGetPointingRay(out pointingRay))
                {
                    Rays[0].CopyRay(pointingRay, PointerExtent);
                }

                if (RayStabilizer != null)
                {
                    RayStabilizer.UpdateStability(Rays[0].Origin, Rays[0].Direction);
                    Rays[0].CopyRay(RayStabilizer.StableRay, PointerExtent);
                }
            }

            /// <inheritdoc />
            public virtual void OnPostRaycast() { }

            /// <inheritdoc />
            public virtual bool TryGetPointerPosition(out Vector3 position)
            {
                position = Vector3.zero;
                return false;
            }

            /// <inheritdoc />
            public virtual bool TryGetPointingRay(out Ray pointingRay)
            {
                pointingRay = default(Ray);
                return false;
            }

            /// <inheritdoc />
            public virtual bool TryGetPointerRotation(out Quaternion rotation)
            {
                rotation = Quaternion.identity;
                return false;
            }

            #region IEquality Implementation

            public static bool Equals(IMixedRealityPointer left, IMixedRealityPointer right)
            {
                return left.Equals(right);
            }

            bool IEqualityComparer.Equals(object left, object right)
            {
                return left.Equals(right);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) { return false; }
                if (ReferenceEquals(this, obj)) { return true; }
                if (obj.GetType() != GetType()) { return false; }

                return Equals((IMixedRealityPointer)obj);
            }

            private bool Equals(IMixedRealityPointer other)
            {
                return other != null && PointerId == other.PointerId && string.Equals(PointerName, other.PointerName);
            }

            int IEqualityComparer.GetHashCode(object obj)
            {
                return obj.GetHashCode();
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    int hashCode = 0;
                    hashCode = (hashCode * 397) ^ (int)PointerId;
                    return hashCode;
                }
            }

            #endregion IEquality Implementation
        }

        private static readonly Dictionary<int, UnityTouchController> ActiveTouches = new Dictionary<int, UnityTouchController>();

        /// <inheritdoc />
        public override void Update()
        {
            for (var i = 0; i < Input.touches.Length; i++)
            {
                Touch touch = Input.touches[i];

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
            foreach (var controller in ActiveTouches)
            {
                if (controller.Value == null) { continue; }

                foreach (var inputSource in InputSystem.DetectedInputSources)
                {
                    if (inputSource.SourceId == controller.Value.InputSource.SourceId)
                    {
                        InputSystem?.RaiseSourceLost(controller.Value.InputSource, controller.Value);
                    }
                }
            }

            ActiveTouches.Clear();
        }

        private void AddTouchController(Touch touch, Ray ray)
        {
            UnityTouchController controller;
            if (!ActiveTouches.TryGetValue(touch.fingerId, out controller))
            {
                var touchPointer = new TouchPointer($"Touch Pointer {touch.fingerId}");
                var inputSource = InputSystem?.RequestNewGenericInputSource($"Touch {touch.fingerId}", new IMixedRealityPointer[] { touchPointer });
                touchPointer.InputSourceParent = inputSource;

                controller = new UnityTouchController(TrackingState.NotApplicable, Handedness.Any, inputSource);
                controller.SetupConfiguration(typeof(UnityTouchController));
                ActiveTouches.Add(touch.fingerId, controller);
            }

            InputSystem?.RaiseSourceDetected(controller.InputSource, controller);
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
            controller.ScreenPointRay = ray;
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
            InputSystem?.RaiseSourceLost(controller.InputSource, controller);
        }
    }
}
