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
    public class ShaderColorStateStyleProperty : ShaderStateStyleProperty
    {
        [SerializeField]
        [Tooltip("")]
        private Color shaderPropertyColorValue;

        /// <summary>
        /// 
        /// </summary>
        public Color ShaderPropertyColorValue
        {
            get => shaderPropertyColorValue;
            set => shaderPropertyColorValue = value;
        }

        public ShaderColorStateStyleProperty()
        {
            AnimatablePropertyName = "ShaderColor";
        }

        public override void SetKeyFrames(AnimationClip animationClip)
        {
            if (Target != null)
            {
                string targetPath = GetTargetPath(Target);

                string propertyName = GetPropertyName(ShaderPropertyName);

                if (propertyName != null)
                {
                    Color currentValue = Target.GetComponent<MeshRenderer>().sharedMaterial.GetColor(propertyName);

                    AnimationCurve curveR = AnimationCurve.EaseInOut(0, currentValue.r, AnimationDuration, ShaderPropertyColorValue.r);
                    AnimationCurve curveG = AnimationCurve.EaseInOut(0, currentValue.g, AnimationDuration, ShaderPropertyColorValue.g);
                    AnimationCurve curveB = AnimationCurve.EaseInOut(0, currentValue.b, AnimationDuration, ShaderPropertyColorValue.b);
                    AnimationCurve curveA = AnimationCurve.EaseInOut(0, currentValue.a, AnimationDuration, ShaderPropertyColorValue.a);

                    animationClip.SetCurve(targetPath, typeof(MeshRenderer), "material." + propertyName + ".r", curveR);
                    animationClip.SetCurve(targetPath, typeof(MeshRenderer), "material." + propertyName + ".g", curveG);
                    animationClip.SetCurve(targetPath, typeof(MeshRenderer), "material." + propertyName + ".b", curveB);
                    animationClip.SetCurve(targetPath, typeof(MeshRenderer), "material." + propertyName + ".a", curveA);
                }
            }
        }

        public override void RemoveKeyFrames(AnimationClip animationClip)
        {
            if (Target != null)
            {
                string targetPath = GetTargetPath(Target);

                animationClip.SetCurve(targetPath, typeof(MeshRenderer), "material." + GetPropertyName(ShaderPropertyName) + ".r", null);
                animationClip.SetCurve(targetPath, typeof(MeshRenderer), "material." + GetPropertyName(ShaderPropertyName) + ".g", null);
                animationClip.SetCurve(targetPath, typeof(MeshRenderer), "material." + GetPropertyName(ShaderPropertyName) + ".b", null);
                animationClip.SetCurve(targetPath, typeof(MeshRenderer), "material." + GetPropertyName(ShaderPropertyName) + ".a", null);
            }
        }
    }
}
