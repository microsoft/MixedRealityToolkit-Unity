// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Definitions.Utilities;
using System;
using System.Collections.Generic;

namespace Microsoft.MixedReality.Toolkit.Core.Interfaces
{
    /// <summary>
    /// Interface for Mixed Reality Toolkit data provider registration.
    /// </summary>
    public interface IMixedRealityDataProviderRegistrar
    {
        /// <summary>
        /// Registers a data provider of the specified type.
        /// </summary>
        /// <typeparam name="T">The interface type of the data provider to be registered.
        /// <param name="dataProviderInstance">Instance of the data provider class.</param>
        /// <returns>True if the data provider was successfully registered, false otherwise.</returns>
        bool RegisterDataProvider<T>(
           IMixedRealityDataProvider dataProviderInstance) where T : IMixedRealityDataProvider;

        /// <summary>
        /// Registers a data provider of the specified type.
        /// </summary>
        /// <typeparam name="T">The interface type of the data provider to be registered.
        /// <returns>True if the data provider was successfully registered, false otherwise.</returns>
        bool RegisterDataProvider<T>(
            Type concreteType,
            SupportedPlatforms supportedPlatforms = (SupportedPlatforms)(-1),
            params object[] args) where T : IMixedRealityDataProvider;

        /// <summary>
        /// Unregisters a data provider.
        /// </summary>
        /// <param name="name">The name of the data provider to unregister.</param>
        /// <returns>True if the data provider was successfully unregistered, false otherwise.</returns>
        bool UnregisterDataProvider(string name); // todo: namespace?

        /// <summary>
        /// Unregisters a data provider of the specified type.
        /// </summary>
        /// <typeparam name="T">The interface type of the data provider to be unregistered.
        /// <param name="name">The name of the data provider to unregister.</param>
        /// <returns>True if the data provider was successfully unregistered, false otherwise.</returns>
        /// <remarks>If the name argument is not specified, the first instance will be unregistered</remarks>
        bool UnregisterDataProvider<T>(string name = null) where T : IMixedRealityDataProvider; // todo: namespace?

        /// <summary>
        /// Unregisters a data provider.
        /// </summary>
        /// <param name="service">The specific data provider instance to unregister.</param>
        /// <returns>True if the data provider was successfully unregistered, false otherwise.</returns>
        bool UnregisterDataProviderService(IMixedRealityDataProvider dataProviderInstance);

        /// <summary>
        /// Unregisters all data providers.
        /// </summary>
        bool UnregisterDataProviders();

        /// <summary>
        /// Unregisters all data providers.
        /// </summary>
        /// <typeparam name="T">The interface type of the data providers to be unregistered.
        bool UnregisterDataProviders<T>() where T : IMixedRealityDataProvider;

        /// <summary>
        /// Gets the instance of the registered data provider.
        /// </summary>
        /// <param name="name">The name of the data provider.</param>
        /// <returns>The registered data provider instance, as IMixedRealityDataProvider.</returns>
        IMixedRealityDataProvider GetDataProvider(string name); // todo: namespace?

        /// <summary>
        /// Gets the instance of the registered data provider of the specified type.
        /// </summary>
        /// <typeparam name="T">The interface type of the data provider.
        /// <param name="name">The name of the data provider.</param>
        /// <returns>The registered data provider instance as the requested type.</returns>
        T GetDataProvider<T>(string name = null) where T : IMixedRealityDataProvider; // todo: namespace?

        /// <summary>
        /// Gets the collection of the registered data provider instances matching the requested type.
        /// </summary>
        /// <returns>Read-only collection of the data provider instances, as IMixedRealitydata provider.</returns>
        IReadOnlyList<IMixedRealityDataProvider> GetDataProviders();

        /// <summary>
        /// Gets the collection of the registered data provider instances matching the requested type.
        /// </summary>
        /// <typeparam name="T">The interface type of the data provider.
        /// <returns>Read-only collection of the data provider instances, as tye requested type.</returns>
        IReadOnlyList<T> GetDataProviders<T>() where T : IMixedRealityDataProvider;
    }
}