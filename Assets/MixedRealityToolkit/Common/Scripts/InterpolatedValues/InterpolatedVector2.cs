// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;

namespace MixedRealityToolkit.Common.InterpolatedValues
{
    /// <summary>
    /// Provides interpolation over Vector2.
    /// </summary>
    [Serializable]
    public class InterpolatedVector2 : InterpolatedValue<Vector2>
    {
        /// <summary>
        /// Instantiates the InterpolatedVector2 with default of Vector2 as initial value and skipping the first update frame.
        /// </summary>
        public InterpolatedVector2() : this(default(Vector2)) { }

        /// <summary>
        /// Instantiates the InterpolatedVector2 with a given Vector2 value as initial value and defaulted to skipping the first update frame.
        /// </summary>
        /// <param name="initialValue">Initial current value to use.</param>
        /// <param name="skipFirstUpdateFrame">A flag to skip first update frame after the interpolation target has been set.</param>
        public InterpolatedVector2(Vector2 initialValue, bool skipFirstUpdateFrame = false)
            : base(initialValue, skipFirstUpdateFrame)
        {
        }

        /// <summary>
        /// Overrides the method to check if two Vector2 are "close enough".
        /// </summary>
        /// <remarks>This method is public because of a Unity compilation bug when dealing with abstract methods on generics.</remarks>
        /// <param name="one">First Vector2 value.</param>
        /// <param name="other">Second Vector2 value.</param>
        /// <returns>True if the Vector2s are close enough.</returns>
        public override bool DoValuesEqual(Vector2 one, Vector2 other)
        {
            return (one - other).sqrMagnitude <= SmallNumberSquared;
        }

        /// <summary>
        /// Overrides the method to calculate the current Vector2 interpolation value by using a Vector2.Lerp function.
        /// </summary>
        /// <remarks>This method is public because of a Unity compilation bug when dealing with abstract methods on generics.</remarks>
        /// <param name="startValue">The Vector2 value that the interpolation started at.</param>
        /// <param name="targetValue">The target Vector2 value that the interpolation is moving to.</param>
        /// <param name="curveValue">A curve evaluated interpolation position value. This will be in range of [0, 1]</param>
        /// <returns>The new calculated Vector2 interpolation value.</returns>
        public override Vector2 ApplyCurveValue(Vector2 startValue, Vector2 targetValue, float curveValue)
        {
            return Vector2.Lerp(startValue, targetValue, curveValue);
        }
    }
}