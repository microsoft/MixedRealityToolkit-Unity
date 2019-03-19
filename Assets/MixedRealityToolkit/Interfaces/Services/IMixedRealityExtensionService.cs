// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Microsoft.MixedReality.Toolkit.Core.Interfaces
{
    /// <summary>
    /// Generic interface for all optional Mixed Reality systems, components, or features that can be added to the <see cref="Definitions.MixedRealityServiceConfiguration"/>
    /// </summary>
    public interface IMixedRealityExtensionService : IMixedRealityService
    {
        // Empty for now, but it is used to filter out the valid class types in the inspector dropdown.
    }
}