// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Physics
{
    /// <summary>
    /// A MonoBehaviour that interpolates a transform's position, rotation or scale.
    /// </summary>
    [AddComponentMenu("Scripts/MRTK/Core/Interpolator")]
    public class Interpolator : MonoBehaviour
    {
        /// <summary>
        /// A very small number that is used in determining if the Interpolator needs to run at all.
        /// </summary>
        private const float Tolerance = 0.0000001f;

        /// <summary>
        /// The event fired when an Interpolation is started.
        /// </summary>
        public event Action InterpolationStarted;

        /// <summary>
        /// The event fired when an Interpolation is completed.
        /// </summary>
        public event Action InterpolationDone;

        [SerializeField]
        [Tooltip("When interpolating, use unscaled time. This is useful for games that have a pause mechanism or otherwise adjust the game timescale.")]
        private bool useUnscaledTime = true;

        [SerializeField]
        [Tooltip("The movement speed in meters per second.")]
        private float positionPerSecond = 30.0f;

        [SerializeField]
        [Tooltip("The rotation speed, in degrees per second.")]
        private float rotationDegreesPerSecond = 720.0f;

        [SerializeField]
        [Tooltip("Adjusts rotation speed based on angular distance.")]
        private float rotationSpeedScaler = 0.0f;

        [SerializeField]
        [Tooltip("The amount to scale per second.")]
        private float scalePerSecond = 5.0f;

        /// <summary>
        /// Lerp the estimated targets towards the object each update, slowing and smoothing movement.
        /// </summary>
        public bool SmoothLerpToTarget { get; set; } = false;
        public float SmoothPositionLerpRatio { get; set; } = 0.5f;
        public float SmoothRotationLerpRatio { get; set; } = 0.5f;
        public float SmoothScaleLerpRatio { get; set; } = 0.5f;

        /// <summary>
        /// If animating position, specifies the target position as specified
        /// by SetTargetPosition. Otherwise returns the current position of
        /// the transform.
        /// </summary>
        public Vector3 TargetPosition => AnimatingPosition ? targetPosition : transform.position;
        private Vector3 targetPosition;

        /// <summary>
        /// If animating rotation, specifies the target rotation as specified
        /// by SetTargetRotation. Otherwise returns the current rotation of
        /// the transform.
        /// </summary>
        public Quaternion TargetRotation => AnimatingRotation ? targetRotation : transform.rotation;
        private Quaternion targetRotation;

        /// <summary>
        /// If animating local rotation, specifies the target local rotation as
        /// specified by SetTargetLocalRotation. Otherwise returns the current
        /// local rotation of the transform.
        /// </summary>
        public Quaternion TargetLocalRotation => AnimatingLocalRotation ? targetLocalRotation : transform.localRotation;
        private Quaternion targetLocalRotation;

        /// <summary>
        /// If animating local scale, specifies the target local scale as
        /// specified by SetTargetLocalScale. Otherwise returns the current
        /// local scale of the transform.
        /// </summary>
        public Vector3 TargetLocalScale => AnimatingLocalScale ? targetLocalScale : transform.localScale;
        private Vector3 targetLocalScale;

        /// <summary>
        /// True if the transform's position is animating; false otherwise.
        /// </summary>
        public bool AnimatingPosition { get; private set; }

        /// <summary>
        /// True if the transform's rotation is animating; false otherwise.
        /// </summary>
        public bool AnimatingRotation { get; private set; }

        /// <summary>
        /// True if the transform's local rotation is animating; false otherwise.
        /// </summary>
        public bool AnimatingLocalRotation { get; private set; }

        /// <summary>
        /// True if the transform's scale is animating; false otherwise.
        /// </summary>
        public bool AnimatingLocalScale { get; private set; }

        /// <summary>
        /// The velocity of a transform whose position is being interpolated.
        /// </summary>
        public Vector3 PositionVelocity { get; private set; }

        private Vector3 oldPosition = Vector3.zero;

        /// <summary>
        /// True if position, rotation or scale are animating; false otherwise.
        /// </summary>
        public bool Running => AnimatingPosition || AnimatingRotation || AnimatingLocalRotation || AnimatingLocalScale;

        #region MonoBehaviour Implementation

        private void Awake()
        {
            targetPosition = transform.position;
            targetRotation = transform.rotation;
            targetLocalRotation = transform.localRotation;
            targetLocalScale = transform.localScale;

            enabled = false;
        }

        private void Update()
        {
            float deltaTime = useUnscaledTime
                ? Time.unscaledDeltaTime
                : Time.deltaTime;

            bool interpOccuredThisFrame = false;

            if (AnimatingPosition)
            {
                Vector3 lerpTargetPosition = targetPosition;
                if (SmoothLerpToTarget)
                {
                    lerpTargetPosition = Vector3.Lerp(transform.position, lerpTargetPosition, SmoothPositionLerpRatio);
                }

                Vector3 newPosition = NonLinearInterpolateTo(transform.position, lerpTargetPosition, deltaTime, positionPerSecond);
                if ((targetPosition - newPosition).sqrMagnitude <= Tolerance)
                {
                    // Snap to final position
                    newPosition = targetPosition;
                    AnimatingPosition = false;
                }
                else
                {
                    interpOccuredThisFrame = true;
                }

                transform.position = newPosition;

                //calculate interpolatedVelocity and store position for next frame
                PositionVelocity = oldPosition - newPosition;
                oldPosition = newPosition;
            }

            // Determine how far we need to rotate
            if (AnimatingRotation)
            {
                Quaternion lerpTargetRotation = targetRotation;
                if (SmoothLerpToTarget)
                {
                    lerpTargetRotation = Quaternion.Lerp(transform.rotation, lerpTargetRotation, SmoothRotationLerpRatio);
                }

                float angleDiff = Quaternion.Angle(transform.rotation, lerpTargetRotation);
                float speedScale = 1.0f + (Mathf.Pow(angleDiff, rotationSpeedScaler) / 180.0f);
                float ratio = Mathf.Clamp01((speedScale * rotationDegreesPerSecond * deltaTime) / angleDiff);

                if (angleDiff < Mathf.Epsilon)
                {
                    AnimatingRotation = false;
                    transform.rotation = targetRotation;
                }
                else
                {
                    // Only lerp rotation here, as ratio is NaN if angleDiff is 0.0f
                    transform.rotation = Quaternion.Slerp(transform.rotation, lerpTargetRotation, ratio);
                    interpOccuredThisFrame = true;
                }
            }

            // Determine how far we need to rotate
            if (AnimatingLocalRotation)
            {
                Quaternion lerpTargetLocalRotation = targetLocalRotation;
                if (SmoothLerpToTarget)
                {
                    lerpTargetLocalRotation = Quaternion.Lerp(transform.localRotation, lerpTargetLocalRotation, SmoothRotationLerpRatio);
                }

                float angleDiff = Quaternion.Angle(transform.localRotation, lerpTargetLocalRotation);
                float speedScale = 1.0f + (Mathf.Pow(angleDiff, rotationSpeedScaler) / 180.0f);
                float ratio = Mathf.Clamp01((speedScale * rotationDegreesPerSecond * deltaTime) / angleDiff);

                if (angleDiff < Mathf.Epsilon)
                {
                    AnimatingLocalRotation = false;
                    transform.localRotation = targetLocalRotation;
                }
                else
                {
                    // Only lerp rotation here, as ratio is NaN if angleDiff is 0.0f
                    transform.localRotation = Quaternion.Slerp(transform.localRotation, lerpTargetLocalRotation, ratio);
                    interpOccuredThisFrame = true;
                }
            }

            if (AnimatingLocalScale)
            {
                Vector3 lerpTargetLocalScale = targetLocalScale;
                if (SmoothLerpToTarget)
                {
                    lerpTargetLocalScale = Vector3.Lerp(transform.localScale, lerpTargetLocalScale, SmoothScaleLerpRatio);
                }

                Vector3 newScale = NonLinearInterpolateTo(transform.localScale, lerpTargetLocalScale, deltaTime, scalePerSecond);
                if ((targetLocalScale - newScale).sqrMagnitude <= Tolerance)
                {
                    // Snap to final scale
                    newScale = targetLocalScale;
                    AnimatingLocalScale = false;
                }
                else
                {
                    interpOccuredThisFrame = true;
                }

                transform.localScale = newScale;
            }

            // If all interpolations have completed, stop updating
            if (!interpOccuredThisFrame)
            {
                InterpolationDone?.Invoke();
                enabled = false;
            }
        }

        /// <summary>
        /// Stops the transform in place and terminates any animations.<para/>
        /// </summary>
        /// <remarks>Reset() is usually reserved as a MonoBehaviour API call in editor, but is used in this case as a convenience method.</remarks>
        public void Reset()
        {
            targetPosition = transform.position;
            targetRotation = transform.rotation;
            targetLocalRotation = transform.localRotation;
            targetLocalScale = transform.localScale;

            AnimatingPosition = false;
            AnimatingRotation = false;
            AnimatingLocalRotation = false;
            AnimatingLocalScale = false;

            enabled = false;
        }

        #endregion MonoBehaviour Implementation

        /// <summary>
        /// Sets the target position for the transform and if position wasn't
        /// already animating, fires the InterpolationStarted event.
        /// </summary>
        /// <param name="target">The new target position to for the transform.</param>
        public void SetTargetPosition(Vector3 target)
        {
            bool wasRunning = Running;

            targetPosition = target;

            float magsq = (targetPosition - transform.position).sqrMagnitude;
            if (magsq > Tolerance)
            {
                AnimatingPosition = true;
                enabled = true;

                if (InterpolationStarted != null && !wasRunning)
                {
                    InterpolationStarted();
                }
            }
            else
            {
                // Set immediately to prevent accumulation of error.
                transform.position = target;
                AnimatingPosition = false;
            }
        }

        /// <summary>
        /// Sets the target rotation for the transform and if rotation wasn't
        /// already animating, fires the InterpolationStarted event.
        /// </summary>
        /// <param name="target">The new target rotation for the transform.</param>
        public void SetTargetRotation(Quaternion target)
        {
            bool wasRunning = Running;

            targetRotation = target;

            if (Quaternion.Dot(transform.rotation, target) < 1.0f)
            {
                AnimatingRotation = true;
                enabled = true;

                if (InterpolationStarted != null && !wasRunning)
                {
                    InterpolationStarted();
                }
            }
            else
            {
                // Set immediately to prevent accumulation of error.
                transform.rotation = target;
                AnimatingRotation = false;
            }
        }

        /// <summary>
        /// Sets the target local rotation for the transform and if rotation
        /// wasn't already animating, fires the InterpolationStarted event.
        /// </summary>
        /// <param name="target">The new target local rotation for the transform.</param>
        public void SetTargetLocalRotation(Quaternion target)
        {
            bool wasRunning = Running;

            targetLocalRotation = target;

            if (Quaternion.Dot(transform.localRotation, target) < 1.0f)
            {
                AnimatingLocalRotation = true;
                enabled = true;

                if (InterpolationStarted != null && !wasRunning)
                {
                    InterpolationStarted();
                }
            }
            else
            {
                // Set immediately to prevent accumulation of error.
                transform.localRotation = target;
                AnimatingLocalRotation = false;
            }
        }

        /// <summary>
        /// Sets the target local scale for the transform and if scale
        /// wasn't already animating, fires the InterpolationStarted event.
        /// </summary>
        /// <param name="target">The new target local rotation for the transform.</param>
        public void SetTargetLocalScale(Vector3 target)
        {
            bool wasRunning = Running;

            targetLocalScale = target;

            float magsq = (targetLocalScale - transform.localScale).sqrMagnitude;
            if (magsq > Mathf.Epsilon)
            {
                AnimatingLocalScale = true;
                enabled = true;

                if (InterpolationStarted != null && !wasRunning)
                {
                    InterpolationStarted();
                }
            }
            else
            {
                // set immediately to prevent accumulation of error
                transform.localScale = target;
                AnimatingLocalScale = false;
            }
        }

        /// <summary>
        /// Interpolates smoothly to a target position.
        /// </summary>
        /// <param name="start">The starting position.</param>
        /// <param name="target">The destination position.</param>
        /// <param name="deltaTime">Caller-provided Time.deltaTime.</param>
        /// <param name="speed">The speed to apply to the interpolation.</param>
        /// <returns>New interpolated position closer to target</returns>
        public static Vector3 NonLinearInterpolateTo(Vector3 start, Vector3 target, float deltaTime, float speed)
        {
            // If no interpolation speed, jump to target value.
            if (speed <= 0.0f)
            {
                return target;
            }

            Vector3 distance = (target - start);

            // When close enough, jump to the target
            if (distance.sqrMagnitude <= Mathf.Epsilon)
            {
                return target;
            }

            // Apply the delta, then clamp so we don't overshoot the target
            Vector3 deltaMove = distance * Mathf.Clamp(deltaTime * speed, 0.0f, 1.0f);

            return start + deltaMove;
        }

        /// <summary>
        /// Snaps to the final target and stops interpolating
        /// </summary>
        public void SnapToTarget()
        {
            if (enabled)
            {
                transform.position = TargetPosition;
                transform.rotation = TargetRotation;
                transform.localRotation = TargetLocalRotation;
                transform.localScale = TargetLocalScale;

                AnimatingPosition = false;
                AnimatingLocalScale = false;
                AnimatingRotation = false;
                AnimatingLocalRotation = false;
                enabled = false;

                InterpolationDone?.Invoke();
            }
        }

        /// <summary>
        /// Stops the interpolation regardless if it has reached the target
        /// </summary>
        public void StopInterpolating()
        {
            if (enabled)
            {
                Reset();
                InterpolationDone?.Invoke();
            }
        }
    }
}