// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace Microsoft.MixedReality.Toolkit.UX
{
    [Serializable]
    /// <summary>
    /// An <see cref="IEffect"> that wraps a PlayableBehaviour which can tint arbitrary types of components.
    /// </summary>
    /// <remarks>
    /// This pattern is to support the tinting of disparate component types that can all be tinted, but
    /// are not necessarily related by class or type (such as Sprites vs Images).
    /// Subclass this abstract class to create tinting behaviour for arbitrary tintable components, such as
    /// SpriteRenderers, TMPros, Graphic components, and even custom materials/meshes.
    /// </remarks>
    internal abstract class TintEffect<T> : PlayableEffect, ISerializationCallbackReceiver
    {
        /// <summary>
        /// A PlayableBehavior that controls the tint effect, based on the
        /// playback time/duration/etc of the playable graph node.
        /// </summary>
        internal abstract class TintBehaviour<U> : PlayableBehaviour
        {
            /// <summary>
            /// The objects targeted by this PlayableBehaviour.
            /// </summary>
            public List<U> Tintables { get; set; }

            /// <summary>
            /// The color to lerp from.
            /// </summary>
            public Color StartColor { get; set; }

            /// <summary>
            /// The color to lerp to.
            /// </summary>
            public Color EndColor { get; set; }

            /// <summary>
            /// How should this tint color be blended onto the existing Graphic color
            /// (including the effects of other GraphicTintEffects?)
            /// </summary>
            public BlendType BlendMode { get; set; }

            // Private cache of the color when the graph stops.
            // Checked when the graph restarts; if it's different,
            // the user probably externally modified the color, and
            // we set our new StartColor to the current color.
            // It's not perfect (for example, if it's changed *during* graph
            // execution, we won't catch it) but it works for most use cases.
            private Color cachedColor;

            public TintBehaviour() { }

            /// <summary>
            /// Implement this to specify how the tintables should have their color set.
            /// </summary>
            protected abstract void ApplyColor(Color color);

            /// <summary>
            /// Implement this to define how a color is retrieved from the tintables.
            /// </summary>
            protected abstract Color GetColor();

            /// <inheritdoc />
            public override void OnGraphStop(Playable playable)
            {
                base.OnGraphStop(playable);

                // Cache the color when the graph stops.
                // We check it again when the graph restarts,
                // and if it changed, it was probably externally modified.
                cachedColor = GetColor();
            }

            /// <inheritdoc />
            public override void OnGraphStart(Playable playable)
            {
                base.OnGraphStart(playable);

                // If the color was externally modified,
                // let's set the StartColor to the modified color.
                if (cachedColor != GetColor())
                {
                    StartColor = GetColor();
                }
            }

            /// <inheritdoc />
            public override void PrepareFrame(Playable playable, FrameData info)
            {
                base.PrepareFrame(playable, info);

                ApplyColor(StartColor);
            }

            /// <inheritdoc />
            public override void ProcessFrame(Playable playable, FrameData info, object playerData)
            {
                base.ProcessFrame(playable, info, playerData);

                float factor = (float)(playable.GetTime() / playable.GetDuration());

                Color currentColor = GetColor();
                Color targetColor = EndColor;

                // Compute our target color based on the specified mix mode.
                if (BlendMode == BlendType.Override)
                {
                    targetColor = EndColor;
                }
                else if (BlendMode == BlendType.Additive)
                {
                    targetColor = currentColor + EndColor;
                }
                else if (BlendMode == BlendType.Multiply)
                {
                    targetColor = currentColor * EndColor;
                }

                ApplyColor(Color.Lerp(currentColor, targetColor, factor));
            }
        }

        /// <summary>
        /// An enum describing the various ways in which <see cref="GraphicTintEffect"/>'s tint color
        /// is applied onto the existing tint stack.
        /// </summary>
        internal enum BlendType
        {
            /// <summary>
            /// Lerp directly to the tint color, overriding any previous colors.
            /// </summary>
            Override,

            /// <summary>
            /// Additively blend the tint color onto to the existing color stack.
            /// </summary>
            Additive,

            /// <summary>
            /// Multiply the tint color onto the existing color stack.
            /// </summary>
            Multiply
        }

        [SerializeField]
        [HideInInspector]
#pragma warning disable CS0414 // Inspector uses this as a helpful label in lists.
        private string name = "Tint";
#pragma warning restore CS0414 // Inspector uses this as a helpful label in lists.

        [SerializeField]
        [Tooltip("The duration of the transition from un-tinted to tinted.")]
        private float transitionDuration = 0.3f;

        /// <inheritdoc />
        protected override float Speed => 1.0f;

        [SerializeReference]
        [Tooltip("The objects to tint. All of them must share the same tint color; use separate TintEffects for different colors.")]
        private List<T> tintables;

        [SerializeField]
        [Tooltip("Tint color.")]
        private Color color = Color.white;

        [SerializeField]
        [Tooltip("Should the playable be played back as a one-shot triggered effect, or should the playback time be directly driven by the state's value?")]
        private PlayableEffect.PlaybackType playbackMode;

        /// <inheritdoc />
        protected override PlayableEffect.PlaybackType PlaybackMode => playbackMode;

        [SerializeField]
        [Tooltip("How should this tint color be blended onto the existing Graphic color (including the effects of other GraphicTintEffects?)")]
        private BlendType blendMode;

        /// <inheritdoc />
        protected BlendType BlendMode => blendMode;

        // Instance of the PlayableBehaviour this Effect wraps.
        private TintBehaviour<T> behaviourInstance;

        public TintEffect() { }

        public TintEffect(PlayableEffect.PlaybackType playbackMode, float transitionDuration, Color color)
        {
            this.playbackMode = playbackMode;
            this.transitionDuration = transitionDuration;
            this.color = color;
        }

        /// <inheritdoc />
        public override void Setup(PlayableGraph graph, GameObject owner)
        {
            // This should also create the Playable.
            behaviourInstance = CreatePlayableAndBehaviour(graph);

            // Populate the behaviour with the relevant settings.
            behaviourInstance.Tintables = tintables;
            behaviourInstance.EndColor = color;
            behaviourInstance.BlendMode = blendMode;

            // Set the playable's duration to the desired transition duration.
            Playable.SetDuration(transitionDuration);
        }

        /// <inheritdoc />
        public override bool Evaluate(float value)
        {
            // Update the PlayableBehaviour settings for runtime editing of playable settings.
            behaviourInstance.Tintables = tintables;
            behaviourInstance.EndColor = color;
            behaviourInstance.BlendMode = blendMode;
            Playable.SetDuration(transitionDuration);
            return base.Evaluate(value);
        }

        /// <summary>
        /// Implement this method to create the subclass's PlayableBehaviour implementation.
        /// Make sure to set the Playable field, and return the behaviour instance.
        /// </summary>
        protected abstract TintBehaviour<T> CreatePlayableAndBehaviour(PlayableGraph graph);

        // Unity doesn't like polymorphic name serialization in ReorderableLists,
        // so we have to do this for the name of the subclasses to show up as the title
        // of each entry in the reorderablelist.
        public void OnBeforeSerialize()
        {
            name = GetType().Name.Replace("Effect", string.Empty);
        }

        public void OnAfterDeserialize()
        {
            name = GetType().Name.Replace("Effect", string.Empty);
        }
    }
}
