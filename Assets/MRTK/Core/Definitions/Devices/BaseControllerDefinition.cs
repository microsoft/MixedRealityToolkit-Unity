// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Defines the base interactions and data that an controller can provide.
    /// </summary>
    public abstract class BaseControllerDefinition : IMixedRealityInputSourceDefinition
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="handedness"></param>
        public BaseControllerDefinition(Handedness handedness)
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
        /// <remarks>Optional, override DefaultInteractions if both handed controllers have identical interactions.</remarks>
        protected virtual MixedRealityInteractionMapping[] DefaultLeftHandedInteractions => DefaultInteractions;

        /// <summary>
        /// The collection of interactions supported by a right-handed instance of this controller.
        /// </summary>
        /// <remarks>Optional, override DefaultInteractions if both handed controllers have identical interactions.</remarks>
        protected virtual MixedRealityInteractionMapping[] DefaultRightHandedInteractions => DefaultInteractions;

        /// <summary>
        /// The collection of interactions supported by this controller.
        /// </summary>
        /// <remarks>Optional, override the specifically-handed properties if each controller has different interactions.</remarks>
        protected virtual MixedRealityInteractionMapping[] DefaultInteractions => null;

        public MixedRealityInteractionMapping[] GetDefaultInteractions()
        {
            switch (Handedness)
            {
                case Handedness.Left:
                    return DefaultLeftHandedInteractions;
                case Handedness.Right:
                    return DefaultRightHandedInteractions;
                default:
                    return DefaultInteractions;
            }
        }
    }
}