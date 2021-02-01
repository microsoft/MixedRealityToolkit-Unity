// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Experimental.StateVisualizer
{
    /// <summary>
    /// Base class for animatable shader properties. 
    /// </summary>
    public class ShaderStateAnimatableProperty : StateAnimatableProperty
    {
        [SerializeField]
        [Tooltip("The name of the shader property to animate. " +
            "\n NOTE: Check capitalization if the keyframes in the animation clip " +
            " have not been set.  This name checks for an underscore character at the start and end of the name, but the underscore" +
            " character might be required for shader property names with more than one word.")]
        private string shaderPropertyName;

        /// <summary>
        /// The name of the shader property to animate.
        /// NOTE: Check capitalization if the keyframes in the animation clip have not been set.
        /// This name checks for an underscore character at the start and end of the name, but the underscore
        /// character might be required for shader property names with more than one word.
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
