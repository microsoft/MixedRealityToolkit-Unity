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
    public class ShaderVectorStateStyleProperty : ShaderStateStyleProperty
    {
        [SerializeField]
        [Tooltip("")]
        private Vector4 shaderPropertyVectorValue;

        /// <summary>
        /// 
        /// </summary>
        public Vector4 ShaderPropertyVectorValue
        {
            get => shaderPropertyVectorValue;
            set => shaderPropertyVectorValue = value;
        }

        public ShaderVectorStateStyleProperty()
        {
            AnimatablePropertyName = "ShaderVector";
        }

        public override void SetKeyFrames(AnimationClip animationClip)
        {
            if (Target != null)
            {
                string targetPath = GetTargetPath(Target);

                string propertyName = GetPropertyName(ShaderPropertyName);

                if (propertyName != null)
                {
                    Vector4 currentValue = Target.GetComponent<MeshRenderer>().sharedMaterial.GetVector(propertyName);

                    AnimationCurve curveX = AnimationCurve.EaseInOut(0, currentValue.x, AnimationDuration, ShaderPropertyVectorValue.x);
                    AnimationCurve curveY = AnimationCurve.EaseInOut(0, currentValue.y, AnimationDuration, ShaderPropertyVectorValue.y);
                    AnimationCurve curveZ = AnimationCurve.EaseInOut(0, currentValue.z, AnimationDuration, ShaderPropertyVectorValue.z);
                    AnimationCurve curveW = AnimationCurve.EaseInOut(0, currentValue.w, AnimationDuration, ShaderPropertyVectorValue.w);

                    animationClip.SetCurve(targetPath, typeof(MeshRenderer), "material." + propertyName + ".x", curveX);
                    animationClip.SetCurve(targetPath, typeof(MeshRenderer), "material." + propertyName + ".y", curveY);
                    animationClip.SetCurve(targetPath, typeof(MeshRenderer), "material." + propertyName + ".z", curveZ);
                    animationClip.SetCurve(targetPath, typeof(MeshRenderer), "material." + propertyName + ".w", curveW);
                }
            }
        }

        public override void RemoveKeyFrames(AnimationClip animationClip)
        {
            if (Target != null)
            {
                string targetPath = GetTargetPath(Target);

                string propertyName = GetPropertyName(ShaderPropertyName);

                animationClip.SetCurve(targetPath, typeof(MeshRenderer), "material." + propertyName + ".x", null);
                animationClip.SetCurve(targetPath, typeof(MeshRenderer), "material." + propertyName + ".y", null);
                animationClip.SetCurve(targetPath, typeof(MeshRenderer), "material." + propertyName + ".z", null);
                animationClip.SetCurve(targetPath, typeof(MeshRenderer), "material." + propertyName + ".x", null);
            }
        }
    }
}
