// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Experimental.StateVisualizer
{
    /// <summary>
    /// Interface for animatable properties, utilized in the State Visualizer component. 
    /// </summary>
    public interface IStateAnimatableProperty
    {
        /// <summary>
        /// The name of the state associated with this animatable property.
        /// </summary>
        string StateName { get; set; }

        /// <summary>
        /// The name of the animatable property.
        /// </summary>
        string AnimatablePropertyName { get; set; }

        /// <summary>
        /// The target game object to receive animations based on the values of the animatable properties.
        /// </summary>
        GameObject Target { get; set; }

        /// <summary>
        /// Sets the keyframes in an animation clip based on the values of the animatable properties. 
        /// </summary>
        /// <param name="animationClip">The animation clip to add keyframes to</param>
        void SetKeyFrames(AnimationClip animationClip);

        /// <summary>
        /// Removes the keyframes in an animation clip. 
        /// </summary>
        /// <param name="animationClip">The animation clip for keyframe removal</param>
        void RemoveKeyFrames(AnimationClip animationClip);
    }
}
