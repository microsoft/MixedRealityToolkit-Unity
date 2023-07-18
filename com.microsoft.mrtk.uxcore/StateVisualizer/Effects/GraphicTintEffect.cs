// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License

using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

namespace Microsoft.MixedReality.Toolkit.UX
{
    /// <summary>
    /// A <see cref="TintEffect{T}"/> that wraps a PlayableBehaviour which can tint Unity <see href="https://docs.unity3d.com/Packages/com.unity.ugui%401.0/api/UnityEngine.UI.Graphic.html">Graphic</see> components.
    /// </summary>
    /// <remarks>
    /// This component is useful for tinting images and text meshes.
    /// </remarks>
    [Serializable]
    internal class GraphicTintEffect : TintEffect<Graphic>
    {
        /// <inheritdoc />
        internal class GraphicTintBehaviour : TintBehaviour<Graphic>
        {
            /// <inheritdoc />
            protected override void ApplyColor(Color color, Graphic graphic)
            {
                if (graphic == null) { return; }
                graphic.color = color;
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
            var playable = ScriptPlayable<GraphicTintBehaviour>.Create(graph);
            GraphicTintBehaviour behaviour = playable.GetBehaviour();

            Playable = playable;
            return behaviour;
        }
    }
}
