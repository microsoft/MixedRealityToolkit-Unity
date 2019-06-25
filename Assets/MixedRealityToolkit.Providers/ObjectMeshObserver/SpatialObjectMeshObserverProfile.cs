// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.SpatialAwareness;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.SpatialObjectMeshObserver
{
    /// <summary>
    /// Configuration profile for the spatial object mesh observer.
    /// </summary>
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Object Mesh Observer Profile", fileName = "ObjectMeshObserverProfile", order = 1000)] // todo
    public class SpatialObjectMeshObserverProfile : MixedRealitySpatialAwarenessMeshObserverProfile
    {
        [SerializeField]
        [Tooltip("The model containing the desired mesh data.")]
        private Mesh objectMesh = null;

        public Mesh ObjectMesh => objectMesh;
    }
}