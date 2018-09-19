// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Attributes;
using Microsoft.MixedReality.Toolkit.Core.Definitions.Utilities;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.Devices;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.VisualizationSystem;
using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Definitions.Visualization
{
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Mixed Reality Visualization Profile", fileName = "MixedRealityVisualizationProfile", order = (int)CreateProfileMenuItemIndices.ControllerVisualization)]
    public class MixedRealityVisualizationProfile : ScriptableObject
    {
        [SerializeField]
        [Tooltip("Enable and configure the controller rendering of the Motion Controllers on Startup.")]
        private bool renderMotionControllers = false;

        /// <summary>
        /// Enable and configure the controller rendering of the Motion Controllers on Startup.
        /// </summary>
        public bool RenderMotionControllers
        {
            get { return renderMotionControllers; }
            private set { renderMotionControllers = value; }
        }

        [SerializeField]
        [Implements(typeof(IMixedRealityVisualizationSystem), TypeGrouping.ByNamespaceFlat)]
        [Tooltip("The concrete Visualizer Manager to use for maintaining active visualizers in the scene.")]
        private SystemType visualizationManager;

        /// <summary>
        /// The concrete Visualizer Manager to use for maintaining active visualizers in the scene
        /// </summary>
        public SystemType VisualizationManager
        {
            get { return visualizationManager; }
            private set { visualizationManager = value; }
        }

        [SerializeField]
        [Implements(typeof(IMixedRealityVisualizer), TypeGrouping.ByNamespaceFlat)]
        [Tooltip("The concrete Visualizer component to use on the rendered controller model.")]
        private SystemType visualizerType;

        /// <summary>
        /// The concrete Controller Visualizer component to use on the rendered controller model
        /// </summary>
        public SystemType VisualizerType
        {
            get { return visualizerType; }
            private set { visualizerType = value; }
        }

        [SerializeField]
        [Tooltip("Use the platform SDK to load the default controller models.")]
        private bool useDefaultModels = false;

        /// <summary>
        /// User the controller model loader provided by the SDK, or provide override models.
        /// </summary>
        public bool UseDefaultModels
        {
            get { return useDefaultModels; }
            private set { useDefaultModels = value; }
        }

        [SerializeField]
        [Tooltip("Override Left Controller Model.")]
        private GameObject globalLeftHandModel;

        /// <summary>
        /// The Default controller model when there is no specific controller model for the Left hand or when no hand is specified (Handedness = none)
        /// </summary>
        /// <remarks>
        /// If the default model for the left hand controller can not be found, the controller will fall back and use this for visualization.
        /// </remarks>
        public GameObject GlobalLeftHandModel
        {
            get { return globalLeftHandModel; }
            private set { globalLeftHandModel = value; }
        }

        [SerializeField]
        [Tooltip("Override Right Controller Model.\nNote: If the default model is not found, the fallback is the global right hand model.")]
        private GameObject globalRightHandModel;

        /// <summary>
        /// The Default controller model when there is no specific controller model for the Right hand.
        /// </summary>
        /// <remarks>
        /// If the default model for the right hand controller can not be found, the controller will fall back and use this for visualization.
        /// </remarks>
        public GameObject GlobalRightHandModel
        {
            get { return globalRightHandModel; }
            private set { globalRightHandModel = value; }
        }

        [SerializeField]
        private MixedRealityVisualizationSetting[] visualizationSettings = new MixedRealityVisualizationSetting[0];

        /// <summary>
        /// The current list of controller visualization settings.
        /// </summary>
        public MixedRealityVisualizationSetting[] VisualizationSettings => visualizationSettings;

        /// <summary>
        /// Gets the override model for a specific controller type and hand
        /// </summary>
        /// <param name="controllerType">The type of controller to query for</param>
        /// <param name="hand">The specific hand assigned to the controller</param>
        public GameObject GetControllerModelOverride(Type controllerType, Handedness hand)
        {
            for (int i = 0; i < visualizationSettings.Length; i++)
            {
                if (visualizationSettings[i].ControllerType != null &&
                    visualizationSettings[i].ControllerType.Type == controllerType &&
                   (visualizationSettings[i].Handedness == hand || visualizationSettings[i].Handedness == Handedness.Both))
                {
                    return visualizationSettings[i].OverrideControllerModel;
                }
            }

            return null;
        }
    }
}