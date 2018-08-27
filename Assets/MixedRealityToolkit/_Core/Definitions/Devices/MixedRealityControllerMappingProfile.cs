// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Definitions.Utilities;
using Microsoft.MixedReality.Toolkit.Internal.Devices.OpenVR;
using Microsoft.MixedReality.Toolkit.Internal.Devices.UnityInput;
using Microsoft.MixedReality.Toolkit.Internal.Devices.WindowsMixedReality;
using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Internal.Definitions.Devices
{
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Mixed Reality Controller Configuration Profile", fileName = "MixedRealityControllerConfigurationProfile", order = (int)CreateProfileMenuItemIndices.Controller)]
    public class MixedRealityControllerMappingProfile : ScriptableObject
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
        public GameObject GlobalLeftHandModel
        {
            get { return globalLeftHandModel; }
            private set { globalLeftHandModel = value; }
        }

        [SerializeField]
        [Tooltip("Override Right Controller Model.")]
        private GameObject globalRightHandModel;

        /// <summary>
        /// The Default controller model when there is no specific controller model for the Right hand
        /// </summary>
        public GameObject GlobalRightHandModel
        {
            get { return globalRightHandModel; }
            private set { globalRightHandModel = value; }
        }

        [SerializeField]
        [Tooltip("The list of controller templates your application can use.")]
        private MixedRealityControllerMapping[] mixedRealityControllerMappingProfiles =
        {
            new MixedRealityControllerMapping(0, "Xbox Controller", typeof(XboxController)),
            new MixedRealityControllerMapping(1, "Windows Mixed Reality Motion Controller Left", typeof(WindowsMixedRealityController), Handedness.Left),
            new MixedRealityControllerMapping(2, "Windows Mixed Reality Motion Controller Right", typeof(WindowsMixedRealityController), Handedness.Right),
            new MixedRealityControllerMapping(3, "Open VR Motion Controller Left", typeof(WindowsMixedRealityOpenVRMotionController), Handedness.Left),
            new MixedRealityControllerMapping(4, "Open VR Motion Controller Right", typeof(WindowsMixedRealityOpenVRMotionController), Handedness.Right),
            new MixedRealityControllerMapping(5, "Windows Mixed Reality Hand Gestures", typeof(WindowsMixedRealityController)),
            new MixedRealityControllerMapping(6, "Vive Wand Controller Left", typeof(ViveWandController), Handedness.Left),
            new MixedRealityControllerMapping(7, "Vive Wand Controller Right", typeof(ViveWandController), Handedness.Right),
            new MixedRealityControllerMapping(8, "Oculus Touch Controller Left", typeof(OculusTouchController), Handedness.Left),
            new MixedRealityControllerMapping(9, "Oculus Touch Controller Right", typeof(OculusTouchController), Handedness.Right),
            new MixedRealityControllerMapping(10, "Oculus Remote Controller", typeof(OculusRemoteController)),
            new MixedRealityControllerMapping(11, "Generic OpenVR Controller Left", typeof(GenericOpenVRController), Handedness.Left, true),
            new MixedRealityControllerMapping(12, "Generic OpenVR Controller Right", typeof(GenericOpenVRController), Handedness.Right, true),
        };

        public MixedRealityControllerMapping[] MixedRealityControllerMappingProfiles => mixedRealityControllerMappingProfiles;

        /// <summary>
        /// Gets the override model for a specific controller type and hand
        /// </summary>
        /// <param name="controllerType">The type of controller to query for</param>
        /// <param name="hand">The specific hand assigned to the controller</param>
        public GameObject GetControllerModelOverride(Type controllerType, Handedness hand)
        {
            for (int i = 0; i < mixedRealityControllerMappingProfiles.Length; i++)
            {
                if (mixedRealityControllerMappingProfiles[i].ControllerType != null &&
                    mixedRealityControllerMappingProfiles[i].ControllerType.Type == controllerType &&
                   (mixedRealityControllerMappingProfiles[i].Handedness == hand || mixedRealityControllerMappingProfiles[i].Handedness == Handedness.Both))
                {
                    return mixedRealityControllerMappingProfiles[i].OverrideControllerModel;
                }
            }

            return null;
        }

        private void Awake()
        {
            for (int i = 0; i < mixedRealityControllerMappingProfiles.Length; i++)
            {
                mixedRealityControllerMappingProfiles[i].SetDefaultInteractionMapping();
            }
        }
    }
}