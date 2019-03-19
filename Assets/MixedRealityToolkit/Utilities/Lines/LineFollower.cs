// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities
{
    /// <summary>
    /// Synchronizes the follower's transform position with the point along the line.
    /// </summary>
    [ExecuteInEditMode]
    public class LineFollower : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("The transform that will follow the point along the line.")]
        private Transform follower;

        /// <summary>
        /// The transform that will follow the point along the line.
        /// </summary>
        public Transform Follower
        {
            get
            {
                if (follower == null)
                {
                    follower = transform;
                }

                return follower;
            }
            set
            {
                follower = value == null ? transform : value;
            }
        }

        [Range(0f, 1f)]
        [SerializeField]
        [Tooltip("Gets a point along the line at the specified normalized length.")]
        private float normalizedLength = 0f;

        /// <summary>
        /// Gets a point along the line at the specified normalized length.
        /// </summary>
        public float NormalizedLength
        {
            get { return normalizedLength; }
            set
            {
                if (value < 0f)
                {
                    normalizedLength = 0f;
                }
                else if (value > 1f)
                {
                    normalizedLength = 1f;
                }
                else
                {
                    normalizedLength = value;
                }
            }
        }

        [SerializeField]
        [HideInInspector]
        private BaseMixedRealityLineDataProvider source = null;

        #region MonoBehaviour Implementation

        private void OnValidate() => EnsureSetup();

        private void Awake() => EnsureSetup();

        private void Update()
        {
            if (source == null || follower == null) { return; }

            Vector3 linePoint = source.GetPoint(normalizedLength);
            follower.position = linePoint;
        }

        #endregion MonoBehaviour Implementation

        private void EnsureSetup()
        {
            if (follower == null)
            {
                follower = transform;
            }

            if (source == null)
            {
                source = GetComponent<BaseMixedRealityLineDataProvider>();
            }

            if (source == null)
            {
                Debug.LogError($"Missing a Mixed Reality Line Data Provider for Line Follower on {gameObject.name}");
            }
        }
    }
}