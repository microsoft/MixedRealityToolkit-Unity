// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Internal.Utilities.Lines
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(LineBase))]
    public class LineFollower : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("The transform that will follow the point along the line.")]
        private Transform follower;

        [Range(0f, 1f)]
        [SerializeField]
        [Tooltip("Gets a point along the line at the specified normalized length.")]
        private float normalizedLength = 0f;

        [SerializeField]
        [HideInInspector]
        private LineBase source = null;

        private void OnValidate()
        {
            if (source == null)
            {
                source = GetComponent<LineBase>();
            }
        }

        private void Update()
        {
            Vector3 linePoint = source.GetPoint(normalizedLength);
            follower.position = linePoint;
        }
    }
}