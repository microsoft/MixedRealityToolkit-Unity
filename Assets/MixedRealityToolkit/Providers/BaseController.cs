// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Definitions.Devices;
using Microsoft.MixedReality.Toolkit.Core.Definitions.Utilities;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.Devices;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.InputSystem;
using Microsoft.MixedReality.Toolkit.Core.Services;
using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Providers
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

        /// <summary>
        /// The default interactions for this controller.
        /// </summary>
        public virtual MixedRealityInteractionMapping[] DefaultInteractions { get; } = null;

        /// <summary>
        /// The Default Left Handed interactions for this controller.
        /// </summary>
        public virtual MixedRealityInteractionMapping[] DefaultLeftHandedInteractions { get; } = null;

        /// <summary>
        /// The Default Right Handed interactions for this controller.
        /// </summary>
        public virtual MixedRealityInteractionMapping[] DefaultRightHandedInteractions { get; } = null;

        #region IMixedRealityController Implementation

        /// <inheritdoc />
        public bool Enabled { get; set; }

        /// <inheritdoc />
        public TrackingState TrackingState { get; protected set; }

        /// <inheritdoc />
        public Handedness ControllerHandedness { get; }

        /// <inheritdoc />
        public IMixedRealityInputSource InputSource { get; }

        public IMixedRealityControllerVisualizer Visualizer { get; private set; }

        /// <inheritdoc />
        public bool IsPositionAvailable { get; protected set; }

        /// <inheritdoc />
        public bool IsPositionApproximate { get; protected set; }

        /// <inheritdoc />
        public bool IsRotationAvailable { get; protected set; }

        /// <inheritdoc />
        public MixedRealityInteractionMapping[] Interactions { get; private set; } = null;

        #endregion IMixedRealityController Implementation

        /// <summary>
        /// Setups up the configuration based on the Mixed Reality Controller Mapping Profile.
        /// </summary>
        /// <param name="controllerType"></param>
        public bool SetupConfiguration(Type controllerType)
        {
            if (MixedRealityToolkit.Instance.ActiveProfile.InputSystemProfile.IsControllerMappingEnabled)
            {
                if (MixedRealityToolkit.Instance.ActiveProfile.InputSystemProfile.ControllerVisualizationProfile.RenderMotionControllers)
                {
                    TryRenderControllerModel(controllerType);
                }

                // We can only enable controller profiles if mappings exist.
                var controllerMappings = MixedRealityToolkit.Instance.ActiveProfile.InputSystemProfile.ControllerMappingProfile.MixedRealityControllerMappingProfiles;

                // Have to test that a controller type has been registered in the profiles,
                // else its Unity Input manager mappings will not have been set up by the inspector.
                bool profileFound = false;

                for (int i = 0; i < controllerMappings?.Length; i++)
                {
                    if (controllerMappings[i].ControllerType.Type == controllerType)
                    {
                        profileFound = true;

                        // If it is an exact match, assign interaction mappings.
                        if (controllerMappings[i].Handedness == ControllerHandedness &&
                            controllerMappings[i].Interactions.Length > 0)
                        {
                            MixedRealityInteractionMapping[] profileInteractions = controllerMappings[i].Interactions;
                            MixedRealityInteractionMapping[] newInteractions = new MixedRealityInteractionMapping[profileInteractions.Length];

                            for (int j = 0; j < profileInteractions.Length; j++)
                            {
                                newInteractions[j] = new MixedRealityInteractionMapping(profileInteractions[j]);
                            }

                            AssignControllerMappings(newInteractions);
                            break;
                        }
                    }
                }

                // If no controller mappings found, warn the user.  Does not stop the project from running.
                if (Interactions == null || Interactions.Length < 1)
                {
                    SetupDefaultInteractions(ControllerHandedness);

                    // We still don't have controller mappings, so this may be a custom controller. 
                    if (Interactions == null || Interactions.Length < 1)
                    {
                        Debug.LogWarning($"No Controller interaction mappings found for {controllerType}.");
                        return false;
                    }
                }

                if (!profileFound)
                {
                    Debug.LogWarning($"No controller profile found for type {controllerType}, please ensure all controllers are defined in the configured MixedRealityControllerConfigurationProfile.");
                    return false;
                }
            }

            return true;
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

        private void TryRenderControllerModel(Type controllerType)
        {
            GameObject controllerModel = null;

            if (!MixedRealityToolkit.Instance.ActiveProfile.InputSystemProfile.ControllerVisualizationProfile.RenderMotionControllers) { return; }

            // If a specific controller template wants to override the global model, assign that instead.
            if (MixedRealityToolkit.Instance.ActiveProfile.InputSystemProfile.IsControllerMappingEnabled &&
                !MixedRealityToolkit.Instance.ActiveProfile.InputSystemProfile.ControllerVisualizationProfile.UseDefaultModels)
            {
                controllerModel = MixedRealityToolkit.Instance.ActiveProfile.InputSystemProfile.ControllerVisualizationProfile.GetControllerModelOverride(controllerType, ControllerHandedness);
            }

            // Get the global controller model for each hand.
            if (controllerModel == null)
            {
                if (ControllerHandedness == Handedness.Left && MixedRealityToolkit.Instance.ActiveProfile.InputSystemProfile.ControllerVisualizationProfile.GlobalLeftHandModel != null)
                {
                    controllerModel = MixedRealityToolkit.Instance.ActiveProfile.InputSystemProfile.ControllerVisualizationProfile.GlobalLeftHandModel;
                }
                else if (ControllerHandedness == Handedness.Right && MixedRealityToolkit.Instance.ActiveProfile.InputSystemProfile.ControllerVisualizationProfile.GlobalRightHandModel != null)
                {
                    controllerModel = MixedRealityToolkit.Instance.ActiveProfile.InputSystemProfile.ControllerVisualizationProfile.GlobalRightHandModel;
                }
            }

            // TODO: add default model assignment here if no prefabs were found, or if settings specified to use them.

            // If we've got a controller model prefab, then place it in the scene.
            if (controllerModel != null)
            {
                var controllerObject = UnityEngine.Object.Instantiate(controllerModel, MixedRealityToolkit.Instance.MixedRealityPlayspace);
                controllerObject.name = $"{ControllerHandedness}_{controllerObject.name}";
                Visualizer = controllerObject.GetComponent<IMixedRealityControllerVisualizer>();

                if (Visualizer != null)
                {
                    Visualizer.Controller = this;
                }
                else
                {
                    Debug.LogError($"{controllerObject.name} is missing a IMixedRealityControllerVisualizer component!");
                }
            }
        }
    }
}