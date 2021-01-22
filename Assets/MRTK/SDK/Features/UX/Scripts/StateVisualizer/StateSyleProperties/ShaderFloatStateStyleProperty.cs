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
    public class ShaderFloatStateStyleProperty : ShaderStateStyleProperty
    {
        [SerializeField]
        [Tooltip("")]
        private float shaderPropertyFloatValue;

        /// <summary>
        /// 
        /// </summary>
        public float ShaderPropertyFloatValue
        {
            get => shaderPropertyFloatValue;
            set => shaderPropertyFloatValue = value;
        }

        public ShaderFloatStateStyleProperty()
        {
            AnimatablePropertyName = "ShaderFloat";
        }

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
