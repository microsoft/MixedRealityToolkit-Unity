// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Definitions.Utilities;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Definitions.Devices
{
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Mixed Reality Controller Mapping Profiles", fileName = "MixedRealityControllerMappingProfiles", order = (int)CreateProfileMenuItemIndices.ControllerMapping)]
    public class MixedRealityControllerMappingProfiles : BaseMixedRealityProfile
    {
        [SerializeField]
        private MouseControllerMappingProfile mouseControllerMappingProfile;

        [SerializeField]
        private TouchScreenControllerMappingProfile touchScreenControllerMappingProfile;

        [SerializeField]
        private XboxControllerMappingProfile xboxControllerMappingProfile;

        [SerializeField]
        private WindowsMixedRealityMotionControllerMappingProfile windowsMixedRealityControllerMappingProfile;

        [SerializeField]
        private ViveWandControllerMappingProfile viveWandControllerMappingProfile;

        [SerializeField]
        private OculusTouchControllerMappingProfile oculusTouchControllerMappingProfile;

        [SerializeField]
        private OculusRemoteControllerMappingProfile oculusRemoteControllerMappingProfile;

        [SerializeField]
        private GenericUnityControllerMappingProfile genericUnityControllerMappingProfile;

        [SerializeField]
        private GenericOpenVRControllerMappingProfile genericOpenVRControllerMappingProfile;

        public List<MixedRealityControllerMapping> MixedRealityControllerMappings
        {
            get
            {
                var mappings = new List<MixedRealityControllerMapping>();

                for (int i = 0; i < mouseControllerMappingProfile?.ControllerMappings?.Length; i++)
                {
                    mappings.Add(mouseControllerMappingProfile.ControllerMappings[i]);
                }

                for (int i = 0; i < touchScreenControllerMappingProfile?.ControllerMappings?.Length; i++)
                {
                    mappings.Add(touchScreenControllerMappingProfile.ControllerMappings[i]);
                }

                for (int i = 0; i < xboxControllerMappingProfile?.ControllerMappings?.Length; i++)
                {
                    mappings.Add(xboxControllerMappingProfile.ControllerMappings[i]);
                }

                for (int i = 0; i < windowsMixedRealityControllerMappingProfile?.ControllerMappings?.Length; i++)
                {
                    mappings.Add(windowsMixedRealityControllerMappingProfile.ControllerMappings[i]);
                }

                for (int i = 0; i < viveWandControllerMappingProfile?.ControllerMappings?.Length; i++)
                {
                    mappings.Add(viveWandControllerMappingProfile.ControllerMappings[i]);
                }

                for (int i = 0; i < oculusTouchControllerMappingProfile?.ControllerMappings?.Length; i++)
                {
                    mappings.Add(oculusTouchControllerMappingProfile.ControllerMappings[i]);
                }

                for (int i = 0; i < oculusRemoteControllerMappingProfile?.ControllerMappings?.Length; i++)
                {
                    mappings.Add(oculusRemoteControllerMappingProfile.ControllerMappings[i]);
                }

                for (int i = 0; i < genericUnityControllerMappingProfile?.ControllerMappings?.Length; i++)
                {
                    mappings.Add(genericUnityControllerMappingProfile.ControllerMappings[i]);
                }

                for (int i = 0; i < genericOpenVRControllerMappingProfile?.ControllerMappings?.Length; i++)
                {
                    mappings.Add(genericOpenVRControllerMappingProfile.ControllerMappings[i]);
                }

                return mappings;
            }
        }
    }
}