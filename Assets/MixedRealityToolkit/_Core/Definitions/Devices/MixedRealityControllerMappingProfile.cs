// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Internal.Definitions.Devices
{
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Mixed Reality Controller Configuration Profile", fileName = "MixedRealityControllerConfigurationProfile", order = 2)]
    public class MixedRealityDevicesConfigurationProfile : ScriptableObject
    {
        [SerializeField]
        [Tooltip("The list of controller templates your application can use.")]
        private MixedRealityControllerMappingProfile[] mixedRealityControllerMappingProfiles = new MixedRealityControllerMappingProfile[0];

        public MixedRealityControllerMappingProfile[] MixedRealityControllerMappingProfiles => mixedRealityControllerMappingProfiles;
    }
}