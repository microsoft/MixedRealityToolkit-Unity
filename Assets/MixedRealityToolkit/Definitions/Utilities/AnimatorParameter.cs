// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities
{
    /// <summary>
    /// A copy of the <see href="https://docs.unity3d.com/ScriptReference/AnimatorControllerParameter.html">AnimatorControllerParameter</see> because that class is not Serializable and cannot be modified in the editor.
    /// </summary>
    [Serializable]
    public struct AnimatorParameter
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">Name of the animation parameter to modify.</param>
        /// <param name="parameterType">Type of the animation parameter to modify.</param>
        /// <param name="defaultInt">If the animation parameter type is an int, value to set. Ignored otherwise.</param>
        /// <param name="defaultFloat">If the animation parameter type is a float, value to set. Ignored otherwise.</param>
        /// <param name="defaultBool">"If the animation parameter type is a bool, value to set. Ignored otherwise.</param>
        public AnimatorParameter(string name, AnimatorControllerParameterType parameterType, int defaultInt = 0, float defaultFloat = 0f, bool defaultBool = false)
        {
            this.parameterType = parameterType;
            this.defaultInt = defaultInt;
            this.defaultFloat = defaultFloat;
            this.defaultBool = defaultBool;
            this.name = name;
            nameStringHash = null;
        }

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
