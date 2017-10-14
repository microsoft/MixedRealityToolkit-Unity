// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;

namespace HoloToolkit.Unity.InputModule
{
    /// <summary>
    /// Data struct for cursor state information for the Animated Cursor, which leverages the Unity animation system..
    /// This defines a modification to an Unity animation parameter, based on cursor state.
    /// </summary>
    [Serializable]
    public struct AnimCursorDatum
    {
        public string Name;
        public CursorStateEnum CursorState;
        public AnimatorParameter Parameter;

        /// <summary>
        /// Types that an animation parameter can have in the Unity animation system.
        /// </summary>
        [Obsolete("Use Parameter")]
        public enum AnimInputTypeEnum
        {
            Int,
            Trigger,
            Bool,
            Float
        }

        [Obsolete("Use Parameter")]
        [Tooltip("Type of the animation parameter to modify.")]
        public AnimInputTypeEnum AnimInputType;

        [Obsolete("Use Parameter")]
        [Tooltip("Name of the animation parameter to modify.")]
        public string AnimParameterName;

        [Obsolete("Use Parameter")]
        [Tooltip("If the animation parameter type is a bool, value to set. Ignored otherwise.")]
        public bool AnimBoolValue;

        [Obsolete("Use Parameter")]
        [Tooltip("If the animation parameter type is an int, value to set. Ignored otherwise.")]
        public int AnimIntValue;

        [Obsolete("Use Parameter")]
        [Tooltip("If the animation parameter type is a float, value to set. Ignored otherwise.")]
        public float AnimFloatValue;
    }
}