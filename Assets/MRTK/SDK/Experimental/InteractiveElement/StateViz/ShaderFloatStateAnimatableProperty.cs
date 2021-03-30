// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Experimental.StateVisualizer
{
    /// <summary>
    /// The ShaderFloat animatable property adds/sets keyframes for a defined shader property of type Float in an animation clip.
    /// </summary>
    public class ShaderFloatStateAnimatableProperty : ShaderStateAnimatableProperty
    {
        [SerializeField]
        [Tooltip("The float value for the defined shader property. ")]
        private float shaderPropertyFloatValue;

        /// <summary>
        /// The float value for the defined shader property. 
        /// </summary>
        public float ShaderPropertyFloatValue
        {
            get => shaderPropertyFloatValue;
            set => shaderPropertyFloatValue = value;
        }

        /// <summary>
        /// Constructor for a Shader Float Animatable Property. Sets the default AnimatablePropertyName.
        /// </summary>
        public ShaderFloatStateAnimatableProperty()
        {
            AnimatablePropertyName = "ShaderFloat";
        }

        /// <inheritdoc/>
        public override void SetKeyFrames(AnimationClip animationClip)
        {
            if (Target != null)
            {
                string targetPath = GetTargetPath(Target);

                string propertyName = GetPropertyName(ShaderPropertyName);

                if (propertyName != null)
                {
                    float currentValue = Target.GetComponent<MeshRenderer>().sharedMaterial.GetFloat(propertyName);

                    AnimationCurve curve = AnimationCurve.EaseInOut(0, currentValue, AnimationDuration, ShaderPropertyFloatValue);

                    animationClip.SetCurve(targetPath, typeof(MeshRenderer), "material." + propertyName, curve);
                }
            }
        }

        /// <inheritdoc/>
        public override void RemoveKeyFrames(AnimationClip animationClip)
        {
            if (Target != null)
            {
                string targetPath = GetTargetPath(Target);

                animationClip.SetCurve(targetPath, typeof(MeshRenderer), "material." + GetPropertyName(ShaderPropertyName), null);
            }
        }
    }
}
