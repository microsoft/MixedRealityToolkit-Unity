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
            if (IsControllerMappingEnabled)
            {
                if (ControllerVisualizationProfile != null &&
                    ControllerVisualizationProfile.RenderMotionControllers)
                {
                    TryRenderControllerModel(controllerType);
                }

                // We can only enable controller profiles if mappings exist.
                var controllerMappings = ControllerMappings;

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
                if (!SetupCurrentOrDefaultInteractions())
                {
                    Debug.LogWarning($"No Controller interaction mappings found for {controllerType}.");
                    return false;
                }

                if (!profileFound)
                {
                    Debug.LogWarning($"No controller profile found for type {controllerType}, please ensure all controllers are defined in the configured MixedRealityControllerConfigurationProfile.");
                    return false;
                }
            }

            return true;
        }

        private bool SetupCurrentOrDefaultInteractions()
        {
            if (Interactions == null || Interactions.Length == 0)
            {
                SetupDefaultInteractions(ControllerHandedness);

                // We still don't have controller mappings, so this may be a custom controller. 
                return Interactions != null && Interactions.Length > 0;
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

        protected virtual bool TryRenderControllerModel(Type controllerType)
        {
            GameObject controllerModel = null;

            if (ControllerVisualizationProfile == null ||
                !ControllerVisualizationProfile.RenderMotionControllers)
            {
                return true;
            }

            // If a specific controller template wants to override the global model, assign that instead.
            if (IsControllerMappingEnabled &&
                ControllerVisualizationProfile != null &&
                !(ControllerVisualizationProfile.GetUseDefaultModelsOverride(controllerType, ControllerHandedness)))
            {
                controllerModel = ControllerVisualizationProfile.GetControllerModelOverride(controllerType, ControllerHandedness);
            }

            // Get the global controller model for each hand.
            if (controllerModel == null &&
                ControllerVisualizationProfile != null)
            {
                if (ControllerHandedness == Handedness.Left &&
                    ControllerVisualizationProfile.GlobalLeftHandModel != null)
                {
                    controllerModel = ControllerVisualizationProfile.GlobalLeftHandModel;
                }
                else if (ControllerHandedness == Handedness.Right &&
                    ControllerVisualizationProfile.GlobalRightHandModel != null)
                {
                    controllerModel = ControllerVisualizationProfile.GlobalRightHandModel;
                }
            }

            if (controllerModel == null)
            {
                Debug.LogError("No controller model available. Failed to add controller game object to scene");
                return false;
            }

            // If we've got a controller model prefab, then create it and place it in the scene.
            var controllerObject = UnityEngine.Object.Instantiate(controllerModel, Playspace);

            return TryAddControllerModelToSceneHierarchy(controllerObject);
        }

        protected bool TryAddControllerModelToSceneHierarchy(GameObject controllerObject)
        {
            if (controllerObject != null)
            {
                controllerObject.name = $"{ControllerHandedness}_{controllerObject.name}";
                var playspace = Playspace;
                if (playspace != null)
                {
                    controllerObject.transform.parent = playspace.transform;
                }
                else
                {
                    Debug.LogWarning("Playspace was not found. No parent transform was applied to the controller object");
                }

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

        private MixedRealityInputSystemProfile InputSystemProfile => MixedRealityToolkit.Instance?.ActiveProfile?.InputSystemProfile;

        protected Transform Playspace => MixedRealityToolkit.Instance?.MixedRealityPlayspace;

        protected MixedRealityControllerVisualizationProfile ControllerVisualizationProfile => InputSystemProfile?.ControllerVisualizationProfile;

        protected bool IsControllerMappingEnabled => InputSystemProfile != null && InputSystemProfile.IsControllerMappingEnabled;

        protected MixedRealityControllerMapping[] ControllerMappings => InputSystemProfile?.ControllerMappingProfile?.MixedRealityControllerMappingProfiles;

        #endregion MRTK instance helpers
    }
}
