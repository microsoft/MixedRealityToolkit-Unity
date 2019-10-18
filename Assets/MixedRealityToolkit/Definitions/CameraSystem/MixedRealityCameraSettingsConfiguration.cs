// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.CameraSystem
{
    [Serializable]
    public struct MixedRealityCameraSettingsConfiguration : IMixedRealityServiceConfiguration
    {
        [SerializeField]
        [Implements(typeof(IMixedRealityCameraSettingsProvider), TypeGrouping.ByNamespaceFlat)]
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
        private BaseCameraSettingsProfile settingsProfile;

        /// <summary>
        /// Camera settings specific configuration profile.
        /// </summary>
        public BaseCameraSettingsProfile SettingsProfile => settingsProfile;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="componentType">The <see cref="Microsoft.MixedReality.Toolkit.Utilities.SystemType"/> of the provider.</param>
        /// <param name="componentName">The friendly name of the provider.</param>
        /// <param name="priority">The load priority of the provider.</param>
        /// <param name="runtimePlatform">The runtime platform(s) supported by the provider.</param>
        /// <param name="settingsProfile">The configuration profile for the provider.</param>
        public MixedRealityCameraSettingsConfiguration(
            SystemType componentType,
            string componentName,
            uint priority,
            SupportedPlatforms runtimePlatform,
            BaseCameraSettingsProfile configurationProfile)
        {
            this.componentType = componentType;
            this.componentName = componentName;
            this.priority = priority;
            this.runtimePlatform = runtimePlatform;
            this.settingsProfile = configurationProfile;
        }
    }
}
