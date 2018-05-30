// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Internal.Definitions.Utilities
{
    /// <summary>
    /// A copy of the <see cref="AnimatorControllerParameter"/> because that class is not Serializable and cannot be modified in the editor.
    /// </summary>
    [Serializable]
    public struct AnimatorParameter
    {
        [SerializeField]
        [Tooltip("Type of the animation parameter to modify.")]
        private AnimatorControllerParameterType parameterType;

        /// <summary>
        /// Type of the animation parameter to modify.
        /// </summary>
        public AnimatorControllerParameterType ParameterType => parameterType;

        [SerializeField]
        [Tooltip("If the animation parameter type is an int, value to set. Ignored otherwise.")]
        private int defaultInt;

        /// <summary>
        /// If the animation parameter type is an int, value to set. Ignored otherwise.
        /// </summary>
        public int DefaultInt => defaultInt;

        [SerializeField]
        [Tooltip("If the animation parameter type is a float, value to set. Ignored otherwise.")]
        private float defaultFloat;

        /// <summary>
        /// If the animation parameter type is a float, value to set. Ignored otherwise.
        /// </summary>
        public float DefaultFloat => defaultFloat;

        [SerializeField]
        [Tooltip("If the animation parameter type is a bool, value to set. Ignored otherwise.")]
        private bool defaultBool;

        /// <summary>
        /// If the animation parameter type is a bool, value to set. Ignored otherwise.
        /// </summary>
        public bool DefaultBool => defaultBool;

        [SerializeField]
        [Tooltip("Name of the animation parameter to modify.")]
        private string name;

        /// <summary>
        /// Name of the animation parameter to modify.
        /// </summary>
        public string Name => name;

        private int? nameStringHash;

        /// <summary>
        /// Animator Name String to Hash.
        /// </summary>
        public int NameHash
        {
            get
            {
                if (!nameStringHash.HasValue && !string.IsNullOrEmpty(Name))
                {
                    nameStringHash = Animator.StringToHash(Name);
                }

                Debug.Assert(nameStringHash != null);
                return nameStringHash.Value;
            }
        }
    }
}
