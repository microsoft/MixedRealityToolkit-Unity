// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.MixedReality.Toolkit
{
    /// <summary>
    /// Generic interface for all optional Mixed Reality systems, components, or features that can be added to the <see cref="MixedRealityServiceConfiguration"/>
    /// </summary>
    public interface IMixedRealityExtensionService : IMixedRealityService
    {
        // Empty for now, but it is used to filter out the valid class types in the inspector dropdown.
    }
}
