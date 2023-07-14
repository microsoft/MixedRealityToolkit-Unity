// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;
using UnityEngine.Playables;

namespace Microsoft.MixedReality.Toolkit.UX
{
    /// <summary>
    /// A Unity object that can be used with <see cref="PlayableAssetEffect"/> to generate a <see href="https://docs.unity3d.com/ScriptReference/Playables.Playable.html">Playable</see> component.
    /// </summary>
    /// <remarks>
    /// Writing custom <see cref="StateVisualAsset"/> components is useful when sharing
    /// common effects across prefab types without applying changes to the prefab.
    /// For prefab specific effects, just add an <see cref="IEffect"/> to the
    /// <see cref="StateVisualizer"/> directly.
    /// </remarks>
    [System.Serializable]
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