// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities.Gltf.Schema
{
    /// <summary>
    /// Combines input and output accessors with an interpolation algorithm to define a keyframe graph (but not its target).
    /// https://github.com/KhronosGroup/glTF/blob/master/specification/2.0/schema/animation.sampler.schema.json
    /// </summary>
    [Serializable]
    public class GltfAnimationSampler : GltfProperty, ISerializationCallbackReceiver
    {
        /// <summary>
        /// The index of an accessor containing keyframe input values, e.G., time.
        /// That accessor must have componentType `FLOAT`. The values represent time in
        /// seconds with `time[0] >= 0.0`, and strictly increasing values,
        /// i.e., `time[n + 1] > time[n]`
        /// </summary>
        public int input;

        /// <summary>
        /// Interpolation algorithm. When an animation targets a node's rotation,
        /// and the animation's interpolation is `\"LINEAR\"`, spherical linear
        /// interpolation (slerp) should be used to interpolate quaternions. When
        /// interpolation is `\"STEP\"`, animated value remains constant to the value
        /// of the first point of the timeframe, until the next timeframe.
        /// </summary>
        public GltfInterpolationType Interpolation { get; set; }

        [SerializeField]
        private string interpolation = null;

        /// <summary>
        /// The index of an accessor, containing keyframe output values. Output and input
        /// accessors must have the same `count`. When sampler is used with TRS target,
        /// output accessors componentType must be `FLOAT`.
        /// </summary>
        public int output;

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            if (Enum.TryParse(interpolation, out GltfInterpolationType result))
            {
                Interpolation = result;
            }
            else
            {
                Interpolation = GltfInterpolationType.LINEAR;
            }
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            interpolation = Interpolation.ToString();
        }
    }
}