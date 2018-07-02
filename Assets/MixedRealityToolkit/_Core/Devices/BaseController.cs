// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Definitions.Devices;
using Microsoft.MixedReality.Toolkit.Internal.Definitions.Utilities;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces.InputSystem;
using Microsoft.MixedReality.Toolkit.Internal.Managers;
using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Internal.Devices
{
    /// <summary>
    /// Base Controller class to inherit from for all controllers.
    /// </summary>
    public abstract class BaseController : IMixedRealityController
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="trackingState"></param>
        /// <param name="controllerHandedness"></param>
        /// <param name="inputSource"></param>
        /// <param name="interactions"></param>
        protected BaseController(TrackingState trackingState, Handedness controllerHandedness, IMixedRealityInputSource inputSource = null, MixedRealityInteractionMapping[] interactions = null)
        {
            TrackingState = trackingState;
            ControllerHandedness = controllerHandedness;
            InputSource = inputSource;
            Interactions = interactions;
        }

        /// <summary>
        /// Returns the current Input System if enabled, otherwise null.
        /// </summary>
        protected IMixedRealityInputSystem InputSystem
        {
            get
            {
                if (inputSystem == null && MixedRealityManager.Instance.ActiveProfile.EnableInputSystem)
                {
                    inputSystem = MixedRealityManager.Instance.GetManager<IMixedRealityInputSystem>();
                }

                return inputSystem;
            }
        }

        private IMixedRealityInputSystem inputSystem;

        /// <inheritdoc />
        public TrackingState TrackingState { get; protected set; }

        /// <inheritdoc />
        public Handedness ControllerHandedness { get; }

        /// <inheritdoc />
        public IMixedRealityInputSource InputSource { get; }

        /// <inheritdoc />
        public MixedRealityInteractionMapping[] Interactions { get; private set; }

        public void SetupConfiguration(Type controllerType)
        {
            if (MixedRealityManager.Instance.ActiveProfile.EnableControllerProfiles)
            {
                // We can only enable controller profiles if mappings exist.
                var controllerMappings = MixedRealityManager.Instance.ActiveProfile.ControllersProfile.MixedRealityControllerMappingProfiles;

                for (int i = 0; i < controllerMappings?.Length; i++)
                {
                    if (controllerMappings[i].Controller.Type == controllerType && controllerMappings[i].Handedness == ControllerHandedness)
                    {
                        AssignControllerMappings(controllerMappings[i].Interactions);
                        break;
                    }
                }

                //If no controller mappings found, warn the user.  Does not stop the project from running.
                if (Interactions == null || Interactions.Length < 1) { Debug.LogWarning($"No Controller mapping found for {controllerType}"); }
            }
        }

        /// <summary>
        /// Load the Interaction mappings for this controller from the configured Controller Mapping profile
        /// </summary>
        /// <param name="mappings">Configured mappings from a controller mapping profile</param>
        public void AssignControllerMappings(MixedRealityInteractionMapping[] mappings)
        {
            var interactions = new MixedRealityInteractionMapping[mappings.Length];

            for (uint i = 0; i < mappings.Length; i++)
            {
                interactions[i] = new MixedRealityInteractionMapping(i, mappings[i].AxisType, mappings[i].InputType, mappings[i].MixedRealityInputAction);
            }

            Interactions = interactions;
        }
    }
}
