// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Definitions;
using System.Collections;
using System.Collections.Generic;

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
        /// The list of Pointers attached to the Input Source
        /// </summary>
        IMixedRealityPointer[] Pointers { get; }

        /// <summary>
        /// Details the list of available buttons / interactions available from the Input Source.
        /// </summary>
        Dictionary<InputType,InteractionDefinition> Interactions { get; }

        /// <summary>
        /// Returns the <see cref="Capabilities"/> that this Input Source can provide.
        /// </summary>
        InputType[] Capabilities { get; }

        // TODO - Need to generalize the Input Type?
        /// <summary>
        /// Setup function to define controller
        /// </summary>
        /// <param name="state">SDK data to inform construction</param>
        void SetupInputSource<T>(T state);

        // TODO - Need to generalize the Input Type?
        /// <summary>
        /// Update function to update controller
        /// </summary>
        /// <param name="state">SDK data to inform update</param>
        void UpdateInputSource<T>(T state);

        /// <summary>
        /// Returns whether the Input Source supports the specified input types.
        /// </summary>
        /// <param name="inputInfo">Input types that we want to get information about.</param>
        bool SupportsCapabilities(InputType[] inputInfo);

        /// <summary>
        /// Returns whether the Input Source supports the specified input types.
        /// </summary>
        /// <param name="inputInfo">Input types that we want to get information about.</param>
        bool SupportsCapability(InputType inputInfo);

        /// <summary>
        /// Register pointers against the controller
        /// </summary>
        /// <param name="pointers">Array of pointers to attach</param>
        void RegisterPointers(IMixedRealityPointer[] pointers);
    }
}
