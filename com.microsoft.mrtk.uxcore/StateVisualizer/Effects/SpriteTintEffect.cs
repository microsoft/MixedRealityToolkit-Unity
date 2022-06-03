// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License

using System;
using UnityEngine;
using UnityEngine.Playables;

namespace Microsoft.MixedReality.Toolkit.UX
{
    [Serializable]
    /// <summary>
    /// A <see cref="TintEffect<T>"> that wraps a PlayableBehaviour which can tint <see cref="SpriteRenderer"> components.
    /// </summary>
    /// <remarks>
    /// Useful for tinting or fading sprites.
    /// </remarks>
    internal class SpriteTintEffect : TintEffect<SpriteRenderer>
    {
        /// <inheritdoc />
        internal class SpriteTintBehaviour : TintBehaviour<SpriteRenderer>
        {
            /// <inheritdoc />
            protected override void ApplyColor(Color color)
            {
                foreach (SpriteRenderer sprite in Tintables)
                {
                    sprite.color = color;
                }
            }

            /// <inheritdoc />
            protected override Color GetColor()
            {
                if (Tintables.Count > 0 && Tintables[0] != null)
                {
                    return Tintables[0].color;
                }
                else
                {
                    return default;
                }
            }
        }

        /// <inheritdoc />
        protected override TintBehaviour<SpriteRenderer> CreatePlayableAndBehaviour(PlayableGraph graph)
        {
            // We construct a ScriptPlayable that wraps our TintBehaviour.
            var playable = ScriptPlayable<SpriteTintBehaviour>.Create(graph);
            SpriteTintBehaviour behaviour = playable.GetBehaviour();

            Playable = playable;
            return behaviour;
        }
    }
}
