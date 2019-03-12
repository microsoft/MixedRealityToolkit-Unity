// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Definitions.Utilities;
using System;
using System.Collections.Generic;

namespace Microsoft.MixedReality.Toolkit.Core.Interfaces
{
    /// <summary>
    /// Interface for Mixed Reality Toolkit extension service registration.
    /// </summary>
    public interface IMixedRealityExtensionServiceRegistrar
    {
        /// <summary>
        /// Registers an extension service of the specified type.
        /// </summary>
        /// <typeparam name="T">The interface type of the service to be registered.
        /// <param name="serviceInstance">Instance of the service class.</param>
        /// <returns>True if the service was successfully registered, false otherwise.</returns>
        bool RegisterExtensionService<T>(
           IMixedRealityExtensionService serviceInstance) where T : IMixedRealityExtensionService;

        /// <summary>
        /// Registers an extension service of the specified type.
        /// </summary>
        /// <typeparam name="T">The interface type of the service to be registered.
        /// <returns>True if the service was successfully registered, false otherwise.</returns>
        bool RegisterExtensionService<T>(
            Type concreteType,
            SupportedPlatforms supportedPlatforms = (SupportedPlatforms)(-1),
            params object[] args) where T : IMixedRealityExtensionService;

        /// <summary>
        /// Unregisters an extension service.
        /// </summary>
        /// <param name="name">The name of the service to unregister.</param>
        /// <returns>True if the service was successfully unregistered, false otherwise.</returns>
        bool UnregisterExtensionService(string name); // todo: namespace?

        /// <summary>
        /// Unregisters a service of the specified type.
        /// </summary>
        /// <typeparam name="T">The interface type of the service to be unregistered.
        /// <param name="name">The name of the service to unregister.</param>
        /// <returns>True if the service was successfully unregistered, false otherwise.</returns>
        /// <remarks>If the name argument is not specified, the first instance will be unregistered</remarks>
        bool UnregisterExtensionService<T>(string name = null) where T : IMixedRealityExtensionService; // todo: namespace?

        /// <summary>
        /// Unregisters an extension service.
        /// </summary>
        /// <param name="service">The specific service instance to unregister.</param>
        /// <returns>True if the service was successfully unregistered, false otherwise.</returns>
        bool UnregisterExtensionService(IMixedRealityExtensionService serviceInstance);

        /// <summary>
        /// Unregisters all extension services.
        /// </summary>
        bool UnregisterExtensionServices();

        /// <summary>
        /// Unregisters all extension services of the specified type.
        /// </summary>
        /// <typeparam name="T">The interface type of the services to be unregistered.
        bool UnregisterExtensionServices<T>() where T : IMixedRealityExtensionService;

        /// <summary>
        /// Checks to see if an extension service has been registered.
        /// </summary>
        /// <param name="name">The name of the service.</param>
        /// <returns>True if the service is registered, false otherwise.</returns>
        bool IsExtensionServiceRegistered(string name);  // todo: namespace?

        /// <summary>
        /// Checks to see if an extension service of the specified type has been registered.
        /// </summary>
        /// <typeparam name="T">The interface type of the service (ex: IMixedRealityBoundarySystem).
        /// <param name="name">The name of the service.</param>
        /// <returns>True if the service is registered, false otherwise.</returns>
        bool IsServiceRegistered<T>(string name = null) where T : IMixedRealityService;  // todo: namespace?

        /// <summary>
        /// Gets the instance of the registered extension service.
        /// </summary>
        /// <param name="name">The name of the service.</param>
        /// <returns>The registered service instance, as IMixedRealityExtensionService.</returns>
        IMixedRealityExtensionService GetExtensionService(string name); // todo: namespace?

        /// <summary>
        /// Gets the instance of the registered extension service.
        /// </summary>
        /// <typeparam name="T">The interface type of the service.
        /// <param name="name">The name of the service.</param>
        /// <returns>The registered service instance as the requested type.</returns>
        T GetExtensionService<T>(string name = null) where T : IMixedRealityExtensionService; // todo: namespace?

        /// <summary>
        /// Gets the collection of the registered extensionservice instances matching the requested type.
        /// </summary>
        /// <returns>Read-only collection of the service instances, as IMixedRealityService.</returns>
        IReadOnlyList<IMixedRealityExtensionService> GetExtensionServices();

        /// <summary>
        /// Gets the collection of the registered extension service instances matching the requested type.
        /// </summary>
        /// <typeparam name="T">The interface type of the service.
        /// <returns>Read-only collection of the service instances, as tye requested type.</returns>
        IReadOnlyList<T> GetExtensionServices<T>() where T : IMixedRealityExtensionService;
    }
}