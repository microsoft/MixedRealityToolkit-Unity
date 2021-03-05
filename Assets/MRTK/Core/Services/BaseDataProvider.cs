// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit
{
    [System.Obsolete("Add a <T> of type IMixedRealityService, which defines the service type this data provider is valid for.")]
    public abstract class BaseDataProvider : BaseDataProvider<IMixedRealitySystem>
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="system">The <see cref="IMixedRealitySystem"/> to which the provider is providing data.</param>
        /// <param name="name">The friendly name of the data provider.</param>
        /// <param name="priority">The registration priority of the data provider.</param>
        /// <param name="profile">The configuration profile for the data provider.</param>
        protected BaseDataProvider(
            IMixedRealitySystem system,
            string name = null,
            uint priority = DefaultPriority,
            BaseMixedRealityProfile profile = null) : base(system, name, priority, profile)
        {
        }
    }

    /// <summary>
    /// The base data provider implements <see cref="IMixedRealityDataProvider"/> and provides default properties for all data providers.
    /// </summary>
    public abstract class BaseDataProvider<T> : BaseService, IMixedRealityDataProvider where T : IMixedRealitySystem
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="system">The <see cref="IMixedRealitySystem"/> to which the provider is providing data.</param>
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
        /// The service instance to which this provider is providing data.
        /// </summary>
        protected T Service { get; set; } = default(T);
    }
}
