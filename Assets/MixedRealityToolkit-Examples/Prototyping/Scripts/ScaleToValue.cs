// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using UnityEngine.Events;

namespace MixedRealityToolkit.Examples.Prototyping
{
    /// <summary>
    /// Animates the scaling of an object with eases
    /// </summary>
    public class ScaleToValue : MonoBehaviour
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

        [Tooltip("The object to scale")]
        public GameObject TargetObject;

        [Tooltip("The scale value to animate to")]
        public Vector3 TargetValue;

        [Tooltip("The ease type")]
        public LerpTypes LerpType;

        [Tooltip("The duration of the scale animation in seconds")]
        public float LerpTime = 1f;

        [Tooltip("Auto start? or status")]
        public bool IsRunning = false;

        [Tooltip("Animation complete!")]
        public UnityEvent OnComplete;

        // animation ticker
        private float mLerpTimeCounter;

        // cached start scale
        private Vector3 mStartValue;

        /// <summary>
        /// get the target object if not set already
        /// Set the cached starting scale
        /// </summary>
        private void Awake()
        {
            if (TargetObject == null)
            {
                TargetObject = this.gameObject;
            }
            mStartValue = GetScale();
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

            mStartValue = GetScale();
            mLerpTimeCounter = 0;
            IsRunning = true;
        }

        /// <summary>
        /// reset the scale value to the cached starting value
        /// </summary>
        public void ResetTransform()
        {
            this.transform.localScale = mStartValue;
            IsRunning = false;
            mLerpTimeCounter = 0;
        }

        /// <summary>
        /// Reverse the scale animation for a ping pong effect
        /// </summary>
        public void Reverse()
        {
            TargetValue = mStartValue;
            mStartValue = TargetValue;
            mLerpTimeCounter = 0;
            IsRunning = true;
        }

        /// <summary>
        /// stop the animation
        /// </summary>
        public void StopRunning()
        {
            IsRunning = false;
        }

        /// <summary>
        /// get the current scale
        /// </summary>
        /// <returns></returns>
        private Vector3 GetScale()
        {
            return TargetObject.transform.localScale;
        }

        /// <summary>
        /// get the new scale based on time and ease settings
        /// </summary>
        /// <param name="currentScale"></param>
        /// <param name="percent"></param>
        /// <returns></returns>
        private Vector3 GetNewScale(Vector3 currentScale, float percent)
        {
            Vector3 newScale = Vector3.one;
            switch (LerpType)
            {
                case LerpTypes.Linear:
                    newScale = Vector3.Lerp(mStartValue, TargetValue, percent);
                    break;
                case LerpTypes.EaseIn:
                    newScale = Vector3.Lerp(mStartValue, TargetValue, QuadEaseIn(0, 1, percent));
                    break;
                case LerpTypes.EaseOut:
                    newScale = Vector3.Lerp(mStartValue, TargetValue, QuadEaseOut(0, 1, percent));
                    break;
                case LerpTypes.EaseInOut:
                    newScale = Vector3.Lerp(mStartValue, TargetValue, QuadEaseInOut(0, 1, percent));
                    break;
                case LerpTypes.Free:
                    newScale = Vector3.Lerp(currentScale, TargetValue, percent);
                    break;
                default:
                    break;
            }

            return newScale;
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
                // get the time
                mLerpTimeCounter += Time.deltaTime;
                float percent = mLerpTimeCounter / LerpTime;

                // set the scale
                this.transform.localScale = GetNewScale(this.transform.localScale, percent);

                // fire the event if complete
                if (percent >= 1)
                {
                    IsRunning = false;
                    OnComplete.Invoke();
                }
            }
            else if (LerpType == LerpTypes.Free)
            {
                bool wasRunning = IsRunning;

                // set the scale
                this.transform.localScale = GetNewScale(this.transform.localScale, LerpTime * Time.deltaTime);
                IsRunning = this.transform.localScale != TargetValue;

                // fire the event if complete
                if (IsRunning != wasRunning && !IsRunning)
                {
                    OnComplete.Invoke();
                }
            }
        }
    }
}
