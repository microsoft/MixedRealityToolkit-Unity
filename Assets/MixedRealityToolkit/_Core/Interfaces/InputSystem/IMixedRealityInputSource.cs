// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections;
using Microsoft.MixedReality.Toolkit.Internal.Definitions;

namespace Microsoft.MixedReality.Toolkit.Internal.Interfaces.InputSystem
{
    /// <summary>
    /// Interface for an input source.
    /// An input source can be any user defined action that generally comes from a physical controller, sensor, or device.
    /// </summary>
    public interface IMixedRealityInputSource : IEqualityComparer
    {
        /// <summary>
        /// The ID assigned to the Controller
        /// </summary>
        uint SourceId { get; }

        /// <summary>
        /// The Name assigned to the Controller
        /// </summary>
        string SourceName { get; }

        /// <summary>
        /// The list of Pointers attached to the Controller
        /// </summary>
        IMixedRealityPointer[] Pointers { get; }

        /// <summary>
        /// List the available capabilities of the controller for a simpler lookup.
        /// </summary>
        InputType[] Capabilities { get; }

        /// <summary>
        /// Returns whether the input source supports the specified input types.
        /// </summary>
        /// <param name="inputInfo">Input types that we want to get information about.</param>
        bool SupportsInputCapability(InputType[] inputInfo);
    }
}
