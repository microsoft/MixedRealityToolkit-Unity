// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;

namespace HoloToolkit.Unity
{
    /// <summary>
    /// Provides interpolation over float.
    /// </summary>
    [Serializable]
    public class InterpolatedFloat : InterpolatedValue<float>
    {
        /// <summary>
        /// Instantiates the InterpolatedFloat with default of float as initial value and skipping the first update frame.
        /// </summary>
        public InterpolatedFloat() : this(0.0f) { }

        /// <summary>
        /// Instantiates the InterpolatedFloat with a given float value as initial value and defaulted to skipping the first update frame.
        /// </summary>
        /// <param name="initialValue">Initial current value to use.</param>
        /// <param name="skipFirstUpdateFrame">A flag to skip first update frame after the interpolation target has been set.</param>
        public InterpolatedFloat(float initialValue, bool skipFirstUpdateFrame = false)
            : base(initialValue, skipFirstUpdateFrame)
        { }

        /// <summary>
        /// Overrides the method to check if two floats are equal.
        /// </summary>
        /// <remarks>This method is public because of a Unity compilation bug when dealing with abstract methods on generics.</remarks>
        /// <param name="one">First float value.</param>
        /// <param name="other">Second float value.</param>
        /// <returns>True if the floats are equal.</returns>
        public override bool DoValuesEqual(float one, float other)
        {
            return Mathf.Abs(one - other) < SmallNumber;
        }

        /// <summary>
        /// Overrides the method to calculate the current float interpolation value by using a Mathf.Lerp function.
        /// </summary>
        /// <remarks>This method is public because of a Unity compilation bug when dealing with abstract methods on generics.</remarks>
        /// <param name="startValue">The float value that the interpolation started at.</param>
        /// <param name="targetValue">The target float value that the interpolation is moving to.</param>
        /// <param name="curveValue">A curve evaluated interpolation position value. This will be in range of [0, 1]</param>
        /// <returns>The new calculated float interpolation value.</returns>
        public override float ApplyCurveValue(float startValue, float targetValue, float curveValue)
        {
            return Mathf.Lerp(startValue, targetValue, curveValue);
        }
    }
}