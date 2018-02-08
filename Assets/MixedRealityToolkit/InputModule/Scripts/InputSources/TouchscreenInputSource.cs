// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using MixedRealityToolkit.Common;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace MixedRealityToolkit.InputModule.InputSources
{
    /// <summary>
    /// Input source supporting basic touchscreen input:
    /// * taps
    /// * holds
    /// Note that a hold-started is raised as soon as a contact exceeds the Epsilon value;
    /// if the contact subsequently qualifies as a tap then a hold-canceled is also raised.
    /// </summary>
    public class TouchscreenInputSource : Singleton<TouchscreenInputSource>
    {
        const float K_CONTACT_EPSILON = 2.0f / 60.0f;

        [SerializeField]
        [Tooltip("Time in seconds to determine if the contact registers as a tap or a hold")]
        protected float MaxTapContactTime = 0.5f;

        private HashSet<TouchInputSource> activeTouches = new HashSet<TouchInputSource>();

        private class TouchInputSource : GenericInputSource
        {
            public readonly Touch TouchData;
            public readonly Ray ScreenPointRay;
            public float Lifetime;

            public TouchInputSource(string name, Touch touch, Ray ray) : base(name, SupportedInputInfo.Position | SupportedInputInfo.Pointing)
            {
                TouchData = touch;
                ScreenPointRay = ray;
                Lifetime = 0.0f;
            }
        }

        #region Unity Methods

        private void Start()
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

        #endregion Unity Methods

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

            var newTouch = new TouchInputSource(string.Format("Touch {0}", touch.fingerId), touch, ray);
            activeTouches.Add(newTouch);
            InputManager.Instance.RaiseSourceDetected(newTouch);
            InputManager.Instance.RaiseHoldStarted(newTouch);
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
                            InputManager.Instance.RaiseHoldCanceled(knownTouch);
                        }
                        else if (knownTouch.Lifetime < MaxTapContactTime)
                        {
                            InputManager.Instance.RaiseHoldCanceled(knownTouch);
                            InputManager.Instance.RaiseInputClicked(knownTouch.Pointers[0], knownTouch.TouchData.tapCount);
                        }
                        else
                        {
                            InputManager.Instance.RaiseHoldCompleted(knownTouch);
                        }
                    }
                    else
                    {
                        InputManager.Instance.RaiseHoldCanceled(knownTouch);
                    }

                    activeTouches.Remove(knownTouch);
                    InputManager.Instance.RaiseSourceLost(knownTouch);
                }
            }
        }

        #endregion Input Touch Events

        #region Base Input Source Implementation

        /// <summary>
        /// Searches through the list of active touches for the input with the matching sourceId.
        /// </summary>
        /// <param name="sourceId">T</param>
        /// <param name="touch"></param>
        /// <returns><see cref="TouchInputSource"/></returns>
        private bool TryGetTouchInputSource(int sourceId, out TouchInputSource touch)
        {
            foreach (var _touch in activeTouches)
            {
                if (_touch.SourceId == sourceId)
                {
                    touch = _touch;
                    return true;
                }
            }

            touch = null;
            return false;
        }

        /// <summary>
        /// Try to get the current Pointer Position input reading from the specified Input Source.
        /// </summary>
        /// <param name="sourceId"></param>
        /// <param name="position"></param>
        /// <returns>True if data is available.</returns>
        public bool TryGetPointerPosition(uint sourceId, out Vector3 position)
        {
            TouchInputSource knownTouch;
            if (TryGetTouchInputSource((int)sourceId, out knownTouch))
            {
                position = knownTouch.TouchData.position;
                return true;
            }

            position = Vector3.zero;
            return false;
        }

        /// <summary>
        /// Try to get the current Pointing Ray input reading from the specified Input Source.
        /// </summary>
        /// <param name="sourceId"></param>
        /// <param name="pointingRay"></param>
        /// <returns>True if data is available.</returns>
        public bool TryGetPointingRay(uint sourceId, out Ray pointingRay)
        {
            TouchInputSource knownTouch;
            if (TryGetTouchInputSource((int)sourceId, out knownTouch))
            {
                pointingRay = knownTouch.ScreenPointRay;
                return true;
            }

            pointingRay = default(Ray);
            return false;
        }

        /// <summary>
        /// Get the Supported Input Info for the specified Input Source.
        /// </summary>
        /// <param name="sourceId"></param>
        /// <returns><see cref="SupportedInputInfo"/></returns>
        public SupportedInputInfo GetSupportedInputInfo(uint sourceId)
        {
            TouchInputSource knownTouch;
            return TryGetTouchInputSource((int)sourceId, out knownTouch)
                ? knownTouch.GetSupportedInputInfo()
                : SupportedInputInfo.None;
        }

        #endregion Base Input Source Implementation
    }
}
