// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.


namespace Microsoft.MixedReality.Toolkit
{
    public abstract class BaseCoreSystem : BaseEventSystem
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="registrar">The <see cref="IMixedRealityServiceRegistrar"/> instance that loaded the service.</param>
        /// <param name="profile">The configuration profile for the service.</param>
        [System.Obsolete("This constructor is obsolete (registrar parameter is no longer required) and will be removed in a future version of the Microsoft Mixed Reality Toolkit.")]
        protected BaseCoreSystem(
            IMixedRealityServiceRegistrar registrar,
            BaseMixedRealityProfile profile = null) : this(profile)
        {
            Registrar = registrar;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="profile">The configuration profile for the service.</param>
        protected BaseCoreSystem(
            BaseMixedRealityProfile profile = null) : base()
        {
            ConfigurationProfile = profile;
            Priority = 5; // Core systems have a higher default priority than other services
        }

        /// <summary>
        /// The service registrar instance that registered this service.
        /// </summary>
        [System.Obsolete("The Registrar property is obsolete and will be removed in a future version of the Microsoft Mixed Reality Toolkit")]
        protected IMixedRealityServiceRegistrar Registrar { get; set; } = null;
    }
}
