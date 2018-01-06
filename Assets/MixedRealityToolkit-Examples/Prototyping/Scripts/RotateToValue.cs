// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using UnityEngine.Events;

namespace MixedRealityToolkit.Examples.Prototyping
{
    /// <summary>
    /// animates the rotation of an object with eases
    /// </summary>
    public class RotateToValue : MonoBehaviour
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

        [Tooltip("The object to animate")]
        public GameObject TargetObject;

        [Tooltip("The rotation value to animate to")]
        public Quaternion TargetValue;

        [Tooltip("The type of ease to apply to the tween")]
        public LerpTypes LerpType;

        [Tooltip("Duration of the animation in seconds")]
        public float LerpTime = 1f;

        [Tooltip("auto start? or status")]
        public bool IsRunning = false;

        [Tooltip("Use the localRotation instead of world rotation")]
        public bool ToLocalTransform = false;

        [Tooltip("animation complete!")]
        public UnityEvent OnComplete;

        // animation ticker
        private float mLerpTimeCounter;

        // starting/current rotation
        private Quaternion mStartValue;

        /// <summary>
        /// get the game object if not set already
        /// set the cached rotation
        /// </summary>
        private void Awake()
        {
            if (TargetObject == null)
            {
                TargetObject = this.gameObject;
            }
            mStartValue = GetRotation();
        }

        /// <summary>
        /// Start the rotation animation
        /// </summary>
        public void StartRunning()
        {
            if (TargetObject == null)
            {
                TargetObject = this.gameObject;
            }

            mStartValue = GetRotation();
            mLerpTimeCounter = 0;
            IsRunning = true;
        }

        /// <summary>
        /// Set the rotation to the cached starting value
        /// </summary>
        public void ResetTransform()
        {
            if (ToLocalTransform)
            {
                this.transform.localRotation = mStartValue;
            }
            else
            {
                this.transform.rotation = mStartValue;
            }
            IsRunning = false;
            mLerpTimeCounter = 0;
        }

        /// <summary>
        /// reverse the rotation - go back
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

        // get the current rotation
        private Quaternion GetRotation()
        {
            return ToLocalTransform ? TargetObject.transform.localRotation : TargetObject.transform.rotation;
        }

        /// <summary>
        /// Calculate the new rotation based on time and ease settings
        /// </summary>
        /// <param name="currentRotation"></param>
        /// <param name="percent"></param>
        /// <returns></returns>
        private Quaternion GetNewRotation(Quaternion currentRotation, float percent)
        {
            Quaternion newPosition = Quaternion.identity;
            switch (LerpType)
            {
                case LerpTypes.Linear:
                    newPosition = Quaternion.Lerp(mStartValue, TargetValue, percent);
                    break;
                case LerpTypes.EaseIn:
                    newPosition = Quaternion.Lerp(mStartValue, TargetValue, QuadEaseIn(0, 1, percent));
                    break;
                case LerpTypes.EaseOut:
                    newPosition = Quaternion.Lerp(mStartValue, TargetValue, QuadEaseOut(0, 1, percent));
                    break;
                case LerpTypes.EaseInOut:
                    newPosition = Quaternion.Lerp(mStartValue, TargetValue, QuadEaseInOut(0, 1, percent));
                    break;
                case LerpTypes.Free:
                    newPosition = Quaternion.Lerp(currentRotation, TargetValue, percent);
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
        /// Animate
        /// </summary>
        private void Update()
        {
            // manual animation, only runs when auto started or StartRunning() is called
            if (IsRunning && LerpType != LerpTypes.Free)
            {
                // get the time
                mLerpTimeCounter += Time.deltaTime;
                float percent = mLerpTimeCounter / LerpTime;

                // set the rotation
                if (ToLocalTransform)
                {
                    this.transform.localRotation = GetNewRotation(this.transform.localRotation, percent);
                }
                else
                {
                    this.transform.rotation = GetNewRotation(this.transform.rotation, percent);
                }

                // fire the event if complete
                if (percent >= 1)
                {
                    IsRunning = false;
                    OnComplete.Invoke();
                }
            }
            else if (LerpType == LerpTypes.Free) // is always running, just waiting for the TargetValue to change
            {
                bool wasRunning = IsRunning;

                // set the rotation
                if (ToLocalTransform)
                {
                    this.transform.localRotation = GetNewRotation(this.transform.localRotation, LerpTime * Time.deltaTime);
                    IsRunning = this.transform.localRotation != TargetValue;
                }
                else
                {
                    this.transform.rotation = GetNewRotation(this.transform.rotation, LerpTime * Time.deltaTime);
                    IsRunning = this.transform.rotation != TargetValue;
                }

                // fire the event if complete
                if (IsRunning != wasRunning && !IsRunning)
                {
                    OnComplete.Invoke();
                }
            }
        }
    }
}
