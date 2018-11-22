// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Definitions;
using Microsoft.MixedReality.Toolkit.Core.Definitions.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.DataProviders.Controllers
{
    [Obsolete("Profile was renamed to MixedRealityControllerMappingProfiles")]
    public class MixedRealityControllerMappingProfile { }

    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Mixed Reality Controller Mapping Profiles", fileName = "MixedRealityControllerMappingProfiles", order = (int)CreateProfileMenuItemIndices.ControllerMappings)]
    public class MixedRealityControllerMappingProfiles : BaseMixedRealityProfile
    {
        [SerializeField]
        private List<BaseMixedRealityControllerMappingProfile> controllerMappingProfiles = new List<BaseMixedRealityControllerMappingProfile>();

        public List<MixedRealityControllerMapping> MixedRealityControllerMappings
        {
            get
            {
                var mappings = new List<MixedRealityControllerMapping>();

                if (controllerMappingProfiles != null)
                {
                    for (int i = 0; i < controllerMappingProfiles.Count; i++)
                    {
                        if (controllerMappingProfiles[i] != null)
                        {
                            for (int j = 0; j < controllerMappingProfiles[i].ControllerMappings?.Length; j++)
                            {

                                mappings.Add(controllerMappingProfiles[i].ControllerMappings[j]);
                            }
                        }
                    }
                }

                return mappings;
            }
        }
    }
}