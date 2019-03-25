// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Definitions;
using Microsoft.MixedReality.Toolkit.Core.Interfaces;

namespace Microsoft.MixedReality.Toolkit.Core.Services
{
    /// <summary>
    /// The base data provider implements <see cref="Interfaces.IMixedRealityDataProvider"/> and provides default properties for all data providers.
    /// </summary>
    /// <remarks>
    /// Empty, but reserved for future use, in case additional <see cref="Interfaces.IMixedRealityDataProvider"/> properties or methods are assigned.
    /// </remarks>
    public abstract class BaseDataProvider : BaseExtensionService, Interfaces.IMixedRealityDataProvider
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="registrar">The <see cref="Interfaces.IMixedRealityServiceRegistrar"/> instance that loaded the data provider.</param>
        /// <param name="service">The <see cref="Interfaces.IMixedRealityService"/> to which the provider is providing data.</param>
        /// <param name="name">The friendly name of the data provider.</param>
        /// <param name="priority">The registration priority of the data provider.</param>
        /// <param name="profile">The configuration profile for the data provider.</param>
        public BaseDataProvider(
            IMixedRealityServiceRegistrar registrar,
            IMixedRealityService service,
            string name = null, 
            uint priority = DefaultPriority, 
            BaseMixedRealityProfile profile = null) : base(registrar, name, priority, profile)
        {
            Service = service;
        }

        /// <summary>
        /// The service instance to which this provider is providing data.
        /// </summary>
        protected IMixedRealityService Service { get; set; } = null;
    }
}