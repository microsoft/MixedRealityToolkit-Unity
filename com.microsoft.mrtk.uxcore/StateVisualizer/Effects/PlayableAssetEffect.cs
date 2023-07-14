// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License

using System;
using UnityEngine;
using UnityEngine.Playables;

namespace Microsoft.MixedReality.Toolkit.UX
{
    /// <summary>
    /// A <see cref="IEffect"/> implementation that generates a 
    /// <see href="https://docs.unity3d.com/ScriptReference/Playables.Playable.html">Playable</see> component 
    /// from a <see cref="StateVisualAsset"/> component.
    /// </summary>
    [Serializable]
    internal class PlayableAssetEffect : PlayableEffect
    {
        [SerializeField]
        [HideInInspector]
#pragma warning disable CS0414 // Inspector uses this as a helpful label in lists.
        private string name = "Playable Asset";
#pragma warning restore CS0414 // Inspector uses this as a helpful label in lists.

        [SerializeField]
        [Tooltip("The scriptable object containing the effect.")]
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