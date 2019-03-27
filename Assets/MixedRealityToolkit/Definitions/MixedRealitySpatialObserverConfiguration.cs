// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.SpatialAwareness
{
    [Serializable]
    public struct MixedRealitySpatialObserverConfiguration : IMixedRealityServiceConfiguration
    {
        [SerializeField]
        [Implements(typeof(IMixedRealitySpatialAwarenessMeshObserver), TypeGrouping.ByNamespaceFlat)]
        private SystemType componentType;

        /// <inheritdoc />
        public SystemType ComponentType => componentType;

        [SerializeField]
        private string componentName;

        /// <inheritdoc />
        public string ComponentName => componentName;

        [SerializeField]
        private uint priority;

        /// <inheritdoc />
        public uint Priority => priority;

        [SerializeField]
        [EnumFlags]
        private SupportedPlatforms runtimePlatform;

        /// <inheritdoc />
        public SupportedPlatforms RuntimePlatform => runtimePlatform;

        [SerializeField]
        private BaseSpatialAwarenessObserverProfile configurationProfile;

        /// <summary>
        /// 
        /// </summary>
        public BaseSpatialAwarenessObserverProfile ConfigurationProfile => configurationProfile;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="componentType"></param>
        /// <param name="componentName"></param>
        /// <param name="priority"></param>
        /// <param name="runtimePlatform"></param>
        /// <param name="configurationProfile"></param>
        public MixedRealitySpatialObserverConfiguration(
            SystemType componentType,
            string componentName,
            uint priority,
            SupportedPlatforms runtimePlatform,
            BaseSpatialAwarenessObserverProfile configurationProfile)
        {
            this.componentType = componentType;
            this.componentName = componentName;
            this.priority = priority;
            this.runtimePlatform = runtimePlatform;
            this.configurationProfile = configurationProfile;
        }
    }
}
