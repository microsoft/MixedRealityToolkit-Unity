// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Attributes;
using Microsoft.MixedReality.Toolkit.Core.DataProviders.Networking.Profiles;
using Microsoft.MixedReality.Toolkit.Core.Definitions.Utilities;
using Microsoft.MixedReality.Toolkit.Core.Interfaces;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.NetworkingSystem;
using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Definitions.NetworkingSystem
{
    /// <summary>
    /// Configuration settings for a registered <see cref="IMixedRealityDataProvider"/>.
    /// </summary>
    [Serializable]
    public struct NetworkDataProviderConfiguration
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="dataProviderType">The concrete type for the system, feature or manager.</param>
        /// <param name="dataProviderName">The simple, human readable name for the system, feature, or manager.</param>
        /// <param name="priority">The priority this system, feature, or manager will be initialized in.</param>
        /// <param name="runtimePlatform">The runtime platform(s) to run this system, feature, or manager on.</param>
        /// <param name="profile">The configuration profile for the system, feature, or manager.</param>
        public NetworkDataProviderConfiguration(SystemType dataProviderType, string dataProviderName, uint priority, SupportedPlatforms runtimePlatform, BaseMixedRealityNetworkProviderProfile profile)
        {
            this.dataProviderType = dataProviderType;
            this.dataProviderName = dataProviderName;
            this.priority = priority;
            this.runtimePlatform = runtimePlatform;
            this.profile = profile;
        }

        [SerializeField]
        [Implements(typeof(IMixedRealityNetworkDataProvider), TypeGrouping.ByNamespaceFlat)]
        private SystemType dataProviderType;

        /// <summary>
        /// The concrete type for the system, feature or manager.
        /// </summary>
        public SystemType DataProviderType => dataProviderType;

        [SerializeField]
        private string dataProviderName;

        /// <summary>
        /// The simple, human readable name for the system, feature, or manager.
        /// </summary>
        public string DataProviderName => dataProviderName;

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

        [SerializeField]
        private BaseMixedRealityNetworkProviderProfile profile;

        /// <summary>
        /// The configuration profile for the system, feature, or manager.
        /// </summary>
        public BaseMixedRealityNetworkProviderProfile Profile => profile;
    }
}