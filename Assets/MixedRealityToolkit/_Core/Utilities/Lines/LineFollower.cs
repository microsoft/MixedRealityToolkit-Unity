// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Utilities.Lines.DataProviders;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Internal.Utilities.Lines
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(BaseMixedRealityLineDataProvider))]
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
        private BaseMixedRealityLineDataProvider source = null;

        private void OnValidate()
        {
            if (source == null)
            {
                source = GetComponent<BaseMixedRealityLineDataProvider>();
            }
        }

        private void Update()
        {
            Vector3 linePoint = source.GetPoint(normalizedLength);
            follower.position = linePoint;
        }
    }
}