// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License

using System;
using UnityEngine;
using UnityEngine.Playables;

namespace Microsoft.MixedReality.Toolkit.UX
{
    /// <summary>
    /// A <see cref="TintEffect{T}"/> implementation that wraps a Unity <see href="https://docs.unity3d.com/ScriptReference/Playables.PlayableBehaviour.html">PlayableBehaviour</see> 
    /// component, and can tint Unity <see href="https://docs.unity3d.com/ScriptReference/SpriteRenderer.html">SpriteRenderer</see> components.
    /// </summary>
    /// <remarks>
    /// Useful for tinting or fading sprites.
    /// </remarks>
    [Serializable]
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
