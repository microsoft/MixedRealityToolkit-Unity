// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities;

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Defines the base interactions and data that an controller can provide.
    /// </summary>
    public class BaseControllerDefinition
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
        public Handedness Handedness { get; private set; }

        /// <summary>
        /// The collection of interactions supported by this controller.
        /// </summary>
        protected virtual MixedRealityInteractionMapping[] DefaultInteractions => System.Array.Empty<MixedRealityInteractionMapping>();

        public MixedRealityInteractionMapping[] GetDefaultInteractions()
        {

        }
    }
}