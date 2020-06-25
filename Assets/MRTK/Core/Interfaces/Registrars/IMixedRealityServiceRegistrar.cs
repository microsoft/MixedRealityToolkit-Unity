// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Collections.Generic;

namespace Microsoft.MixedReality.Toolkit
{
    /// <summary>
    /// Interface for Mixed Reality Toolkit service registration.
    /// </summary>
    public interface IMixedRealityServiceRegistrar
    {
        #region IMixedRealityService registration

        /// <summary>
        /// Registers a service of the specified type.
        /// </summary>
        /// <typeparam name="T">The interface type of the service to be registered (ex: IMixedRealityBoundarySystem).</typeparam>
        /// <param name="serviceInstance">An instance of the service to be registered.</param>
        bool RegisterService<T>(T serviceInstance) where T : IMixedRealityService;

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
        /// Unregisters a service of the specified type.
        /// </summary>
        /// <typeparam name="T">The interface type of the service to be unregistered (ex: IMixedRealityBoundarySystem).</typeparam>
        /// <param name="name">The name of the service to unregister.</param>
        /// <returns>True if the service was successfully unregistered, false otherwise.</returns>
        /// <remarks>If the name argument is not specified, the first instance will be unregistered</remarks>
        bool UnregisterService<T>(string name = null) where T : IMixedRealityService;

        /// <summary>
        /// Unregisters a service.
        /// </summary>
        /// <typeparam name="T">The interface type of the service to be unregistered (ex: IMixedRealityBoundarySystem).</typeparam>
        /// <param name="service">The specific service instance to unregister.</param>
        /// <returns>True if the service was successfully unregistered, false otherwise.</returns>
        bool UnregisterService<T>(T serviceInstance) where T : IMixedRealityService;

        /// <summary>
        /// Checks to see if a service of the specified type has been registered.
        /// </summary>
        /// <typeparam name="T">The interface type of the service (ex: IMixedRealityBoundarySystem).</typeparam>
        /// <param name="name">The name of the service.</param>
        /// <returns>True if the service is registered, false otherwise.</returns>
        bool IsServiceRegistered<T>(string name = null) where T : IMixedRealityService;

        /// <summary>
        /// Gets the instance of the registered service.
        /// </summary>
        /// <typeparam name="T">The interface type of the service (ex: IMixedRealityBoundarySystem).</typeparam>
        /// <param name="name">The name of the service.</param>
        /// <param name="showLogs">Indicates whether or not diagnostic logging should be performed in case of an error</param>
        /// <returns>The registered service instance as the requested type.</returns>
        T GetService<T>(string name = null, bool showLogs = true) where T : IMixedRealityService;

        /// <summary>
        /// Gets the collection of the registered service instances matching the requested type.
        /// </summary>
        /// <typeparam name="T">The interface type of the service (ex: IMixedRealityBoundarySystem).</typeparam>
        /// <param name="name">Friendly name of the service.</param>
        /// <returns>Read-only collection of the service instances, as the requested type.</returns>
        IReadOnlyList<T> GetServices<T>(string name = null) where T : IMixedRealityService;

        #endregion IMixedRealityServce registration
    }
}
