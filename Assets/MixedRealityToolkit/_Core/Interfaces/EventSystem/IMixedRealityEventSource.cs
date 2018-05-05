// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections;

namespace Microsoft.MixedReality.Toolkit.Internal.Interfaces.Events
{
    /// <summary>
    /// Interface to implement an event source.
    /// </summary>
    public interface IMixedRealityEventSource : IEqualityComparer
    {
        uint SourceId { get; }

        string SourceName { get; }
    }
}