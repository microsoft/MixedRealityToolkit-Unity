// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections.Generic;

namespace Microsoft.MixedReality.Toolkit.Input
{
    public interface IMixedRealityInputSourceDefinition
    {
        /// <summary>
        /// Provides the default interactions for this source type with a specific handedness.
        /// </summary>
        /// <param name="handedness">The handedness the mappings should be provided for.</param>
        /// <returns>The default interactions for this source with a specific handedness.</returns>
        IReadOnlyList<MixedRealityInputActionMapping> GetDefaultMappings(Handedness handedness);
    }
}
