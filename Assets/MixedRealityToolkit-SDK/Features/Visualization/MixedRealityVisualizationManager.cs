// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Definitions.Utilities;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.Devices;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.VisualizationSystem;
using Microsoft.MixedReality.Toolkit.Core.Managers;
using Microsoft.MixedReality.Toolkit.Core.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Microsoft.MixedReality.Toolkit.SDK.VisualizationSystem
{
    /// <summary>
    /// The Visualization system controls the presentation and display of controllers in a scene.
    /// </summary>
    public class MixedRealityVisualizationManager : MixedRealityEventManager, IMixedRealityVisualizationSystem
    {
        #region IMixedRealityVisualizationSystem Implementation
        /// <inheritdoc />
        public HashSet<IMixedRealityVisualizer> DetectedVisualizers { get; } = new HashSet<IMixedRealityVisualizer>();

        #endregion IMixedRealityVisualizationSystem Implementation

        #region IMixedRealityManager Implementation

        /// <inheritdoc/>
        public override void Initialize()
        {
            base.Initialize();
            InitializeInternal();
        }

        /// <summary>
        /// Performs initialization tasks for the BoundaryManager.
        /// </summary>
        private void InitializeInternal()
        {

        }

        /// <inheritdoc/>
        public override void Reset()
        {
            base.Reset();
            InitializeInternal();
        }

        public override void Destroy()
        {
            // Cleanup game objects created during execution.
            if (Application.isPlaying)
            {
            }
        }

        #endregion IMixedRealityManager Implementation

        #region IMixedRealtyEventSystem Implementation

        /// <inheritdoc />
        public override void HandleEvent<T>(BaseEventData eventData, ExecuteEvents.EventFunction<T> eventHandler)
        {
            base.HandleEvent(eventData, eventHandler);
        }

        /// <summary>
        /// Registers the <see cref="GameObject"/> to listen for boundary events.
        /// </summary>
        /// <param name="listener"></param>
        public override void Register(GameObject listener)
        {
            base.Register(listener);
        }

        /// <summary>
        /// UnRegisters the <see cref="GameObject"/> to listen for boundary events.
        /// /// </summary>
        /// <param name="listener"></param>
        public override void Unregister(GameObject listener)
        {
            base.Unregister(listener);
        }

        #endregion

        public void RegisterVisualizerForController(IMixedRealityController controller)
        {
            GameObject controllerModel = null;

            if (!MixedRealityManager.Instance.ActiveProfile.InputSystemProfile.VisualizationProfile.RenderMotionControllers) { return; }

            // If a specific controller template wants to override the global model, assign that instead.
            if (MixedRealityManager.Instance.ActiveProfile.InputSystemProfile.IsControllerMappingEnabled &&
                !MixedRealityManager.Instance.ActiveProfile.InputSystemProfile.VisualizationProfile.UseDefaultModels)
            {
                // TODO: Test type passes correctly.
                controllerModel = MixedRealityManager.Instance.ActiveProfile.InputSystemProfile.VisualizationProfile.GetControllerModelOverride(controller.GetType(), controller.ControllerHandedness);
            }

            // Get the global controller model for each hand.
            if (controllerModel == null)
            {
                if (controller.ControllerHandedness == Handedness.Left && MixedRealityManager.Instance.ActiveProfile.InputSystemProfile.VisualizationProfile.GlobalLeftHandModel != null)
                {
                    controllerModel = MixedRealityManager.Instance.ActiveProfile.InputSystemProfile.VisualizationProfile.GlobalLeftHandModel;
                }
                else if (controller.ControllerHandedness == Handedness.Right && MixedRealityManager.Instance.ActiveProfile.InputSystemProfile.VisualizationProfile.GlobalRightHandModel != null)
                {
                    controllerModel = MixedRealityManager.Instance.ActiveProfile.InputSystemProfile.VisualizationProfile.GlobalRightHandModel;
                }
            }

            // If we've got a controller model prefab, then place it in the scene.
            if (controllerModel != null)
            {
                var controllerObject = UnityEngine.Object.Instantiate(controllerModel, CameraCache.Main.transform.parent);
                controllerObject.name = $"{controller.ControllerHandedness}_{controllerObject.name}";
                var controllerVisualizer = controllerObject.GetComponent<IMixedRealityVisualizer>();

                //if the prefab does not have a Visualizer script attached, add the configured componentS
                if (controllerVisualizer == null && MixedRealityManager.Instance.ActiveProfile.InputSystemProfile?.VisualizationProfile?.VisualizerType != null)
                {
                    controllerVisualizer = controllerObject.AddComponent(MixedRealityManager.Instance.ActiveProfile.InputSystemProfile.VisualizationProfile.VisualizerType.Type) as IMixedRealityVisualizer;
                }

                if (controllerVisualizer != null)
                { 
                    controllerVisualizer.Controller = controller;
                    controllerVisualizer.VisualizationManager = this;
                    DetectedVisualizers.Add(controllerVisualizer);
                }
                else
                {
                    Debug.LogError($"{controllerObject.name} is missing a IMixedRealityControllerVisualizer component!");
                }
            }
        }
    }
}
