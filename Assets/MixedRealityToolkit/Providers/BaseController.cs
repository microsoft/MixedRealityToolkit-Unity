// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
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

        private IMixedRealityInputSystem inputSystem = null;

        /// <summary>
        /// The active instance of the input system.
        /// </summary>
        protected IMixedRealityInputSystem InputSystem
        {
            get
            {
                if (inputSystem == null)
                {
                    MixedRealityServiceRegistry.TryGetService<IMixedRealityInputSystem>(out inputSystem);
                }
                return inputSystem;
            }
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

        public IMixedRealityControllerVisualizer Visualizer { get; protected set; }

        /// <inheritdoc />
        public bool IsPositionAvailable { get; protected set; }

        /// <inheritdoc />
        public bool IsPositionApproximate { get; protected set; }

        /// <inheritdoc />
        public bool IsRotationAvailable { get; protected set; }

        /// <inheritdoc />
        public MixedRealityInteractionMapping[] Interactions { get; private set; } = null;

        public Vector3 AngularVelocity { get; protected set; }

        public Vector3 Velocity { get; protected set; }

        public virtual bool IsInPointingPose
        {
            get
            {
                return true;
            }
        }

        #endregion IMixedRealityController Implementation

        /// <summary>
        /// Setups up the configuration based on the Mixed Reality Controller Mapping Profile.
        /// </summary>
        /// <param name="controllerType"></param>
        public bool SetupConfiguration(Type controllerType, InputSourceType inputSourceType = InputSourceType.Controller)
        {
            if (IsControllerMappingEnabled())
            {
                if (GetControllerVisualizationProfile() != null &&
                    GetControllerVisualizationProfile().RenderMotionControllers)
                {
                    TryRenderControllerModel(controllerType, inputSourceType);
                }

                // We can only enable controller profiles if mappings exist.
                var controllerMappings = GetControllerMappings();

                // Have to test that a controller type has been registered in the profiles,
                // else its Unity Input manager mappings will not have been set up by the inspector.
                bool profileFound = false;
                if (controllerMappings != null)
                {
                    for (int i = 0; i < controllerMappings.Length; i++)
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

        protected virtual bool TryRenderControllerModel(Type controllerType, InputSourceType inputSourceType)
        {
            GameObject controllerModel = null;

            if (GetControllerVisualizationProfile() == null ||
                !GetControllerVisualizationProfile().RenderMotionControllers)
            {
                return true;
            }

            // If a specific controller template wants to override the global model, assign that instead.
            if (IsControllerMappingEnabled() &&
                GetControllerVisualizationProfile() != null &&
                inputSourceType == InputSourceType.Controller &&
                !(GetControllerVisualizationProfile().GetUseDefaultModelsOverride(controllerType, ControllerHandedness)))
            {
                controllerModel = GetControllerVisualizationProfile().GetControllerModelOverride(controllerType, ControllerHandedness);
            }

            // Get the global controller model for each hand.
            if (controllerModel == null &&
                GetControllerVisualizationProfile() != null)
            {
                if (inputSourceType == InputSourceType.Controller)
                {
                    if (ControllerHandedness == Handedness.Left &&
                        GetControllerVisualizationProfile().GlobalLeftHandModel != null)
                    {
                        controllerModel = GetControllerVisualizationProfile().GlobalLeftHandModel;
                    }
                    else if (ControllerHandedness == Handedness.Right &&
                        GetControllerVisualizationProfile().GlobalRightHandModel != null)
                    {
                        controllerModel = GetControllerVisualizationProfile().GlobalRightHandModel;
                    }
                }
            
                else if (inputSourceType == InputSourceType.Hand)
                {
                    if (ControllerHandedness == Handedness.Left &&
                        GetControllerVisualizationProfile().GlobalLeftHandVisualizer != null)
                    {
                        controllerModel = GetControllerVisualizationProfile().GlobalLeftHandVisualizer;
                    }
                    else if (ControllerHandedness == Handedness.Right &&
                        GetControllerVisualizationProfile().GlobalRightHandVisualizer != null)
                    {
                        controllerModel = GetControllerVisualizationProfile().GlobalRightHandVisualizer;
                    }
                }
            }

            if (controllerModel == null)
            {
                // no controller model available
                return false;
            }

            // If we've got a controller model prefab, then create it and place it in the scene.
            GameObject controllerObject = UnityEngine.Object.Instantiate(controllerModel);
            MixedRealityPlayspace.AddChild(controllerObject.transform);

            return TryAddControllerModelToSceneHierarchy(controllerObject);
        }

        protected bool TryAddControllerModelToSceneHierarchy(GameObject controllerObject)
        {
            if (controllerObject != null)
            {
                controllerObject.name = $"{ControllerHandedness}_{controllerObject.name}";

                Visualizer = controllerObject.GetComponent<IMixedRealityControllerVisualizer>();

                if (Visualizer != null)
                {
                    Visualizer.Controller = this;
                    return true;
                }
                else
                {
                    Debug.LogError($"{controllerObject.name} is missing a IMixedRealityControllerVisualizer component!");
                    return false;
                }
            }

            return false;
        }

        #region MRTK instance helpers
        protected MixedRealityControllerVisualizationProfile GetControllerVisualizationProfile()
        {
            if (InputSystem?.InputSystemProfile != null)
            {
                return InputSystem.InputSystemProfile.ControllerVisualizationProfile;
            }

            return null;
        }

        protected bool IsControllerMappingEnabled()
        {
            if (InputSystem?.InputSystemProfile != null)
            {
                return InputSystem.InputSystemProfile.IsControllerMappingEnabled;
            }

            return false;
        }

        protected MixedRealityControllerMapping[] GetControllerMappings()
        {
            if (InputSystem?.InputSystemProfile?.ControllerMappingProfile != null)
            {
                return InputSystem.InputSystemProfile.ControllerMappingProfile.MixedRealityControllerMappingProfiles;
            }

            return null;
        }

        #endregion MRTK instance helpers
    }
}