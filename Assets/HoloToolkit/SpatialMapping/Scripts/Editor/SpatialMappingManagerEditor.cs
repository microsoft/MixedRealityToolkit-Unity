using UnityEditor;
using UnityEngine;
using Debug = System.Diagnostics.Debug;

namespace HoloToolkit.Unity.SpatialMapping
{
    [CustomEditor(typeof(SpatialMappingManager))]
    public partial class SpatialMappingManagerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            SpatialMappingManager spatialMappingManager = target as SpatialMappingManager;
            Debug.Assert(spatialMappingManager != null, "spatialMappingManager != null");

            spatialMappingManager.SurfaceMaterial = EditorGUILayoutExtensions.ObjectField(
                new GUIContent("Surface Material", "The material to use for rendering spatial mapping data."),
                spatialMappingManager.SurfaceMaterial);

            spatialMappingManager.autoStartObserver = EditorGUILayout.Toggle(
                    new GUIContent("Auto Start", "Determines if the surface observer should be automatically started."),
                    spatialMappingManager.autoStartObserver);

            spatialMappingManager.DrawVisualMeshes = EditorGUILayout.Toggle(
                    new GUIContent("Draw Visual Meshes", "Determines if spatial mapping data will be rendered."),
                    spatialMappingManager.DrawVisualMeshes);

            spatialMappingManager.CastShadows = EditorGUILayout.Toggle(
                new GUIContent("Cast Shadows", "Determines if spatial mapping data will cast shadows."),
                spatialMappingManager.CastShadows);
        }
    }
}