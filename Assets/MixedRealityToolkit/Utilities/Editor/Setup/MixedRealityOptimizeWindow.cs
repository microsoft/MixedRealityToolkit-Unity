using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Editor
{
    /// <summary>
    /// Build window - Utility for developers to automate optimization of their Unity scene/project
    /// </summary>
    public class MixedRealityOptimizeWindow : EditorWindow
    {
        private bool showProjectOptimizations = true;
        private bool showSceneOptimizations = true;
        private bool showShaderOptimizations = true;

        private bool setQualityLevel = true;
        private bool singlePassInstanced = true;
        private bool enableDepthBufferSharing = true;
        private bool enable16BitDepthBuffer = true;

        private bool disableRealtimeGlobalIllumination = true;
        private bool disableBakedGlobalIllumination = true;

        private bool onlyUnityShader = true;

        private Vector2 scrollPos_Discovered = new Vector2();
        private Vector2 scrollPos_Converted = new Vector2();
        private List<Material> discoveredMaterials = new List<Material>();
        private List<UndoMaterial> convertedMaterials = new List<UndoMaterial>();

        private class UndoMaterial
        {
            public Material material;
            public Shader shader;
        }

        private Shader replacementShader;
        private Shader unityStandardShader;
        private Shader errorShader;

        [MenuItem("Mixed Reality Toolkit/Utilities/Optimize Window", false, 0)]
        public static void OpenWindow()
        {
            // Dock it next to the Scene View.
            var window = GetWindow<MixedRealityOptimizeWindow>(typeof(SceneView));
            window.titleContent = new GUIContent("Optimize Window");
            window.StartUp();
            window.Show();
        }

        public void StartUp()
        {
            replacementShader = Shader.Find("Mixed Reality Toolkit/Standard");
            unityStandardShader = Shader.Find("Standard");
            errorShader = Shader.Find("Hidden/InternalErrorShader");
        }

        private void OnGUI()
        {
            // TODO: Check that XR settings virtual reality supported checked?
            // TODO: Hide things if already set/optimized
            // TODO: Filter or look at current build target => Ie also enable depth buffer sharing for oculus?
            EditorGUILayout.HelpBox("This tool automates the process of initializing your project, currently open scene, and material assets to recommended settings for Mixed Reality", UnityEditor.MessageType.Info);
            EditorGUILayout.Space();

            showProjectOptimizations = EditorGUILayout.Foldout(showProjectOptimizations, "Project Optimizations");
            //GUILayout.Label("Project Optimizations", EditorStyles.boldLabel);

            if (showProjectOptimizations)
            {
                using (new EditorGUI.IndentLevelScope())
                {

                    EditorGUILayout.BeginHorizontal();
                        singlePassInstanced = EditorGUILayout.ToggleLeft("Set Single Pass Instanced Rendering", singlePassInstanced);
                        if (GUILayout.Button(EditorGUIUtility.IconContent("_Help", "|Learn more"), GUILayout.Width(24), GUILayout.Height(24)))
                        {
                            // TODO: Look at help link?
                        }
                        //EditorGUILayout.LabelField(EditorGUIUtility.IconContent("_Help", "|Help tooltip here"));
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                        setQualityLevel = EditorGUILayout.ToggleLeft("Set Quality Level to Lowest", setQualityLevel);
                        if(GUILayout.Button(EditorGUIUtility.IconContent("_Help", "|Learn more"), GUILayout.Width(24), GUILayout.Height(24)))
                        {
                        // TODO: Look at help link?
                        //EditorApplication.ExecuteMenuItem("Edit/Project Settings/Quality");
                    }

                    EditorGUILayout.EndHorizontal();

                    enableDepthBufferSharing = EditorGUILayout.BeginToggleGroup("Enable Depth Buffer Sharing", enableDepthBufferSharing);
                        enable16BitDepthBuffer = EditorGUILayout.ToggleLeft("Set Depth Buffer to 16-bit", enable16BitDepthBuffer);
                        EditorGUILayout.HelpBox("16-bit is generally more performant than 24-bit", UnityEditor.MessageType.Info);
                        // TODO: Set camera to far plane of 100m? Or have this automatically in scene configure?
                    EditorGUILayout.EndToggleGroup();
                    EditorGUILayout.Space();

                    if (GUILayout.Button("Optimize Project")) // change size?
                    {
                        OptimizeProject();
                    }
                }
            }

            //GUILayout.Label("Scene Optimizations", EditorStyles.boldLabel);
            EditorGUILayout.Space();
            showSceneOptimizations = EditorGUILayout.Foldout(showSceneOptimizations, "Scene Optimizations");

            if (showSceneOptimizations)
            {
                disableRealtimeGlobalIllumination = EditorGUILayout.ToggleLeft("Disable Realtime Global Illumination", disableRealtimeGlobalIllumination);
                disableBakedGlobalIllumination = EditorGUILayout.ToggleLeft("Disable Baked Global Illumination", disableBakedGlobalIllumination);
                EditorGUILayout.Space();

                // TODO: need to offer option to set per-scene? ==> AssetDatabase and just modify files?

                // TODO: Analyze current scene gameobjects for high poly count??

                if (GUILayout.Button("Optimize Current Scene"))
                {
                    OptimizeScene();
                }
            }

            showShaderOptimizations = EditorGUILayout.Foldout(showShaderOptimizations, "Shader Optimizations");

            if (showShaderOptimizations)
            {
                EditorGUILayout.HelpBox("It is recommended to utilize the MRTK Standard Shader for most materials", MessageType.Info);
                EditorGUILayout.Space();

                replacementShader = (Shader)EditorGUILayout.ObjectField("Replacement Shader", replacementShader, typeof(Shader), false);

                onlyUnityShader = EditorGUILayout.ToggleLeft("Only Discover Materials with Unity Standard Shader", onlyUnityShader);
                EditorGUILayout.Space();

                EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button("Discover"))
                    {
                        DiscoverMaterials();
                    }

                    if (GUILayout.Button("Optimize"))
                    {
                        ConvertMaterials();
                    }

                    if (GUILayout.Button("Undo"))
                    {
                        UndoMaterials();
                    }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                    scrollPos_Discovered = EditorGUILayout.BeginScrollView(scrollPos_Discovered);
                        if (discoveredMaterials.Count > 0)
                        {
                            EditorGUILayout.LabelField("Discovered " + discoveredMaterials.Count + " materials to convert");

                            for (int i = 0; i < discoveredMaterials.Count; i++)
                            {
                                discoveredMaterials[i] = (Material)EditorGUILayout.ObjectField(discoveredMaterials[i], typeof(Material), false);
                            }
                        }
                    EditorGUILayout.EndScrollView();
                EditorGUILayout.EndHorizontal();

            }
        }

        private void OptimizeProject()
        {
            if (singlePassInstanced)
            {
                PlayerSettings.stereoRenderingPath = StereoRenderingPath.Instancing;
            }

            // TODO: Enable Depth Buffer Sharing may be enabled, but we would still like them to be at 16-bit
            if (enableDepthBufferSharing)
            {
#if UNITY_2019
                PlayerSettings.VRWindowsMixedReality.depthBufferSharingEnabled = true
                if (enable16BitDepthBuffer)
                {
                    PlayerSettings.VRWindowsMixedReality.depthBufferFormat = PlayerSettings.VRWindowsMixedReality.DepthBufferFormat.DepthBufferFormat16Bit;
                }
#else
                var playerStettings = getPlayerSettings();
                ChangeProperty(playerStettings, "vrSettings.hololens.depthBufferSharingEnabled", property => property.boolValue = true);

                if (enable16BitDepthBuffer)
                {
                    ChangeProperty(playerStettings, "vrSettings.hololens.depthFormat", property => property.intValue = 0);
                }
#endif
            }

            if (setQualityLevel)
            {
                // TODO: Seems to set only at runtime and not per-platform in editor
                // Try to get serializedobject as well?
                string[] names = QualitySettings.names;
                QualitySettings.SetQualityLevel(0, false);

                var qualitySettings = Unsupported.GetSerializedAssetInterfaceSingleton("QualitySettings");

                var so = new SerializedObject(qualitySettings);
            }
        }

        private void OptimizeScene()
        {
            var lightmapSettings = getLighmapSettings();

            if (disableRealtimeGlobalIllumination)
            {
                ChangeProperty(lightmapSettings, "m_GISettings.m_EnableRealtimeLightmaps", property => property.boolValue = false);
            }

            if (disableBakedGlobalIllumination)
            {
                ChangeProperty(lightmapSettings, "m_GISettings.m_EnableBakedLightmaps", property => property.boolValue = false);
            }
        }

        private void DiscoverMaterials()
        {
            discoveredMaterials.Clear();

            string[] guids = AssetDatabase.FindAssets("t:Material");
            for (int i = 0; i < guids.Length; i++)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
                Material asset = AssetDatabase.LoadAssetAtPath<Material>(assetPath);

                if ((asset.shader != replacementShader && (!onlyUnityShader || asset.shader == unityStandardShader))
                    || asset.shader == errorShader)
                {
                    discoveredMaterials.Add(asset);
                }
            }
        }

        private void ConvertMaterials()
        {
            convertedMaterials.Clear();

            foreach (Material asset in this.discoveredMaterials)
            {
                if (asset != null)
                {
                    if ((asset.shader != replacementShader && (!onlyUnityShader || asset.shader == unityStandardShader))
                        || asset.shader == errorShader)
                    {
                        /*
                        convertedMaterials.Add(new UndoMaterial
                        {
                            material = asset,
                            shader = asset.shader,
                        });*/

                        asset.shader = replacementShader;
                    }
                }
            }

            discoveredMaterials.Clear();
        }

        private void UndoMaterials()
        {
            foreach (UndoMaterial um in this.convertedMaterials)
            {
                um.material.shader = um.shader;
            }

            convertedMaterials.Clear();
        }

        public static void ChangeProperty(SerializedObject target, string name, Action<SerializedProperty> changer)
        {
            var prop = target.FindProperty(name);
            if (prop != null)
            {
                changer(prop);
                target.ApplyModifiedProperties();
            }
            else Debug.LogError("property not found: " + name);
        }

        private static SerializedObject getLighmapSettings()
        {
            var getLightmapSettingsMethod = typeof(LightmapEditorSettings).GetMethod("GetLightmapSettings", BindingFlags.Static | BindingFlags.NonPublic);
            var lightmapSettings = getLightmapSettingsMethod.Invoke(null, null) as UnityEngine.Object;
            return new SerializedObject(lightmapSettings);
        }

        private static SerializedObject getPlayerSettings()
        {
            var playerSettings = Unsupported.GetSerializedAssetInterfaceSingleton("PlayerSettings");

            return new SerializedObject(playerSettings);
        }
    }
}