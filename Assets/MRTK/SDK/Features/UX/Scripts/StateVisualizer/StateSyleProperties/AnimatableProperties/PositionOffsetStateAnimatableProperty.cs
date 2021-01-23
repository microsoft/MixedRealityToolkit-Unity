﻿// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License

using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI.Interaction
{
    /// <summary>
    /// 
    /// </summary>
    public class PositionOffsetStateAnimatableProperty : StateAnimatableProperty
    {
        [SerializeField]
        [Tooltip("")]
        private Vector3 positionOffset;

        /// <summary>
        /// 
        /// </summary>
        public Vector3 PositionOffset
        {
            get => positionOffset;
            set => positionOffset = value;
        }

        public PositionOffsetStateAnimatableProperty()
        {
            AnimatablePropertyName = "PositionOffset";
        }

        public override void SetKeyFrames(AnimationClip animationClip)
        {
            if (Target != null)
            {
                string targetPath = GetTargetPath(Target);

                AnimationCurve curveX = AnimationCurve.EaseInOut(0, Target.transform.localPosition.x, AnimationDuration, Target.transform.localPosition.x + PositionOffset.x);
                AnimationCurve curveY = AnimationCurve.EaseInOut(0, Target.transform.localPosition.y, AnimationDuration, Target.transform.localPosition.y + PositionOffset.y);
                AnimationCurve curveZ = AnimationCurve.EaseInOut(0, Target.transform.localPosition.z, AnimationDuration, Target.transform.localPosition.z + PositionOffset.z);

                animationClip.SetCurve(targetPath, typeof(Transform), "localPosition.x", curveX);
                animationClip.SetCurve(targetPath, typeof(Transform), "localPosition.y", curveY);
                animationClip.SetCurve(targetPath, typeof(Transform), "localPosition.z", curveZ);
            }
        }

        public override void RemoveKeyFrames(AnimationClip animationClip)
        {
            if (Target != null)
            {
                string targetPath = GetTargetPath(Target);

                animationClip.SetCurve(targetPath, typeof(Transform), "localPosition", null);
            }
        }
    }
}
