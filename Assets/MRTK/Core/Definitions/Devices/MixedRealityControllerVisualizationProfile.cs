// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Profile that determines relevant overrides and properties for controller visualization
    /// </summary>
    [CreateAssetMenu(menuName = "Mixed Reality/Toolkit/Profiles/Mixed Reality Controller Visualization Profile", fileName = "MixedRealityControllerVisualizationProfile", order = (int)CreateProfileMenuItemIndices.ControllerVisualization)]
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
            get => renderMotionControllers;
            private set => renderMotionControllers = value;
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
            get => defaultControllerVisualizationType;
            private set => defaultControllerVisualizationType = value;
        }

        [SerializeField]
        [Tooltip("Check to obtain controller models from the platform SDK. If left unchecked, the global models will be used. Note: this value is overridden by controller definitions.")]
        [FormerlySerializedAs("useDefaultModels")]
        private bool usePlatformModels = false;

        /// <summary>
        /// Check to obtain controller models from the platform SDK. If left unchecked, the global models will be used. Note: this value is overridden by controller definitions.
        /// </summary>
        public bool UsePlatformModels
        {
            get => usePlatformModels;
            private set => usePlatformModels = value;
        }

        /// <summary>
        /// Check to obtain controller models from the platform SDK. If left unchecked, the global models will be used. Note: this value is overridden by controller definitions.
        /// </summary>
        [Obsolete("Use UsePlatformModels instead.")]
        public bool UseDefaultModels => usePlatformModels;

        [SerializeField]
        [Tooltip("The default controller model material when loading platform SDK controller models. This value is used as a fallback if no controller definition exists with a custom material type.")]
        [FormerlySerializedAs("defaultControllerModelMaterial")]
        private Material platformModelMaterial;

        /// <summary>
        /// The default controller model material when loading platform SDK controller models. This value is used as a fallback if no controller definition exists with a custom material type.
        /// </summary>
        public Material PlatformModelMaterial
        {
            get => platformModelMaterial;
            private set => platformModelMaterial = value;
        }

        /// <summary>
        /// The default controller model material when loading platform SDK controller models. This value is used as a fallback if no controller definition exists with a custom material type.
        /// </summary>
        [Obsolete("Use PlatformModelMaterial instead.")]
        public Material DefaultControllerModelMaterial => platformModelMaterial;

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
            get => globalLeftControllerModel;
            private set => globalLeftControllerModel = value;
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
            get => globalRightControllerModel;
            private set => globalRightControllerModel = value;
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
            get => globalLeftHandVisualizer;
            private set => globalLeftHandVisualizer = value;
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
            get => globalRightHandVisualizer;
            private set => globalRightHandVisualizer = value;
        }

        [SerializeField]
        private MixedRealityControllerVisualizationSetting[] controllerVisualizationSettings = Array.Empty<MixedRealityControllerVisualizationSetting>();

        /// <summary>
        /// The current list of controller visualization settings.
        /// </summary>
        public MixedRealityControllerVisualizationSetting[] ControllerVisualizationSettings => controllerVisualizationSettings;

        private MixedRealityControllerVisualizationSetting? GetControllerVisualizationDefinition(Type controllerType, Handedness hand)
        {
            for (int i = 0; i < controllerVisualizationSettings.Length; i++)
            {
                if (SettingContainsParameters(controllerVisualizationSettings[i], controllerType, hand))
                {
                    return controllerVisualizationSettings[i];
                }
            }
            return null;
        }

        /// <summary>
        /// Gets the override model for a specific controller type and hand
        /// </summary>
        /// <param name="controllerType">The type of controller to query for</param>
        /// <param name="hand">The specific hand assigned to the controller</param>
        public GameObject GetControllerModelOverride(Type controllerType, Handedness hand)
        {
            MixedRealityControllerVisualizationSetting? setting = GetControllerVisualizationDefinition(controllerType, hand);
            return setting.HasValue ? setting.Value.OverrideControllerModel : null;
        }

        /// <summary>
        /// Gets the override <see cref="IMixedRealityControllerVisualizer"/> type for a specific controller type and hand.
        /// If the requested controller type is not defined, DefaultControllerVisualizationType is returned.
        /// </summary>
        /// <param name="controllerType">The type of controller to query for</param>
        /// <param name="hand">The specific hand assigned to the controller</param>
        public SystemType GetControllerVisualizationTypeOverride(Type controllerType, Handedness hand)
        {
            MixedRealityControllerVisualizationSetting? setting = GetControllerVisualizationDefinition(controllerType, hand);
            return setting.HasValue ? setting.Value.ControllerVisualizationType : DefaultControllerVisualizationType;
        }

        /// <summary>
        /// Gets the UsePlatformModels value defined for the specified controller definition.
        /// If the requested controller type is not defined, the default UsePlatformModels is returned.
        /// </summary>
        /// <param name="controllerType">The type of controller to query for</param>
        /// <param name="hand">The specific hand assigned to the controller</param>
        /// <remarks>
        /// GetUseDefaultModelsOverride is obsolete and will be removed in a future Mixed Reality Toolkit release. Please use GetUsePlatformModelsOverride.
        /// </remarks>
        [Obsolete("GetUseDefaultModelsOverride is obsolete and will be removed in a future Mixed Reality Toolkit release. Please use GetUsePlatformModelsOverride.")]
        public bool GetUseDefaultModelsOverride(Type controllerType, Handedness hand)
        {
            return GetUsePlatformModelsOverride(controllerType, hand);
        }

        /// <summary>
        /// Gets the UsePlatformModels value defined for the specified controller definition.
        /// If the requested controller type is not defined, the default UsePlatformModels is returned.
        /// </summary>
        /// <param name="controllerType">The type of controller to query for</param>
        /// <param name="hand">The specific hand assigned to the controller</param>
        public bool GetUsePlatformModelsOverride(Type controllerType, Handedness hand)
        {
            MixedRealityControllerVisualizationSetting? setting = GetControllerVisualizationDefinition(controllerType, hand);
            return setting.HasValue ? setting.Value.UsePlatformModels : usePlatformModels;
        }

        /// <summary>
        /// Gets the DefaultModelMaterial value defined for the specified controller definition.
        /// If the requested controller type is not defined, the global platformModelMaterial is returned.
        /// </summary>
        /// <param name="controllerType">The type of controller to query for</param>
        /// <param name="hand">The specific hand assigned to the controller</param>
        /// <remarks>
        /// GetDefaultControllerModelMaterialOverride is obsolete and will be removed in a future Mixed Reality Toolkit release. Please use GetPlatformModelMaterialOverride.
        /// </remarks>
        [Obsolete("GetDefaultControllerModelMaterialOverride is obsolete and will be removed in a future Mixed Reality Toolkit release. Please use GetPlatformModelMaterial.")]
        public Material GetDefaultControllerModelMaterialOverride(Type controllerType, Handedness hand)
        {
            return GetPlatformModelMaterialOverride(controllerType, hand);
        }

        /// <summary>
        /// Gets the PlatformModelMaterial value defined for the specified controller definition.
        /// If the requested controller type is not defined, the global platformModelMaterial is returned.
        /// </summary>
        /// <param name="controllerType">The type of controller to query for</param>
        /// <param name="hand">The specific hand assigned to the controller</param>
        public Material GetPlatformModelMaterialOverride(Type controllerType, Handedness hand)
        {

            MixedRealityControllerVisualizationSetting? setting = GetControllerVisualizationDefinition(controllerType, hand);
            return setting.HasValue ? setting.Value.PlatformModelMaterial : platformModelMaterial;
        }

        private bool SettingContainsParameters(MixedRealityControllerVisualizationSetting setting, Type controllerType, Handedness hand)
        {
            return setting.ControllerType != null &&
                setting.ControllerType.Type == controllerType &&
                setting.Handedness.IsMaskSet(hand) && setting.Handedness != Handedness.None;
        }
    }
}
