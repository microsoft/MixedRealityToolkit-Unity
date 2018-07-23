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

            IsPositionAvailable = false;
            IsPositionApproximate = false;
            IsRotationAvailable = false;

            Enabled = true;
        }

        public bool Enabled { get; set; }

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
        public bool IsPositionAvailable { get; protected set; }

        /// <inheritdoc />
        public bool IsPositionApproximate { get; protected set; }

        /// <inheritdoc />
        public bool IsRotationAvailable { get; protected set; }

        /// <inheritdoc />
        public MixedRealityInteractionMapping[] Interactions { get; private set; } = null;

        /// <inheritdoc />
        public virtual InputManagerAxis[] ControllerAxisMappings { get; } = null;

        /// <summary>
        /// Setups up the configuration based on the Mixed Reality Controller Mapping Profile.
        /// </summary>
        /// <param name="controllerType"></param>
        public void SetupConfiguration(Type controllerType)
        {
            if (MixedRealityManager.Instance.ActiveProfile.EnableControllerProfiles)
            {
                // We can only enable controller profiles if mappings exist.
                var controllerMappings = MixedRealityManager.Instance.ActiveProfile.ControllersProfile.MixedRealityControllerMappingProfiles;

                //Have to test that a controller type has been registered in the profiles, else it's Unity Input manager mappings will not have been setup by the inspector
                bool profileFound = false;

                for (int i = 0; i < controllerMappings?.Length; i++)
                {
                    if (!profileFound && controllerMappings[i].Controller.Type == controllerType)
                    {
                        profileFound = true;
                    }
                    if (!controllerMappings[i].UseCustomInteractionMappings && controllerMappings[i].Controller.Type == controllerType && controllerMappings[i].Handedness == ControllerHandedness)
                    {
                        AssignControllerMappings(controllerMappings[i].Interactions);
                        break;
                    }
                }

                if (!profileFound)
                {
                    Debug.LogError($"No controller profile found for type {controllerType}, please ensure all controllers are defined in the configured MixedRealityControllerConfigurationProfile.");
                    return;
                }

                //If no controller mappings found, warn the user.  Does not stop the project from running.
                if (Interactions == null || Interactions.Length < 1)
                {
                    SetupDefaultInteractions(ControllerHandedness);

                    // We still don't have controller mappings, so this may be a custom controller. 
                    if (Interactions == null || Interactions.Length < 1)
                    {
                        Debug.LogWarning($"No Controller interaction mappings found for {controllerType} using the {ControllerHandedness} hand");
                        Enabled = false;
                    }
                }
            }
        }

        /// <summary>
        /// Assign the default interactions based on controller handedness if necessary. 
        /// </summary>
        /// <param name="controllerHandedness"></param>
        public abstract void SetupDefaultInteractions(Handedness controllerHandedness);

        /// <summary>
        /// Load the Interaction mappings for this controller from the configured Controller Mapping profile
        /// </summary>
        /// <param name="mappings">Configured mappings from a controller mapping profile</param>
        public void AssignControllerMappings(MixedRealityInteractionMapping[] mappings)
        {
            Interactions = mappings;
        }
    }
}