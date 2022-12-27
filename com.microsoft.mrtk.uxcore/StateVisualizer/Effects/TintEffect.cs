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
            private List<U> tintables = new List<U>();

            /// <summary>
            /// The objects targeted by this PlayableBehaviour.
            /// </summary>
            public List<U> Tintables
            {
                get => tintables;
                set
                {
                    tintables = value;
                    if (startColors == null || startColors.Length != tintables.Count ||
                        cachedColors == null || cachedColors.Length != tintables.Count)
                    {
                        startColors = new Color[tintables.Count];
                        cachedColors = new Color[tintables.Count];
                    }
                }
            }

            /// <summary>
            /// The color to lerp to.
            /// </summary>
            public Color TintColor { get; set; }

            /// <summary>
            /// How should this tint color be blended onto the existing Graphic color
            /// (including the effects of other GraphicTintEffects?)
            /// </summary>
            public BlendType BlendMode { get; set; }

            // Each individual tintable's "base color", which is blended
            // upon by the tint behavior based on the blend type and
            // factor.
            private Color[] startColors;

            // Private cache of the color when the graph stops.
            // Checked when the graph restarts; if it's different,
            // the user probably externally modified the color, and
            // we set our new StartColor to the current color.
            // It's not perfect (for example, if it's changed *during* graph
            // execution, we won't catch it) but it works for most use cases.
            private Color[] cachedColors;

            public TintBehaviour() { }

            /// <summary>
            /// Implement this to specify how the tintables should have their color set.
            /// </summary>
            protected abstract void ApplyColor(Color color, U tintable);

            /// <summary>
            /// Implement this to define how colors are retrieved from the tintables.
            /// </summary>
            /// <returns> true if color was successfully retrieved. </returns>
            protected abstract bool GetColor(U tintable, out Color color);

            /// <inheritdoc />
            public override void OnGraphStop(Playable playable)
            {
                base.OnGraphStop(playable);

                // Cache the color when the graph stops.
                // We check it again when the graph restarts,
                // and if it changed, it was probably externally modified.
                for (int i = 0; i < tintables.Count; i++)
                {
                    if (GetColor(tintables[i], out Color color))
                    {
                        cachedColors[i] = color;
                    }
                }
            }

            /// <inheritdoc />
            public override void OnGraphStart(Playable playable)
            {
                base.OnGraphStart(playable);

                // If any color was externally modified (while graph was dormant),
                // let's set the StartColor to the modified color.
                for (int i = 0; i < tintables.Count; i++)
                {
                    // If the current color is different than the cached color,
                    // update StartColor.
                    if (GetColor(tintables[i], out Color color) && cachedColors[i] != color)
                    {
                        startColors[i] = color;
                    }
                }
            }

            /// <inheritdoc />
            public override void PrepareFrame(Playable playable, FrameData info)
            {
                base.PrepareFrame(playable, info);

                // At the beginning of a frame/graph execution,
                // we apply the starting colors to the tintables,
                // so that the blending operation can begin again
                // across all participating tint behaviors.
                for (int i = 0; i < tintables.Count; i++)
                {
                    ApplyColor(startColors[i], tintables[i]);
                }
                
            }

            /// <inheritdoc />
            public override void ProcessFrame(Playable playable, FrameData info, object playerData)
            {
                base.ProcessFrame(playable, info, playerData);

                float factor = (float)(playable.GetTime() / playable.GetDuration());
                Color targetColor;

                for (int i = 0; i < tintables.Count; i++)
                {
                    // We grab the *current* color, because this behavior isn't necessarily
                    // the only thing tinting this tintable! TintBehaviors can stack and blend.
                    if (!GetColor(tintables[i], out Color currentColor)) { continue; }

                    // Compute our target color based on the specified mix mode.
                    if (BlendMode == BlendType.Override)
                    {
                        targetColor = TintColor;
                    }
                    else if (BlendMode == BlendType.Additive)
                    {
                        targetColor = currentColor + TintColor;
                    }
                    else if (BlendMode == BlendType.Multiply)
                    {
                        targetColor = currentColor * TintColor;
                    }
                    else
                    {
                        targetColor = TintColor;
                    }

                    ApplyColor(Color.Lerp(currentColor, targetColor, factor), tintables[i]);
                }
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
            behaviourInstance.TintColor = color;
            behaviourInstance.BlendMode = blendMode;

            // Set the playable's duration to the desired transition duration.
            Playable.SetDuration(transitionDuration);
        }

        /// <inheritdoc />
        public override bool Evaluate(float value)
        {
            // Update the PlayableBehaviour settings for runtime editing of playable settings.
            behaviourInstance.Tintables = tintables;
            behaviourInstance.TintColor = color;
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
