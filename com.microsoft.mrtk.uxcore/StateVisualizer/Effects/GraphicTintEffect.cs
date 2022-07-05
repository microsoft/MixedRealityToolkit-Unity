// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License

using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

namespace Microsoft.MixedReality.Toolkit.UX
{
    [Serializable]
    /// <summary>
    /// A <see cref="TintEffect<T>"> that wraps a PlayableBehaviour which can tint <see cref="Graphic"> components.
    /// </summary>
    /// <remarks>
    /// Useful for tinting UI Image/RawImages, TMPros, etc.
    /// </remarks>
    internal class GraphicTintEffect : TintEffect<Graphic>
    {
        /// <inheritdoc />
        internal class GraphicTintBehaviour : TintBehaviour<Graphic>
        {
            /// <inheritdoc />
            protected override void ApplyColor(Color color)
            {
                if (Tintables == null) { return; }

                foreach (Graphic graphic in Tintables)
                {
                    if (graphic != null)
                    {
                        graphic.color = color;
                    }
                }
            }

            /// <inheritdoc />
            protected override Color GetColor()
            {
                if (Tintables == null) { return default; }

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
        protected override TintBehaviour<Graphic> CreatePlayableAndBehaviour(PlayableGraph graph)
        {
            // We construct a ScriptPlayable that wraps our TintBehaviour.
            var playable = ScriptPlayable<GraphicTintBehaviour>.Create(graph);
            GraphicTintBehaviour behaviour = playable.GetBehaviour();

            Playable = playable;
            return behaviour;
        }
    }
}
