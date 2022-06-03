// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License

namespace Microsoft.MixedReality.Toolkit.UX
{
    /// <summary>
    /// An <see cref="IPlayableEffect"/> that plugs into an AnimationLayerMixerPlayable.
    /// </summary>
    public interface IAnimationMixableEffect : IPlayableEffect
    {
        /// <summary>
        /// How should this state's animation be blended and weighted?
        /// </summary>
        public enum WeightType
        {
            /// <summary>
            /// The animation is always active, and the first keyframe will be continuously applied.
            /// May hide animations below it in the stack.
            /// </summary>
            Constant,

            /// <summary>
            /// The weight of the animation will default to zero, but will transition to 1 when the
            /// state becomes active.
            /// </summary>
            Transition,

            /// <summary>
            /// Similar to Transition, but the weight of the layer will match the value of the state.
            /// </summary>
            MatchStateValue,
        }

        /// <summary>
        /// How should this state's animation be blended and weighted?
        /// </summary>
        WeightType WeightMode { get; }

        /// <summary>
        /// If WeightMode is set to Transition, this is the time it will take to transition to a weight of 1.0.
        /// </summary>
        float TransitionDuration { get; }
    }
}
