// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Microsoft.MixedReality.Toolkit
{
    /// <summary>
    /// An interface that all poke-like interactors implement.
    /// Interactors that implement this interface are expected to use
    /// the <see cref="IXRInteractor"/> attachTransform to specify
    /// the point at which the poke occurs; typically, this would be
    /// the tip of the index finger for an articulated hand, or a
    /// predetermined poking point on a motion controller rig.
    /// </summary>
    public interface IPokeInteractor : IXRHoverInteractor, IXRSelectInteractor
    {
        struct PokePath
        {
            public PokePath(Vector3 start, Vector3 end)
            {
                Start = start;
                End = end;
            }

            /// <summary>
            /// The poke position at the beginning of the poking path.
            /// </summary>
            public Vector3 Start;

            /// <summary>
            /// The poke position at the end of the poking path.
            /// </summary>
            public Vector3 End;
        }

        /// <summary>
        /// The radius of the poking point. This is used to pad
        /// a pressable/pushable surface so that the surface of the finger
        /// is treated as the pressing point, not the center of the finger.
        /// </summary>
        float PokeRadius { get; }

        /// <summary>
        /// The path that the poke has taken over the course of the last
        /// frame. Typically, the endpoint of the path is the current attachTransform,
        /// but this is not guaranteed.
        /// </summary>
        PokePath PokeTrajectory { get; }
    }
}
