// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEditor;
using UnityEngine;
using System.IO;

namespace Microsoft.MixedReality.Toolkit.Editor
{
    public class TextureCombinerWindow : EditorWindow
    {
        private enum Channel
        {
            Red = 0,
            Green = 1,
            Blue = 2,
            Alpha = 3,
            RGBAverage = 4
        }

        private enum TextureFormat
        {
            TGA = 0,
            PNG = 1,
            JPG = 2
        }

        private static readonly string[] textureExtensions = new string[] { "tga", "png", "jpg" };
        private const float defaultUniformValue = -0.01f;

        private Texture2D metallicMap;
        private Channel metallicMapChannel = Channel.Red;
        private float metallicUniform = defaultUniformValue;
        private Texture2D occlusionMap;
        private Channel occlusionMapChannel = Channel.Green;
        private float occlusionUniform = defaultUniformValue;
        private Texture2D emissionMap;
        private Channel emissionMapChannel = Channel.RGBAverage;
        private float emissionUniform = defaultUniformValue;
        private Texture2D smoothnessMap;
        private Channel smoothnessMapChannel = Channel.Alpha;
        private float smoothnessUniform = defaultUniformValue;
        private Material standardMaterial;
        private TextureFormat textureFormat = TextureFormat.TGA;

        private const string StandardShaderName = "Standard";
        private const string StandardRoughnessShaderName = "Standard (Roughness setup)";
        private const string StandardSpecularShaderName = "Standard (Specular setup)";

        [MenuItem("Mixed Reality Toolkit/Utilities/Texture Combiner")]
        private static void ShowWindow()
        {
            TextureCombinerWindow window = GetWindow<TextureCombinerWindow>();
            window.titleContent = new GUIContent("Texture Combiner");
            window.minSize = new Vector2(380.0f, 700.0f);
            window.Show();
        }

        private void OnGUI()
        {
            GUILayout.Label("Import", EditorStyles.boldLabel);
            GUI.enabled = metallicUniform < 0.0f;
            metallicMap = (Texture2D)EditorGUILayout.ObjectField("Metallic Map", metallicMap, typeof(Texture2D), false);
            metallicMapChannel = (Channel)EditorGUILayout.EnumPopup("Input Channel", metallicMapChannel);
            GUI.enabled = true;
            metallicUniform = EditorGUILayout.Slider(new GUIContent("Metallic Uniform"), metallicUniform, defaultUniformValue, 1.0f);
            GUILayout.Box("Output Channel: Red", EditorStyles.helpBox, System.Array.Empty<GUILayoutOption>());
            EditorGUILayout.Separator();
            GUI.enabled = occlusionUniform < 0.0f;
            occlusionMap = (Texture2D)EditorGUILayout.ObjectField("Occlusion Map", occlusionMap, typeof(Texture2D), false);
            occlusionMapChannel = (Channel)EditorGUILayout.EnumPopup("Input Channel", occlusionMapChannel);
            GUI.enabled = true;
            occlusionUniform = EditorGUILayout.Slider(new GUIContent("Occlusion Uniform"), occlusionUniform, defaultUniformValue, 1.0f);
            GUILayout.Box("Output Channel: Green", EditorStyles.helpBox, System.Array.Empty<GUILayoutOption>());
            EditorGUILayout.Separator();
            GUI.enabled = emissionUniform < 0.0f;
            emissionMap = (Texture2D)EditorGUILayout.ObjectField("Emission Map", emissionMap, typeof(Texture2D), false);
            emissionMapChannel = (Channel)EditorGUILayout.EnumPopup("Input Channel", emissionMapChannel);
            GUI.enabled = true;
            emissionUniform = EditorGUILayout.Slider(new GUIContent("Emission Uniform"), emissionUniform, defaultUniformValue, 1.0f);
            GUILayout.Box("Output Channel: Blue", EditorStyles.helpBox, System.Array.Empty<GUILayoutOption>());
            EditorGUILayout.Separator();
            GUI.enabled = smoothnessUniform < 0.0f;
            smoothnessMap = (Texture2D)EditorGUILayout.ObjectField("Smoothness Map", smoothnessMap, typeof(Texture2D), false);
            smoothnessMapChannel = (Channel)EditorGUILayout.EnumPopup("Input Channel", smoothnessMapChannel);
            GUI.enabled = true;
            smoothnessUniform = EditorGUILayout.Slider(new GUIContent("Smoothness Uniform"), smoothnessUniform, defaultUniformValue, 1.0f);
            GUILayout.Box("Output Channel: Alpha", EditorStyles.helpBox, System.Array.Empty<GUILayoutOption>());
            EditorGUILayout.Separator();

            standardMaterial = (Material)EditorGUILayout.ObjectField("Standard Material", standardMaterial, typeof(Material), false);

            GUI.enabled = standardMaterial != null && IsUnityStandardMaterial(standardMaterial);

            if (GUILayout.Button("Auto-populate from Standard Material"))
            {
                Autopopulate();
            }

            GUI.enabled = CanSave();

            EditorGUILayout.Separator();

            GUILayout.Label("Export", EditorStyles.boldLabel);

            textureFormat = (TextureFormat)EditorGUILayout.EnumPopup("Texture Format", textureFormat);

            if (GUILayout.Button("Save Channel Map"))
            {
                Save();
            }

            GUILayout.Box("Metallic (Red), Occlusion (Green), Emission (Blue), Smoothness (Alpha)", EditorStyles.helpBox, System.Array.Empty<GUILayoutOption>());
        }

        private void Autopopulate()
        {
            metallicUniform = defaultUniformValue;
            occlusionUniform = defaultUniformValue;
            emissionUniform = defaultUniformValue;
            smoothnessUniform = defaultUniformValue;

            occlusionMap = (Texture2D)standardMaterial.GetTexture("_OcclusionMap");
            occlusionMapChannel = occlusionMap != null ? Channel.Green : occlusionMapChannel;
            emissionMap = (Texture2D)standardMaterial.GetTexture("_EmissionMap");
            emissionMapChannel = emissionMap != null ? Channel.RGBAverage : emissionMapChannel;

            if (standardMaterial.shader.name == StandardShaderName)
            {
                metallicMap = (Texture2D)standardMaterial.GetTexture("_MetallicGlossMap");
                metallicMapChannel = metallicMap != null ? Channel.Red : metallicMapChannel;
                smoothnessMap = ((int)standardMaterial.GetFloat("_SmoothnessTextureChannel") == 0) ? metallicMap : (Texture2D)standardMaterial.GetTexture("_MainTex");
                smoothnessMapChannel = smoothnessMap != null ? Channel.Alpha : smoothnessMapChannel;
            }
            else if (standardMaterial.shader.name == StandardRoughnessShaderName)
            {
                metallicMap = (Texture2D)standardMaterial.GetTexture("_MetallicGlossMap");
                metallicMapChannel = metallicMap != null ? Channel.Red : metallicMapChannel;
                smoothnessMap = (Texture2D)standardMaterial.GetTexture("_SpecGlossMap");
                smoothnessMapChannel = smoothnessMap != null ? Channel.Red : smoothnessMapChannel;
            }
            else
            {
                smoothnessMap = ((int)standardMaterial.GetFloat("_SmoothnessTextureChannel") == 0) ? (Texture2D)standardMaterial.GetTexture("_SpecGlossMap") :
                                                                                                     (Texture2D)standardMaterial.GetTexture("_MainTex");
                smoothnessMapChannel = smoothnessMap != null ? Channel.Alpha : smoothnessMapChannel;
            }
        }

        private void Save()
        {
            int width;
            int height;
            Texture[] textures = new Texture[] { metallicMap, occlusionMap, emissionMap, smoothnessMap };
            CalculateChannelMapSize(textures, out width, out height);

            Texture2D channelMap = new Texture2D(width, height);
            RenderTexture renderTexture = RenderTexture.GetTemporary(width, height, 0, RenderTextureFormat.Default, RenderTextureReadWrite.Linear);

            // Use the GPU to pack the various texture maps into a single texture.
            Material channelPacker = new Material(Shader.Find("Hidden/ChannelPacker"));
            channelPacker.SetTexture("_MetallicMap", metallicMap);
            channelPacker.SetInt("_MetallicMapChannel", (int)metallicMapChannel);
            channelPacker.SetFloat("_MetallicUniform", metallicUniform);
            channelPacker.SetTexture("_OcclusionMap", occlusionMap);
            channelPacker.SetInt("_OcclusionMapChannel", (int)occlusionMapChannel);
            channelPacker.SetFloat("_OcclusionUniform", occlusionUniform);
            channelPacker.SetTexture("_EmissionMap", emissionMap);
            channelPacker.SetInt("_EmissionMapChannel", (int)emissionMapChannel);
            channelPacker.SetFloat("_EmissionUniform", emissionUniform);
            channelPacker.SetTexture("_SmoothnessMap", smoothnessMap);
            channelPacker.SetInt("_SmoothnessMapChannel", (int)smoothnessMapChannel);
            channelPacker.SetFloat("_SmoothnessUniform", smoothnessUniform);
            Graphics.Blit(null, renderTexture, channelPacker);
            DestroyImmediate(channelPacker);

            // Save the last render texture to a texture.
            RenderTexture previous = RenderTexture.active;
            RenderTexture.active = renderTexture;
            channelMap.ReadPixels(new Rect(0.0f, 0.0f, width, height), 0, 0);
            channelMap.Apply();
            RenderTexture.active = previous;
            RenderTexture.ReleaseTemporary(renderTexture);

            // Save the texture to disk.
            string filename = string.Format("{0}{1}.{2}", GetChannelMapName(textures), "_Channel", textureExtensions[(int)textureFormat]);
            string path = EditorUtility.SaveFilePanel("Save Channel Map", "", filename, textureExtensions[(int)textureFormat]);

            if (path.Length != 0)
            {
                byte[] textureData = null;

                switch (textureFormat)
                {
                    case TextureFormat.TGA:
                        textureData = channelMap.EncodeToTGA();
                        break;
                    case TextureFormat.PNG:
                        textureData = channelMap.EncodeToPNG();
                        break;
                    case TextureFormat.JPG:
                        textureData = channelMap.EncodeToJPG();
                        break;
                }

                if (textureData != null)
                {
                    File.WriteAllBytes(path, textureData);
                    Debug.LogFormat("Saved channel map to: {0}", path);
                    AssetDatabase.Refresh();
                }
            }
        }

        private bool CanSave()
        {
            return metallicMap != null || occlusionMap != null || emissionMap != null || smoothnessMap != null ||
                   metallicUniform >= 0.0f || occlusionUniform >= 0.0f || emissionUniform >= 0.0f || smoothnessUniform >= 0.0f;
        }

        private static bool IsUnityStandardMaterial(Material material)
        {
            if (material != null)
            {
                if (material.shader.name == StandardShaderName ||
                    material.shader.name == StandardRoughnessShaderName ||
                    material.shader.name == StandardSpecularShaderName)
                {
                    return true;
                }
            }

            return false;
        }

        private static string GetChannelMapName(Texture[] textures)
        {
            // Use the first named texture as the channel map name.
            foreach (Texture texture in textures)
            {
                if (texture != null && !string.IsNullOrEmpty(texture.name))
                {
                    return texture.name;
                }
            }

            return string.Empty;
        }

        private static void CalculateChannelMapSize(Texture[] textures, out int width, out int height)
        {
            width = 4;
            height = 4;

            // Find the max extents of all texture maps.
            foreach (Texture texture in textures)
            {
                width = texture != null ? Mathf.Max(texture.width, width) : width;
                height = texture != null ? Mathf.Max(texture.height, height) : height;
            }
        }
    }
}
