// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Definitions;

namespace Microsoft.MixedReality.Toolkit.Internal.Interfaces.InputSystem
{
    /// <summary>
    /// Interface for an Input Action.
    /// </summary>
    public interface IMixedRealityInputAction
    {
        /// <summary>
        /// The Unique Id of this Input Action.
        /// </summary>
        uint Id { get; }

        /// <summary>
        /// A short description of the Input Action.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// The Axis constraint for the Input Action
        /// </summary>
        AxisType AxisConstraint { get; }
    }
}
