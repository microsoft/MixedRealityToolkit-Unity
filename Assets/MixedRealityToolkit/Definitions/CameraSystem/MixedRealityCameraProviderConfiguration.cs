// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.CameraSystem
{
    [Serializable]
    public struct MixedRealityCameraProviderConfiguration : IMixedRealityServiceConfiguration
    {
        [SerializeField]
        [Implements(typeof(IMixedRealityCameraProvider), TypeGrouping.ByNamespaceFlat)]
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

        // todo
        //[SerializeField]
        //private BaseCameraProviderProfile providerProfile;

        ///// <summary>
        ///// 
        ///// </summary>
        //public BaseCameraProviderProfile ProviderProfile => providerProfile;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="componentType">The <see cref="Microsoft.MixedReality.Toolkit.Utilities.SystemType"/> of the provider.</param>
        /// <param name="componentName">The friendly name of the provider.</param>
        /// <param name="priority">The load priority of the provider.</param>
        /// <param name="runtimePlatform">The runtime platform(s) supported by the provider.</param>
        
        //    /// <param name="configurationProfile">The configuration profile for the observer.</param>
        public MixedRealityCameraProviderConfiguration(
            SystemType componentType,
            string componentName,
            uint priority,
            SupportedPlatforms runtimePlatform) //,
            //BaseSpatialAwarenessObserverProfile configurationProfile)
        {
            this.componentType = componentType;
            this.componentName = componentName;
            this.priority = priority;
            this.runtimePlatform = runtimePlatform;
            // todo
            //this.observerProfile = configurationProfile;
        }
    }
}
