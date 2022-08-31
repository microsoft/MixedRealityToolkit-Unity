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
    /// A <see cref="TintEffect<T>"> that wraps a PlayableBehaviour which can fade
    /// only the alpha component of <see cref="Graphic"> components. The rgb
    /// channels of the color will be unaffected.
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
            protected override void ApplyColor(Color color, Graphic graphic)
            {
                if (graphic == null) { return; }

                // Apply only the alpha channel; leave color channels unaffected.
                graphic.color = new Color(graphic.color.r, graphic.color.g, graphic.color.b, color.a);
            }

            /// <inheritdoc />
            protected override bool GetColor(Graphic graphic, out Color color)
            {
                color = default;
                if (graphic == null) { return false; }
                color = graphic.color;
                return true;
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
