// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Microsoft.MixedReality.Toolkit.Core.Services
{
    /// <summary>
    /// The base service implements <see cref="Interfaces.IMixedRealityService"/> and provides default properties for all services.
    /// </summary>
    public class BaseService : Interfaces.IMixedRealityService
    {
        /// <inheritdoc />
        public virtual string Name { get; set; }

        /// <inheritdoc />
        public virtual uint Priority { get; set; } = 5;

        /// <inheritdoc />
        public virtual void Initialize() { }

        /// <inheritdoc />
        public virtual void Reset() { }

        /// <inheritdoc />
        public virtual void Enable() { }

        /// <inheritdoc />
        public virtual void Update() { }

        /// <inheritdoc />
        public virtual void Disable() { }

        /// <inheritdoc />
        public virtual void Destroy() { }
    }
}
