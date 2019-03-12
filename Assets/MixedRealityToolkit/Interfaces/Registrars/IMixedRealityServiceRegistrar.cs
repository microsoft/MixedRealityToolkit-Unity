// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Definitions.Utilities;
using System;
using System.Collections.Generic;

namespace Microsoft.MixedReality.Toolkit.Core.Interfaces
{
    /// <summary>
    /// Interface for Mixed Reality Toolkit service registration.
    /// </summary>
    public interface IMixedRealityServiceRegistrar
    {
        /// <summary>
        /// Registers a service of the specified type.
        /// </summary>
        /// <typeparam name="T">The interface type of the service to be registered (ex: IMixedRealityBoundarySystem).</typeparam>
        /// <param name="serviceInstance">Instance of the service class.</param>
        /// <returns>True if the service was successfully registered, false otherwise.</returns>
        bool RegisterService<T>(
            T serviceInstance) where T : IMixedRealityService;

        /// <summary>
        /// Registers a service of the specified type.
        /// </summary>
        /// <typeparam name="T">The interface type of the service to be registered (ex: IMixedRealityBoundarySystem).</typeparam>
        /// <param name="concreteType">The concrete type to instantiate.</param>
        /// <param name="supportedPlatforms">The platform(s) on which the service is supported.</param>
        /// <param name="args">Optional arguments used when instantiating the concrete type.</param>
        /// <returns>True if the service was successfully registered, false otherwise.</returns>
        bool RegisterService<T>(
            Type concreteType,
            SupportedPlatforms supportedPlatforms = (SupportedPlatforms)(-1),
            params object[] args) where T : IMixedRealityService;

        /// <summary>
        /// Unregisters a service.
        /// </summary>
        /// <param name="name">The name of the service to unregister.</param>
        /// <returns>True if the service was successfully unregistered, false otherwise.</returns>
        bool UnregisterService(string name); // todo: namespace?

        /// <summary>
        /// Unregisters a service of the specified type.
        /// </summary>
        /// <typeparam name="T">The interface type of the service to be unregistered (ex: IMixedRealityBoundarySystem).
        /// <param name="name">The name of the service to unregister.</param>
        /// <returns>True if the service was successfully unregistered, false otherwise.</returns>
        /// <remarks>If the name argument is not especified, the first instance will be unregistered</remarks>
        bool UnregisterService<T>(string name = null) where T : IMixedRealityService; // todo: namespace?

        /// <summary>
        /// Unregisters a service.
        /// </summary>
        /// <param name="service">The specific service instance to unregister.</param>
        /// <returns>True if the service was successfully unregistered, false otherwise.</returns>
        bool UnregisterService(IMixedRealityService serviceInstance);

        /// <summary>
        /// Unregisters all services.
        /// </summary>
        void UnregisterServices();

        /// <summary>
        /// Unregisters all services of the specified type.
        /// </summary>
        /// <typeparam name="T">The interface type of the services to be unregistered (ex: IMixedRealityBoundarySystem).
        void UnregisterServices<T>() where T : IMixedRealityService;

        /// <summary>
        /// Checks to see if a service has been registered.
        /// </summary>
        /// <param name="name">The name of the service.</param>
        /// <returns>True if the service is registered, false otherwise.</returns>
        bool IsServiceRegistered(string name);  // todo: namespace?

        /// <summary>
        /// Checks to see if a service of the specified type has been registered.
        /// </summary>
        /// <typeparam name="T">The interface type of the service (ex: IMixedRealityBoundarySystem).
        /// <param name="name">The name of the service.</param>
        /// <returns>True if the service is registered, false otherwise.</returns>
        bool IsServiceRegistered<T>(string name = null) where T : IMixedRealityService;  // todo: namespace?

        /// <summary>
        /// Gets the instance of the registered service.
        /// </summary>
        /// <param name="name">The name of the service.</param>
        /// <returns>The registered service instance, as IMixedRealityService.</returns>
        IMixedRealityService GetService(string name);  // todo: namespace?

        /// <summary>
        /// Gets the instance of the registered service.
        /// </summary>
        /// <typeparam name="T">The interface type of the service (ex: IMixedRealityBoundarySystem).
        /// <param name="name">The name of the service.</param>
        /// <returns>The registered service instance as the requested type.</returns>
        T GetService<T>(string name = null) where T : IMixedRealityService;  // todo: namespace?

        /// <summary>
        /// Gets the collection of the registered service instances matching the requested type.
        /// </summary>
        /// <returns>Read-only collection of the service instances, as IMixedRealityService.</returns>
        IReadOnlyList<IMixedRealityService> GetServices();

        /// <summary>
        /// Gets the collection of the registered service instances matching the requested type.
        /// </summary>
        /// <typeparam name="T">The interface type of the service (ex: IMixedRealityBoundarySystem).
        /// <returns>Read-only collection of the service instances, as tye requested type.</returns>
        IReadOnlyList<T> GetServices<T>() where T : IMixedRealityService;
    }
}
