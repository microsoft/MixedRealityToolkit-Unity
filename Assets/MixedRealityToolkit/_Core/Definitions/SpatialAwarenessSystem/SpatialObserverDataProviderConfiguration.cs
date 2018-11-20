// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Attributes;
using Microsoft.MixedReality.Toolkit.Core.Definitions.Utilities;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.DataProviders.SpatialObservers;
using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Definitions.SpatialAwarenessSystem
{
    /// <summary>
    /// The configuration setting for a <see cref="IMixedRealitySpatialAwarenessObserver"/>
    /// </summary>
    [Serializable]
    public struct SpatialObserverDataProviderConfiguration
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="spatialObserverType"></param>
        /// <param name="spatialObserverName"></param>
        /// <param name="priority"></param>
        /// <param name="runtimePlatform"></param>
        public SpatialObserverDataProviderConfiguration(SystemType spatialObserverType, string spatialObserverName, uint priority, SupportedPlatforms runtimePlatform)
        {
            this.spatialObserverType = spatialObserverType;
            this.spatialObserverName = spatialObserverName;
            this.priority = priority;
            this.runtimePlatform = runtimePlatform;
        }

        [SerializeField]
        [Tooltip("The concrete type of spatial observer to register.")]
        [Implements(typeof(IMixedRealitySpatialAwarenessObserver), TypeGrouping.ByNamespaceFlat)]
        private SystemType spatialObserverType;

        /// <summary>
        /// The concrete type to use for this spatial observer.
        /// </summary>
        public SystemType SpatialObserverType => spatialObserverType;

        [SerializeField]
        private string spatialObserverName;

        /// <summary>
        /// The simple, human readable name for the system, feature, or manager.
        /// </summary>
        public string SpatialObserverName => spatialObserverName;

        [SerializeField]
        private uint priority;

        /// <summary>
        /// The priority this system, feature, or manager will be initialized in.
        /// </summary>
        public uint Priority => priority;

        [EnumFlags]
        [SerializeField]
        private SupportedPlatforms runtimePlatform;

        /// <summary>
        /// The runtime platform(s) to run this system, feature, or manager on.
        /// </summary>
        public SupportedPlatforms RuntimePlatform => runtimePlatform;
    }
}