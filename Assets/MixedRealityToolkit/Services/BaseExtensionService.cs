// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Definitions;
using Microsoft.MixedReality.Toolkit.Core.Interfaces;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Services
{
    /// <summary>
    /// The base extension service implements <see cref="Interfaces.IMixedRealityExtensionService"/> and provides default properties for all extension services.
    /// </summary>
    /// <remarks>
    /// Empty, but reserved for future use, in case additional <see cref="IMixedRealityExtensionService"/> properties or methods are assigned.
    /// </remarks>
    public abstract class BaseExtensionService : BaseServiceWithConstructor, Interfaces.IMixedRealityExtensionService
    {
        /// <summary>
        /// Configuration Profile
        /// </summary>
        protected BaseMixedRealityProfile ConfigurationProfile { get; set; } = null;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">Friendly name of the service.</param>
        /// <param name="priority">Service priority. Used to determine order of instantiation.</param>
        /// <param name="profile">The service's configuration profile.</param>
        public BaseExtensionService(string name, uint priority, BaseMixedRealityProfile profile) : base(name, priority)
        {
            ConfigurationProfile = profile;
        }
    }
}
