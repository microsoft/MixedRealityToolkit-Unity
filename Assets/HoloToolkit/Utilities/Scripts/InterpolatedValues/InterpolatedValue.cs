// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;

namespace HoloToolkit.Unity
{
    /// <summary>
    /// Base class that provides the common logic for interpolating between values. This class does not
    /// inherit from MonoBehaviour in order to enable various scenarios under which it used. To perform the 
    /// interpolation step, call FrameUpdate.
    /// </summary>
    /// <typeparam name="T">Type of value used for interpolation.</typeparam>
    [Serializable]
    public abstract class InterpolatedValue<T>
    {
        public const float SmallNumber = 0.0000001f;
        public const float SmallNumberSquared = SmallNumber * SmallNumber;

        /// <summary>
        /// Implicit cast operator that returns the current value of the Interpolator.
        /// </summary>
        /// <param name="interpolatedValue">The interpolator casting from.</param>
        public static implicit operator T(InterpolatedValue<T> interpolatedValue)
        {
            return interpolatedValue.Value;
        }

        /// <summary>
        /// Event that is triggered when interpolation starts.
        /// </summary>
        public event Action<InterpolatedValue<T>> Started;

        /// <summary>
        /// Event that is triggered when interpolation completes.
        /// </summary>
        public event Action<InterpolatedValue<T>> Completed;

        /// <summary>
        /// Event that is triggered when the current interpolated value is changed.
        /// </summary>
        public event Action<InterpolatedValue<T>> ValueChanged;

        private T targetValue;
        private T startValue;
        private float timeInterpolationStartedAt;
        private bool firstUpdateFrameSkipped;
        private bool skipFirstUpdateFrame;
        private bool performingInterpolativeSnap;

        [SerializeField]
        [Tooltip("Time the interpolator takes to get from current value to the target value")]
        private float duration = 1.0f;

        /// <summary>
        /// Time the interpolator takes to get from current value to the target value.
        /// </summary>
        public float Duration
        {
            get { return duration; }
            set { duration = value; }
        }

        [SerializeField]
        [Tooltip("Time the interpolator takes to get from current value to the target value")]
        private AnimationCurve curve = null;

        /// <summary>
        /// The AnimationCurve used for evaluating the interpolation value.
        /// </summary>
        public AnimationCurve Curve
        {
            get { return curve; }
            set { curve = value; }
        }

        /// <summary>
        /// Checks if the interpolator can be used by ensuring an AnimatorCurve has been set.
        /// </summary>
        public bool IsValid { get { return Curve != null; } }

        /// <summary>
        /// Checks whether the interpolator is currently interpolating.
        /// </summary>
        public bool IsRunning { get; private set; }

        private T value;
        
        /// <summary>
        /// Returns the current interpolated value.
        /// </summary>
        public T Value
        {
            get { return value; }
            private set
            {
                if (!DoValuesEqual(this.value, value))
                {
                    this.value = value;
                    ValueChanged.RaiseEvent(this);
                }
            }
        }

        /// <summary>
        /// Returns the current interpolation target value.
        /// </summary>
        public T Target
        {
            get { return targetValue; }
        }

        /// <summary>
        /// Wrapper for getting time that supports EditTime updates.
        /// </summary>
        protected float CurrentTime
        {
            get
            {
#if UNITY_EDITOR
                return !Application.isPlaying ? (float)UnityEditor.EditorApplication.timeSinceStartup : Time.time;
#else
                return Time.time;
#endif
            }
        }

        /// <summary>
        /// Instantiates a new InterpolatedValue with an initial value and a setting of whether to skip first update frame.
        /// </summary>
        /// <param name="initialValue">Initial current value to use.</param>
        /// <param name="skipFirstUpdateFrame">A flag to skip first update frame after the interpolation target has been set.</param>
        public InterpolatedValue(T initialValue, bool skipFirstUpdateFrame)
        {
            IsRunning = false;
            Value = targetValue = initialValue;
            Duration = 1.0f;
            firstUpdateFrameSkipped = false;
            this.skipFirstUpdateFrame = skipFirstUpdateFrame;
            performingInterpolativeSnap = false;
        }

        /// <summary>
        /// Updates the target value and starts the interpolator if it is not running already.
        /// </summary>
        /// <param name="updateTargetValue">The new target value.</param>
        public void UpdateTarget(T updateTargetValue)
        {
            UpdateTarget(updateTargetValue, false);
        }

        /// <summary>
        /// Updates the target value and starts the interpolator if it is not running already.
        /// </summary>
        /// <param name="updateTargetValue">The new target value.</param>
        /// <param name="forceUpdate">A flag for forcing an update propagation.</param>
        public void UpdateTarget(T updateTargetValue, bool forceUpdate)
        {
            performingInterpolativeSnap = false;

            targetValue = updateTargetValue;

            startValue = Value;
            timeInterpolationStartedAt = CurrentTime;

            if (!DoValuesEqual(updateTargetValue, Value))
            {
                EnsureEnabled();
            }
            else if (forceUpdate && !IsRunning)
            {
                ValueChanged.RaiseEvent(this);
            }
        }

        /// <summary>
        /// Snap (set) the interpolated value to the current target value.
        /// </summary>
        public void SnapToTarget()
        {
            SnapToTarget(targetValue);
        }

        /// <summary>
        /// Update the target to a new value and snap (set) the interpolated value to it.
        /// </summary>
        /// <param name="snapTargetValue">The new target value.</param>
        public void SnapToTarget(T snapTargetValue)
        {
            performingInterpolativeSnap = false;
            Value = startValue = snapTargetValue;
            EnsureDisabled();
        }

        /// <summary>
        /// Interpolative snap to target will interpolate until it reaches the given target value, after which subsequent calls to this method it will snap to the target value given.
        /// </summary>
        /// <remarks>SnapToTarget and UpdateTarget resets this.</remarks>
        /// <param name="snapTargetValue">The target value to set and interpolate to.</param>
        public void InterpolateThenSnapToTarget(T snapTargetValue)
        {
            if (performingInterpolativeSnap)
            {
                // If we are running, just update the target, otherwise just snap to target
                if (IsRunning)
                {
                    targetValue = snapTargetValue;
                }
                else
                {
                    Value = startValue = snapTargetValue;
                }
            }
            else
            {
                UpdateTarget(snapTargetValue);
                performingInterpolativeSnap = true;
            }
        }

        /// <summary>
        /// Starts interpolation if it is not currently running.
        /// </summary>
        /// <remarks>This forces a start if not currently running and does not check if the interpolated value is at the target value.</remarks>
        public void EnsureEnabled()
        {
            if (!IsRunning)
            {
                if (skipFirstUpdateFrame)
                {
                    firstUpdateFrameSkipped = false;
                }

                IsRunning = true;

                Started.RaiseEvent(this);
            }
        }

        /// <summary>
        /// Stops the interpolation if it is currently running.
        /// </summary>
        /// <remarks>This forces a stop if currently running and does not check if the interpolated value has not reached the target value.</remarks>
        public void EnsureDisabled()
        {
            if (IsRunning)
            {
                IsRunning = false;

                Completed.RaiseEvent(this);
            }
        }

        /// <summary>
        /// Increments the interpolation step. This function should be called each frame.
        /// </summary>
        /// <remarks>To enable multiple scenarios for using the InterpolatedValues, the class does not inherit from MonoBehaviour.</remarks>
        /// <returns>The new interpolated value after performing the interpolation step.</returns>
        public T FrameUpdate()
        {
            if (IsRunning)
            {
                if (skipFirstUpdateFrame && !firstUpdateFrameSkipped)
                {
                    firstUpdateFrameSkipped = true;
                    timeInterpolationStartedAt = CurrentTime;
                    return Value;
                }

                float timeDelta = CurrentTime - timeInterpolationStartedAt;

                // Normalize the delta to curve duration
                timeDelta *= Curve.Duration() / Duration;

                Value = ApplyCurveValue(startValue, targetValue, Curve.Evaluate(timeDelta));
                if (timeDelta >= Curve.Duration())
                {
                    EnsureDisabled();
                }
            }

            return Value;
        }

        /// <summary>
        /// A method to check whether two values are equal. This should be overridden by inheriting classes.
        /// </summary>
        /// <remarks>This method is public because of a Unity compilation bug when dealing with abstract methods on generics.</remarks>
        /// <param name="one">First value.</param>
        /// <param name="other">Second value.</param>
        /// <returns>True if values are equal or are "close enough".</returns>
        public abstract bool DoValuesEqual(T one, T other);

        /// <summary>
        /// A method to calculate the current interpolated value based on the start value, a target value and the curve evaluated interpolation position value. This should be overridden by inheriting classes.
        /// </summary>
        /// <remarks>This method is public because of a Unity compilation bug when dealing with abstract methods on generics.</remarks>
        /// <param name="startValue">The value that the interpolation started at.</param>
        /// <param name="targetValue">The target value that the interpolation is moving to.</param>
        /// <param name="curveValue">A curve evaluated interpolation position value. This will be in range of [0, 1]</param>
        /// <returns>The new calculated interpolation value.</returns>
        public abstract T ApplyCurveValue(T startValue, T targetValue, float curveValue);
    }
}