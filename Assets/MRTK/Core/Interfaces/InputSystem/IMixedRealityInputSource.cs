// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

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
