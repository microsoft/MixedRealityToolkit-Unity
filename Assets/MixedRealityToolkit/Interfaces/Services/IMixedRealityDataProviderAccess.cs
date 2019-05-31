// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;

namespace Microsoft.MixedReality.Toolkit
{
    /// <summary>
    /// Allows systems to provide access to their managed data providers.
    /// </summary>
    public interface IMixedRealityDataProviderAccess
    {
        /// <summary>
        /// Gets the collection of registered data providers.
        /// </summary>
        /// <returns>
        /// Read only copy of the list of registered data providers.
        /// </returns>
        IReadOnlyList<IMixedRealityDataProvider> GetDataProviders();

        /// <summary>
        /// Get the collection of registered observers of the specified type.
        /// </summary>
        /// <typeparam name="T">The desired data provider type</typeparam>
        /// <returns>
        /// Read-only copy of the list of registered data providers that implement the specified type.
        /// </returns>
        IReadOnlyList<T> GetDataProviders<T>() where T : IMixedRealityDataProvider;

        /// <summary>
        /// Get the data provider that is registered under the specified name.
        /// </summary>
        /// <param name="name">The friendly name of the data provider.</param>
        /// <returns>
        /// The requested data provider, or null if one cannot be found.
        /// </returns>
        /// <remarks>
        /// If more than one data provider is registered under the specified name, the first will be returned.
        /// </remarks>
        IMixedRealityDataProvider GetDataProvider(string name);

        /// <summary>
        /// Get the data provider that is registered under the specified name (optional) and matching the specified type.
        /// </summary>
        /// <typeparam name="T">The desired data provider type.</typeparam>
        /// <param name="name">The friendly name of the data provider.</param>
        /// <returns>
        /// The requested data provider, or null if one cannot be found.
        /// </returns>
        /// <remarks>
        /// If more than one data provider is registered under the specified name, the first will be returned.
        /// </remarks>
        T GetDataProvider<T>(string name = null) where T : IMixedRealityDataProvider;
    }
}
