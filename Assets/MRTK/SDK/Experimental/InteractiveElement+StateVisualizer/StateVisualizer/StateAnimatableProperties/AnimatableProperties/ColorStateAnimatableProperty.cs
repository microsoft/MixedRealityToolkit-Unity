// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Experimental.StateVisualizer
{
    /// <summary>
    /// The Color animatable property adds/sets keyframes for the "material._Color" property in an animation clip.
    /// </summary>
    public class ColorStateAnimatableProperty : StateAnimatableProperty
    {
        [SerializeField]
        [Tooltip("Sets the last keyframe of the material._Color property to this Color in the animation clip" +
            "for a state.  The color to transition to. ")]
        private Color color;

        /// <summary>
        /// Sets the last keyframe of the "material._Color" property to this Color in the animation clip
        /// for a state.  The color to transition to. 
        /// </summary>
        public Color Color
        {
            get => color;
            set => color = value;
        }

        /// <summary>
        /// Constructor for a Color Animatable Property. Sets the default AnimatablePropertyName.
        /// </summary>
        public ColorStateAnimatableProperty()
        {
            AnimatablePropertyName = "Color";
        }

        /// <inheritdoc/>
        public override void SetKeyFrames(AnimationClip animationClip)
        {
            if (Target != null && Target.GetComponent<MeshRenderer>() != null)
            {
                float colorR = Color.r;
                float colorG = Color.g;
                float colorB = Color.b;
                float colorA = Color.a;

                Color currentColor = Target.GetComponent<MeshRenderer>().sharedMaterial.color;

                AnimationCurve curveR = AnimationCurve.EaseInOut(0, currentColor.r, AnimationDuration, colorR);
                AnimationCurve curveG = AnimationCurve.EaseInOut(0, currentColor.g, AnimationDuration, colorG);
                AnimationCurve curveB = AnimationCurve.EaseInOut(0, currentColor.b, AnimationDuration, colorB);
                AnimationCurve curveA = AnimationCurve.EaseInOut(0, currentColor.a, AnimationDuration, colorA);

                SetColorAnimationCurve(animationClip, curveR, curveG, curveB, curveA);

            }
            else
            {
                Debug.LogError("The target game object does not have a mesh renderer component attached. Attach a mesh renderer component to animate the Color property.");
            }
        }

        /// <inheritdoc/>
        public override void RemoveKeyFrames(AnimationClip animationClip)
        {
            if (Target != null && Target.GetComponent<MeshRenderer>() != null)
            {
                SetColorAnimationCurve(animationClip, null, null, null, null); 
            }
        }

        private void SetColorAnimationCurve(AnimationClip animationClip, AnimationCurve curveR, AnimationCurve curveG, AnimationCurve curveB, AnimationCurve curveA)
        {
            string targetPath = GetTargetPath(Target);

            animationClip.SetCurve(targetPath, typeof(MeshRenderer), "material._Color.r", curveR);
            animationClip.SetCurve(targetPath, typeof(MeshRenderer), "material._Color.g", curveG);
            animationClip.SetCurve(targetPath, typeof(MeshRenderer), "material._Color.b", curveB);
            animationClip.SetCurve(targetPath, typeof(MeshRenderer), "material._Color.a", curveA);
        }
    }
}
