// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License

using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI.Interaction
{
    /// <summary>
    /// 
    /// </summary>
    public class ColorStateStyleProperty : StateStyleProperty
    {
        [SerializeField]
        [Tooltip("")]
        private Color color;

        /// <summary>
        /// 
        /// </summary>
        public Color Color
        {
            get => color;
            set => color = value;
        }

        public ColorStateStyleProperty()
        {
            StylePropertyName = "Color";
        }

        public override void SetKeyFrames(AnimationClip animationClip)
        {
            if (Target != null)
            {
                string targetPath = GetTargetPath(Target);

                float colorR = Color.r;
                float colorG = Color.g;
                float colorB = Color.b;
                float colorA = Color.a;

                if (Target.EnsureComponent<MeshRenderer>() != null)
                {
                    Color currentColor = Target.GetComponent<MeshRenderer>().sharedMaterial.color;

                    AnimationCurve curveR = AnimationCurve.EaseInOut(0, currentColor.r, AnimationDuration, colorR);
                    AnimationCurve curveG = AnimationCurve.EaseInOut(0, currentColor.g, AnimationDuration, colorG);
                    AnimationCurve curveB = AnimationCurve.EaseInOut(0, currentColor.b, AnimationDuration, colorB);
                    AnimationCurve curveA = AnimationCurve.EaseInOut(0, currentColor.a, AnimationDuration, colorA);

                    animationClip.SetCurve(targetPath, typeof(MeshRenderer), "material._Color.r", curveR);
                    animationClip.SetCurve(targetPath, typeof(MeshRenderer), "material._Color.g", curveG);
                    animationClip.SetCurve(targetPath, typeof(MeshRenderer), "material._Color.b", curveB);
                    animationClip.SetCurve(targetPath, typeof(MeshRenderer), "material._Color.a", curveA);
                }
                else
                {
                    Debug.LogError("The target game object does not have a mesh renderer component attached. Attach a mesh renderer component to animate the Color property.");
                }
            }
        }


        public override void RemoveKeyFrames(AnimationClip animationClip)
        {
            if (Target != null)
            {
                string targetPath = GetTargetPath(Target);

                if (Target.GetComponent<MeshRenderer>() != null)
                {
                    animationClip.SetCurve(targetPath, typeof(MeshRenderer), "material._Color.r", null);
                    animationClip.SetCurve(targetPath, typeof(MeshRenderer), "material._Color.g", null);
                    animationClip.SetCurve(targetPath, typeof(MeshRenderer), "material._Color.b", null);
                    animationClip.SetCurve(targetPath, typeof(MeshRenderer), "material._Color.a", null);
                }
            }
        }
    }
}
