// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using UnityEngine.VR.WSA;

public class SpatialMappingCollider : SMBaseAbstract
{
    /// <summary>
    /// The layer mask to apply to the mesh.
    /// </summary>
    public LayerMask MeshLayer;

    /// <summary>
    /// Material describing the physical properties of the mesh.
    /// </summary>
    public PhysicMaterial PhysicMaterial;

    /// <summary>
    /// Whether or not colliders are enabled on mesh
    /// </summary>
    private bool _enableCollisions = true;
    /// <summary>
    /// Toggles whether colliders are enabled on mesh
    /// </summary>
    public bool EnableCollisions
    {
        get { return _enableCollisions; }
        set { _enableCollisions = value; ToggleColliders(); }
    }

    protected void Awake()
    {
        bakeMeshes = true;
    }

    /// <summary>
    /// Handler for RequestMeshAsync which will be used to set the layer, material, and collision options on the resulting mesh
    /// </summary>
    /// <param name="bakedData">The resulting data from the RequestMeshAsync call</param>
    /// <param name="outputWritten">Whether or not the output was written</param>
    /// <param name="elapsedBakeTimeSeconds">How long the baking took in seconds</param>
    protected override void SurfaceObserver_OnDataReady(SurfaceData bakedData, bool outputWritten, float elapsedBakeTimeSeconds)
    {
        if (bakedData.outputMesh != null)
        {
            base.SurfaceObserver_OnDataReady(bakedData, outputWritten, elapsedBakeTimeSeconds);
            bakedData.outputCollider.gameObject.layer = MeshLayer;
            if (PhysicMaterial != null)
            {
                bakedData.outputCollider.material = PhysicMaterial;
            }
            bakedData.outputCollider.enabled = _enableCollisions;
        }
    }

    /// <summary>
    /// Helper to update the active state of the colliders
    /// </summary>
    private void ToggleColliders()
    {
        foreach (GameObject go in SpatialMeshObjects.Values)
        {
            if (go != null)
            {
                MeshCollider collider = go.GetComponent<MeshCollider>();
                if (collider != null)
                {
                    collider.enabled = _enableCollisions;
                }
            }
        }

        foreach (RemovedSurfaceHolder rsh in RemovedMeshObjects.Values)
        {
            rsh.SetColliderEnabled(_enableCollisions);

            if (ShouldBeActiveWhileRemoved(rsh.gameObject))
            {
                MeshCollider collider = rsh.gameObject.GetComponent<MeshCollider>();
                if (collider != null)
                {
                    collider.enabled = _enableCollisions;
                }
            }
        }
    }
}
