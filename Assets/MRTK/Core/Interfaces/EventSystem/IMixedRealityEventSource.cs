// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections;

namespace Microsoft.MixedReality.Toolkit
{
    /// <summary>
    /// Interface to implement an event source.
    /// </summary>
    public interface IMixedRealityEventSource : IEqualityComparer
    {
        /// <summary>
        /// The unique source id of this event source.
        /// </summary>
        uint SourceId { get; }

        /// <summary>
        /// The name of this event source.
        /// </summary>
        string SourceName { get; }
    }
}