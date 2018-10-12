// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Definitions.SpatialAwarenessSystem;
using Microsoft.MixedReality.Toolkit.Core.EventDatum.SpatialAwarenessSystem;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.SpatialAwarenessSystem.Handlers;
using Microsoft.MixedReality.Toolkit.Core.Managers;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.SDK.SpatialAwarenessSystem.Handlers
{
    /// <summary>
    /// Class providing the default implementation of the <see cref="IMixedRealitySpatialAwarenessMeshHandler"/> interface.
    /// </summary>
    public class MixedRealitySpatialAwarenessMeshHandler : MonoBehaviour, IMixedRealitySpatialAwarenessMeshHandler
    {
        /// <inheritdoc />
        public void OnMeshAdded(MixedRealitySpatialAwarenessEventData eventData)
        {
            //ApplyMeshMaterial(eventData.GameObject);
            eventData.GameObject.SetActive(true);
        }

        /// <inheritdoc />
        public void OnMeshUpdated(MixedRealitySpatialAwarenessEventData eventData)
        {
            //ApplyMeshMaterial(eventData.GameObject);
            eventData.GameObject.SetActive(true);
        }

        /// <inheritdoc />
        public void OnMeshRemoved(MixedRealitySpatialAwarenessEventData eventData)
        {
            eventData.GameObject.SetActive(false);
        }

        /// <summary>
        /// Applies the appropriate material based on the value of <see cref="MeshDisplayOption"/> and sets the appropriate
        /// visual state (active or inactive).
        /// </summary>
        /// <param name="mesh">The mesh <see cref="GameObject" on which the material is to be applied./></param>
        private void ApplyMeshMaterial(GameObject mesh)
        {
            // Set the appropriate material on the mesh.
            if (MixedRealityManager.SpatialAwarenessSystem.MeshDisplayOption == SpatialMeshDisplayOptions.None)
            {
                mesh.SetActive(false);
                return;
            }

            Renderer renderer = mesh.GetComponent<Renderer>();

            if (renderer != null)
            {
                renderer.sharedMaterial = (MixedRealityManager.SpatialAwarenessSystem.MeshDisplayOption == SpatialMeshDisplayOptions.Visible) ?
                    MixedRealityManager.SpatialAwarenessSystem.MeshVisibleMaterial :
                    MixedRealityManager.SpatialAwarenessSystem.MeshOcclusionMaterial;
                mesh.SetActive(true);
            }
        }
    }
}
