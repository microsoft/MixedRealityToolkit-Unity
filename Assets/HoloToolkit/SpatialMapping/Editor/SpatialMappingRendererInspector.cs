// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See LICENSE in the project root for license information.

#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SpatialMappingRenderer))]
public class SpatialMappingRendererInspector : Editor
{
    /// <summary>
    /// The SpatialMappingRenderer we are setting options for
    /// </summary>
    private SpatialMappingRenderer _smRenderer;

    void OnEnable()
    {
        _smRenderer = target as SpatialMappingRenderer;
    }

    public override void OnInspectorGUI()
    {
        GUIContent[] renderingSettingChoices = { new GUIContent("Occlusion"), new GUIContent("Material"), new GUIContent("None") };
        _smRenderer.CurrentRenderingSetting = (SpatialMappingRenderer.RenderingSetting)EditorGUILayout.Popup(new GUIContent("Render Mode"), (int)_smRenderer.CurrentRenderingSetting, renderingSettingChoices);
        switch (_smRenderer.CurrentRenderingSetting)
        {
            case SpatialMappingRenderer.RenderingSetting.Occlusion:
                Material occlusionMaterial = _smRenderer.OcclusionMaterial;
                _smRenderer.OcclusionMaterial = (Material)EditorGUILayout.ObjectField(new GUIContent("Occlusion Material", "Material used for surface occlusion"), (UnityEngine.Object)occlusionMaterial, typeof(Material), false);
                break;
            case SpatialMappingRenderer.RenderingSetting.Material:
                Material defaultMaterial = _smRenderer.RenderingMaterial;
                _smRenderer.RenderingMaterial = (Material)EditorGUILayout.ObjectField(new GUIContent("Render Material", "Material used to render the mesh"), (UnityEngine.Object)defaultMaterial, typeof(Material), false);
                break;
        }

        _smRenderer.FreezeMeshUpdates = EditorGUILayout.Toggle(new GUIContent("Freeze Mesh Updates", ""), _smRenderer.FreezeMeshUpdates);
        GUIContent[] boundsChoices = { new GUIContent("Bounding Box"), new GUIContent("Sphere") };
        int currentChoice = _smRenderer.UseSphereBounds ? 1 : 0;
        int choice = EditorGUILayout.Popup(new GUIContent("Bounding Volume", "The shape of the bounds for the observed region"), currentChoice, boundsChoices);
        switch (choice)
        {
            case 0:
                _smRenderer.UseSphereBounds = false;
                _smRenderer.Extents = EditorGUILayout.Vector3Field(new GUIContent("Extents", "The extents of the observation volume"), _smRenderer.Extents);
                break;
            case 1:
                _smRenderer.UseSphereBounds = true;
                _smRenderer.SphereRadius = EditorGUILayout.FloatField(new GUIContent("Radius", "The radius of the observation sphere volume"), _smRenderer.SphereRadius);
                break;
            default:
                throw new System.Exception("Unexpected option!");
        }

        GUIContent[] lodChoices = { new GUIContent("High"), new GUIContent("Medium"), new GUIContent("Low") };
        _smRenderer.LevelOfDetail = (SMBaseAbstract.MeshLevelOfDetail)EditorGUILayout.Popup(new GUIContent("Level of Detail", "The quality of the resulting mesh. Lower is better for performance and physics while higher will look more accurate and is better for rendering"), (int)_smRenderer.LevelOfDetail, lodChoices);
        _smRenderer.TimeBetweenUpdates = EditorGUILayout.FloatField(new GUIContent("Time Between Updates", "How long to wait (in seconds) between Spatial Mapping updates"), _smRenderer.TimeBetweenUpdates);
    }
}
#endif
