// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.


namespace Microsoft.MixedReality.Toolkit
{
    /// <summary>
    /// The base extension service implements <see cref="IMixedRealityExtensionService"/> and provides default properties for all extension services.
    /// </summary>
    /// <remarks>
    /// Empty, but reserved for future use, in case additional <see cref="IMixedRealityExtensionService"/> properties or methods are assigned.
    /// </remarks>
    public abstract class BaseExtensionService : BaseService, IMixedRealityExtensionService
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="registrar">The <see cref="IMixedRealityServiceRegistrar"/> instance that loaded the service.</param>
        /// <param name="name">The friendly name of the service.</param>
        /// <param name="priority">The registration priority of the service.</param>
        /// <param name="profile">The configuration profile for the service.</param>
        [System.Obsolete("This constructor is obsolete (registrar parameter is no longer required) and will be removed in a future version of the Microsoft Mixed Reality Toolkit.")]
        protected BaseExtensionService(
            IMixedRealityServiceRegistrar registrar,
            string name = null,
            uint priority = DefaultPriority,
            BaseMixedRealityProfile profile = null) : this(name, priority, profile)
        {
            Registrar = registrar;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">The friendly name of the service.</param>
        /// <param name="priority">The registration priority of the service.</param>
        /// <param name="profile">The configuration profile for the service.</param>
        protected BaseExtensionService(
            string name = null,
            uint priority = DefaultPriority,
            BaseMixedRealityProfile profile = null) : base()
        {
            Name = name;
            Priority = priority;
            ConfigurationProfile = profile;
        }

        /// <summary>
        /// The service registrar instance that registered this service.
        /// </summary>
        [System.Obsolete("The Registrar property is obsolete and will be removed in a future version of the Microsoft Mixed Reality Toolkit")]
        protected IMixedRealityServiceRegistrar Registrar { get; set; } = null;
    }
}
