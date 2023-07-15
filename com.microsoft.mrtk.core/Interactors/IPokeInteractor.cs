// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Microsoft.MixedReality.Toolkit
{
    /// <summary>
    /// An interface that all poke-like interactors implement.
    /// </summary>
    /// <remarks>
    /// Interactors that implement this interface are expected to use
    /// the <see cref="IXRInteractor.GetAttachTransform(IXRInteractable)"/> to specify
    /// the point at which the poke occurs. Typically, this would be
    /// the tip of the index finger for an articulated hand, or a
    /// predetermined poking point on a motion controller rig.
    /// </remarks>
    public interface IPokeInteractor : IXRHoverInteractor, IXRSelectInteractor
    {
        /// <summary>
        /// The path of a poking type movement.
        /// </summary>
        struct PokePath
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="PokePath"/> class.
            /// </summary>
            /// <param name="start">The poke position at the beginning of the poking path.</param>
            /// <param name="end">The poke position at the end of the poking path.</param>
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
        /// The radius of the poking point.
        /// </summary>
        /// <remarks>
        /// This is used to pad a pressable or push surface, so that the surface of the finger
        /// is treated as the pressing point and not the center of the finger.
        /// </remarks>
        float PokeRadius { get; }

        /// <summary>
        /// The path that the poke has taken over the course of the last
        /// frame.
        /// </summary>
        /// <remarks>
        /// Typically, the endpoint of the path is the current attachTransform,
        /// but this is not guaranteed.
        /// </remarks>
        PokePath PokeTrajectory { get; }
    }
}
