// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Definitions.Devices;
using Microsoft.MixedReality.Toolkit.Core.Definitions.Utilities;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.DataProviders.Controllers.OpenVR
{
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Mixed Reality Controller Mappings/Generic OpenVR Controller Mapping Profile", fileName = "GenericOpenVRControllerMappingProfile")]
    public class GenericOpenVRControllerMappingProfile : BaseMixedRealityControllerMappingProfile
    {
        /// <inheritdoc />
        public override SupportedControllerType ControllerType => SupportedControllerType.GenericOpenVR;

        protected override void Awake()
        {
            if (!HasSetupDefaults)
            {
                ControllerMappings = new[]
                {
                    new MixedRealityControllerMapping("Generic OpenVR Controller Left", typeof(GenericOpenVRController), Handedness.Left, true),
                    new MixedRealityControllerMapping("Generic OpenVR Controller Right", typeof(GenericOpenVRController), Handedness.Right, true),
                };
            }

            base.Awake();
        }
    }
}