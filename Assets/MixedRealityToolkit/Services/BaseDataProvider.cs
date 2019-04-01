// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


namespace Microsoft.MixedReality.Toolkit
{
    /// <summary>
    /// The base data provider implements <see cref="IMixedRealityDataProvider"/> and provides default properties for all data providers.
    /// </summary>
    /// <remarks>
    /// Empty, but reserved for future use, in case additional <see cref="IMixedRealityDataProvider"/> properties or methods are assigned.
    /// </remarks>
    public abstract class BaseDataProvider : BaseService, IMixedRealityDataProvider
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="registrar">The <see cref="IMixedRealityServiceRegistrar"/> instance that loaded the data provider.</param>
        /// <param name="service">The <see cref="IMixedRealityService"/> to which the provider is providing data.</param>
        /// <param name="name">The friendly name of the data provider.</param>
        /// <param name="priority">The registration priority of the data provider.</param>
        /// <param name="profile">The configuration profile for the data provider.</param>
        public BaseDataProvider(
            IMixedRealityServiceRegistrar registrar,
            IMixedRealityService service,
            string name = null, 
            uint priority = DefaultPriority, 
            BaseMixedRealityProfile profile = null) : base()
        {
            Registrar = registrar;
            Service = service;
            Name = name;
            Priority = priority;
            ConfigurationProfile = profile;
        }

        /// <summary>
        /// The service registrar instance that registered this service.
        /// </summary>
        protected IMixedRealityServiceRegistrar Registrar { get; set; } = null;

        /// <summary>
        /// The service instance to which this provider is providing data.
        /// </summary>
        protected IMixedRealityService Service { get; set; } = null;

        /// <summary>
        /// Configuration Profile
        /// </summary>
        protected BaseMixedRealityProfile ConfigurationProfile { get; set; } = null;
    }
}