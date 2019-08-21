// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.SpatialAwareness;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.SpatialObjectMeshObserver
{
    /// <summary>
    /// Configuration profile for the spatial object mesh observer.
    /// </summary>
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Profiles/Spatial Object Mesh Observer Profile", fileName = "SpatialObjectMeshObserverProfile", order = 100)]
    public class SpatialObjectMeshObserverProfile : MixedRealitySpatialAwarenessMeshObserverProfile
    {
        [SerializeField]
        [Tooltip("The model containing the desired mesh data.")]
        private GameObject spatialMeshObject = null;

        /// <summary>
        /// The model containing the desired mesh data.
        /// </summary>
        public GameObject SpatialMeshObject => spatialMeshObject;
    }
}