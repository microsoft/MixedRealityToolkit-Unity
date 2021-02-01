// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License

using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Experimental.StateVisualizer
{
    /// <summary>
    /// The PositionOffset animatable property adds/sets keyframes for the "localPosition" property in an animation clip.
    /// </summary>
    public class PositionOffsetStateAnimatableProperty : StateAnimatableProperty
    {
        [SerializeField]
        [Tooltip("The position offset added to the current position of the target object.")]
        private Vector3 positionOffset;

        /// <summary>
        /// The position offset added to the current position of the target object.
        /// </summary>
        public Vector3 PositionOffset
        {
            get => positionOffset;
            set => positionOffset = value;
        }

        /// <summary>
        /// Constructor for a Position Offset Animatable Property. Sets the default AnimatablePropertyName.
        /// </summary>
        public PositionOffsetStateAnimatableProperty()
        {
            AnimatablePropertyName = "PositionOffset";
        }

        /// <inheritdoc/>
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

        /// <inheritdoc/>
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

