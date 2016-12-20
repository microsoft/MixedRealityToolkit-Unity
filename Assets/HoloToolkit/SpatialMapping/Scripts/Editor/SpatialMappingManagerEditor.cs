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

            if (spatialMappingManager.SurfaceMaterial.HasProperty("_PulseColor"))
            {
                spatialMappingManager.SurfaceMaterial.SetColor(
                    "_PulseColor",
                    EditorGUILayout.ColorField(
                        new GUIContent("Pulse Color", "The Color the Pulse will have when user taps the spatial mesh"),
                        spatialMappingManager.SurfaceMaterial.GetColor("_PulseColor")
                    )
                );
            }

            if (spatialMappingManager.SurfaceMaterial.HasProperty("_PulseWidth"))
            {
                spatialMappingManager.SurfaceMaterial.SetFloat(
                    "_PulseWidth",
                    EditorGUILayout.FloatField(
                        new GUIContent("Pulse Width", "Width of Pulse when flowing over spatial map"),
                        spatialMappingManager.SurfaceMaterial.GetFloat("_PulseWidth")
                    )
                );
            }

            if (spatialMappingManager.SurfaceMaterial.HasProperty("_UseWireframe"))
            {
                bool useWireframe = EditorGUILayout.Toggle(new GUIContent("Use Wireframe"), spatialMappingManager.SurfaceMaterial.GetInt("_UseWireframe") == 1);
                if (useWireframe)
                {
                    spatialMappingManager.SurfaceMaterial.SetInt("_UseWireframe", 1);
                    if (spatialMappingManager.SurfaceMaterial.HasProperty("_WireframeColor"))
                    {
                        spatialMappingManager.SurfaceMaterial.SetColor(
                            "_WireframeColor",
                            EditorGUILayout.ColorField(
                                new GUIContent("Wireframe Color", "The Color the wireframe on the spatial map will have"),
                                spatialMappingManager.SurfaceMaterial.GetColor("_WireframeColor")
                            )
                        );
                    }

                    if (spatialMappingManager.SurfaceMaterial.HasProperty("_WireframeFill"))
                    {
                        spatialMappingManager.SurfaceMaterial.SetFloat(
                            "_WireframeFill",
                            EditorGUILayout.Slider(
                                new GUIContent("Wireframe thickness", "Thickness of wireframe lines on spatial map"),
                                spatialMappingManager.SurfaceMaterial.GetFloat("_WireframeFill"),
                                0.0f,
                                1.0f
                            )
                        );
                    }
                }
                else
                {
                    spatialMappingManager.SurfaceMaterial.SetInt("_UseWireframe", 0);
                }
            }
        }
    }
}