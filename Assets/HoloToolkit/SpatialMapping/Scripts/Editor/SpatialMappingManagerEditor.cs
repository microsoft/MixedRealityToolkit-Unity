using UnityEditor;
using UnityEngine;
using Debug = System.Diagnostics.Debug;

namespace HoloToolkit.Unity.SpatialMapping
{
    [CustomEditor(typeof(SpatialMappingManager))]
    public partial class SpatialMappingManagerEditor : Editor
    {
        private SerializedProperty castShadowsProperty;
        private SerializedProperty pulseMaximumProperty;
        private SerializedProperty drawVisualMeshProperty;
        private SerializedProperty surfaceMaterialProperty;
        private SerializedProperty autoStartObserverProperty;

        private void OnEnable()
        {
            castShadowsProperty = serializedObject.FindProperty("castShadows");
            pulseMaximumProperty = serializedObject.FindProperty("PulseMaximum");
            drawVisualMeshProperty = serializedObject.FindProperty("drawVisualMeshes");
            surfaceMaterialProperty = serializedObject.FindProperty("surfaceMaterial");
            autoStartObserverProperty = serializedObject.FindProperty("AutoStartObserver");
        }

        public override void OnInspectorGUI()
        {
            SpatialMappingManager spatialMappingManager = target as SpatialMappingManager;
            Debug.Assert(spatialMappingManager != null, "spatialMappingManager != null");

            EditorGUILayout.PropertyField(
                autoStartObserverProperty,
                new GUIContent("Auto Start", "Determines if the surface observer should be automatically started."));

            EditorGUILayout.PropertyField(
                drawVisualMeshProperty,
                new GUIContent("Draw Visual Meshes", "Determines if spatial mapping data will be rendered."));

            if (spatialMappingManager.DrawVisualMeshes)
            {
                EditorGUILayout.PropertyField(
                    surfaceMaterialProperty,
                    new GUIContent("Surface Material", "The material to use for rendering spatial mapping data."));

                if (spatialMappingManager.SurfaceMaterial != null)
                {
                    EditorGUILayout.PropertyField(
                        castShadowsProperty,
                        new GUIContent("Cast Shadows", "Determines if spatial mapping renderer will cast shadows."));

                    if (spatialMappingManager.SurfaceMaterial.HasProperty("_Speed"))
                    {
                        spatialMappingManager.SurfaceMaterial.SetFloat(
                            "_Speed",
                            EditorGUILayout.Slider(
                                new GUIContent("Pulse Speed", "Speed of Pulse when flowing over spatial map"),
                                spatialMappingManager.SurfaceMaterial.GetFloat("_Speed"),
                                0.001f,
                                100.0f
                            )
                        );

                        EditorGUILayout.PropertyField(
                            pulseMaximumProperty,
                            new GUIContent("Pulse Maximum", "The Maximum radius from the tap, that the pulse will expand from"));
                    }

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
                            EditorGUILayout.Slider(
                                new GUIContent("Pulse Width", "Width of Pulse when flowing over spatial map"),
                                spatialMappingManager.SurfaceMaterial.GetFloat("_PulseWidth"),
                                0.001f,
                                2.0f
                            )
                        );

                        spatialMappingManager.SurfaceMaterial.SetFloat("_Radius", -spatialMappingManager.SurfaceMaterial.GetFloat("_PulseWidth"));
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
                else
                {
                    spatialMappingManager.CastShadows = false;
                }
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}