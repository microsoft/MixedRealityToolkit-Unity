// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Microsoft.MixedReality.Toolkit.Internal.Definitions.Utilities
{
    /// <summary>
    /// Easing types
    /// </summary>
    public enum EasingTypes
    {
        /// <summary>
        /// Steady Linear progression.
        /// </summary>
        Linear = 0,
        /// <summary>
        /// Ramp up in speed
        /// </summary>
        In,
        /// <summary>
        /// Ramp down in speed
        /// </summary>
        Out,
        /// <summary>
        /// Ramp up then down in speed
        /// </summary>
        InOut,
        /// <summary>
        /// Super ease - just updates as the TargetValue changes
        /// </summary>
        Free
    }
}