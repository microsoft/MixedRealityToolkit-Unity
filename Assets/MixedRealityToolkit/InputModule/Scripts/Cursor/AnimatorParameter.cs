// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;

namespace MixedRealityToolkit.InputModule.Cursor
{
    /// <summary>
    /// Created a copy of the AnimatorControllerParameter because that class is not Serializable
    /// and cannot be modified in the editor.
    /// </summary>
    [Serializable]
    public struct AnimatorParameter
    {
        [Tooltip("Type of the animation parameter to modify.")]
        public AnimatorControllerParameterType Type;

        [Tooltip("If the animation parameter type is an int, value to set. Ignored otherwise.")]
        public int DefaultInt;

        [Tooltip("If the animation parameter type is a float, value to set. Ignored otherwise.")]
        public float DefaultFloat;

        [Tooltip("If the animation parameter type is a bool, value to set. Ignored otherwise.")]
        public bool DefaultBool;

        [Tooltip("Name of the animation parameter to modify.")]
        public string Name;

        private int? nameStringHash;
        public int NameHash
        {
            get
            {
                if (!nameStringHash.HasValue && !string.IsNullOrEmpty(Name))
                {
                    nameStringHash = Animator.StringToHash(Name);
                }
                return nameStringHash.Value;
            }
        }
    }
}