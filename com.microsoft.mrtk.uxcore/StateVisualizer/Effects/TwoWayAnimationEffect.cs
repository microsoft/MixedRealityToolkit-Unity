// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License

using System;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;
using WeightType = Microsoft.MixedReality.Toolkit.UX.IAnimationMixableEffect.WeightType;

namespace Microsoft.MixedReality.Toolkit.UX
{
    [Serializable]
    /// <summary>
    /// An <see cref="IEffect"> that plays two different <see cref="AnimationClip"/>s,
    /// one for the forward direction, and one for the reverse direction.
    /// </summary>
    internal class TwoWayAnimationEffect : PlayableEffect, IAnimationMixableEffect
    {
        [SerializeField]
        [HideInInspector]
#pragma warning disable CS0414 // Inspector uses this as a helpful label in lists.
        private string name = "Two Way Animation";
#pragma warning restore CS0414 // Inspector uses this as a helpful label in lists.

        [SerializeField]
        [Tooltip("The animation clip for the forward direction.")]
        private AnimationClip forwardClip;

        [SerializeField]
        [Tooltip("The animation clip for the reverse direction.")]
        private AnimationClip reverseClip;

        [SerializeField]
        [Tooltip("Playback speed of the overall blended animation.")]
        private float speed = 1.0f;

        /// <inheritdoc />
        protected override float Speed => speed;

        /// <inheritdoc />
        /// Two-way animations are always one-shot playback.
        protected override PlayableEffect.PlaybackType PlaybackMode => PlayableEffect.PlaybackType.OneShot;

        #region IAnimationMixableEffect

        [SerializeField]
        [Tooltip("How should this state's animation be blended and weighted? Constant: Always weighted 1.0, Transition: Transitions to 1 when state becomes active, MatchStateValue: Always weighted by the state's value.")]
        private WeightType weightMode;

        /// <inheritdoc />
        public WeightType WeightMode => weightMode;

        [SerializeField]
        [DrawIf("weightMode", WeightType.Transition)]
        [Tooltip("How long should it take to transition to a weight of 1.0 when the state becomes active?")]
        private float transitionDuration;

        /// <inheritdoc />
        public float TransitionDuration => transitionDuration;

        #endregion IAnimationMixableEffect

        private Playable forwardPlayable;

        private Playable reversePlayable;

        private float lastValue;

        public TwoWayAnimationEffect() { }

        public TwoWayAnimationEffect(AnimationClip forwardClip, AnimationClip reverseClip, float speed)
        {
            this.forwardClip = forwardClip;
            this.reverseClip = reverseClip;
            this.speed = speed;
        }

        /// <inheritdoc />
        public override void Setup(PlayableGraph graph, GameObject owner)
        {
            Playable = AnimationMixerPlayable.Create(graph, 2);

            forwardPlayable = AnimationClipPlayable.Create(graph, forwardClip);
            forwardPlayable.SetDuration(forwardClip.length);
            reversePlayable = AnimationClipPlayable.Create(graph, reverseClip);
            reversePlayable.SetDuration(reverseClip.length);

            Playable.SetInputWeight(0, 1.0f);
            Playable.SetInputWeight(1, 0.0f);

            Playable.SetDuration(Mathf.Max(forwardClip.length, reverseClip.length));

            graph.Connect(forwardPlayable, 0, Playable, 0);
            graph.Connect(reversePlayable, 0, Playable, 1);
        }

        /// <inheritdoc />
        public override bool Evaluate(float stateValue)
        {
            if (stateValue > 0.001f)
            {
                Playable.SetInputWeight(0, 1.0f);
                Playable.SetInputWeight(1, 0.0f);
            }
            else
            {
                Playable.SetInputWeight(0, 0.0f);
                Playable.SetInputWeight(1, 1.0f);
            }

            if (reversePlayable.GetTime() < 0) { reversePlayable.SetTime(0.0f); }
            if (forwardPlayable.GetTime() < 0) { forwardPlayable.SetTime(0.0f); }

            return base.Evaluate(stateValue);
        }
    }
}