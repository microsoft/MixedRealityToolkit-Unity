// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using Microsoft.MixedReality.Toolkit.Core.Definitions.Utilities;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Definitions
{
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Mixed Reality Registered Service Providers Profile", fileName = "MixedRealityRegisteredServiceProvidersProfile", order = (int)CreateProfileMenuItemIndices.RegisteredServiceProviders)]
    public class MixedRealityRegisteredServiceProvidersProfile : BaseMixedRealityProfile
    {
        [SerializeField]
        private MixedRealityServiceConfiguration[] configurations = null;

        /// <summary>
        /// Currently registered system and manager configurations.
        /// </summary>
        public MixedRealityServiceConfiguration[] Configurations => configurations;
    }
}