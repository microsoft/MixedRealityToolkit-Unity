// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Blend
{
    /// <summary>
    /// Animates the scaling of an object with eases
    /// </summary>
    public class BlendScale : Blend<Vector3>
    {
        /// <summary>
        /// get the current scale
        /// </summary>
        /// <returns></returns>
        public override Vector3 GetValue()
        {
            return TargetObject.transform.localScale;
        }

        /// <summary>
        /// set the scale
        /// </summary>
        /// <param name="value"></param>
        public override void SetValue(Vector3 value)
        {
            TargetObject.transform.localScale = value;
        }

        /// <summary>
        /// is the scale animation complete?
        /// </summary>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        /// <returns></returns>
        public override bool CompareValues(Vector3 value1, Vector3 value2)
        {
            return value1 == value2;
        }

        /// <summary>
        /// animate the scale
        /// </summary>
        /// <param name="startValue"></param>
        /// <param name="targetValue"></param>
        /// <param name="percent"></param>
        /// <returns></returns>
        public override Vector3 LerpValues(Vector3 startValue, Vector3 targetValue, float percent)
        {
            return Vector3.Lerp(startValue, targetValue, percent);
        }
    }
}
