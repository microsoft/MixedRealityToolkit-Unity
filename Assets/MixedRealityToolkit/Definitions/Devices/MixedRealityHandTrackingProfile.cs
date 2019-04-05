// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;
using UnityEngine.Serialization;

namespace Microsoft.MixedReality.Toolkit.Input
{
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Mixed Reality Hand Tracking Profile", fileName = "MixedRealityHandTrackingProfile", order = (int)CreateProfileMenuItemIndices.HandTracking)]
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

        [SerializeField]
        [Tooltip("If true and the hand mesh is available, try to access the hand mesh from the system. Note: this could reduce performance")]
        [FormerlySerializedAs("enableHandMeshUpdates")]
        private bool enableHandMeshVisualization = false;
        public bool EnableHandMeshVisualization
        {
            get
            {
                return enableHandMeshVisualization;
            }

            set
            {
                enableHandMeshVisualization = value;
            }
        }

        [SerializeField]
        [Tooltip("Renders the hand joints. Note: this could reduce performance")]
        private bool enableHandJointVisualization = false;
        public bool EnableHandJointVisualization
        {
            get
            {
                return enableHandJointVisualization;
            }

            set
            {
                enableHandJointVisualization = value;
            }
        }

        
    }
}