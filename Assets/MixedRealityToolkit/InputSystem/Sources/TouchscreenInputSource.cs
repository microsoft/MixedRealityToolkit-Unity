// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.InputSystem.Pointers;
using Microsoft.MixedReality.Toolkit.Internal.Definitions.InputSystem;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces.Devices;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces.InputSystem;
using Microsoft.MixedReality.Toolkit.Internal.Utilities;
using Microsoft.MixedReality.Toolkit.Internal.Utilities.Async;
using Microsoft.MixedReality.Toolkit.Internal.Utilities.Async.AwaitYieldInstructions;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.InputSystem.Sources
{
    /// <summary>
    /// Input source supporting basic touchscreen input:
    /// * taps
    /// * holds
    /// Note that a hold-started is raised as soon as a contact exceeds the Epsilon value;
    /// if the contact subsequently qualifies as a tap then a hold-canceled is also raised.
    /// </summary>
    public class TouchscreenInputSource : BaseGenericInputSource
    {
        private class TouchPointer : GenericPointer
        {
            public readonly Touch TouchData;
            public readonly Ray ScreenPointRay;
            public float Lifetime;

            public TouchPointer(string name, Touch touch, Ray ray, IMixedRealityInputSource inputSource) : base(name, inputSource)
            {
                TouchData = touch;
                ScreenPointRay = ray;
                Lifetime = 0.0f;
            }

            /// <inheritdoc />
            public override IMixedRealityController Controller { get; set; }
        }

        private const float K_CONTACT_EPSILON = 2.0f / 60.0f;

        /// <summary>
        /// Constructor.
        /// </summary>
        public TouchscreenInputSource() : base("Touch Screen Input Source")
        {
            if (Application.isPlaying)
            {
                Run();
            }
        }

        public override IMixedRealityPointer[] Pointers
        {
            get
            {
                var pointers = new IMixedRealityPointer[activeTouches.Count];
                int count = 0;

                foreach (var touch in activeTouches)
                {
                    pointers[count++] = touch;
                }

                return pointers;
            }
        }

        [SerializeField]
        [Tooltip("Time in seconds to determine if the contact registers as a tap or a hold")]
        protected float MaxTapContactTime = 0.5f;

        private readonly HashSet<TouchPointer> activeTouches = new HashSet<TouchPointer>();

        private static readonly WaitForUpdate NextUpdate = new WaitForUpdate();

        /// <summary>
        /// Action to raise when pointer down event is raised by Touch Screen Input Source.
        /// </summary>
        public static MixedRealityInputAction PointerAction { get; set; } = MixedRealityInputAction.None;

        /// <summary>
        /// Action to raise when hold event is raised by Touch Screen Input Source.
        /// </summary>
        public static MixedRealityInputAction HoldAction { get; set; } = MixedRealityInputAction.None;

        private async void Run()
        {
            while (true)
            {
                for (var i = 0; i < Input.touches.Length; i++)
                {
                    Touch touch = Input.touches[i];
                    // Construct a ray from the current touch coordinates
                    Ray ray = CameraCache.Main.ScreenPointToRay(touch.position);

                    switch (touch.phase)
                    {
                        case TouchPhase.Began:
                        case TouchPhase.Moved:
                        case TouchPhase.Stationary:
                            AddOrUpdateTouch(touch, ray);
                            break;

                        case TouchPhase.Ended:
                        case TouchPhase.Canceled:
                            RemoveTouch(touch);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                await NextUpdate;
            }
        }

        #region Input Touch Events

        private void AddOrUpdateTouch(Touch touch, Ray ray)
        {
            foreach (var knownTouch in activeTouches)
            {
                if (knownTouch.TouchData.fingerId == touch.fingerId)
                {
                    knownTouch.Lifetime += Time.deltaTime;
                    return;
                }
            }

            if (activeTouches.Count == 0)
            {
                InputSystem.RaiseSourceDetected(this);
            }

            var newTouch = new TouchPointer($"Touch {touch.fingerId}", touch, ray, this);
            activeTouches.Add(newTouch);
            InputSystem.RaisePointerDown(newTouch, PointerAction);
            InputSystem.RaiseHoldStarted(this, HoldAction);
        }

        private void RemoveTouch(Touch touch)
        {
            foreach (var knownTouch in activeTouches)
            {
                if (knownTouch.TouchData.fingerId == touch.fingerId)
                {
                    if (touch.phase == TouchPhase.Ended)
                    {
                        if (knownTouch.Lifetime < K_CONTACT_EPSILON)
                        {
                            InputSystem.RaiseHoldCanceled(this, HoldAction);
                        }
                        else if (knownTouch.Lifetime < MaxTapContactTime)
                        {
                            InputSystem.RaiseHoldCanceled(this, HoldAction);
                            InputSystem.RaisePointerClicked(knownTouch, PointerAction, knownTouch.TouchData.tapCount);
                        }
                        else
                        {
                            InputSystem.RaiseHoldCompleted(this, HoldAction);
                        }
                    }
                    else
                    {
                        InputSystem.RaiseHoldCanceled(this, HoldAction);
                    }

                    InputSystem.RaisePointerUp(knownTouch, PointerAction);
                    activeTouches.Remove(knownTouch);

                    if (activeTouches.Count == 0)
                    {
                        InputSystem.RaiseSourceLost(this);
                    }
                }
            }
        }

        #endregion Input Touch Events

        /// <summary>
        /// Searches through the list of active touches for the input with the matching  Pointer Id.
        /// </summary>
        /// <param name="pointerId">T</param>
        /// <param name="touch"></param>
        /// <returns><see cref="TouchPointer"/></returns>
        private bool TryGetTouchPointer(uint pointerId, out TouchPointer touch)
        {
            foreach (var activeTouch in activeTouches)
            {
                if (activeTouch.PointerId == pointerId)
                {
                    touch = activeTouch;
                    return true;
                }
            }

            touch = null;
            return false;
        }

        /// <summary>
        /// Try to get the current Pointer Position input reading from the specified Pointer Id.
        /// </summary>
        /// <param name="pointerId"></param>
        /// <param name="position"></param>
        /// <returns>True if data is available.</returns>
        public bool TryGetPointerPosition(uint pointerId, out Vector3 position)
        {
            TouchPointer knownTouch;
            if (TryGetTouchPointer(pointerId, out knownTouch))
            {
                position = knownTouch.TouchData.position;
                return true;
            }

            position = Vector3.zero;
            return false;
        }

        /// <summary>
        /// Try to get the current Pointing Ray input reading from the specified Pointer Id.
        /// </summary>
        /// <param name="pointerId"></param>
        /// <param name="pointingRay"></param>
        /// <returns>True if data is available.</returns>
        public bool TryGetPointingRay(uint pointerId, out Ray pointingRay)
        {
            TouchPointer knownTouch;
            if (TryGetTouchPointer(pointerId, out knownTouch))
            {
                pointingRay = knownTouch.ScreenPointRay;
                return true;
            }

            pointingRay = default(Ray);
            return false;
        }
    }
}
