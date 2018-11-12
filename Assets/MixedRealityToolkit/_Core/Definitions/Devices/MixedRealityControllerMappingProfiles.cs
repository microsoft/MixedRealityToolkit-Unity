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
        private MouseControllerMappingProfile mouseControllerMappingProfile = null;

        [SerializeField]
        private TouchScreenControllerMappingProfile touchScreenControllerMappingProfile = null;

        [SerializeField]
        private XboxControllerMappingProfile xboxControllerMappingProfile = null;

        [SerializeField]
        private WindowsMixedRealityMotionControllerMappingProfile windowsMixedRealityControllerMappingProfile = null;

        [SerializeField]
        private ViveWandControllerMappingProfile viveWandControllerMappingProfile = null;

        [SerializeField]
        private OculusTouchControllerMappingProfile oculusTouchControllerMappingProfile = null;

        [SerializeField]
        private OculusRemoteControllerMappingProfile oculusRemoteControllerMappingProfile = null;

        [SerializeField]
        private GenericUnityControllerMappingProfile genericUnityControllerMappingProfile = null;

        [SerializeField]
        private GenericOpenVRControllerMappingProfile genericOpenVRControllerMappingProfile = null;

        [SerializeField]
        private List<CustomMixedRealityControllerMappingProfile> customControllerProfiles = null;

        public List<MixedRealityControllerMapping> MixedRealityControllerMappings
        {
            get
            {
                var mappings = new List<MixedRealityControllerMapping>();

                if (mouseControllerMappingProfile != null)
                {
                    for (int i = 0; i < mouseControllerMappingProfile.ControllerMappings?.Length; i++)
                    {
                        mappings.Add(mouseControllerMappingProfile.ControllerMappings[i]);
                    }
                }

                if (touchScreenControllerMappingProfile != null)
                {
                    for (int i = 0; i < touchScreenControllerMappingProfile.ControllerMappings?.Length; i++)
                    {
                        mappings.Add(touchScreenControllerMappingProfile.ControllerMappings[i]);
                    }
                }

                if (xboxControllerMappingProfile != null)
                {
                    for (int i = 0; i < xboxControllerMappingProfile.ControllerMappings?.Length; i++)
                    {
                        mappings.Add(xboxControllerMappingProfile.ControllerMappings[i]);
                    }
                }

                if (windowsMixedRealityControllerMappingProfile != null)
                {
                    for (int i = 0; i < windowsMixedRealityControllerMappingProfile.ControllerMappings?.Length; i++)
                    {
                        mappings.Add(windowsMixedRealityControllerMappingProfile.ControllerMappings[i]);
                    }
                }

                if (viveWandControllerMappingProfile != null)
                {
                    for (int i = 0; i < viveWandControllerMappingProfile.ControllerMappings?.Length; i++)
                    {
                        mappings.Add(viveWandControllerMappingProfile.ControllerMappings[i]);
                    }
                }

                if (oculusTouchControllerMappingProfile != null)
                {
                    for (int i = 0; i < oculusTouchControllerMappingProfile.ControllerMappings?.Length; i++)
                    {
                        mappings.Add(oculusTouchControllerMappingProfile.ControllerMappings[i]);
                    }
                }

                if (oculusRemoteControllerMappingProfile != null)
                {
                    for (int i = 0; i < oculusRemoteControllerMappingProfile.ControllerMappings?.Length; i++)
                    {
                        mappings.Add(oculusRemoteControllerMappingProfile.ControllerMappings[i]);
                    }
                }

                if (genericUnityControllerMappingProfile != null)
                {
                    for (int i = 0; i < genericUnityControllerMappingProfile.ControllerMappings?.Length; i++)
                    {
                        mappings.Add(genericUnityControllerMappingProfile.ControllerMappings[i]);
                    }
                }

                if (genericOpenVRControllerMappingProfile != null)
                {
                    for (int i = 0; i < genericOpenVRControllerMappingProfile.ControllerMappings?.Length; i++)
                    {
                        mappings.Add(genericOpenVRControllerMappingProfile.ControllerMappings[i]);
                    }
                }

                for (int i = 0; i < customControllerProfiles?.Count; i++)
                {
                    if (customControllerProfiles[i] != null)
                    {
                        for (int j = 0; j < customControllerProfiles[i].ControllerMappings?.Length; j++)
                        {
                            mappings.Add(customControllerProfiles[i].ControllerMappings[j]);
                        }
                    }
                }

                return mappings;
            }
        }
    }
}