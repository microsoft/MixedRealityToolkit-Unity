// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

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