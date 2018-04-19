// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections;

namespace Microsoft.MixedReality.Toolkit.Internal.Interfaces
{
    public interface IMixedRealityEventSource : IEqualityComparer
    {
        uint SourceId { get; }

        string SourceName { get; }
    }
}