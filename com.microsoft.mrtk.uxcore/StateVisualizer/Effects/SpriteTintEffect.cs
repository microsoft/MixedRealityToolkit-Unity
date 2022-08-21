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
            protected override void ApplyColor(Color color, SpriteRenderer sprite)
            {
                if (sprite == null) { return; }
                sprite.color = color;
            }

            /// <inheritdoc />
            protected override bool GetColor(SpriteRenderer sprite, out Color color)
            {
                color = default;
                if (sprite == null) { return false; }
                color = sprite.color;
                return true;
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
