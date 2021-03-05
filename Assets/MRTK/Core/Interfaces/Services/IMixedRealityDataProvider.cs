// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.MixedReality.Toolkit
{
    /// <summary>
    /// Interface that defines a Mixed Reality data provider. Data providers are the components
    /// that supply services with required information (ex: input controller state).
    /// </summary>
    public interface IMixedRealityDataProvider : IMixedRealitySystem
    {
        // todo: There should be _something_ that differentiates a data provider from a system beyond the interface name
    }
}