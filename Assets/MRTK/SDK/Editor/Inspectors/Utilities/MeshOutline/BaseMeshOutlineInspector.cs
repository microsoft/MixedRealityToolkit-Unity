// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using static Microsoft.MixedReality.Toolkit.Utilities.StandardShaderUtility;

namespace Microsoft.MixedReality.Toolkit.Utilities.Editor.MeshOutline
{
    /// <summary>
    /// A custom inspector for BaseMeshOutline.
    /// Used for create or fix outline material
    /// </summary>
    [CustomEditor(typeof(BaseMeshOutline), true)]
    public class BaseMeshOutlineInspector : UnityEditor.Editor
    {
        private BaseMeshOutline instance;
        private SerializedProperty m_Script;
        private SerializedProperty outlineMaterial;

        private readonly Dictionary<string, object> defaultOutlineMaterialSettings = new Dictionary<string, object>()
        {
            { "_Mode", 5f },
            { "_CustomMode", 0f },
            { "_ZWrite", 0f },
            { "_VertexExtrusion", 1f },
            { "_VertexExtrusionValue",  0.01f },
            { "_VertexExtrusionSmoothNormals", 1f },
            { "_VERTEX_EXTRUSION", true },
            { "_VERTEX_EXTRUSION_SMOOTH_NORMALS", true },
        };

        private void OnEnable()
        {
            instance = target as BaseMeshOutline;
            m_Script = serializedObject.FindProperty("m_Script");
            outlineMaterial = serializedObject.FindProperty(nameof(outlineMaterial));
        }

        public override void OnInspectorGUI()
        {
            DrawReadonlyPropertyField(m_Script);

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(outlineMaterial);

            var currentMat = instance.OutlineMaterial;
            if (currentMat == null)
            {
                EditorGUILayout.HelpBox($"Outline Material field is empty, please create or select material", MessageType.Warning);

                if (GUILayout.Button("Create && set new material"))
                {
                    outlineMaterial.objectReferenceValue = CreateNewMaterial();
                }
            }
            else if(!IsCorrectMaterial(currentMat))
            {
                EditorGUILayout.HelpBox($"Material may not be configured correctly, check or reset to default", MessageType.Info);

                if (GUILayout.Button("Update material settings to default"))
                {
                    ForceUpdateToDefaultOutlineMaterial(ref currentMat);
                }
            }

            // Draw other properties
            DrawPropertiesExcluding(serializedObject, nameof(m_Script), nameof(outlineMaterial));

            if (EditorGUI.EndChangeCheck())
                serializedObject.ApplyModifiedProperties();
        }

        private Material CreateNewMaterial()
        {
            var material = new Material(MrtkStandardShader);
            ForceUpdateToDefaultOutlineMaterial(ref material);
            AssetDatabase.CreateAsset(material, $"Assets/{Selection.activeGameObject.name}Mat.mat");
            return material;
        }

        private void ForceUpdateToDefaultOutlineMaterial(ref Material material)
        {
            if (!IsUsingMrtkStandardShader(material))
            {
                material.shader = MrtkStandardShader;
            }

            foreach (var pair in defaultOutlineMaterialSettings)
            {
                switch (pair.Value.GetType().Name)
                {
                    case nameof(System.Single):
                        material.SetFloat(pair.Key, (float)pair.Value);
                        break;
                    case nameof(System.Boolean):
                        var val = (bool)pair.Value;
                        if (val) 
                        {
                            material.EnableKeyword(pair.Key);
                        }
                        else
                        {
                            material.DisableKeyword(pair.Key);
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        private bool IsCorrectMaterial(Material material)
        {
            if (!IsUsingMrtkStandardShader(material))
            {
                return false;
            }

            return defaultOutlineMaterialSettings.All(x => 
            {
                switch (x.Value.GetType().Name)
                {
                    case nameof(System.Single):
                        return material.GetFloat(x.Key) == (float)x.Value;
                    case nameof(System.Boolean):
                        var val = (bool)x.Value;
                        if (val)
                        {
                            return material.IsKeywordEnabled(x.Key);
                        }
                        else
                        {
                            return !material.IsKeywordEnabled(x.Key);
                        }
                    default:
                        // Default return value
                        return false;
                }
            });
        }

        private void DrawReadonlyPropertyField(SerializedProperty property, params GUILayoutOption[] options)
        {
            GUI.enabled = false;
            EditorGUILayout.PropertyField(property, options);
            GUI.enabled = true;
        }
    }
}
