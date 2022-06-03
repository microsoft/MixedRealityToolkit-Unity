// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Microsoft.MixedReality.Toolkit.UX
{
    [System.Serializable]
    [CreateAssetMenu(
            fileName = "MRTKHandsAggregatorConfig.asset",
            menuName = "MRTK/State Visualizer/Animation PlayableAsset")]
    /// <summary>
    /// A ScriptableObject-based asset that can be used with <see cref="PlayableAssetEffect">.
    /// </summary>
    /// <remarks>
    /// This playable asset wraps an AnimationClip; however, just using <see cref="AnimationEffect"/>
    /// is generally the best way to apply animation clips to a <see cref="StateVisualizer"/>.
    /// <see cref="StateVisualAnimationAsset"/> can be used to share common definitions across prefabs,
    /// or store other state visuals as shared assets.
    /// </remarks>
    internal class StateVisualAnimationAsset : StateVisualAsset
    {
        [SerializeField]
        [Tooltip("The animation clip to wrap in a Playable.")]
        private AnimationClip clip;

        // Factory method that generates a playable based on this asset
        public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
        {
            var playable = AnimationClipPlayable.Create(graph, clip);
            playable.SetDuration(clip.length);
            return playable;
        }
    }
}
