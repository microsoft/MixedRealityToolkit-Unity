// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Microsoft.MixedReality.Toolkit.Core.Services
{
    /// <summary>
    /// The base extension service implements <see cref="Interfaces.IMixedRealityExtensionService"/> and provides default properties for all extension services.
    /// </summary>
    public class BaseExtensionService : BaseService, Interfaces.IMixedRealityExtensionService
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="priority"></param>
        public BaseExtensionService(string name, uint priority)
        {
            this.name = name;
            this.priority = priority;
        }

        private readonly string name;

        /// <inheritdoc />
        public override string Name => name;

        private readonly uint priority;

        /// <inheritdoc />
        public override uint Priority => priority;
    }
}