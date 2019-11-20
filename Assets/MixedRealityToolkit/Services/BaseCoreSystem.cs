// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


namespace Microsoft.MixedReality.Toolkit
{
    public abstract class BaseCoreSystem : BaseEventSystem
    {
        public BaseCoreSystem(
            IMixedRealityServiceRegistrar registrar,
            BaseMixedRealityProfile profile = null) : base()
        {
            Registrar = registrar;
            ConfigurationProfile = profile;
            Priority = 5; // Core systems have a higher default priority than other services
        }

        /// <summary>
        /// The service registrar instance that registered this service.
        /// </summary>
        protected IMixedRealityServiceRegistrar Registrar { get; set; } = null;
    }
}
