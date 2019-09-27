// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
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
        private readonly string[] ToolbarTitles = { "Setting Optimizations", "Scene Analysis", "Shader Analysis" };
        private enum ToolbarSection
        {
            Settings = 0,
            Scene,
            Shader
        };

        private Vector2 windowScrollPosition = new Vector2();

        // Scene Optimizations
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

        private const string OptimizeWindow_URL = "https://microsoft.github.io/MixedRealityToolkit-Unity/Documentation/Tools/OptimizeWindow.html";
        private const string SinglePassInstanced_URL = "https://docs.unity3d.com/Manual/SinglePassInstancing.html";
        private const string DepthBufferSharing_URL = "https://microsoft.github.io/MixedRealityToolkit-Unity/Documentation/Hologram-Stabilization.html#Depth-Buffer-Sharing";
        private const string DepthBufferFormat_URL = "https://microsoft.github.io/MixedRealityToolkit-Unity/Documentation/Hologram-Stabilization.html#depth-buffer-format";
        private const string GlobalIllumination_URL = "https://docs.unity3d.com/Manual/GlobalIllumination.html";

        private readonly int[] SceneLightCountMax = { 1, 2, 4 };

        [SerializeField]
        private PerformanceTarget PerfTarget = PerformanceTarget.AR_Headsets;

        [MenuItem("Mixed Reality Toolkit/Utilities/Optimize Window", false, 0)]
        public static void OpenWindow()
        {
            // Dock it next to the Scene View.
            var window = GetWindow<MixedRealityOptimizeWindow>(typeof(SceneView));
            window.titleContent = new GUIContent("Optimize Window", EditorGUIUtility.IconContent("d_PreMatCube").image);
            window.StartUp();
            window.Show();
        }

        public void StartUp()
        {
            FindShaders();
        }

        private void OnGUI()
        {
            windowScrollPosition = EditorGUILayout.BeginScrollView(windowScrollPosition);

            MixedRealityInspectorUtility.RenderMixedRealityToolkitLogo();

            // Render Title
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("Mixed Reality Toolkit Optimize Window", MixedRealityStylesUtility.BoldLargeTitleStyle);
                InspectorUIUtility.RenderDocumentationButton(OptimizeWindow_URL);
            }

            EditorGUILayout.LabelField("This tool automates the process of updating your project, currently open scene, and material assets to recommended settings for Mixed Reality", EditorStyles.wordWrappedLabel);
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Active Build Target: ", EditorUserBuildSettings.activeBuildTarget.ToString());

            PerfTarget = (PerformanceTarget)EditorGUILayout.Popup("Performance Target", (int)this.PerfTarget, PerformanceTargetEnums);
            EditorGUILayout.HelpBox(PerformanceTargetDescriptions[(int)PerfTarget], MessageType.Info);
            EditorGUILayout.Space();

            if (!PlayerSettings.virtualRealitySupported)
            {
                EditorGUILayout.HelpBox("Virtual reality support is not enabled in player settings", MessageType.Error);
                if (GUILayout.Button("Enable Virtual Reality Support"))
                {
                    PlayerSettings.virtualRealitySupported = true;
                }
            }
            else
            {
                selectedToolbarIndex = GUILayout.Toolbar(selectedToolbarIndex, ToolbarTitles);
                if (selectedToolbarIndex == 0)
                {
                    RenderSettingOptimizations();
                }
                else if (selectedToolbarIndex == 1)
                {
                    RenderSceneOptimizations();
                }
                else
                {
                    RenderShaderOptimizations();
                }
            }

            EditorGUILayout.EndScrollView();
        }

        private void RenderShaderOptimizations()
        {
            GUILayout.BeginVertical("Box");
            EditorGUILayout.LabelField(this.ToolbarTitles[(int)ToolbarSection.Shader], MixedRealityStylesUtility.BoldLargeTitleStyle);
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
                    if (InspectorUIUtility.RenderIndentedButton(new GUIContent("Use MRTK Standard Shader", "Set Replacement Shader to MRKT Standard Shader"), EditorStyles.miniButton, GUILayout.Width(200f)))
                    {
                        FindShaders();
                    }
                }
                else
                {
                    onlyUnityShader = EditorGUILayout.ToggleLeft("Only Discover Materials with Unity Standard Shader", onlyUnityShader);
                    EditorGUILayout.Space();

                    using (new EditorGUILayout.HorizontalScope())
                    {
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
                    }

                    if (discoveredMaterials.Count > 0)
                    {
                        showDiscoveredMaterials = EditorGUILayout.Foldout(showDiscoveredMaterials, "Discovered Materials", true);
                        if (showDiscoveredMaterials)
                        {
                            using (new EditorGUI.IndentLevelScope())
                            {
                                EditorGUILayout.LabelField("Discovered " + discoveredMaterials.Count + " materials to convert");

                                using (new EditorGUILayout.HorizontalScope())
                                {
                                    EditorGUILayout.LabelField("Current Shader", EditorStyles.boldLabel);
                                    EditorGUILayout.LabelField("Material Asset", EditorStyles.boldLabel);
                                }

                                for (int i = 0; i < discoveredMaterials.Count; i++)
                                {
                                    if (discoveredMaterials[i] != null)
                                    {
                                        using (new EditorGUILayout.HorizontalScope())
                                        {
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
                                        }
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
            EditorGUILayout.LabelField(this.ToolbarTitles[(int)ToolbarSection.Scene], MixedRealityStylesUtility.BoldLargeTitleStyle);
            using (new EditorGUI.IndentLevelScope())
            {
                EditorGUILayout.LabelField("This section provides controls and performance information for the currently opened scene. Any optimizations performed are only for the active scene at any moment.", EditorStyles.wordWrappedLabel);

                BuildSection("Live Scene Analysis", null, null, () =>
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
                                    if (l != null && l.shadows != LightShadows.None)
                                    {
                                        EditorGUILayout.ObjectField("Disable shadows", l, typeof(Light), true);
                                    }
                                }
                            }
                            EditorGUILayout.Space();
                        }

                        // Mesh Analysis
                        EditorGUILayout.LabelField("Polygon Count Analysis", EditorStyles.boldLabel);

                        using (new EditorGUILayout.HorizontalScope())
                        {
                            EditorGUILayout.LabelField(TotalPolyCountStr);
                            EditorGUILayout.LabelField(TotalActivePolyCountStr);
                            EditorGUILayout.LabelField(TotalInactivePolyCountStr);
                        }

                        EditorGUILayout.LabelField("Top " + TopListSize + " GameObjects in scene with highest polygon count");
                        for (int i = 0; i < LargestMeshes.Length; i++)
                        {
                            if (LargestMeshes[i] != null)
                            {
                                using (new EditorGUILayout.HorizontalScope())
                                {
                                    EditorGUILayout.LabelField("Num of Polygons: " + this.LargestMeshSizes[i].ToString("N0"));
                                    EditorGUILayout.ObjectField(this.LargestMeshes[i], typeof(GameObject), true);
                                    if (GUILayout.Button(new GUIContent("View", "Selects & view this asset in inspector"), EditorStyles.miniButton, GUILayout.Width(42f)))
                                    {
                                        Selection.activeObject = this.LargestMeshes[i];
                                    }
                                }
                            }
                        }
                    }
                });
            }

            GUILayout.EndVertical();
        }

        private void RenderSettingOptimizations()
        {
            GUILayout.BeginVertical("Box");
            EditorGUILayout.LabelField(this.ToolbarTitles[(int)ToolbarSection.Settings], MixedRealityStylesUtility.BoldLargeTitleStyle);
            using (new EditorGUI.IndentLevelScope())
            {
                bool isSinglePassInstancedEnabled = PlayerSettings.stereoRenderingPath == StereoRenderingPath.Instancing;
                BuildSection("Single Pass Instanced Rendering", SinglePassInstanced_URL, GetTitleIcon(isSinglePassInstancedEnabled), () =>
                {
                    EditorGUILayout.LabelField("Single Pass Instanced Rendering is an option in the Unity graphics pipeline to more efficiently render your scene and optimize CPU & GPU work.");

                    EditorGUILayout.HelpBox("This rendering configuration requires shaders to be written to support GPU instancing which is automatic in all Unity & MRTK shaders.Click the \"Documentation\" button for instruction to update your custom shaders to support instancing.", MessageType.Info);

                    using (new GUIEnabledWrapper(!isSinglePassInstancedEnabled))
                    {
                        if (InspectorUIUtility.RenderIndentedButton("Enable Single Pass Instanced rendering"))
                        {
                            PlayerSettings.stereoRenderingPath = StereoRenderingPath.Instancing;
                        }
                    }
                });

                // TODO: Put in Quality settings section

                bool isDepthBufferSharingEnabled = MixedRealityOptimizeUtils.IsDepthBufferSharingEnabled();
                BuildSection("Depth Buffer Sharing", DepthBufferSharing_URL, GetTitleIcon(isDepthBufferSharingEnabled), () =>
                {
                    EditorGUILayout.LabelField("This option shares the application's depth buffer with the running platform which allows the platform to more accurately stabilize holograms and content.", EditorStyles.wordWrappedLabel);

                    EditorGUILayout.HelpBox("Depth buffer sharing requires that a valid depth buffer is submitted to the platform. Click the \"Documentation\" button for instructions to ensure that transparent & text gameobjects write to depth.", MessageType.Info);

                    using (new GUIEnabledWrapper(!isDepthBufferSharingEnabled))
                    {
                        if (InspectorUIUtility.RenderIndentedButton("Enable Depth Buffer Sharing"))
                        {
                            MixedRealityOptimizeUtils.SetDepthBufferSharing(true);
                        }
                    }
                });

                bool is16BitDepthFormat = MixedRealityOptimizeUtils.IsWMRDepthBufferFormat16bit();
                BuildSection("Depth Buffer Format", DepthBufferFormat_URL, GetTitleIcon(is16BitDepthFormat), () =>
                {
                    EditorGUILayout.LabelField("If sharing the depth buffer with the underlying mixed reality platform, it is generally recommended to utilize a 16-bit depth format buffer to save on performance.", EditorStyles.wordWrappedLabel);

                    EditorGUILayout.HelpBox("Although 16-bit depth format is better performing, it can result in z-fighting if the far clip plane is too far. Click the \"Documentation\" button to learn more", MessageType.Info);

                    using (new GUIEnabledWrapper(!is16BitDepthFormat))
                    {
                        if (InspectorUIUtility.RenderIndentedButton("Enable 16-bit depth format"))
                        {
                            MixedRealityOptimizeUtils.SetDepthBufferFormat(true);
                        }
                    }
                });

                bool isGIEnabled = MixedRealityOptimizeUtils.IsRealtimeGlobalIlluminationEnabled();
                BuildSection("Real-time Global Illumination", GlobalIllumination_URL, GetTitleIcon(!isGIEnabled), () =>
                {
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        EditorGUILayout.LabelField("Real-time Global Illumination can produce great visual results but at great expense. It is recommended to disable this feature in lighting settings.", EditorStyles.wordWrappedLabel);
                        if (GUILayout.Button(new GUIContent("View Lighting Settings", "Open Lighting Settings"), EditorStyles.miniButton, GUILayout.Width(160f)))
                        {
                            EditorApplication.ExecuteMenuItem("Window/Rendering/Lighting Settings");
                        }
                    }

                    EditorGUILayout.HelpBox("Note: Real-time Global Illumination is a per-scene setting.", MessageType.Info);

                    using (new GUIEnabledWrapper(isGIEnabled))
                    {
                        if (InspectorUIUtility.RenderIndentedButton("Disable Real-time Global Illumination"))
                        {
                            MixedRealityOptimizeUtils.SetRealtimeGlobalIlluminationEnabled(false);
                        }
                    }
                });
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
                if (f != null && f.sharedMesh != null)
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
                && !asset.shader.name.StartsWith("Mixed Reality Toolkit")
                && ((asset.shader != replacementShader && (!onlyUnityShader || asset.shader == unityStandardShader))
                        || asset.shader == errorShader);
        }

        private static void BuildTitle(string title, string url, Texture titleIcon = null)
        {
            EditorGUILayout.LabelField(string.Empty, GUI.skin.horizontalSlider);
            // Section Title
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField(new GUIContent(title, titleIcon), EditorStyles.boldLabel);
                InspectorUIUtility.RenderDocumentationButton(url);
            }
        }

        private static void BuildSection(string title, string url, Texture titleIcon = null, Action renderContent = null)
        {
            EditorGUILayout.BeginVertical();
                // Section Title
                BuildTitle(title, url, titleIcon);
                renderContent?.Invoke();
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
        }

        private void FindShaders()
        {
            replacementShader = StandardShaderUtility.MrtkStandardShader;
            unityStandardShader = Shader.Find("Standard");
            errorShader = Shader.Find("Hidden/InternalErrorShader");
        }

        private bool IsHololensTargeted()
        {
            return PerfTarget == PerformanceTarget.AR_Headsets && EditorUserBuildSettings.activeBuildTarget == BuildTarget.WSAPlayer;
        }

        private Texture GetTitleIcon(bool isValid)
        {
            return isValid ? InspectorUIUtility.SuccessIcon: InspectorUIUtility.WarningIcon;
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