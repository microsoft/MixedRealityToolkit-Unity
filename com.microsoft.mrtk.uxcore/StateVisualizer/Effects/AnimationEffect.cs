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
    /// An <see cref="IEffect"> that plays an <see cref="AnimationClip"/>.
    /// </summary>
    internal class AnimationEffect : PlayableEffect, IAnimationMixableEffect
    {
        [SerializeField]
        [HideInInspector]
#pragma warning disable CS0414 // Inspector uses this as a helpful label in lists.
        private string name = "Animation";
#pragma warning restore CS0414 // Inspector uses this as a helpful label in lists.

        [SerializeField]
        [Tooltip("The animation clip for this effect.")]
        private AnimationClip clip;

        [SerializeField]
        [Tooltip("Playback speed of the playable.")]
        private float speed = 1.0f;

        /// <inheritdoc />
        protected override float Speed => speed;

        [SerializeField]
        [Tooltip("Should the playable be played back as a one-shot triggered effect, or should the playback time be directly driven by the state's value?")]
        private PlayableEffect.PlaybackType playbackMode;

        /// <inheritdoc />
        protected override PlayableEffect.PlaybackType PlaybackMode => playbackMode;

        #region IAnimationMixableEffect

        [SerializeField]
        [Tooltip("How should this state's animation be blended and weighted? Constant: Always weighted 1.0, Transition: Transitions to 1 when state becomes active, MatchStateValue: Always weighted by the state's value.")]
        private WeightType weightMode;

        /// <inheritdoc />
        public WeightType WeightMode => weightMode;

        [SerializeField]
        [Tooltip("How long should it take to transition to a weight of 1.0 when the state becomes active?")]
        private float transitionDuration;

        /// <inheritdoc />
        public float TransitionDuration => transitionDuration;

        #endregion IAnimationMixableEffect

        public AnimationEffect() { }

        public AnimationEffect(AnimationClip clip, PlayableEffect.PlaybackType playbackMode, float speed)
        {
            this.clip = clip;
            this.playbackMode = playbackMode;
            this.speed = speed;
        }

        /// <inheritdoc />
        public override void Setup(PlayableGraph graph, GameObject owner)
        {
            Playable = AnimationClipPlayable.Create(graph, clip);
            Playable.SetDuration(clip.length);
        }
    }
}