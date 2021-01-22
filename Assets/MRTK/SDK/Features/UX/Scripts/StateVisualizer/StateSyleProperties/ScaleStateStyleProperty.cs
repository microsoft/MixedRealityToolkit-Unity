// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License

using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI.Interaction
{
    /// <summary>
    /// 
    /// </summary>
    public class ScaleStateStyleProperty : StateAnimatableProperty
    {
        [SerializeField]
        [Tooltip("")]
        private Vector3 scale;

        /// <summary>
        /// 
        /// </summary>
        public Vector3 Scale
        {
            get => scale;
            set => scale = value;
        }

        public ScaleStateStyleProperty()
        {
            AnimatablePropertyName = "Scale";
        }

        public override void SetKeyFrames(AnimationClip animationClip)
        {
            if (Target != null)
            {
                string targetPath = GetTargetPath(Target);

                AnimationCurve curveX = AnimationCurve.EaseInOut(0, Target.transform.localScale.x, AnimationDuration, Target.transform.localScale.x + scale.x);
                AnimationCurve curveY = AnimationCurve.EaseInOut(0, Target.transform.localScale.y, AnimationDuration, Target.transform.localScale.y + scale.y);
                AnimationCurve curveZ = AnimationCurve.EaseInOut(0, Target.transform.localScale.z, AnimationDuration, Target.transform.localScale.z + scale.z);

                animationClip.SetCurve(targetPath, typeof(Transform), "localScale.x", curveX);
                animationClip.SetCurve(targetPath, typeof(Transform), "localScale.y", curveY);
                animationClip.SetCurve(targetPath, typeof(Transform), "localScale.z", curveZ);
            }
        }

        public override void RemoveKeyFrames(AnimationClip animationClip)
        {
            if (Target != null)
            {
                string targetPath = GetTargetPath(Target);

                animationClip.SetCurve(targetPath, typeof(Transform), "localScale", null);
            }
        }
    }
}
