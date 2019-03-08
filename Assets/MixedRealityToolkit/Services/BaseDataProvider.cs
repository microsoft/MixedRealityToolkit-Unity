// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Definitions;
using Microsoft.MixedReality.Toolkit.Core.Interfaces;

namespace Microsoft.MixedReality.Toolkit.Core.Services
{
    /// <summary>
    /// The base data provider implements <see cref="Microsoft.MixedReality.Toolkit.Core.Interfaces.IMixedRealityDataProvider"/> and provides default properties for all data providers.
    /// </summary>
    /// <remarks>
    /// Empty, but reserved for future use, in case additional <see cref="Microsoft.MixedReality.Toolkit.Core.Interfaces.IMixedRealityDataProvider"/> properties or methods are assigned.
    /// </remarks>
    public abstract class BaseDataProvider : BaseExtensionService, Interfaces.IMixedRealityDataProvider
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="priority"></param>
        /// <param name="profile"></param>
        public BaseDataProvider(string name, uint priority, BaseMixedRealityProfile profile) : base(name, priority, profile) { }
    }
}