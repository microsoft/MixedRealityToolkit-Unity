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
    /// A <see cref="TintEffect<T>"> that wraps a PlayableBehaviour which can fade the alpha of <see cref="Graphic"> components.
    /// </summary>
    /// <remarks>
    /// Useful for fading UI Image/RawImages, TMPros, etc.
    /// </remarks>
    internal class GraphicFadeEffect : TintEffect<Graphic>
    {
        /// <inheritdoc />
        internal class GraphicFadeBehaviour : TintBehaviour<Graphic>
        {
            /// <inheritdoc />
            protected override void ApplyColor(Color color)
            {
                foreach (Graphic graphic in Tintables)
                {
                    graphic.color = new Color(graphic.color.r, graphic.color.g, graphic.color.b, color.a);
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
        protected override TintBehaviour<Graphic> CreatePlayableAndBehaviour(PlayableGraph graph)
        {
            // We construct a ScriptPlayable that wraps our TintBehaviour.
            var playable = ScriptPlayable<GraphicFadeBehaviour>.Create(graph);
            GraphicFadeBehaviour behaviour = playable.GetBehaviour();

            Playable = playable;
            return behaviour;
        }
    }
}
