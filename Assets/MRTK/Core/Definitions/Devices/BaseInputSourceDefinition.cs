// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections.Generic;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Defines the base interactions and data that an controller can provide.
    /// </summary>
    public abstract class BaseInputSourceDefinition : IMixedRealityInputSourceDefinition
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="handedness">The handedness that this definition instance represents.</param>
        public BaseInputSourceDefinition(Handedness handedness)
        {
            Handedness = handedness;
        }

        /// <summary>
        /// The <see cref="Handedness"/> (ex: Left, Right, None) of this controller.
        /// </summary>
        public Handedness Handedness { get; }

        /// <summary>
        /// The collection of interactions supported by a left-handed instance of this controller.
        /// </summary>
        /// <remarks>Optional. Override DefaultInteractions if both handed controllers have identical interactions.</remarks>
        protected virtual MixedRealityInputActionMapping[] DefaultLeftHandedMappings => DefaultMappings;

        /// <summary>
        /// The collection of interactions supported by a right-handed instance of this controller.
        /// </summary>
        /// <remarks>Optional. Override DefaultInteractions if both handed controllers have identical interactions.</remarks>
        protected virtual MixedRealityInputActionMapping[] DefaultRightHandedMappings => DefaultMappings;

        /// <summary>
        /// The collection of interactions supported by this controller.
        /// </summary>
        /// <remarks>Optional. Override the specifically-handed properties if each controller has different interactions.</remarks>
        protected virtual MixedRealityInputActionMapping[] DefaultMappings => null;

        /// <inheritdoc />
        public IReadOnlyList<MixedRealityInputActionMapping> GetDefaultMappings(Handedness handedness)
        {
            switch (handedness)
            {
                case Handedness.Left:
                    return DefaultLeftHandedMappings;
                case Handedness.Right:
                    return DefaultRightHandedMappings;
                default:
                    return DefaultMappings;
            }
        }
    }
}