// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Interface for an input source.
    /// An input source is the origin of user input and generally comes from a physical controller, sensor, or other hardware device.
    /// </summary>
    public interface IMixedRealityInputSource : IMixedRealityEventSource
    {
        /// <summary>
        /// Array of pointers associated with this input source.
        /// </summary>
        IMixedRealityPointer[] Pointers { get; }

        /// <summary>
        /// The type of input source this object represents.
        /// </summary>
        InputSourceType SourceType { get; }
    }
}
