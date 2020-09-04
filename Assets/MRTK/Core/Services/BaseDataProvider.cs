// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit
{
    [System.Obsolete("Add a <T> of type IMixedRealityService, which defines the service type this data provider is valid for.")]
    public abstract class BaseDataProvider : BaseDataProvider<IMixedRealityService>
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="registrar">The <see cref="IMixedRealityServiceRegistrar"/> instance that loaded the data provider.</param>
        /// <param name="service">The <see cref="IMixedRealityService"/> to which the provider is providing data.</param>
        /// <param name="name">The friendly name of the data provider.</param>
        /// <param name="priority">The registration priority of the data provider.</param>
        /// <param name="profile">The configuration profile for the data provider.</param>
        [System.Obsolete("This constructor is obsolete (registrar parameter is no longer required) and will be removed in a future version of the Microsoft Mixed Reality Toolkit.")]
        protected BaseDataProvider(
            IMixedRealityServiceRegistrar registrar,
            IMixedRealityService service,
            string name = null,
            uint priority = DefaultPriority,
            BaseMixedRealityProfile profile = null) : this(service, name, priority, profile)
        {
            Registrar = registrar;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="service">The <see cref="IMixedRealityService"/> to which the provider is providing data.</param>
        /// <param name="name">The friendly name of the data provider.</param>
        /// <param name="priority">The registration priority of the data provider.</param>
        /// <param name="profile">The configuration profile for the data provider.</param>
        protected BaseDataProvider(
            IMixedRealityService service,
            string name = null,
            uint priority = DefaultPriority,
            BaseMixedRealityProfile profile = null) : base(service, name, priority, profile)
        {
        }
    }

    /// <summary>
    /// The base data provider implements <see cref="IMixedRealityDataProvider"/> and provides default properties for all data providers.
    /// </summary>
    public abstract class BaseDataProvider<T> : BaseService, IMixedRealityDataProvider where T : IMixedRealityService
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="service">The <see cref="IMixedRealityService"/> to which the provider is providing data.</param>
        /// <param name="name">The friendly name of the data provider.</param>
        /// <param name="priority">The registration priority of the data provider.</param>
        /// <param name="profile">The configuration profile for the data provider.</param>
        protected BaseDataProvider(
            T service,
            string name = null,
            uint priority = DefaultPriority,
            BaseMixedRealityProfile profile = null) : base()
        {
            if (service == null)
            {
                Debug.LogError($"{name} requires a valid service instance.");
            }

            Service = service;
            Name = name;
            Priority = priority;
            ConfigurationProfile = profile;
        }

        /// <summary>
        /// The service registrar instance that registered this service.
        /// </summary>
        [System.Obsolete("The Registrar property is obsolete and will be removed in a future version of the Microsoft Mixed Reality Toolkit")]
        protected IMixedRealityServiceRegistrar Registrar { get; set; } = null;

        /// <summary>
        /// The service instance to which this provider is providing data.
        /// </summary>
        protected T Service { get; set; } = default(T);
    }
}
