// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License

using UnityEngine;
using UnityEngine.Playables;

namespace Microsoft.MixedReality.Toolkit.UX
{
    /// <summary>
    /// An interface defining a state effect that <see cref="StateVisualizer"/> can use.
    /// </summary>
    public interface IEffect
    {
        /// <summary>
        /// Called by the <see cref="StateVisualizer"/> on Start().
        /// Playable-based effects should initialize their <see cref="Playable"/> here.
        /// </summary>
        void Setup(PlayableGraph graph, GameObject owner);

        /// <summary>
        /// Called by the <see cref="StateVisualizer"/> every frame.Playable-based effects should
        /// update their internal state and control their <see cref="Playable"/> here.
        /// </summary>
        /// <returns>
        /// True if the effect is "done". If all registered <see cref="IEffect"/>s return true,
        /// the PlayableGraph will be stopped and the animator disabled until an interaction occurs.
        /// </returns>
        bool Evaluate(float parameter);
    }
}