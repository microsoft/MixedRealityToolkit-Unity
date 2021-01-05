// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections.Generic;

namespace Microsoft.MixedReality.Toolkit.Input
{
    public interface IMixedRealityInputSourceDefinition
    {
        IReadOnlyList<MixedRealityInteractionMapping> GetDefaultInteractions(Handedness handedness = Handedness.None);
    }
}
