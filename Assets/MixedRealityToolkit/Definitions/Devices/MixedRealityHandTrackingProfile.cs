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

        public bool EnableHandMeshVisualization
        {
            get
            {
                return IsSupportedApplicationMode(handMeshVisualizationModes);
            }

            set
            {
                handMeshVisualizationModes = UpdateSupportedApplicationMode(value, handMeshVisualizationModes);
            }
        }

        public bool EnableHandJointVisualization
        {
            get
            {
                return IsSupportedApplicationMode(handJointVisualizationModes);
            }

            set
            {
                handJointVisualizationModes = UpdateSupportedApplicationMode(value, handJointVisualizationModes);
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

        /// <summary>
        /// Returns true if the modes specified by the specified SupportedApplicationModes matches
        /// the current mode that the code is running in.
        /// </summary>
        /// <remarks>
        /// For example, if the code is currently running in editor mode (for testing in-editor
        /// simulation), this would return true if modes contained the SupportedApplicationModes.Editor 
        /// bit.
        /// </remarks>
        private static bool IsSupportedApplicationMode(SupportedApplicationModes modes)
        {
#if UNITY_EDITOR
            return (modes & SupportedApplicationModes.Editor) != 0;
#else // !UNITY_EDITOR
            return (modes & SupportedApplicationModes.Player) != 0;
#endif
        }

        /// <summary>
        /// Updates the given SupportedApplicationModes by setting the bit associated with the
        /// currently active application mode.
        /// </summary>
        /// <remarks>
        /// For example, if the code is currently running in editor mode (for testing in-editor
        /// simulation), and modes is currently SupportedApplicationModes.Player | SupportedApplicationModes.Editor
        /// and enabled is 'false', this would return SupportedApplicationModes.Player.
        /// </remarks>
        private static SupportedApplicationModes UpdateSupportedApplicationMode(bool enabled, SupportedApplicationModes modes)
        {
#if UNITY_EDITOR
            var bitValue = enabled ? SupportedApplicationModes.Editor : 0;
            return (modes & ~SupportedApplicationModes.Editor) | bitValue;
#else // !UNITY_EDITOR
            var bitValue = enabled ? SupportedApplicationModes.Player : 0;
            return (modes & ~SupportedApplicationModes.Player) | bitValue;
#endif
        }
    }
}