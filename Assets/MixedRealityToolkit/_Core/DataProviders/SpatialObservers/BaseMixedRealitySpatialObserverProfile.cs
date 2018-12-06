// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Definitions;
using Microsoft.MixedReality.Toolkit.Core.Definitions.Devices;

namespace Microsoft.MixedReality.Toolkit.Core.DataProviders.SpatialObservers
{
    /// <summary>
    /// Base Mixed Reality Observer Profile.
    /// </summary>
    public abstract class BaseMixedRealitySpatialObserverProfile : BaseMixedRealityProfile
    {
        /// <summary>
        /// The Spatial Observer Type.
        /// </summary>
        public virtual SpatialObserverType SpatialObserverType => SpatialObserverType.None;
    }
}