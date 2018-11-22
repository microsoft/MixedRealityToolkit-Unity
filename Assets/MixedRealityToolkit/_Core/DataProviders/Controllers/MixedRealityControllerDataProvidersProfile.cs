// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Definitions;
using Microsoft.MixedReality.Toolkit.Core.Definitions.InputSystem;
using Microsoft.MixedReality.Toolkit.Core.Definitions.Utilities;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.DataProviders.Controllers
{
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Mixed Reality Controller Data Providers Profiles", fileName = "MixedRealityControllerDataModelsProfile", order = (int)CreateProfileMenuItemIndices.ControllerDataProviders)]
    public class MixedRealityControllerDataProvidersProfile : BaseMixedRealityProfile
    {
        [SerializeField]
        private ControllerDataProviderConfiguration[] registeredControllerDataProviders;

        /// <summary>
        /// The currently registered controller data providers for this input system.
        /// </summary>
        public ControllerDataProviderConfiguration[] RegisteredControllerDataProviders => registeredControllerDataProviders;
    }
}