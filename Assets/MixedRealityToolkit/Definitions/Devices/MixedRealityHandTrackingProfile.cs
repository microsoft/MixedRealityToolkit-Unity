// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Microsoft.MixedReality.Toolkit.Input
{
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Profiles/Mixed Reality Hand Tracking Profile", fileName = "MixedRealityHandTrackingProfile", order = (int)CreateProfileMenuItemIndices.HandTracking)]
    [HelpURL("https://microsoft.github.io/MixedRealityToolkit-Unity/Documentation/InputSystem/HandTracking.html")]
    public class MixedRealityHandTrackingProfile : BaseMixedRealityProfile
    {
        [SerializeField]
        [Tooltip("The joint prefab to use.")]
        private GameObject jointPrefab = null;

        [SerializeField]
        [Tooltip("The joint prefab to use for palm.")]
        private GameObject palmPrefab = null;

        [SerializeField]
        [Tooltip("The joint prefab to use for the index tip (point of interaction.")]
        private GameObject fingertipPrefab = null;

        /// <summary>
        /// The joint prefab to use.
        /// </summary>
        public GameObject JointPrefab => jointPrefab;

        /// <summary>
        /// The joint prefab to use for palm
        /// </summary>
        public GameObject PalmJointPrefab => palmPrefab;

        /// <summary>
        /// The joint prefab to use for finger tip
        /// </summary>
        public GameObject FingerTipPrefab => fingertipPrefab;

        [SerializeField]
        [Tooltip("If this is not null and hand system supports hand meshes, use this mesh to render hand mesh.")]
        private GameObject handMeshPrefab = null;

        /// <summary>
        /// The hand mesh prefab to use to render the hand
        /// </summary>
        public GameObject HandMeshPrefab => handMeshPrefab;

        /// <summary>
        /// The hand mesh visualization enable/disable state of the current application mode.
        /// </summary>
        /// <remarks>
        /// If this property is called while in-editor, this will only affect the in-editor settings
        /// (i.e. the SupportedApplicationModes.Editor flag of HandMeshVisualizationModes).
        /// If this property is called while in-player, this will only affect the in-player settings
        /// (i.e. the SupportedApplicationModes.Player flag of HandMeshVisualizationModes).
        /// </remarks>
        public bool EnableHandMeshVisualization
        {
            get
            {
                return PlatformUtility.IsSupportedApplicationMode(handMeshVisualizationModes);
            }

            set
            {
                handMeshVisualizationModes = PlatformUtility.UpdateSupportedApplicationMode(value, handMeshVisualizationModes);
            }
        }

        /// <summary>
        /// The hand joint visualization enable/disable state of the current application mode.
        /// </summary>
        /// <remarks>
        /// If this property is called while in-editor, this will only affect the in-editor settings
        /// (i.e. the SupportedApplicationModes.Editor flag of HandJointVisualizationModes).
        /// If this property is called while in-player, this will only affect the in-player settings
        /// (i.e. the SupportedApplicationModes.Player flag of HandJointVisualizationModes).
        /// </remarks>
        public bool EnableHandJointVisualization
        {
            get
            {
                return PlatformUtility.IsSupportedApplicationMode(handJointVisualizationModes);
            }

            set
            {
                handJointVisualizationModes = PlatformUtility.UpdateSupportedApplicationMode(value, handJointVisualizationModes);
            }
        }

        [EnumFlags]
        [SerializeField]
        [Tooltip("The application modes in which hand mesh visualizations will display. " +
                 "Will only show if the system provides hand mesh data. Note: this could reduce performance")]
        private SupportedApplicationModes handMeshVisualizationModes = 0;
        public SupportedApplicationModes HandMeshVisualizationModes
        {
            get
            {
                return handMeshVisualizationModes;
            }
            set
            {
                handMeshVisualizationModes = value;
            }
        }

        [EnumFlags]
        [SerializeField]
        [Tooltip("The application modes in which hand joint visualizations will display. " +
                 "Will only show if the system provides joint data. Note: this could reduce performance")]
        private SupportedApplicationModes handJointVisualizationModes = 0;
        public SupportedApplicationModes HandJointVisualizationModes
        {
            get
            {
                return handJointVisualizationModes;
            }
            set
            {
                handJointVisualizationModes = value;
            }
        }
    }
}