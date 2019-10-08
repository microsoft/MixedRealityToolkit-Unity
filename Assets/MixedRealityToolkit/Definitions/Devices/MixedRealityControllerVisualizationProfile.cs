// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Profiles/Mixed Reality Controller Visualization Profile", fileName = "MixedRealityControllerVisualizationProfile", order = (int)CreateProfileMenuItemIndices.ControllerVisualization)]
    [MixedRealityServiceProfile(typeof(IMixedRealityControllerVisualizer))]
    public class MixedRealityControllerVisualizationProfile : BaseMixedRealityProfile
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
        [Implements(typeof(IMixedRealityControllerVisualizer), TypeGrouping.ByNamespaceFlat)]
        [Tooltip("The default controller visualization type. This value is used as a fallback if no controller definition exists with a custom visualization type.")]
        private SystemType defaultControllerVisualizationType;

        /// <summary>
        /// The default controller visualization type. This value is used as a fallback if no controller definition exists with a custom visualization type.
        /// </summary>
        public SystemType DefaultControllerVisualizationType
        {
            get { return defaultControllerVisualizationType; }
            private set { defaultControllerVisualizationType = value; }
        }

        [SerializeField]
        [Tooltip("Check to obtain controller models from the platform sdk. If left unchecked, the global models will be used. Note: this value is overridden by controller definitions.")]
        private bool useDefaultModels = false;

        /// <summary>
        /// Check to obtain controller models from the platform sdk. If left unchecked, the global models will be used. Note: this value is overridden by controller definitions.
        /// </summary>
        public bool UseDefaultModels
        {
            get { return useDefaultModels; }
            private set { useDefaultModels = value; }
        }

        [SerializeField]
        [Tooltip("Override Left Controller Model.")]
        private GameObject globalLeftControllerModel;

        /// <summary>
        /// The Default controller model when there is no specific controller model for the Left hand or when no hand is specified (Handedness = none)
        /// </summary>
        /// <remarks>
        /// If the default model for the left hand controller can not be found, the controller will fall back and use this for visualization.
        /// </remarks>
        public GameObject GlobalLeftHandModel
        {
            get { return globalLeftControllerModel; }
            private set { globalLeftControllerModel = value; }
        }

        [SerializeField]
        [Tooltip("Override Right Controller Model.\nNote: If the default model is not found, the fallback is the global right hand model.")]
        private GameObject globalRightControllerModel;

        /// <summary>
        /// The Default controller model when there is no specific controller model for the Right hand.
        /// </summary>
        /// <remarks>
        /// If the default model for the right hand controller can not be found, the controller will fall back and use this for visualization.
        /// </remarks>
        public GameObject GlobalRightHandModel
        {
            get { return globalRightControllerModel; }
            private set { globalRightControllerModel = value; }
        }

        [SerializeField]
        [Tooltip("Override Left Hand Visualizer.")]
        private GameObject globalLeftHandVisualizer;

        /// <summary>
        /// The Default controller model when there is no specific controller model for the Left hand or when no hand is specified (Handedness = none)
        /// </summary>
        /// <remarks>
        /// If the default model for the left hand controller can not be found, the controller will fall back and use this for visualization.
        /// </remarks>
        public GameObject GlobalLeftHandVisualizer
        {
            get { return globalLeftHandVisualizer; }
            private set { globalLeftHandVisualizer = value; }
        }

        [SerializeField]
        [Tooltip("Override Right Controller Model.\nNote: If the default model is not found, the fallback is the global right hand model.")]
        private GameObject globalRightHandVisualizer;

        /// <summary>
        /// The Default hand model when there is no specific hand model for the Right hand.
        /// </summary>
        /// <remarks>
        /// If the default model for the right hand can not be found, the hand will fall back and use this for visualization.
        /// </remarks>
        public GameObject GlobalRightHandVisualizer
        {
            get { return globalRightHandVisualizer; }
            private set { globalRightHandVisualizer = value; }
        }

        [SerializeField]
        private MixedRealityControllerVisualizationSetting[] controllerVisualizationSettings = new MixedRealityControllerVisualizationSetting[0];

        /// <summary>
        /// The current list of controller visualization settings.
        /// </summary>
        public MixedRealityControllerVisualizationSetting[] ControllerVisualizationSettings => controllerVisualizationSettings;

        /// <summary>
        /// Gets the override model for a specific controller type and hand
        /// </summary>
        /// <param name="controllerType">The type of controller to query for</param>
        /// <param name="hand">The specific hand assigned to the controller</param>
        public GameObject GetControllerModelOverride(Type controllerType, Handedness hand)
        {
            for (int i = 0; i < controllerVisualizationSettings.Length; i++)
            {
                if (SettingContainsParameters(controllerVisualizationSettings[i], controllerType, hand))
                {
                    return controllerVisualizationSettings[i].OverrideControllerModel;
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the override <see cref="IMixedRealityControllerVisualizer"/> type for a specific controller type and hand.
        /// If the requested controller type is not defined, DefaultControllerVisualizationType is returned.
        /// </summary>
        /// <param name="controllerType">The type of controller to query for</param>
        /// <param name="hand">The specific hand assigned to the controller</param>
        public SystemType GetControllerVisualizationTypeOverride(Type controllerType, Handedness hand)
        {
            for (int i = 0; i < controllerVisualizationSettings.Length; i++)
            {
                if (SettingContainsParameters(controllerVisualizationSettings[i], controllerType, hand))
                {
                    return controllerVisualizationSettings[i].ControllerVisualizationType;
                }
            }

            return defaultControllerVisualizationType;
        }

        /// <summary>
        /// Gets the UseDefaultModels value defined for the specified controller definition.
        /// If the requested controller type is not defined, the default UseDefaultModels is returned.
        /// </summary>
        /// <param name="controllerType">The type of controller to query for</param>
        /// <param name="hand">The specific hand assigned to the controller</param>
        public bool GetUseDefaultModelsOverride(Type controllerType, Handedness hand)
        {
            for (int i = 0; i < controllerVisualizationSettings.Length; i++)
            {
                if (SettingContainsParameters(controllerVisualizationSettings[i], controllerType, hand))
                {
                    return controllerVisualizationSettings[i].UseDefaultModel;
                }
            }

            return useDefaultModels;
        }

        private bool SettingContainsParameters(MixedRealityControllerVisualizationSetting setting, Type controllerType, Handedness hand)
        {
            return setting.ControllerType != null &&
                setting.ControllerType.Type == controllerType &&
                (setting.Handedness == hand || setting.Handedness == Handedness.Both);
        }
    }
}