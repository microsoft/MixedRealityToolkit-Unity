// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Definitions.Devices;
using Microsoft.MixedReality.Toolkit.Core.Definitions.Utilities;
using Microsoft.MixedReality.Toolkit.Core.DataProviders.Controllers.OpenVR;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.DataProviders.Controllers.WindowsMixedReality.Profiles
{
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Mixed Reality Controller Mappings/Windows Mixed Reality Controller Mapping Profile", fileName = "WindowsMixedRealityControllerMappingProfile")]
    public class WindowsMixedRealityMotionControllerMappingProfile : BaseMixedRealityControllerMappingProfile
    {
        /// <inheritdoc />
        public override SupportedControllerType ControllerType => SupportedControllerType.WindowsMixedReality;

        protected override void Awake()
        {
            if (!HasSetupDefaults)
            {
                ControllerMappings = new[]
                {
                    new MixedRealityControllerMapping("Windows Mixed Reality HoloLens Hand Input", typeof(WindowsMixedRealityController)),
                    new MixedRealityControllerMapping("Windows Mixed Reality Motion Controller Left", typeof(WindowsMixedRealityController), Handedness.Left),
                    new MixedRealityControllerMapping("Windows Mixed Reality Motion Controller Right", typeof(WindowsMixedRealityController), Handedness.Right),
                    new MixedRealityControllerMapping("Open VR Motion Controller Left", typeof(WindowsMixedRealityOpenVRMotionController), Handedness.Left),
                    new MixedRealityControllerMapping("Open VR Motion Controller Right", typeof(WindowsMixedRealityOpenVRMotionController), Handedness.Right),
                };
            }

            base.Awake();
        }
    }
}