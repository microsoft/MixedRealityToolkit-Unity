// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;
using UnityEngine.Playables;

namespace Microsoft.MixedReality.Toolkit.UX
{
    [System.Serializable]
    /// <summary>
    /// A ScriptableObject-based asset that can be used with <see cref="PlayableAssetEffect">.
    /// </summary>
    /// <remarks>
    /// Writing custom <see cref="StateVisualAsset"/>s is useful if you would like to share
    /// common effects across prefab types without apply changes to the prefab.
    /// For prefab-specific effects, just add an <see cref="IEffect"/> to the
    /// <see cref="StateVisualizer"/> directly.
    /// </remarks>
    internal abstract class StateVisualAsset : PlayableAsset
    {
        [SerializeField]
        [Tooltip("Should the playable be played back as a one-shot triggered effect, or should the playback time be directly driven by the state's value?")]
        private PlayableEffect.PlaybackType playbackMode;

        /// <summary>
        /// Should the playable be played back as a one-shot triggered effect,
        /// or should the playback time be directly driven by the state's value?
        /// </summary>
        public PlayableEffect.PlaybackType PlaybackMode => playbackMode;
    }
}