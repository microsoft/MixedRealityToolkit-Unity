// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Experimental.StateVisualizer
{
    /// <summary>
    /// The ShaderColor animatable property adds/sets keyframes for a defined shader property of type Color in an animation clip.
    /// </summary>
    public class ShaderColorStateAnimatableProperty : ShaderStateAnimatableProperty
    {
        [SerializeField]
        [Tooltip("The color value of the given shader property. This color value refers to a shader property " +
            " of type Color and not the main color of the material.")]
        private Color shaderPropertyColorValue;

        /// <summary>
        /// The color value of the given shader property. This color value refers to a shader property 
        /// of type Color and not the main color of the material.
        /// </summary>
        public Color ShaderPropertyColorValue
        {
            get => shaderPropertyColorValue;
            set => shaderPropertyColorValue = value;
        }

        /// <summary>
        /// Constructor for a Shader Color Animatable Property. Sets the default AnimatablePropertyName.
        /// </summary>
        public ShaderColorStateAnimatableProperty()
        {
            AnimatablePropertyName = "ShaderColor";
        }

        /// <inheritdoc/>
        public override void SetKeyFrames(AnimationClip animationClip)
        {
            if (Target != null)
            {
                string propertyName = GetPropertyName(ShaderPropertyName);

                if (propertyName != null)
                {
                    Color currentValue = Target.GetComponent<MeshRenderer>().sharedMaterial.GetColor(propertyName);

                    AnimationCurve curveR = AnimationCurve.EaseInOut(0, currentValue.r, AnimationDuration, ShaderPropertyColorValue.r);
                    AnimationCurve curveG = AnimationCurve.EaseInOut(0, currentValue.g, AnimationDuration, ShaderPropertyColorValue.g);
                    AnimationCurve curveB = AnimationCurve.EaseInOut(0, currentValue.b, AnimationDuration, ShaderPropertyColorValue.b);
                    AnimationCurve curveA = AnimationCurve.EaseInOut(0, currentValue.a, AnimationDuration, ShaderPropertyColorValue.a);

                    SetColorAnimationCurve(animationClip, propertyName, curveR, curveG, curveB, curveA);
                }
            }
        }

        /// <inheritdoc/>
        public override void RemoveKeyFrames(AnimationClip animationClip)
        {
            if (Target != null)
            {
                string propertyName = GetPropertyName(ShaderPropertyName);

                if (propertyName != null)
                {
                    SetColorAnimationCurve(animationClip, propertyName, null, null, null, null);
                }
            }
        }

        private void SetColorAnimationCurve(AnimationClip animationClip, string propertyName, AnimationCurve curveR, AnimationCurve curveG, AnimationCurve curveB, AnimationCurve curveA)
        {
            string targetPath = GetTargetPath(Target);

            animationClip.SetCurve(targetPath, typeof(MeshRenderer), "material." + propertyName + ".r", curveR);
            animationClip.SetCurve(targetPath, typeof(MeshRenderer), "material." + propertyName + ".g", curveG);
            animationClip.SetCurve(targetPath, typeof(MeshRenderer), "material." + propertyName + ".b", curveB);
            animationClip.SetCurve(targetPath, typeof(MeshRenderer), "material." + propertyName + ".a", curveA);
        }
    }
}
