// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License

using System;
using UnityEngine;
using UnityEngine.Playables;

namespace Microsoft.MixedReality.Toolkit.UX
{
    [Serializable]
    /// <summary>
    /// A <see cref="IEffect"> that generates a <see cref="Playable"> from a <see cref="StateVisualAsset"/>.
    /// </summary>
    internal class PlayableAssetEffect : PlayableEffect
    {
        [SerializeField]
        [HideInInspector]
#pragma warning disable CS0414 // Inspector uses this as a helpful label in lists.
        private string name = "Playable Asset";
#pragma warning restore CS0414 // Inspector uses this as a helpful label in lists.

        [SerializeField]
        [Tooltip("The ScriptableObject containing the effect.")]
        private StateVisualAsset playableAsset;

        [SerializeField]
        [Tooltip("Playback speed of the playable.")]
        private float speed = 1.0f;

        /// <inheritdoc />
        protected override float Speed => speed;

        /// <inheritdoc />
        protected override PlayableEffect.PlaybackType PlaybackMode => playableAsset.PlaybackMode;

        public PlayableAssetEffect() { }

        public PlayableAssetEffect(StateVisualAsset playableAsset, float speed)
        {
            this.playableAsset = playableAsset;
            this.speed = speed;
        }

        /// <inheritdoc />
        public override void Setup(PlayableGraph graph, GameObject owner)
        {
            // Generate the playable from the specified playable asset.
            Playable = playableAsset.CreatePlayable(graph, owner);
        }
    }
}