// Copyright(c) Microsoft Corporation.All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Blend
{
    /// <summary>
    /// A position animation component that supports easing and local position
    /// The animation can be triggered through code or the inspector
    /// 
    /// Supports easing, reverse and reseting positions.
    /// Free easing allows the object to animate by just updating the TargetValue for a more organic feel
    /// 
    /// Typical use:
    /// Set the TargetValue
    /// Call StartRunning();
    /// </summary>
    public class BlendPosition : Blend<Vector3>
    {

        [Tooltip("use the local position instead of world position")]
        public bool ToLocalTransform = false;

        /// <summary>
        /// get the current position
        /// </summary>
        /// <returns></returns>
        public override Vector3 GetValue()
        {
            return ToLocalTransform ? TargetObject.transform.localPosition : TargetObject.transform.position;
        }

        /// <summary>
        /// set the position
        /// </summary>
        /// <param name="value"></param>
        public override void SetValue(Vector3 value)
        {
            if (ToLocalTransform)
            {
                TargetObject.transform.localPosition = value;
            }
            else
            {
                TargetObject.transform.position = value;
            }
        }

        /// <summary>
        /// is the position animation complete?
        /// </summary>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        /// <returns></returns>
        public override bool CompareValues(Vector3 value1, Vector3 value2)
        {
            return value1 == value2;
        }

        /// <summary>
        /// animate the position
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
