// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using UnityEngine.VR.WSA;

public class SpatialMappingRenderer : SMBaseAbstract
{
    /// <summary>
    /// Enum representing the choices for rendering
    /// 
    /// Ensure any updates here are reflected in SpatialMappingRendererInspector
    /// </summary>
    public enum RenderingSetting { Occlusion = 0, Material = 1, None = 2 }

    /// <summary>
    /// The current rendering setting
    /// </summary>
    [SerializeField]
    private RenderingSetting _currentRenderingSetting = RenderingSetting.Occlusion;
    /// <summary>
    /// The current option for how rendering will be handled
    /// </summary>
    public RenderingSetting CurrentRenderingSetting
    {
        get { return _currentRenderingSetting; }
        set { _currentRenderingSetting = value; ApplyRenderingSettingToCache(); }
    }

    /// <summary>
    /// The material used to render the mesh if _currentRenderingSetting is RenderingSetting.Material
    /// </summary>
    [SerializeField]
    private Material _renderingMaterial = null;

    /// <summary>
    /// The material used to render the mesh if _currentRenderingSetting is RenderingSetting.Material
    /// </summary>
    public Material RenderingMaterial
    {
        get { return _renderingMaterial; }
        set { _renderingMaterial = value; ApplyRenderingSettingToCache(); }
    }
    /// <summary>
    /// The material used to render the mesh if _currentRenderingSetting is RenderingSetting.Occlusion
    /// </summary>
    [SerializeField]
    private Material _occlusionMaterial = null;

    /// <summary>
    /// The material used to render the mesh if _currentRenderingSetting is RenderingSetting.Occlusion
    /// </summary>
    public Material OcclusionMaterial
    {
        get { return _occlusionMaterial; }
        set { _occlusionMaterial = value; ApplyRenderingSettingToCache(); }
    }

    protected override void Start()
    {
        base.Start();
    }

    /// <summary>
    /// Handler for RequestMeshAsync which will be used to set the material on the resulting mesh
    /// </summary>
    /// <param name="bakedData">The resulting data from the RequestMeshAsync call</param>
    /// <param name="outputWritten">Whether or not the output was written</param>
    /// <param name="elapsedBakeTimeSeconds">How long the baking took in seconds</param>
    protected override void SurfaceObserver_OnDataReady(SurfaceData bakedData, bool outputWritten, float elapsedBakeTimeSeconds)
    {
        if (bakedData.outputMesh != null)
        {
            GameObject go;
            if (SpatialMeshObjects.TryGetValue(bakedData.id, out go) && go != null)
            {
                if (go.GetComponent<MeshRenderer>() == null)
                {
                    MeshRenderer meshRenderer = go.AddComponent<MeshRenderer>();
                    meshRenderer.receiveShadows = false;
                    meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                }

                ApplyRenderingSetting(bakedData.outputMesh.GetComponent<MeshRenderer>());
            }
        }
    }

    /// <summary>
    /// Gives the default Level of Detail for the inspector.
    /// 
    /// For rendering, we default to MeshLevelOfDetail.Medium
    /// </summary>
    /// <returns>MeshLevelOfDetail.Medium</returns>
    protected override MeshLevelOfDetail GetDefaultLevelOfDetail()
    {
        return MeshLevelOfDetail.Medium;
    }

    /// <summary>
    /// Helper to update the currently cached SpatialMeshObjects to the new rendering setting or material
    /// </summary>
    private void ApplyRenderingSettingToCache()
    {
        foreach (GameObject go in SpatialMeshObjects.Values)
        {
            if (go != null)
            {
                ApplyRenderingSetting(go.GetComponent<MeshRenderer>());
            }
        }

        foreach (RemovedSurfaceHolder rsh in RemovedMeshObjects.Values)
        {
            rsh.SetRendererEnabled(CurrentRenderingSetting != RenderingSetting.None);
            MeshRenderer r = rsh.gameObject.GetComponent<MeshRenderer>();
            if (r != null)
            {
                ApplyRenderingSetting(r);

                if (ShouldBeActiveWhileRemoved(rsh.gameObject))
                {
                    r.enabled = CurrentRenderingSetting != RenderingSetting.None;
                }
            }
        }
    }

    /// <summary>
    /// Helper to actually apply the new render setting and material to a single MeshRenderer instance
    /// </summary>
    /// <param name="r">The MeshRenderer to apply the current rendering setting and material to</param>
    private void ApplyRenderingSetting(MeshRenderer r)
    {
        if (r == null)
        {
            return;
        }

        switch (CurrentRenderingSetting)
        {
            case RenderingSetting.Material:
                r.material = RenderingMaterial;
                break;
            case RenderingSetting.Occlusion:
                r.material = OcclusionMaterial;
                break;
            case RenderingSetting.None:
                r.enabled = false;
                break;
        }
    }
}
