// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Definitions.Devices;
using Microsoft.MixedReality.Toolkit.Core.Definitions.Utilities;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.DataProviders.Controllers;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.InputSystem;
using Microsoft.MixedReality.Toolkit.Core.Services;
using Microsoft.MixedReality.Toolkit.Core.Utilities.Gltf.Serialization;
using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.DataProviders.Controllers
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

        /// <inheritdoc />
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
            // We can only enable controller profiles if mappings exist.
            var controllerMappings = MixedRealityToolkit.Instance.ActiveProfile.InputSystemProfile.ControllerMappingProfiles.MixedRealityControllerMappings;

            // Have to test that a controller type has been registered in the profiles,
            // else it's Unity Input manager mappings will not have been setup by the inspector
            bool profileFound = false;

            for (int i = 0; i < controllerMappings?.Count; i++)
            {
                if (!profileFound && controllerMappings[i].ControllerType.Type == controllerType)
                {
                    profileFound = true;
                }

                // Assign any known interaction mappings.
                if (controllerMappings[i].ControllerType.Type == controllerType &&
                    controllerMappings[i].Handedness == ControllerHandedness &&
                    controllerMappings[i].Interactions.Length > 0)
                {
                    AssignControllerMappings(controllerMappings[i].Interactions);
                    break;
                }

                // If no controller mappings found, warn the user.  Does not stop the project from running.
                if (Interactions == null || Interactions.Length < 1)
                {
                    SetupDefaultInteractions(ControllerHandedness);

                    // We still don't have controller mappings, so this may be a custom controller. 
                    if (Interactions == null || Interactions.Length < 1)
                    {
                        Debug.LogWarning($"No Controller interaction mappings found for {controllerMappings[i].Description}.");
                        return false;
                    }
                }
            }

            if (!profileFound)
            {
                Debug.LogWarning($"No controller profile found for type {controllerType}, please ensure all controllers are defined in the configured MixedRealityControllerConfigurationProfile.");
                return false;
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

        /// <summary>
        /// Attempts to load the controller model render settings from the <see cref="MixedRealityControllerVisualizationProfile"/>
        /// to render the controllers in the scene.
        /// </summary>
        /// <param name="controllerType">The controller type.</param>
        /// <param name="glbData">The raw binary glb data of the controller model, typically loaded from the driver.</param>
        /// <returns>True, if controller model is being properly rendered.</returns>
        internal async void TryRenderControllerModel(Type controllerType, byte[] glbData = null)
        {
            await TryRenderControllerModelAsync(controllerType, glbData);
        }

        /// <summary>
        /// Attempts to load the controller model render settings from the <see cref="MixedRealityControllerVisualizationProfile"/>
        /// to render the controllers in the scene.
        /// </summary>
        /// <param name="controllerType">The controller type.</param>
        /// <param name="glbData">The raw binary glb data of the controller model, typically loaded from the driver.</param>
        /// <returns>True, if controller model is being properly rendered.</returns>
        internal async Task TryRenderControllerModelAsync(Type controllerType, byte[] glbData = null)
        {
            Debug.Log("Attempting to render controller models...");
            var visualizationProfile = MixedRealityToolkit.Instance.ActiveProfile.InputSystemProfile.ControllerVisualizationProfile;

            if (visualizationProfile == null)
            {
                Debug.LogError("Missing ControllerVisualizationProfile!");
                return;
            }

            if (!visualizationProfile.RenderMotionControllers) { return; }

            GameObject controllerModel = null;

            // If a specific controller template wants to override the global model, assign that instead.
            if (!visualizationProfile.UseDefaultModels)
            {
                Debug.Log("Getting the override models");
                controllerModel = visualizationProfile.GetControllerModelOverride(controllerType, ControllerHandedness);
            }

            // Attempt to load the controller model from glbData.
            if (controllerModel == null && glbData != null)
            {
                Debug.Log("Attempting to load glb data...");
                var gltfObject = GltfUtility.GetGltfObjectFromGlb(glbData);
                await gltfObject.ConstructAsync();
                controllerModel = gltfObject.GameObjectReference;
                controllerModel.transform.SetParent(MixedRealityToolkit.Instance.MixedRealityPlayspace.transform);
                controllerModel.AddComponent(visualizationProfile.ControllerVisualizationType.Type);
                Visualizer = controllerModel.GetComponent<IMixedRealityControllerVisualizer>();

                if (Visualizer != null)
                {
                    Visualizer.Controller = this;
                    return; // Nothing left to do;
                }

                Debug.LogError("Failed to load controller model from driver!");
            }

            // If we didn't get an override model, and we didn't load the driver model,
            // then get the global controller model for each hand.
            if (controllerModel == null)
            {
                Debug.Log("Setting the fallback models");
                if (ControllerHandedness == Handedness.Left && visualizationProfile.GlobalLeftHandModel != null)
                {
                    controllerModel = visualizationProfile.GlobalLeftHandModel;
                }
                else if (ControllerHandedness == Handedness.Right && visualizationProfile.GlobalRightHandModel != null)
                {
                    controllerModel = visualizationProfile.GlobalRightHandModel;
                }
            }

            // If we've got a controller model prefab, then place it in the scene.
            if (controllerModel != null)
            {
                Debug.Log("Attempting to Instantiate Model Prefab...");
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

            if (Visualizer == null)
            {
                Debug.LogError("Failed to render controller model!");
            }
        }
    }
}