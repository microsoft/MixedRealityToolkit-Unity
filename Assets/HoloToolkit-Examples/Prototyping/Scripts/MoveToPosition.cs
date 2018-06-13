// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace HoloToolkit.Examples.Prototyping
{
    /// <summary>
    /// A position animation component that supports easing and local position
    /// The animation can be triggered through code or the inspector
    /// 
    /// Supports easing, reverse and reseting positions.
    /// Free easing allows the object to animate by just updating the TargetValue for a more organic feel
    /// 
    /// Typical use:
    /// Set the TargetValue
    /// Call StartRunning();
    /// </summary>
    public class MoveToPosition : MonoBehaviour
    {
        /// <summary>
        /// ease types
        ///     Linear: steady progress
        ///     EaseIn: ramp up in speed
        ///     EaseOut: ramp down in speed
        ///     EaseInOut: ramp up then down in speed
        ///     Free: super ease - just updates as the TargetValue changes
        /// </summary>
        public enum LerpTypes { Linear, EaseIn, EaseOut, EaseInOut, Free }

        [Tooltip("The GameObject with the position to animate")]
        public GameObject TargetObject;

        [Tooltip("The target Vector3 to animate to")]
        public Vector3 TargetValue;

        [Tooltip("Ease type to use for the tween")]
        public LerpTypes LerpType;

        [Tooltip("Duration of the animation in seconds")]
        public float LerpTime = 1f;

        [Tooltip("Auto start? or status")]
        public bool IsRunning = false;

        [Tooltip("Use the local position instead of world position")]
        public bool ToLocalTransform = false;

        [Tooltip("animation is complete!")]
        public UnityEvent OnComplete;

        // animation ticker
        private float mLerpTimeCounter;

        // cached start value, updates every time a new animation starts
        private Vector3 mStartValue;

        /// <summary>
        /// Get the game object and set the start values
        /// </summary>
        private void Awake()
        {
            if (TargetObject == null)
            {
                TargetObject = this.gameObject;
            }
            mStartValue = GetPosition();
        }

        /// <summary>
        /// Start the animation
        /// </summary>
        public void StartRunning()
        {
            if (TargetObject == null)
            {
                TargetObject = this.gameObject;
            }

            mStartValue = GetPosition();
            mLerpTimeCounter = 0;
            IsRunning = true;
        }

        /// <summary>
        /// Reset the position back to the cached starting position
        /// </summary>
        public void ResetTransform()
        {
            if (ToLocalTransform)
            {
                TargetObject.transform.localPosition = mStartValue;
            }
            else
            {
                TargetObject.transform.position = mStartValue;
            }
            IsRunning = false;
            mLerpTimeCounter = 0;
        }

        /// <summary>
        /// Run the animation backwards for a ping-pong effect
        /// </summary>
        public void Reverse()
        {
            TargetValue = mStartValue;
            mStartValue = TargetValue;
            mLerpTimeCounter = 0;
            IsRunning = true;
        }

        /// <summary>
        /// Stop the animation
        /// </summary>
        public void StopRunning()
        {
            IsRunning = false;
        }

        /// <summary>
        /// get the current position
        /// </summary>
        /// <returns></returns>
        private Vector3 GetPosition()
        {
            return ToLocalTransform ? TargetObject.transform.localPosition : TargetObject.transform.position;
        }

        /// <summary>
        /// Calculate the new position based on time and ease settings
        /// </summary>
        /// <param name="currentPosition"></param>
        /// <param name="percent"></param>
        /// <returns></returns>
        private Vector3 GetNewPosition(Vector3 currentPosition, float percent)
        {
            Vector3 newPosition = Vector3.zero;
            switch (LerpType)
            {
                case LerpTypes.Linear:
                    newPosition = Vector3.Lerp(mStartValue, TargetValue, percent);
                    break;
                case LerpTypes.EaseIn:
                    newPosition = Vector3.Lerp(mStartValue, TargetValue, QuadEaseIn(0, 1, percent));
                    break;
                case LerpTypes.EaseOut:
                    newPosition = Vector3.Lerp(mStartValue, TargetValue, QuadEaseOut(0, 1, percent));
                    break;
                case LerpTypes.EaseInOut:
                    newPosition = Vector3.Lerp(mStartValue, TargetValue, QuadEaseInOut(0, 1, percent));
                    break;
                case LerpTypes.Free:
                    newPosition = Vector3.Lerp(currentPosition, TargetValue, percent);
                    break;
                default:
                    break;
            }

            return newPosition;
        }

        // ease curves
        public static float QuadEaseIn(float s, float e, float v)
        {
            return e * (v /= 1) * v + s;
        }

        public static float QuadEaseOut(float s, float e, float v)
        {
            return -e * (v /= 1) * (v - 2) + s;
        }

        public static float QuadEaseInOut(float s, float e, float v)
        {
            if ((v /= 0.5f) < 1)
                return e / 2 * v * v + s;

            return -e / 2 * ((--v) * (v - 2) - 1) + s;
        }

        /// <summary>
        /// Animate!
        /// </summary>
        private void Update()
        {
            if (IsRunning && LerpType != LerpTypes.Free)
            {

                mLerpTimeCounter += Time.deltaTime;
                float percent = mLerpTimeCounter / LerpTime;

                if (ToLocalTransform)
                {
                    TargetObject.transform.localPosition = GetNewPosition(TargetObject.transform.localPosition, percent);
                }
                else
                {
                    TargetObject.transform.position = GetNewPosition(TargetObject.transform.position, percent);
                }

                if (percent >= 1)
                {
                    IsRunning = false;
                    OnComplete.Invoke();
                }
            }
            else if (LerpType == LerpTypes.Free)
            {
                bool wasRunning = IsRunning;
                if (ToLocalTransform)
                {
                    TargetObject.transform.localPosition = GetNewPosition(TargetObject.transform.localPosition, LerpTime * Time.deltaTime);
                    IsRunning = TargetObject.transform.localPosition != TargetValue;
                }
                else
                {
                    TargetObject.transform.position = GetNewPosition(TargetObject.transform.position, LerpTime * Time.deltaTime);
                    IsRunning = TargetObject.transform.localPosition != TargetValue;
                }

                if (IsRunning != wasRunning && !IsRunning)
                {
                    OnComplete.Invoke();
                }
            }
        }
    }
}
