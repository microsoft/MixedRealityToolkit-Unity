// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Definitions;
using System.Collections;

namespace Microsoft.MixedReality.Toolkit.Internal.Interfaces.InputSystem
{
    /// <summary>
    /// Interface for a Mixed Reality Toolkit input source.
    /// An input source can be any user defined input that generally comes from a physical controller, sensor, or device.
    /// </summary>
    public interface IMixedRealityInputSource : IEqualityComparer
    {
        /// <summary>
        /// The ID assigned to the Input Source
        /// </summary>
        uint SourceId { get; }

        /// <summary>
        /// The Name assigned to the Input Source
        /// </summary>
        string SourceName { get; }

        /// <summary>
        /// Outputs the current state of the Input Source, whether it is tracked or not. As defined by the SDK / Unity.
        /// </summary>
        InputSourceState InputSourceState { get; }

        /// <summary>
        /// The designated hand that the Input Source is managing, as defined by the SDK / Unity.
        /// </summary>
        Handedness Handedness { get; }

        /// <summary>
        /// The list of Pointers attached to the Input Source
        /// </summary>
        IMixedRealityPointer[] Pointers { get; }

        /// <summary>
        /// List the available capabilities of the Input Source for a simpler lookup.
        /// </summary>
        InputType[] Capabilities { get; }

        /// <summary>
        /// Details the list of available buttons / interactions available from the Input Source.
        /// </summary>
        InteractionDefinition[] Interactions { get; }

        /// <summary>
        /// Returns whether the Input Source supports the specified input types.
        /// </summary>
        /// <param name="inputInfo">Input types that we want to get information about.</param>
        bool SupportsInputCapability(InputType[] inputInfo);
    }
}
