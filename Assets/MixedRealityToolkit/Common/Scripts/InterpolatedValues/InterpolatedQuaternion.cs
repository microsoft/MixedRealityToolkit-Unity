// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;

namespace MixedRealityToolkit.Common.InterpolatedValues
{
    /// <summary>
    /// Provides interpolation over Quaternion.
    /// </summary>
    [Serializable]
    public class InterpolatedQuaternion : InterpolatedValue<Quaternion>
    {
        /// <summary>
        /// Instantiates the InterpolatedQuaternion with default of Quaternion as initial value and skipping the first update frame.
        /// </summary>
        public InterpolatedQuaternion() : this(default(Quaternion)) { }

        /// <summary>
        /// Instantiates the InterpolatedQuaternion with a given Quaternion value as initial value and defaulted to skipping the first update frame.
        /// </summary>
        /// <param name="initialValue">Initial current value to use.</param>
        /// <param name="skipFirstUpdateFrame">A flag to skip first update frame after the interpolation target has been set.</param>
        public InterpolatedQuaternion(Quaternion initialValue, bool skipFirstUpdateFrame = false) : base(initialValue, skipFirstUpdateFrame) { }

        /// <summary>
        /// Overrides the method to calculate the current Quaternion interpolation value by using a Quaternion.Lerp function.
        /// </summary>
        /// <remarks>This method is public because of a Unity compilation bug when dealing with abstract methods on generics.</remarks>
        /// <param name="startValue">The Quaternion value that the interpolation started at.</param>
        /// <param name="targetValue">The target Quaternion value that the interpolation is moving to.</param>
        /// <param name="curveValue">A curve evaluated interpolation position value. This will be in range of [0, 1]</param>
        /// <returns>The new calculated Quaternion interpolation value.</returns>
        public override Quaternion ApplyCurveValue(Quaternion startValue, Quaternion targetValue, float curveValue)
        {
            return Quaternion.Slerp(startValue, targetValue, curveValue);
        }

        /// <summary>
        /// Overrides the method to check if two Quaternions are "close enough".
        /// </summary>
        /// <remarks>This method is public because of a Unity compilation bug when dealing with abstract methods on generics.</remarks>
        /// <param name="one">First Quaternion value.</param>
        /// <param name="other">Second Quaternion value.</param>
        /// <returns>True if the Quaternions are "close enough".</returns>
        public override bool DoValuesEqual(Quaternion one, Quaternion other)
        {
            return Quaternion.Angle(one, other) < SmallNumber;
        }
    }
}