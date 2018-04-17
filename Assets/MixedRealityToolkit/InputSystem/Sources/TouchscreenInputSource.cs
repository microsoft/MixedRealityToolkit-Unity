// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.MixedReality.Toolkit.InputSystem.Pointers;
using Microsoft.MixedReality.Toolkit.Internal.Definitions;
using Microsoft.MixedReality.Toolkit.Internal.Utilities;
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
    [DisallowMultipleComponent]
    public class TouchscreenInputSource : BaseInputSource
    {
        private const float K_CONTACT_EPSILON = 2.0f / 60.0f;

        public override InputType[] Capabilities => new[] { InputType.Pointer, InputType.PointerPosition };

        public override IPointer[] Pointers
        {
            get
            {
                var pointers = new IPointer[activeTouches.Count];
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

        private class TouchPointer : GenericPointer
        {
            public readonly Touch TouchData;
            public readonly Ray ScreenPointRay;
            public float Lifetime;

            public TouchPointer(string name, Touch touch, Ray ray, IInputSource inputSource) : base(name, inputSource)
            {
                TouchData = touch;
                ScreenPointRay = ray;
                Lifetime = 0.0f;
            }
        }

        #region Monobehaviour Implementation

        private void Awake()
        {
            // Disable the input source if not supported by the device
            if (!Input.touchSupported)
            {
                enabled = false;
            }
        }

        private void Update()
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
        }

        #endregion Monobehaviour Implementation

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
            InputSystem.RaisePointerDown(newTouch);
            InputSystem.RaiseHoldStarted(this);
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
                            InputSystem.RaiseHoldCanceled(this);
                        }
                        else if (knownTouch.Lifetime < MaxTapContactTime)
                        {
                            InputSystem.RaiseHoldCanceled(this);
                            InputSystem.RaiseInputClicked(knownTouch, knownTouch.TouchData.tapCount);
                        }
                        else
                        {
                            InputSystem.RaiseHoldCompleted(this);
                        }
                    }
                    else
                    {
                        InputSystem.RaiseHoldCanceled(this);
                    }

                    InputSystem.RaisePointerUp(knownTouch);
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
