// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Definitions;
using Microsoft.MixedReality.Toolkit.Core.Definitions.Devices;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.DataProviders.Controllers
{
    /// <summary>
    /// Base Controller Mapping profile.
    /// </summary>
    public abstract class BaseMixedRealityControllerMappingProfile : BaseMixedRealityProfile
    {
        [SerializeField]
        private bool hasSetupDefaults = false;

        /// <summary>
        /// Has this controller profile setup the default interactions?
        /// </summary>
        protected bool HasSetupDefaults => hasSetupDefaults;

        /// <summary>
        /// The supported controller type for this profile.
        /// </summary>
        public virtual SupportedControllerType ControllerType => SupportedControllerType.GenericUnity;

        [SerializeField]
        private MixedRealityControllerMapping[] controllerMappings;

        /// <summary>
        /// The controller mappings for this device.
        /// </summary>
        public MixedRealityControllerMapping[] ControllerMappings
        {
            get { return controllerMappings; }
            protected set { controllerMappings = value; }
        }

        protected virtual void Awake()
        {
            if (!hasSetupDefaults)
            {
                for (int i = 0; i < controllerMappings?.Length; i++)
                {
                    controllerMappings[i].SetDefaultInteractionMapping();
                }

                hasSetupDefaults = true;
            }
        }
    }
}