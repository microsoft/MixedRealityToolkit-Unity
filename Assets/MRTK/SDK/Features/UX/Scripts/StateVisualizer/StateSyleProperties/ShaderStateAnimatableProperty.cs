// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI.Interaction
{
    /// <summary>
    /// 
    /// </summary>
    public class ShaderStateAnimatableProperty : StateAnimatableProperty
    {
        [SerializeField]
        [Tooltip("The name of the shader property to animate. NOTE: Check capitalization if the keyframes" +
            " have not been set.")]
        private string shaderPropertyName;

        /// <summary>
        /// The name of the shader property to animate. NOTE: Check capitalization if the keyframes
        /// have not been set.
        /// </summary>
        public string ShaderPropertyName
        {
            get => shaderPropertyName;
            set => shaderPropertyName = value;
        }

        protected string GetPropertyName(string propertyName)
        {
            string singleUnderscoreName = "_" + propertyName;
            string doubleUnderscoreName = "_" + propertyName + "_";

            if (Target.GetComponent<MeshRenderer>() != null)
            {
                Material material = Target.GetComponent<MeshRenderer>().sharedMaterial;

                if (material.HasProperty(singleUnderscoreName))
                {
                    return singleUnderscoreName;
                }
                else if (material.HasProperty(doubleUnderscoreName))
                {
                    return doubleUnderscoreName;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                Debug.LogError($"The {Target.name} game object does not have a renderer component attached. A renderer component is required on a target object;");
                return null;
            }
        }
    }
}
