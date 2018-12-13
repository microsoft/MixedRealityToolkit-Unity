// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Interfaces;

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
        /// Constructor.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="priority"></param>
        public BaseExtensionService(string name, uint priority) : base(name, priority) { }
    }
}