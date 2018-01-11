// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;

namespace MixedRealityToolkit.Common.InterpolatedValues
{
    /// <summary>
    /// Provides interpolation over Vector3.
    /// </summary>
    [Serializable]
    public class InterpolatedVector3 : InterpolatedValue<Vector3>
    {
        /// <summary>
        /// Instantiates the InterpolatedVector3 with default of Vector3 as initial value and skipping the first update frame.
        /// </summary>
        public InterpolatedVector3() : this(default(Vector3)) { }

        /// <summary>
        /// Instantiates the InterpolatedVector3 with a given Vector3 value as initial value and defaulted to skipping the first update frame.
        /// </summary>
        /// <param name="initialValue">Initial current value to use.</param>
        /// <param name="skipFirstUpdateFrame">A flag to skip first update frame after the interpolation target has been set.</param>
        public InterpolatedVector3(Vector3 initialValue, bool skipFirstUpdateFrame = false) : base(initialValue, skipFirstUpdateFrame) { }

        /// <summary>
        /// Overrides the method to check if two Vector3s are close enough.
        /// </summary>
        /// <remarks>This method is public because of a Unity compilation bug when dealing with abstract methods on generics.</remarks>
        /// <param name="one">First Vector3 value.</param>
        /// <param name="other">Second Vector3 value.</param>
        /// <returns>True if the Vector3 are close enough.</returns>
        public override bool DoValuesEqual(Vector3 one, Vector3 other)
        {
            return (one - other).sqrMagnitude <= SmallNumberSquared;
        }

        /// <summary>
        /// Overrides the method to calculate the current Vector3 interpolation value by using a Vector3.Lerp function.
        /// </summary>
        /// <remarks>This method is public because of a Unity compilation bug when dealing with abstract methods on generics.</remarks>
        /// <param name="startValue">The Vector3 value that the interpolation started at.</param>
        /// <param name="targetValue">The target Vector3 value that the interpolation is moving to.</param>
        /// <param name="curveValue">A curve evaluated interpolation position value. This will be in range of [0, 1]</param>
        /// <returns>The new calculated Vector3 interpolation value.</returns>
        public override Vector3 ApplyCurveValue(Vector3 startValue, Vector3 targetValue, float curveValue)
        {
            return Vector3.Lerp(startValue, targetValue, curveValue);
        }
    }
}