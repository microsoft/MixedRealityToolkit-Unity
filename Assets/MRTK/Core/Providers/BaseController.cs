// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

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
        protected BaseController(
            TrackingState trackingState,
            Handedness controllerHandedness,
            IMixedRealityInputSource inputSource = null,
            MixedRealityInteractionMapping[] interactions = null,
            IMixedRealityInputSourceDefinition definition = null)
        {
            TrackingState = trackingState;
            ControllerHandedness = controllerHandedness;
            InputSource = inputSource;
            Interactions = interactions;
            Definition = definition;

            IsPositionAvailable = false;
            IsPositionApproximate = false;
            IsRotationAvailable = false;

            Type controllerType = GetType();

            if (IsControllerMappingEnabled() && Interactions == null)
            {
                // We can only enable controller profiles if mappings exist.
                MixedRealityControllerMapping[] controllerMappings = GetControllerMappings();

                // Have to test that a controller type has been registered in the profiles,
                // else its Unity input manager mappings will not have been set up by the inspector.
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

                // If no controller mappings found, try to use default interactions.
                if (Interactions == null || Interactions.Length < 1)
                {
                    SetupDefaultInteractions();

                    // We still don't have controller mappings, so this may be a custom controller.
                    if (Interactions == null || Interactions.Length < 1)
                    {
                        Debug.LogWarning($"No controller interaction mappings found for {controllerType}.");
                        return;
                    }
                }

                // If no profile was found, warn the user. Does not stop the project from running.
                if (!profileFound)
                {
                    Debug.LogWarning($"No controller profile found for type {controllerType}; please ensure all controllers are defined in the configured MixedRealityControllerConfigurationProfile.");
                }
            }

            if (GetControllerVisualizationProfile() != null &&
                GetControllerVisualizationProfile().RenderMotionControllers &&
                InputSource != null)
            {
                TryRenderControllerModel(controllerType, InputSource.SourceType);
            }

            Enabled = true;
        }

        /// <summary>
        /// The default interactions for this controller.
        /// </summary>
        public virtual MixedRealityInteractionMapping[] DefaultInteractions => BuildInteractions(Definition?.GetDefaultMappings(ControllerHandedness));

        /// <summary>
        /// The default left-handed interactions for this controller.
        /// </summary>
        public virtual MixedRealityInteractionMapping[] DefaultLeftHandedInteractions => BuildInteractions(Definition?.GetDefaultMappings(Handedness.Left));

        /// <summary>
        /// The default right-handed interactions for this controller.
        /// </summary>
        public virtual MixedRealityInteractionMapping[] DefaultRightHandedInteractions => BuildInteractions(Definition?.GetDefaultMappings(Handedness.Right));

        private MixedRealityInteractionMapping[] BuildInteractions(System.Collections.Generic.IReadOnlyList<MixedRealityInputActionMapping> definitionInteractions)
        {
            if (definitionInteractions == null)
            {
                return null;
            }

            MixedRealityInteractionMapping[] defaultInteractions = new MixedRealityInteractionMapping[definitionInteractions.Count];
            for (int i = 0; i < definitionInteractions.Count; i++)
            {
                defaultInteractions[i] = new MixedRealityInteractionMapping((uint)i, definitionInteractions[i]);
            }
            return defaultInteractions;
        }

        /// <summary>
        /// Represents the archetypal definition of what this controller supports and can perform.
        /// </summary>
        protected virtual IMixedRealityInputSourceDefinition Definition { get; } = null;

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

        /// <inheritdoc />
        public virtual bool IsInPointingPose => true;

        #endregion IMixedRealityController Implementation

        /// <summary>
        /// Sets up the configuration based on the Mixed Reality Controller Mapping Profile.
        /// </summary>
        [Obsolete("This method is no longer used. Configuration now happens in the constructor. You can check this controller's Enabled property for configuration state.")]
        public bool SetupConfiguration(Type controllerType, InputSourceType inputSourceType = InputSourceType.Controller)
        {
            // If the constructor succeeded in finding interactions, Enabled will be true.
            return Enabled;
        }

        /// <summary>
        /// Sets up the configuration based on the Mixed Reality Controller Mapping Profile.
        /// </summary>
        /// <param name="controllerType">The type this controller represents.</param>
        [Obsolete("This method is no longer used. Configuration now happens in the constructor. You can check this controller's Enabled property for configuration state.")]
        public bool SetupConfiguration(Type controllerType)
        {
            // If the constructor succeeded in finding interactions, Enabled will be true.
            return Enabled;
        }

        /// <summary>
        /// Assign the default interactions based on controller handedness, if necessary.
        /// </summary>
        [Obsolete("The handedness parameter is no longer used. This method now reads from the controller's handedness.")]
        public virtual void SetupDefaultInteractions(Handedness controllerHandedness) => SetupDefaultInteractions();

        /// <summary>
        /// Assign the default interactions based on this controller's handedness, if necessary.
        /// </summary>
        public virtual void SetupDefaultInteractions()
        {
            switch (ControllerHandedness)
            {
                case Handedness.Left:
                    AssignControllerMappings(DefaultLeftHandedInteractions ?? DefaultInteractions);
                    break;
                case Handedness.Right:
                    AssignControllerMappings(DefaultRightHandedInteractions ?? DefaultInteractions);
                    break;
                default:
                    AssignControllerMappings(DefaultInteractions);
                    break;
            }
        }

        /// <summary>
        /// Load the Interaction mappings for this controller from the configured Controller Mapping profile
        /// </summary>
        /// <param name="mappings">Configured mappings from a controller mapping profile</param>
        public void AssignControllerMappings(MixedRealityInteractionMapping[] mappings)
        {
            Interactions = mappings;
        }

        /// <summary>
        /// Try to render a controller model for this controller from the visualization profile.
        /// </summary>
        /// <param name="controllerType">The type of controller to load the model for.</param>
        /// <param name="inputSourceType">Whether the model represents a hand or a controller.</param>
        /// <returns>True if a model was successfully loaded or model rendering is disabled. False if a model tried to load but failed.</returns>
        protected virtual bool TryRenderControllerModel(Type controllerType, InputSourceType inputSourceType)
        {
            MixedRealityControllerVisualizationProfile controllerVisualizationProfile = GetControllerVisualizationProfile();
            bool controllerVisualizationProfilePresent = controllerVisualizationProfile != null;
           
            if (!controllerVisualizationProfilePresent || !controllerVisualizationProfile.RenderMotionControllers)
            {
                return true;
            }

            GameObject controllerModel = null;
            bool usePlatformModels = controllerVisualizationProfile.GetUsePlatformModelsOverride(controllerType, ControllerHandedness);

            // If a specific controller template wants to override the global model, assign it
            if (!usePlatformModels)
            {
                controllerModel = controllerVisualizationProfile.GetControllerModelOverride(controllerType, ControllerHandedness);
            }

            // If the Controller model is still null in the end, use the global defaults.
            if (controllerModel == null)
            {
                if (inputSourceType == InputSourceType.Controller)
                {
                    if (ControllerHandedness == Handedness.Left &&
                        controllerVisualizationProfile.GlobalLeftHandModel != null)
                    {
                        controllerModel = controllerVisualizationProfile.GlobalLeftHandModel;
                    }
                    else if (ControllerHandedness == Handedness.Right &&
                        controllerVisualizationProfile.GlobalRightHandModel != null)
                    {
                        controllerModel = controllerVisualizationProfile.GlobalRightHandModel;
                    }
                }
                else if (inputSourceType == InputSourceType.Hand)
                {
                    if (ControllerHandedness == Handedness.Left &&
                        controllerVisualizationProfile.GlobalLeftHandVisualizer != null)
                    {
                        controllerModel = controllerVisualizationProfile.GlobalLeftHandVisualizer;
                    }
                    else if (ControllerHandedness == Handedness.Right &&
                        controllerVisualizationProfile.GlobalRightHandVisualizer != null)
                    {
                        controllerModel = controllerVisualizationProfile.GlobalRightHandVisualizer;
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

            return TryAddControllerModelToSceneHierarchy(controllerObject);
        }

        protected bool TryAddControllerModelToSceneHierarchy(GameObject controllerObject)
        {
            if (controllerObject != null)
            {
                controllerObject.name = $"{ControllerHandedness}_{controllerObject.name}";

                MixedRealityPlayspace.AddChild(controllerObject.transform);

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

        protected static MixedRealityControllerVisualizationProfile GetControllerVisualizationProfile()
        {
            if (CoreServices.InputSystem?.InputSystemProfile != null)
            {
                return CoreServices.InputSystem.InputSystemProfile.ControllerVisualizationProfile;
            }

            return null;
        }

        protected static bool IsControllerMappingEnabled()
        {
            if (CoreServices.InputSystem?.InputSystemProfile != null)
            {
                return CoreServices.InputSystem.InputSystemProfile.IsControllerMappingEnabled;
            }

            return false;
        }

        protected static MixedRealityControllerMapping[] GetControllerMappings()
        {
            if (CoreServices.InputSystem?.InputSystemProfile != null &&
                CoreServices.InputSystem.InputSystemProfile.ControllerMappingProfile != null)
            {
                // We can only enable controller profiles if mappings exist.
                return CoreServices.InputSystem.InputSystemProfile.ControllerMappingProfile.MixedRealityControllerMappings;
            }

            return null;
        }

        #endregion MRTK instance helpers
    }
}
