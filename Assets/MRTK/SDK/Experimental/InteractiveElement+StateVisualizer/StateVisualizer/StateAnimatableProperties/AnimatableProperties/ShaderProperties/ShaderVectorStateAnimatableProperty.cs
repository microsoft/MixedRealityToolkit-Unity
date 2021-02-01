// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Experimental.StateVisualizer
{
    /// <summary>
    /// The ShaderVector animatable property adds/sets keyframes for a defined shader property of type Vector4 in an animation clip.
    /// </summary>
    public class ShaderVectorStateAnimatableProperty : ShaderStateAnimatableProperty
    {
        [SerializeField]
        [Tooltip("The Vector4 value for the defined shader property. ")]
        private Vector4 shaderPropertyVectorValue;

        /// <summary>
        /// The Vector4 value for the defined shader property. 
        /// </summary>
        public Vector4 ShaderPropertyVectorValue
        {
            get => shaderPropertyVectorValue;
            set => shaderPropertyVectorValue = value;
        }

        /// <summary>
        /// Constructor for a Shader Vector Animatable Property. Sets the default AnimatablePropertyName.
        /// </summary>
        public ShaderVectorStateAnimatableProperty()
        {
            AnimatablePropertyName = "ShaderVector";
        }

        /// <inheritdoc/>
        public override void SetKeyFrames(AnimationClip animationClip)
        {
            if (Target != null)
            {
                string propertyName = GetPropertyName(ShaderPropertyName);

                if (propertyName != null)
                {
                    Vector4 currentValue = Target.GetComponent<MeshRenderer>().sharedMaterial.GetVector(propertyName);

                    AnimationCurve curveX = AnimationCurve.EaseInOut(0, currentValue.x, AnimationDuration, ShaderPropertyVectorValue.x);
                    AnimationCurve curveY = AnimationCurve.EaseInOut(0, currentValue.y, AnimationDuration, ShaderPropertyVectorValue.y);
                    AnimationCurve curveZ = AnimationCurve.EaseInOut(0, currentValue.z, AnimationDuration, ShaderPropertyVectorValue.z);
                    AnimationCurve curveW = AnimationCurve.EaseInOut(0, currentValue.w, AnimationDuration, ShaderPropertyVectorValue.w);

                    SetVectorAnimationCurve(animationClip, propertyName, curveX, curveY, curveZ, curveW);
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
                    SetVectorAnimationCurve(animationClip, propertyName, null, null, null, null);
                }
            }
        }

        private void SetVectorAnimationCurve(AnimationClip animationClip, string propertyName, AnimationCurve curveX, AnimationCurve curveY, AnimationCurve curveZ, AnimationCurve curveW)
        {
            string targetPath = GetTargetPath(Target);

            animationClip.SetCurve(targetPath, typeof(MeshRenderer), "material." + propertyName + ".x", curveX);
            animationClip.SetCurve(targetPath, typeof(MeshRenderer), "material." + propertyName + ".y", curveY);
            animationClip.SetCurve(targetPath, typeof(MeshRenderer), "material." + propertyName + ".z", curveZ);
            animationClip.SetCurve(targetPath, typeof(MeshRenderer), "material." + propertyName + ".w", curveW);
        }
    }
}
