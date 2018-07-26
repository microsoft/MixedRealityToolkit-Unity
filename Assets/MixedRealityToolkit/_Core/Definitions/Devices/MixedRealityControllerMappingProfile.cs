// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Definitions.Utilities;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Internal.Definitions.Devices
{
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Mixed Reality Controller Configuration Profile", fileName = "MixedRealityControllerConfigurationProfile", order = 2)]
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
        [Tooltip("Left Controller Model.")]
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
        [Tooltip("Left Controller Model Offset Pose.")]
        private MixedRealityPose leftHandModelPoseOffset = MixedRealityPose.ZeroIdentity;

        /// <summary>
        /// The offset pose for the left hand rendered model
        /// </summary>
        public MixedRealityPose LeftHandModelPoseOffset
        {
            get { return leftHandModelPoseOffset; }
            private set { leftHandModelPoseOffset = value; }
        }

        [SerializeField]
        [Tooltip("Right Controller Model.")]
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
        [Tooltip("Right Controller Model Offset Pose.")]
        private MixedRealityPose rightHandModelPoseOffset = MixedRealityPose.ZeroIdentity;

        /// <summary>
        /// The offset pose for the right hand rendered model
        /// </summary>
        public MixedRealityPose RightHandModelPoseOffset
        {
            get { return rightHandModelPoseOffset; }
            private set { rightHandModelPoseOffset = value; }
        }

        [SerializeField]
        [Tooltip("The list of controller templates your application can use.")]
        private MixedRealityControllerMapping[] mixedRealityControllerMappingProfiles = new MixedRealityControllerMapping[0];

        public MixedRealityControllerMapping[] MixedRealityControllerMappingProfiles => mixedRealityControllerMappingProfiles;
    }
}