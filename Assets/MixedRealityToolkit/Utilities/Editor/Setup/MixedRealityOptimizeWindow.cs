// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Editor
{
    /// <summary>
    /// Build window - Utility for developers to automate optimization of their Unity scene/project
    /// </summary>
    public class MixedRealityOptimizeWindow : EditorWindow
    {
        private int selectedToolbarIndex = 0;
        private readonly string[] ToolbarTitles = { "Project Optimizations", "Scene Optimizations", "Shader Optimizations" };
        private enum ToolbarSection
        {
            Project = 0,
            Scene,
            Shader
        };

        private Vector2 windowScrollPosition = new Vector2();

        // Project Optimizations
        private bool singlePassInstanced = true;
        private bool enableDepthBufferSharing = true;
        //private bool enable16BitDepthBuffer = true;

        // Scene Optimizations
        private bool disableRealtimeGlobalIllumination = true;
        private bool disableBakedGlobalIllumination = true;
        private static DateTime? lastAnalyzedTime = null;
        private Light[] sceneLights;
        private const uint TopListSize = 5;
        private long totalActivePolyCount, totalInActivePolyCount = 0;
        private long totalPolyCount
        {
            get
            {
                return totalActivePolyCount + totalInActivePolyCount;
            }
        }
        private string TotalPolyCountStr, TotalActivePolyCountStr, TotalInactivePolyCountStr;
        private GameObject[] LargestMeshes = new GameObject[TopListSize];
        private int[] LargestMeshSizes = new int[TopListSize];

        // Shader Optimizations
        private bool showDiscoveredMaterials = true;
        private bool onlyUnityShader = true;
        private Shader replacementShader;
        private Shader unityStandardShader;
        private Shader errorShader;
        private List<Material> discoveredMaterials = new List<Material>();

        // Internal structure to easily search mesh polycounts in scene
        private struct MeshNode
        {
            public int polycount;
            public MeshFilter filter;
        }

        protected enum PerformanceTarget
        {
            AR_Headsets,
            VR_Standalone,
            VR_Tethered,
        };

        // NOTE: These array must match the number of enums in PerformanceTarget
        private readonly string[] PerformanceTargetEnums = { "AR Headsets", "VR Standalone", "VR Tethered" };
        private readonly string[] PerformanceTargetDescriptions = {
            "Suggest performance optimizations for AR devices with mobile class specifications",
            "Suggest performance optimizations for mobile VR devices with mobile class specifications",
            "Suggest performance optimizations for VR devices tethered to a PC" };

        private readonly int[] SceneLightCountMax = { 1, 2, 4 };

        [SerializeField]
        private PerformanceTarget PerfTarget = PerformanceTarget.AR_Headsets;

        protected static GUIStyle HelpIconStyle;
        protected static GUIStyle BoldLargeTitle;

        [MenuItem("Mixed Reality Toolkit/Utilities/Optimize Window", false, 0)]
        public static void OpenWindow()
        {
            // Dock it next to the Scene View.
            var window = GetWindow<MixedRealityOptimizeWindow>(typeof(SceneView));
            window.StartUp();
            window.Show();
        }

        public void StartUp()
        {
            FindShaders();
        }

        private void OnEnable()
        {
            // Weird Unity bug where GUIStyle properties do not serialize property on window refreshes
            CreateStyles();

            this.titleContent = new GUIContent("Optimize Window", EditorGUIUtility.IconContent("d_PreMatCube").image);
        }

        private void OnGUI()
        {
            windowScrollPosition = EditorGUILayout.BeginScrollView(windowScrollPosition);

            MixedRealityEditorUtility.RenderMixedRealityToolkitLogo();

            // Render Title
            EditorGUILayout.LabelField("Mixed Reality Toolkit Optimize Window", BoldLargeTitle);
                EditorGUILayout.LabelField("This tool automates the process of updating your project, currently open scene, and material assets to recommended settings for Mixed Reality", EditorStyles.wordWrappedLabel);
                EditorGUILayout.Space();

                PerfTarget = (PerformanceTarget)EditorGUILayout.Popup("Performance Target", (int)this.PerfTarget, PerformanceTargetEnums);
                EditorGUILayout.HelpBox(PerformanceTargetDescriptions[(int)PerfTarget], MessageType.Info);
                EditorGUILayout.Space();

                if (!PlayerSettings.virtualRealitySupported)
                {
                    EditorGUILayout.HelpBox("Current build target does not have virtual reality support enabled", MessageType.Warning);
                }

                selectedToolbarIndex = GUILayout.Toolbar(selectedToolbarIndex, ToolbarTitles);
                if (selectedToolbarIndex == 0)
                {
                    RenderProjectOptimizations();
                }
                else if (selectedToolbarIndex == 1)
                {
                    RenderSceneOptimizations();
                }
                else
                {
                    RenderShaderOptimizations();
                }

            EditorGUILayout.EndScrollView();
        }

        private void RenderShaderOptimizations()
        {
            GUILayout.BeginVertical("Box");
            EditorGUILayout.LabelField(this.ToolbarTitles[(int)ToolbarSection.Shader], BoldLargeTitle);
            using (new EditorGUI.IndentLevelScope())
            {
                EditorGUILayout.LabelField("The Unity standard shader is generally not performant or optimized for Mixed Reality development. The MRTK Standard shader can be a more performant option. "
                + "This tool allows developers to discover and automatically convert materials in their project to the replacement shader option below. It is recommended to utilize the MRTK Standard Shader."
                    , EditorStyles.wordWrappedLabel);
                EditorGUILayout.Space();

                replacementShader = (Shader)EditorGUILayout.ObjectField("Replacement Shader", replacementShader, typeof(Shader), false);
                if (replacementShader == null)
                {
                    EditorGUILayout.HelpBox("Please set a replacement shader to utilize this tool", MessageType.Error);
                    if (MixedRealityEditorUtility.RenderIndentedButton(new GUIContent("Use MRTK Standard Shader", "Set Replacement Shader to MRKT Standard Shader"), EditorStyles.miniButton, GUILayout.Width(200f)))
                    {
                        FindShaders();
                    }
                }
                else
                {
                    onlyUnityShader = EditorGUILayout.ToggleLeft("Only Discover Materials with Unity Standard Shader", onlyUnityShader);
                    EditorGUILayout.Space();

                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button("Discover Materials in Assets"))
                    {
                        DiscoverMaterials();
                    }

                    bool wasGUIEnabled = GUI.enabled;
                    GUI.enabled = wasGUIEnabled && discoveredMaterials.Count > 0;
                    if (GUILayout.Button("Convert All Discovered Materials"))
                    {
                        ConvertMaterials();
                    }
                    GUI.enabled = wasGUIEnabled;
                    EditorGUILayout.EndHorizontal();

                    if (discoveredMaterials.Count > 0)
                    {
                        showDiscoveredMaterials = EditorGUILayout.Foldout(showDiscoveredMaterials, "Discovered Materials", true);
                        if (showDiscoveredMaterials)
                        {
                            using (new EditorGUI.IndentLevelScope())
                            {
                                EditorGUILayout.LabelField("Discovered " + discoveredMaterials.Count + " materials to convert");

                                EditorGUILayout.BeginHorizontal();
                                EditorGUILayout.LabelField("Current Shader", EditorStyles.boldLabel);
                                EditorGUILayout.LabelField("Material Asset", EditorStyles.boldLabel);
                                EditorGUILayout.EndHorizontal();

                                for (int i = 0; i < discoveredMaterials.Count; i++)
                                {
                                    if (discoveredMaterials[i] != null)
                                    {
                                        EditorGUILayout.BeginHorizontal();
                                            EditorGUILayout.LabelField(discoveredMaterials[i].shader.name);
                                            discoveredMaterials[i] = (Material)EditorGUILayout.ObjectField(discoveredMaterials[i], typeof(Material), false);
                                            if (GUILayout.Button(new GUIContent("View", "Selects & views this asset in inspector"), EditorStyles.miniButton, GUILayout.Width(42f)))
                                            {
                                                Selection.activeObject = this.discoveredMaterials[i];
                                            }
                                            if (GUILayout.Button(new GUIContent("Convert", "Converts this material's shader to the replacement shader"), EditorStyles.miniButton, GUILayout.Width(64f)))
                                            {
                                                Undo.RecordObject(this.discoveredMaterials[i], "Convert to MRTK Standard shader");
                                                ConvertMaterial(this.discoveredMaterials[i]);
                                            }
                                        EditorGUILayout.EndHorizontal();
                                    }
                                }
                            }
                        }
                    }
                }
            }
            GUILayout.EndVertical();
            EditorGUILayout.Space();
        }

        private void RenderSceneOptimizations()
        {
            GUILayout.BeginVertical("Box");
            EditorGUILayout.LabelField(this.ToolbarTitles[(int)ToolbarSection.Scene], BoldLargeTitle);
            using (new EditorGUI.IndentLevelScope())
            {
                EditorGUILayout.LabelField("This section provides controls and performance information for the currently opened scene. Any optimizations performed are only for the active scene at any moment. Also be aware that changes made while in editor Play mode will not be saved.", EditorStyles.wordWrappedLabel);

                BuildSection("Lighting Settings", "https://docs.unity3d.com/Manual/GlobalIllumination.html", () =>
                {
                    EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Global Illumination can produce great visual results but sometimes at great expense.", EditorStyles.wordWrappedLabel);
                        if (GUILayout.Button(new GUIContent("View Lighting Settings", "Open Lighting Settings"), EditorStyles.miniButton, GUILayout.Width(160f)))
                        {
                            EditorApplication.ExecuteMenuItem("Window/Rendering/Lighting Settings");
                        }
                    EditorGUILayout.EndHorizontal();

                    disableRealtimeGlobalIllumination = EditorGUILayout.ToggleLeft("Disable Realtime Global Illumination", disableRealtimeGlobalIllumination);

                    if (this.PerfTarget == PerformanceTarget.AR_Headsets)
                    {
                        disableBakedGlobalIllumination = EditorGUILayout.ToggleLeft("Disable Baked Global Illumination", disableBakedGlobalIllumination);
                    }

                    if (MixedRealityEditorUtility.RenderIndentedButton("Optimize Lighting"))
                    {
                        OptimizeScene();
                    }
                });

                BuildSection("Live Scene Analysis", null, () =>
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(lastAnalyzedTime == null ? "Click analysis button for MRTK to scan your currently opened scene" : "Scanned " + GetRelativeTime(lastAnalyzedTime));

                    if (GUILayout.Button("Analyze Scene", GUILayout.Width(160f)))
                    {
                        AnalyzeScene();
                        lastAnalyzedTime = DateTime.UtcNow;
                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.Space();

                    if (lastAnalyzedTime != null)
                    {
                        // Lighting analysis
                        bool showNumOfSceneLights = this.sceneLights != null && this.sceneLights.Length > SceneLightCountMax[(int)PerfTarget];
                        bool showDisableShadows = this.PerfTarget == PerformanceTarget.AR_Headsets;
                        if (showNumOfSceneLights || showDisableShadows)
                        {
                            EditorGUILayout.LabelField("Lighting Analysis", EditorStyles.boldLabel);
                            if (showNumOfSceneLights)
                            {
                                EditorGUILayout.LabelField(this.sceneLights.Length + " lights in the scene. Consider reducing the number of lights.");
                            }

                            if (showDisableShadows)
                            {
                                foreach (var l in this.sceneLights)
                                {
                                    if (l.shadows != LightShadows.None)
                                    {
                                        EditorGUILayout.ObjectField("Disable shadows", l, typeof(Light), true);
                                    }
                                }
                            }
                            EditorGUILayout.Space();
                        }

                        // Mesh Analysis
                        EditorGUILayout.LabelField("Polygon Count Analysis", EditorStyles.boldLabel);

                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField(TotalPolyCountStr);
                        EditorGUILayout.LabelField(TotalActivePolyCountStr);
                        EditorGUILayout.LabelField(TotalInactivePolyCountStr);
                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.LabelField("Top " + TopListSize + " GameObjects in scene with highest polygon count");
                        for (int i = 0; i < LargestMeshes.Length; i++)
                        {
                            if (LargestMeshes[i] != null)
                            {
                                EditorGUILayout.BeginHorizontal();
                                EditorGUILayout.LabelField("Num of Polygons: " + this.LargestMeshSizes[i].ToString("N0"));
                                EditorGUILayout.ObjectField(this.LargestMeshes[i], typeof(GameObject), true);
                                if (GUILayout.Button(new GUIContent("View", "Selects & view this asset in inspector"), EditorStyles.miniButton, GUILayout.Width(42f)))
                                {
                                    Selection.activeObject = this.LargestMeshes[i];
                                }
                                EditorGUILayout.EndHorizontal();
                            }
                        }
                    }
                });
            }

            GUILayout.EndVertical();
        }

        private void RenderProjectOptimizations()
        {
            GUILayout.BeginVertical("Box");
            EditorGUILayout.LabelField(this.ToolbarTitles[(int)ToolbarSection.Project], BoldLargeTitle);
            using (new EditorGUI.IndentLevelScope())
            {
                BuildSection("Single Pass Instanced Rendering", "https://docs.unity3d.com/Manual/SinglePassStereoRendering.html", () =>
                {
                    EditorGUILayout.LabelField("Single Pass Instanced Rendering is an option in the Unity render pipeline to more efficiently render your scene and optimize CPU & GPU work. This path requires shaders though to be written to support instancing which is automatic in all Unity & MRTK shaders. Click the \"Learn More\" button to learn how to update your custom shaders to support instancing.", EditorStyles.wordWrappedLabel);

                    EditorGUILayout.BeginHorizontal();
                        singlePassInstanced = EditorGUILayout.ToggleLeft("Set Single Pass Instanced Rendering", singlePassInstanced);
                        if (GUILayout.Button(new GUIContent("Learn More", "Learn more about Single Pass Instanced Rendering"), EditorStyles.miniButton, GUILayout.Width(120f)))
                        {
                            Application.OpenURL("https://docs.unity3d.com/Manual/SinglePassInstancing.html");
                        }
                    EditorGUILayout.EndHorizontal();
                });

                // TODO: Put in Quality settings section

                BuildSection("Depth Buffer Sharing", null, () =>
                {
                    EditorGUILayout.LabelField("This option shares the application's depth buffer with the running platform which allows the platform to more accurately stabilize holograms and content.", EditorStyles.wordWrappedLabel);
                    EditorGUILayout.LabelField("Note: Depth buffer sharing & format is a platform dependent feature.", EditorStyles.wordWrappedLabel);
                    enableDepthBufferSharing = EditorGUILayout.ToggleLeft("Enable Depth Buffer Sharing", enableDepthBufferSharing);

                    /*
                     * TODO: Finish rationalizing this section per platform
                    EditorGUILayout.LabelField("The depth format determines the level of precision for the depth texture (i.e 16-bit vs 24-bit etc). Using a lower precision format (i.e 16-bit) will generally result in performance gains however this also results in lower precision to resolve one object being farther than another. Thus, particularly for AR devices, it is recommended to lower the camera's far plane value so there is a reduced range of possible values.", EditorStyles.wordWrappedLabel);
                    enableDepthBufferSharing = EditorGUILayout.BeginToggleGroup("Enable Depth Buffer Sharing", enableDepthBufferSharing);

                    if (this.PerfTarget == PerformanceTarget.AR_Headsets)
                    {
                        enable16BitDepthBuffer = EditorGUILayout.ToggleLeft("Set Depth Buffer to 16-bit", enable16BitDepthBuffer);
                        enable16BitDepthBuffer = EditorGUILayout.ToggleLeft("Set MRTK Camera Far Plane to 50m", enable16BitDepthBuffer);
                    }
                    else if (this.PerfTarget == PerformanceTarget.VR_Standalone)
                    {
                        enable16BitDepthBuffer = EditorGUILayout.ToggleLeft("Set Depth Buffer to 16-bit", enable16BitDepthBuffer);
                        enable16BitDepthBuffer = EditorGUILayout.ToggleLeft("Set MRTK Camera Far Plane to 50m", enable16BitDepthBuffer);
                    }
                    EditorGUILayout.EndToggleGroup();
                    */
                });

                if (MixedRealityEditorUtility.RenderIndentedButton("Optimize Project"))
                {
                    OptimizeProject();
                }
            }

            GUILayout.EndVertical();
        }

        private void AnalyzeScene()
        {
            this.sceneLights = FindObjectsOfType<Light>();

            // TODO: Consider searching for particle renderers count?

            totalActivePolyCount = totalInActivePolyCount = 0;
            var filters = FindObjectsOfType<MeshFilter>();
            var meshes = new List<MeshNode>();
            foreach(var f in filters)
            {
                int count = f.sharedMesh.triangles.Length / 3;

                meshes.Add(new MeshNode
                {
                    polycount = count,
                    filter = f
                });

                if (f.gameObject.activeInHierarchy)
                {
                    totalActivePolyCount += count;
                }
                else
                {
                    totalInActivePolyCount += count;
                }
            }

            TotalPolyCountStr = "Total Scene PolyCount: " + totalPolyCount.ToString("N0") + " ";
            TotalActivePolyCountStr = "Total PolyCount (Active): " + totalActivePolyCount.ToString("N0") + " ";
            TotalInactivePolyCountStr = "Total PolyCount (Inactive): "+ totalInActivePolyCount.ToString("N0") + " ";

            var sortedMeshList = meshes.OrderByDescending(s => s.polycount).ToList();
            for(int i = 0; i < TopListSize; i++)
            {
                this.LargestMeshSizes[i] = 0;
                this.LargestMeshes[i] = null;
                if (i < meshes.Count)
                {
                    this.LargestMeshSizes[i] = sortedMeshList[i].polycount;
                    this.LargestMeshes[i] = sortedMeshList[i].filter.gameObject;
                }
            }
        }

        private void OptimizeProject()
        {
            if (singlePassInstanced)
            {
                PlayerSettings.stereoRenderingPath = StereoRenderingPath.Instancing;
            }

            if (enableDepthBufferSharing)
            {
                MixedRealityOptimizeUtils.SetDepthBufferSharing(enableDepthBufferSharing);
                // TODO: This value needs to be per-perf target
                //MixedRealityOptimizeUtils.SetDepthBufferFormat(enable16BitDepthBuffer);
            }
        }

        private void OptimizeScene()
        {
            var lightmapSettings = MixedRealityOptimizeUtils.GetLighmapSettings();

            if (disableRealtimeGlobalIllumination)
            {
                MixedRealityOptimizeUtils.ChangeProperty(lightmapSettings, "m_GISettings.m_EnableRealtimeLightmaps", property => property.boolValue = false);
            }

            if (disableBakedGlobalIllumination)
            {
                MixedRealityOptimizeUtils.ChangeProperty(lightmapSettings, "m_GISettings.m_EnableBakedLightmaps", property => property.boolValue = false);
            }
        }

        private void DiscoverMaterials()
        {
            discoveredMaterials.Clear();

            string[] guids = AssetDatabase.FindAssets("t:Material");
            for (int i = 0; i < guids.Length; i++)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
                if (assetPath.EndsWith(".mat"))
                {
                    Material asset = AssetDatabase.LoadAssetAtPath<Material>(assetPath);

                    if (CanConvertMaterial(asset))
                    {
                        discoveredMaterials.Add(asset);
                    }
                }
            }
        }
        private void ConvertMaterials()
        {
            Undo.RecordObjects(this.discoveredMaterials.ToArray(), "Convert to MRTK Standard shader");

            foreach (Material asset in this.discoveredMaterials)
            {
                ConvertMaterial(asset);
            }

            discoveredMaterials.Clear();
        }

        private void ConvertMaterial(Material asset)
        {
            if (asset != null && CanConvertMaterial(asset))
            {
                asset.shader = replacementShader;
            }
        }

        private bool CanConvertMaterial(Material asset)
        {
            return asset != null
                && ((asset.shader != replacementShader && (!onlyUnityShader || asset.shader == unityStandardShader))
                        || asset.shader == errorShader);
        }

        private static void BuildTitle(string title, string url)
        {
            // Section Title
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(title, EditorStyles.boldLabel);
            if (!string.IsNullOrEmpty(url))
            {
                BuildHelpIconButton(() => Application.OpenURL(url));
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.LabelField(string.Empty, GUI.skin.horizontalSlider);
        }

        private static void BuildHelpIconButton(Action onClick)
        {
            if (GUILayout.Button(EditorGUIUtility.IconContent("_Help", "|Learn more"), HelpIconStyle))
            {
                onClick();
            }
        }

        private static void BuildSection(string title, string url, Action renderContent)
        {
            EditorGUILayout.BeginVertical();
                // Section Title
                BuildTitle(title, url);

                renderContent();
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();
        }

        private void FindShaders()
        {
            replacementShader = Shader.Find("Mixed Reality Toolkit/Standard");
            unityStandardShader = Shader.Find("Standard");
            errorShader = Shader.Find("Hidden/InternalErrorShader");
        }

        private static void CreateStyles()
        {
            HelpIconStyle = new GUIStyle()
            {
                fixedWidth = 24,
                border = new RectOffset(0, 0, 0, 0),
            };

            BoldLargeTitle = new GUIStyle()
            {
                fontSize = 12,
                fontStyle = FontStyle.Bold,
            };
        }

        private static string GetRelativeTime(DateTime? time)
        {
            if (time == null) return string.Empty;

            var delta = new TimeSpan(DateTime.UtcNow.Ticks - time.Value.Ticks);

            if (Math.Abs(delta.TotalDays) > 1.0)
            {
                return (int)Math.Abs(delta.TotalDays) + " days ago";
            }
            else if (Math.Abs(delta.TotalHours) > 1.0)
            {
                return (int)Math.Abs(delta.TotalHours) + " hours ago";
            }
            else if (Math.Abs(delta.TotalMinutes) > 1.0)
            {
                return (int)Math.Abs(delta.TotalMinutes) + " minutes ago";
            }
            else 
            {
                return (int)Math.Abs(delta.TotalSeconds) + " seconds ago";
            }
        }
    }
}