// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Physics;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;
using UnityEngine.Serialization;
using UnityPhysics = UnityEngine.Physics;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Configuration profile settings for setting up and consuming Input Actions.
    /// </summary>
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Profiles/Mixed Reality Gaze Profile", fileName = "MixedRealityGazeProfile", order = (int)CreateProfileMenuItemIndices.InputActions)]
    public class MixedRealityGazeProfile : BaseMixedRealityProfile
    {
        [Prefab]
        [SerializeField]
        [Tooltip("The gaze cursor prefab to use on the Gaze pointer.")]
        private GameObject gazeCursorPrefab = null;

        /// <summary>
        /// The gaze cursor prefab to use on the Gaze pointer.
        /// </summary>
        public GameObject GazeCursorPrefab => gazeCursorPrefab;

        [SerializeField]
        [Tooltip("If true, the gaze cursor will disappear when the pointer's focus is locked, to prevent the cursor from floating idly in the world.")]
        private bool setCursorInvisibleWhenFocusLocked = false;

        /// <summary>
        /// If true, the gaze cursor will disappear when the pointer's focus is locked, to prevent the cursor from floating idly in the world.
        /// </summary>
        public bool SetCursorInvisibleWhenFocusLocked
        {
            get
            {
                return setCursorInvisibleWhenFocusLocked;
            }
            set
            {
                setCursorInvisibleWhenFocusLocked = value;
            }
        }

        /// <summary>
        /// Maximum distance at which the gaze can hit a GameObject.
        /// </summary>
        [SerializeField]
        [Tooltip("Maximum distance at which the gaze can hit a GameObject.")]
        private float maxGazeCollisionDistance = 10.0f;

        public float MaxGazeCollisionDistance
        {
            get
            {
                return maxGazeCollisionDistance;
            }
            set
            {
                maxGazeCollisionDistance = value;
            }
        }

        [SerializeField]
        [Tooltip("The LayerMasks, in prioritized order, that are used to determine the GazeTarget when raycasting.")]
        private LayerMask[] raycastLayerMasks = { UnityPhysics.DefaultRaycastLayers };

        /// <summary>
        /// The LayerMasks, in prioritized order, that are used to determine the GazeTarget when raycasting.
        /// <example>
        /// <para>Allow the cursor to hit SR, but first prioritize any DefaultRaycastLayers (potentially behind SR)</para>
        /// <code language="csharp"><![CDATA[
        /// int sr = LayerMask.GetMask("SR");
        /// int nonSR = Physics.DefaultRaycastLayers &amp; ~sr;
        /// GazeProvider.Instance.RaycastLayerMasks = new LayerMask[] { nonSR, sr };
        /// ]]></code>
        /// </example>
        /// </summary>
        public LayerMask[] RaycastLayerMasks
        {
            get
            {
                return raycastLayerMasks;
            }
            set
            {
                raycastLayerMasks = value;
            }
        }


        [SerializeField]
        [Tooltip("Stabilizer, if any, used to smooth out the gaze ray data.")]
        private GazeStabilizer stabilizer = null;

        /// <summary>
        /// Current stabilization method, used to smooth out the gaze ray data.
        /// If left null, no stabilization will be performed.
        /// </summary>
        public GazeStabilizer Stabilizer
        {
            get
            {
                return stabilizer;
            }
            set
            {
                stabilizer = value;
            }
        }

        [SerializeField]
        [Tooltip("Transform that should be used to represent the gaze position and rotation. Defaults to CameraCache.Main")]
        private Transform gazeTransform = null;

        /// <summary>
        /// Transform that should be used as the source of the gaze position and rotation.
        /// Defaults to the main camera.
        /// </summary>
        public Transform GazeTransform
        {
            get
            {
                return gazeTransform;
            }
            set
            {
                gazeTransform = value;
            }
        }


        [SerializeField]
        [Range(0.01f, 1f)]
        [Tooltip("Minimum head velocity threshold")]
        private float minHeadVelocityThreshold = 0.5f;

        /// <summary>
        /// Minimum head velocity threshold.
        /// </summary>
        public float MinHeadVelocityThreshold
        {
            get
            {
                return minHeadVelocityThreshold;
            }
            set
            {
                minHeadVelocityThreshold = value;
            }
        }

        [SerializeField]
        [Range(0.1f, 5f)]
        [Tooltip("Maximum head velocity threshold")]
        private float maxHeadVelocityThreshold = 2f;

        /// <summary>
        /// Maximum head velocity threshold.
        /// </summary>
        public float MaxHeadVelocityThreshold
        {
            get
            {
                return maxHeadVelocityThreshold;
            }
            set
            {
                maxHeadVelocityThreshold = value;
            }
        }

        [SerializeField]
        [Tooltip("If true, eye-based tracking will be used when available. Requires the 'Gaze Input' permission and device eye calibration to have been run.")]
        [FormerlySerializedAs("preferEyeTracking")]
        private bool useEyeTracking = true;

        /// <summary>
        /// If true, eye-based tracking will be used when available.
        /// </summary>
        /// <remarks>
        /// The usage of eye-based tracking depends on having the Gaze Input permission set
        /// and user approved, along with proper device eye calibration. This will fallback to head-based
        /// gaze when eye-based tracking is not available.
        /// </remarks>
        public bool UseEyeTracking
        {
            get { return useEyeTracking; }
            set { useEyeTracking = value; }
        }
    }
}