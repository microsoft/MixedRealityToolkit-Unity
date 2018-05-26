// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Definitions;
using System.Collections;

namespace Microsoft.MixedReality.Toolkit.Internal.Interfaces.InputSystem
{
    /// <summary>
    /// Interface for an input source.
    /// An input source is the origin of user input and generally comes from a physical controller, sensor, or other hardware device.
    /// </summary>
    public interface IMixedRealityInputSource : IEqualityComparer
    {
        /// <summary>
        /// The Unique Source Id of this Input Source.
        /// </summary>
        uint SourceId { get; }

        /// <summary>
        /// The Name of this Input Source.
        /// </summary>
        string SourceName { get; }

        /// <summary>
        /// Array of pointers associated with this input source.
        /// </summary>
        IMixedRealityPointer[] Pointers { get; }

        /// <summary>
        /// Details the list of available buttons / interactions available from the Input Source.
        /// </summary>
        InteractionDefinition[] Interactions { get; }

        /// <summary>
        /// Returns whether the Input Source supports the specified input type.
        /// </summary>
        /// <param name="inputInfo">Input types that we want to get information about.</param>
        /// <returns>Returns true if the Input Source supports the provided input type.</returns>
        bool SupportsCapability(Definitions.Devices.DeviceInputType inputInfo);
    }
}
