// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Definitions.Utilities;
using UnityEngine;
using UnityEngine.Events;

namespace Microsoft.MixedReality.Toolkit.SDK.UX.Utilities
{
    /// <summary>
    /// Animates the rotation of an object with easing's
    /// </summary>
    public class RotateToValue : MonoBehaviour
    {
        #region Properties and fields

        [Tooltip("The object to animate")]
        [SerializeField]
        private GameObject targetObject;

        public GameObject TargetObject
        {
            get { return targetObject; }
            set { targetObject = value; }
        }

        [Tooltip("The rotation value to animate to")]
        [SerializeField]
        private Quaternion targetValue;

        public Quaternion TargetValue
        {
            get { return targetValue; }
            set { targetValue = value; }
        }

        [Tooltip("The type of ease to apply to the tween")]
        [SerializeField]
        private EasingTypes lerpType;

        public EasingTypes LerpType
        {
            get { return lerpType; }
            set { lerpType = value; }
        }

        [Tooltip("Duration of the animation in seconds")]
        [SerializeField]
        private float lerpTime = 1f;

        public float LerpTime
        {
            get { return lerpTime; }
            set { lerpTime = value; }
        }

        [Tooltip("Is the Easing function running?")]
        [SerializeField]
        private bool isRunning = false;

        public bool IsRunning
        {
            get { return isRunning; }
            set { isRunning = value; }
        }

        [Tooltip("Use the localRotation instead of world rotation")]
        [SerializeField]
        private bool toLocalTransform = false;

        public bool ToLocalTransform
        {
            get { return toLocalTransform = false; }
            set { toLocalTransform = value; }
        }

        [Tooltip("Animation complete!")]
        [SerializeField]
        private UnityEvent onComplete;

        public UnityEvent OnComplete
        {
            get { return onComplete; }
            set { onComplete = value; }
        }

        // animation ticker
        private float mLerpTimeCounter;

        // starting/current rotation
        private Quaternion mStartValue;

        #endregion Properties and fields

        #region Monobehaviour Implementation

        /// <summary>
        /// get the game object if not set already
        /// set the cached rotation
        /// </summary>
        private void Awake()
        {
            if (targetObject == null)
            {
                targetObject = this.gameObject;
            }
            mStartValue = GetRotation();
        }

        /// <summary>
        /// Animate
        /// </summary>
        private void Update()
        {
            // manual animation, only runs when auto started or StartRunning() is called
            if (isRunning && lerpType != EasingTypes.Free)
            {
                // get the time
                mLerpTimeCounter += Time.deltaTime;
                float percent = mLerpTimeCounter / lerpTime;

                // set the rotation
                if (toLocalTransform)
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
                    isRunning = false;
                    onComplete.Invoke();
                }
            }
            else if (lerpType == EasingTypes.Free) // is always running, just waiting for the TargetValue to change
            {
                bool wasRunning = IsRunning;

                // set the rotation
                if (toLocalTransform)
                {
                    this.transform.localRotation = GetNewRotation(this.transform.localRotation, LerpTime * Time.deltaTime);
                    isRunning = this.transform.localRotation != targetValue;
                }
                else
                {
                    this.transform.rotation = GetNewRotation(this.transform.rotation, LerpTime * Time.deltaTime);
                    isRunning = this.transform.rotation != targetValue;
                }

                // fire the event if complete
                if (isRunning != wasRunning && !isRunning)
                {
                    OnComplete.Invoke();
                }
            }
        }

        #endregion Monobehaviour Implementation

        #region Easing Methods

        /// <summary>
        /// Start the rotation animation
        /// </summary>
        public void StartRunning()
        {
            if (targetObject == null)
            {
                targetObject = this.gameObject;
            }

            mStartValue = GetRotation();
            mLerpTimeCounter = 0;
            isRunning = true;
        }

        /// <summary>
        /// Set the rotation to the cached starting value
        /// </summary>
        public void ResetTransform()
        {
            if (toLocalTransform)
            {
                this.transform.localRotation = mStartValue;
            }
            else
            {
                this.transform.rotation = mStartValue;
            }
            isRunning = false;
            mLerpTimeCounter = 0;
        }

        /// <summary>
        /// reverse the rotation - go back
        /// </summary>
        public void Reverse()
        {
            targetValue = mStartValue;
            mStartValue = TargetValue;
            mLerpTimeCounter = 0;
            isRunning = true;
        }

        /// <summary>
        /// Stop the animation
        /// </summary>
        public void StopRunning()
        {
            isRunning = false;
        }

        // get the current rotation
        private Quaternion GetRotation()
        {
            return toLocalTransform ? targetObject.transform.localRotation : targetObject.transform.rotation;
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
            switch (lerpType)
            {
                case EasingTypes.Linear:
                    newPosition = Quaternion.Lerp(mStartValue, TargetValue, percent);
                    break;
                case EasingTypes.In:
                    newPosition = Quaternion.Lerp(mStartValue, TargetValue, QuadEaseIn(0, 1, percent));
                    break;
                case EasingTypes.Out:
                    newPosition = Quaternion.Lerp(mStartValue, TargetValue, QuadEaseOut(0, 1, percent));
                    break;
                case EasingTypes.InOut:
                    newPosition = Quaternion.Lerp(mStartValue, TargetValue, QuadEaseInOut(0, 1, percent));
                    break;
                case EasingTypes.Free:
                    newPosition = Quaternion.Lerp(currentRotation, TargetValue, percent);
                    break;
                default:
                    break;
            }

            return newPosition;
        }

        #endregion Easing Methods

        #region Easing Functions

        /// <summary>
        /// Easing in Curve
        /// </summary>
        /// <param name="s"></param>
        /// <param name="e"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        public static float QuadEaseIn(float s, float e, float v)
        {
            return e * (v /= 1) * v + s;
        }

        /// <summary>
        /// Easing out curve
        /// </summary>
        /// <param name="s"></param>
        /// <param name="e"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        public static float QuadEaseOut(float s, float e, float v)
        {
            return -e * (v /= 1) * (v - 2) + s;
        }

        /// <summary>
        /// Ease in and out curve
        /// </summary>
        /// <param name="s"></param>
        /// <param name="e"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        public static float QuadEaseInOut(float s, float e, float v)
        {
            if ((v /= 0.5f) < 1)
            {
                return e / 2 * v * v + s;
            }

            return -e / 2 * ((--v) * (v - 2) - 1) + s;
        }

        #endregion Easing Functions
    }
}