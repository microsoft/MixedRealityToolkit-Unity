// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
        private const int TopListSize = 5;
        private long totalActivePolyCount, totalInActivePolyCount = 0;
        private int totalRaycastableUnityUI_Text = 0;
        private int totalRaycastableUnityUI_TMP_UGUI = 0;

        private long TotalPolyCount
        {
            get => totalActivePolyCount + totalInActivePolyCount;
        }

        private string TotalPolyCountStr
        {
            get => $"Total Scene PolyCount: {TotalPolyCount.ToString("N0")} ";
        }

        private string TotalActivePolyCountStr
        {
            get => $"Total PolyCount (Active): {totalActivePolyCount.ToString("N0")} ";
        }

        private string TotalInactivePolyCountStr
        {
            get => $"Total PolyCount (Inactive): {totalInActivePolyCount.ToString("N0")} ";
        }

        private MeshFilter[] MeshesOrderedByPolyCountDesc;

        // Shader Optimizations
        private bool showDiscoveredMaterials = true;
        private bool onlyUnityShader = true;
        private Shader replacementShader;
        private Shader unityStandardShader;
        private Shader errorShader;
        private List<Material> discoveredMaterials = new List<Material>();

        protected enum PerformanceTarget
        {
            AR_Headsets,
            VR_Standalone,
            VR_Tethered,
        };

        private static readonly GUIContent ViewAssetLabel = new GUIContent("View", "Selects & view this asset in inspector");
        private static readonly GUIContent ConvertMaterialLabel = new GUIContent("Convert", "Converts this material's shader to the replacement shader");
        private static readonly GUIContent UseMRTKStandardShaderLabel = new GUIContent("Use MRTK Standard Shader", "Set Replacement Shader to MRKT Standard Shader");

        // NOTE: These array must match the number of enums in PerformanceTarget
        private readonly string[] PerformanceTargetEnums = { "AR Headsets", "VR Standalone", "VR Tethered" };
        private readonly string[] PerformanceTargetDescriptions = {
            "Suggest performance optimizations for AR devices with mobile class specifications",
            "Suggest performance optimizations for mobile VR devices with mobile class specifications",
            "Suggest performance optimizations for VR devices tethered to a PC" };

        private const string OptimizeWindow_URL = "https://microsoft.github.io/MixedRealityToolkit-Unity/Documentation/Tools/OptimizeWindow.html";
#if UNITY_ANDROID
        private const string OptimalRenderingPath_URL = "https://docs.unity3d.com/Manual/SinglePassStereoRendering.html";
#else
        private const string OptimalRenderingPath_URL = "https://docs.unity3d.com/Manual/SinglePassInstancing.html";
#endif
        private const string DepthBufferSharing_URL = "https://microsoft.github.io/MixedRealityToolkit-Unity/Documentation/hologram-stabilization.html#depth-buffer-sharing";
        private const string DepthBufferFormat_URL = "https://microsoft.github.io/MixedRealityToolkit-Unity/Documentation/hologram-stabilization.html#depth-buffer-format";
        private const string GlobalIllumination_URL = "https://docs.unity3d.com/Manual/GlobalIllumination.html";

#if UNITY_ANDROID
        private const string RenderingMode = "Single Pass Stereo Rendering";
        private const string GpuMode = "Single Pass Stereo Rendering";
#else
        private const string RenderingMode = "Single Pass Instanced Rendering";
        private const string GpuMode = "GPU Instancing";
#endif

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
                GUILayout.Label("Mixed Reality Toolkit Optimize Window", MixedRealityStylesUtility.BoldLargeTitleStyle);
                InspectorUIUtility.RenderDocumentationButton(OptimizeWindow_URL);
            }

            EditorGUILayout.LabelField("This tool automates the process of updating your project, currently open scene, and material assets to recommended settings for Mixed Reality", EditorStyles.wordWrappedLabel);
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Active Build Target: ", EditorUserBuildSettings.activeBuildTarget.ToString());

            PerfTarget = (PerformanceTarget)EditorGUILayout.Popup("Performance Target", (int)this.PerfTarget, PerformanceTargetEnums);
            EditorGUILayout.HelpBox(PerformanceTargetDescriptions[(int)PerfTarget], MessageType.Info);
            EditorGUILayout.Space();

            if (!XRSettingsUtilities.LegacyXREnabled)
            {
                EditorGUILayout.HelpBox("Virtual reality support is not enabled in player settings", MessageType.Error);
                if (GUILayout.Button("Enable Virtual Reality Support"))
                {
                    XRSettingsUtilities.LegacyXREnabled = true;
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

        #region Shader Optimizations

        private void RenderShaderOptimizations()
        {
            using (new GUILayout.VerticalScope("Box"))
            {
                GUILayout.Label(ToolbarTitles[(int)ToolbarSection.Shader], MixedRealityStylesUtility.BoldLargeTitleStyle);
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
                        if (InspectorUIUtility.RenderIndentedButton(UseMRTKStandardShaderLabel, EditorStyles.miniButton, GUILayout.Width(200f)))
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

                            using (new EditorGUI.DisabledGroupScope(discoveredMaterials.Count == 0))
                            {
                                if (GUILayout.Button("Convert All Discovered Materials"))
                                {
                                    ConvertMaterials();
                                }
                            }
                        }

                        RenderDiscoveredMaterials();
                    }
                }
            }
            EditorGUILayout.Space();
        }

        private void RenderDiscoveredMaterials()
        {
            if (discoveredMaterials.Count > 0)
            {
                showDiscoveredMaterials = EditorGUILayout.Foldout(showDiscoveredMaterials, "Discovered Materials", true);
                if (showDiscoveredMaterials)
                {
                    using (new EditorGUI.IndentLevelScope())
                    {
                        EditorGUILayout.LabelField($"Discovered {discoveredMaterials.Count} materials to convert");

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

                                    RenderViewAssetButton(discoveredMaterials[i]);

                                    if (GUILayout.Button(ConvertMaterialLabel, EditorStyles.miniButton, GUILayout.Width(64f)))
                                    {
                                        Undo.RecordObject(discoveredMaterials[i], "Convert to MRTK Standard shader");
                                        ConvertMaterial(discoveredMaterials[i]);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        #endregion

        #region Render Scene Optimization Sections

        private void RenderSceneOptimizations()
        {
            using (new GUILayout.VerticalScope("Box"))
            {
                GUILayout.Label(ToolbarTitles[(int)ToolbarSection.Scene], MixedRealityStylesUtility.BoldLargeTitleStyle);
                using (new EditorGUI.IndentLevelScope())
                {
                    EditorGUILayout.LabelField("This section provides controls and performance information for the currently opened scene. Any optimizations performed are only for the active scene at any moment.", EditorStyles.wordWrappedLabel);
                    EditorGUILayout.Space();

                    using (new EditorGUILayout.HorizontalScope())
                    {
                        string analysisTimeLabel = lastAnalyzedTime == null ? "Click analysis button for MRTK to scan your currently opened scene" : "Scanned " + lastAnalyzedTime.Value.GetRelativeTime();
                        EditorGUILayout.LabelField(analysisTimeLabel);

                        if (GUILayout.Button("Analyze Scene", GUILayout.Width(160f)))
                        {
                            AnalyzeScene();
                            lastAnalyzedTime = DateTime.UtcNow;
                        }
                    }
                    EditorGUILayout.Space();

                    if (lastAnalyzedTime != null)
                    {
                        // Lighting analysis
                        RenderLightingSceneAnalysisSection();

                        // Mesh Analysis
                        RenderMeshSceneAnalysisSection();

                        // Unity UI Raycast Analysis
                        RenderRaycastAnalysisSection();
                    }
                }
            }
        }

        private void RenderMeshSceneAnalysisSection()
        {
            EditorGUILayout.LabelField("Polygon Count Analysis", EditorStyles.boldLabel);

            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField(TotalPolyCountStr);
                EditorGUILayout.LabelField(TotalActivePolyCountStr);
                EditorGUILayout.LabelField(TotalInactivePolyCountStr);
            }

            EditorGUILayout.LabelField($"Top {TopListSize} GameObjects in scene with highest polygon count");
            int length = Math.Min(MeshesOrderedByPolyCountDesc.Length, TopListSize);
            for (int i = 0; i < length; i++)
            {
                var meshFilter = MeshesOrderedByPolyCountDesc[i];
                if (meshFilter != null)
                {
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        int polyCount = meshFilter.sharedMesh.triangles.Length / 3;
                        EditorGUILayout.LabelField($"Num of Polygons: {polyCount.ToString("N0")}");

                        using (new EditorGUI.DisabledGroupScope(true))
                        {
                            EditorGUILayout.ObjectField(meshFilter, typeof(MeshFilter), true);
                        }

                        RenderViewAssetButton(meshFilter);
                    }
                }
            }

            EditorGUILayout.Space();
        }

        private void RenderRaycastAnalysisSection()
        {
            EditorGUILayout.LabelField("Unity UI Raycast Analysis", EditorStyles.boldLabel);
            EditorGUILayout.LabelField($"To optimize Graphics Raycast operations, disable Raycast Target property for all elements that do not require this functionality.");
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Num of GameObjects with Raycast Target enabled", EditorStyles.miniBoldLabel);
            RenderRaycastItem<Text>(totalRaycastableUnityUI_Text);
            RenderRaycastItem<TextMeshProUGUI>(totalRaycastableUnityUI_TMP_UGUI);

            EditorGUILayout.Space();
        }

        private void RenderRaycastItem<T>(int itemCount) where T : Graphic
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                string typeName = typeof(T).Name;
                EditorGUILayout.LabelField($"{typeName}:", $"{itemCount.ToString("N0")}");
                GUILayout.FlexibleSpace();
                if (GUILayout.Button($"Disable Raycast Target for all {typeName}"))
                {
                    DisableRaycastTargetAll<T>();

                    AnalyzeRaycastTargets();
                }
            }
        }

        private void RenderLightingSceneAnalysisSection()
        {
            bool showNumOfSceneLights = sceneLights != null && sceneLights.Length > SceneLightCountMax[(int)PerfTarget];
            bool showDisableShadows = PerfTarget == PerformanceTarget.AR_Headsets;
            if (showNumOfSceneLights || showDisableShadows)
            {
                EditorGUILayout.LabelField("Lighting Analysis", EditorStyles.boldLabel);
                if (showNumOfSceneLights)
                {
                    EditorGUILayout.LabelField(this.sceneLights.Length + " lights in the scene. Consider reducing the number of lights.");
                }

                if (showDisableShadows)
                {
                    foreach (var l in sceneLights)
                    {
                        if (l != null && l.shadows != LightShadows.None)
                        {
                            EditorGUILayout.ObjectField("Disable shadows", l, typeof(Light), true);
                        }
                    }
                }
                EditorGUILayout.Space();
            }
        }

        #endregion

        private void RenderSettingOptimizations()
        {
            using (new GUILayout.VerticalScope("Box"))
            {
                GUILayout.Label(ToolbarTitles[(int)ToolbarSection.Settings], MixedRealityStylesUtility.BoldLargeTitleStyle);
                using (new EditorGUI.IndentLevelScope())
                {
                    RenderOptimalRenderingSection();

                    RenderDepthBufferSharingSection();

                    RenderDepthBufferFormatSection();

                    RenderGlobalIlluminationSection();
                }
            }
        }

        private void RenderGlobalIlluminationSection()
        {
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

                if (isGIEnabled)
                {
                    EditorGUILayout.HelpBox("Note: Real-time Global Illumination is a per-scene setting.", MessageType.Info);

                    if (InspectorUIUtility.RenderIndentedButton("Disable Real-time Global Illumination"))
                    {
                        MixedRealityOptimizeUtils.SetRealtimeGlobalIlluminationEnabled(false);
                    }
                }
            });
        }

        private void RenderDepthBufferFormatSection()
        {
            bool is16BitDepthFormat = MixedRealityOptimizeUtils.IsWMRDepthBufferFormat16bit();
            BuildSection("Depth Buffer Format", DepthBufferFormat_URL, GetTitleIcon(is16BitDepthFormat), () =>
            {
                EditorGUILayout.LabelField("If sharing the depth buffer with the underlying mixed reality platform, it is generally recommended to utilize a 16-bit depth format buffer to save on performance.", EditorStyles.wordWrappedLabel);

                if (!is16BitDepthFormat)
                {
                    EditorGUILayout.HelpBox("Although 16-bit depth format is better performing, it can result in z-fighting if the far clip plane is too far. Furthermore, no stencil buffer will be created with 16-bit selected. Click the \"Documentation\" button to learn more", MessageType.Info);

                    if (InspectorUIUtility.RenderIndentedButton("Enable 16-bit depth format"))
                    {
                        MixedRealityOptimizeUtils.SetDepthBufferFormat(true);
                    }
                }
            });
        }

        private void RenderDepthBufferSharingSection()
        {
            bool isDepthBufferSharingEnabled = MixedRealityOptimizeUtils.IsDepthBufferSharingEnabled();
            BuildSection("Depth Buffer Sharing", DepthBufferSharing_URL, GetTitleIcon(isDepthBufferSharingEnabled), () =>
            {
                EditorGUILayout.LabelField("This option shares the application's depth buffer with the running platform which allows the platform to more accurately stabilize holograms and content.", EditorStyles.wordWrappedLabel);

                if (!isDepthBufferSharingEnabled)
                {
                    EditorGUILayout.HelpBox("Depth buffer sharing requires that a valid depth buffer is submitted to the platform. Click the \"Documentation\" button for instructions to ensure that transparent & text GameObjects write to depth.", MessageType.Info);

                    if (InspectorUIUtility.RenderIndentedButton("Enable Depth Buffer Sharing"))
                    {
                        MixedRealityOptimizeUtils.SetDepthBufferSharing(true);
                    }
                }
            });
        }

        private void RenderOptimalRenderingSection()
        {
            bool isOptimalRenderingPath = MixedRealityOptimizeUtils.IsOptimalRenderingPath();
            BuildSection(RenderingMode, OptimalRenderingPath_URL, GetTitleIcon(isOptimalRenderingPath), () =>
            {
                EditorGUILayout.LabelField($"{RenderingMode} is an option in the Unity graphics pipeline to more efficiently render your scene and optimize CPU & GPU work.");

                if (!isOptimalRenderingPath)
                {
                    EditorGUILayout.HelpBox($"This rendering configuration requires shaders to be written to support {GpuMode} which is automatic in all Unity & MRTK shaders.Click the \"Documentation\" button for instruction to update your custom shaders to support {GpuMode}.", MessageType.Info);

                    if (InspectorUIUtility.RenderIndentedButton(RenderingMode))
                    {
                        MixedRealityOptimizeUtils.SetOptimalRenderingPath();
                    }
                }
            });
        }

        #region Utility Helpers

        private void AnalyzeScene()
        {
            sceneLights = FindObjectsOfType<Light>();

            AnalyzeRaycastTargets();

            // TODO: Consider searching for particle renderers count?

            MeshesOrderedByPolyCountDesc = FindObjectsOfType<MeshFilter>()
                .Where(f => f != null && f.sharedMesh != null)
                .OrderByDescending(f => f.sharedMesh.triangles.Length)
                .ToArray();

            totalActivePolyCount = totalInActivePolyCount = 0;
            for (int i = 0; i < MeshesOrderedByPolyCountDesc.Length; i++)
            {
                var f = MeshesOrderedByPolyCountDesc[i];

                int count = f.sharedMesh.triangles.Length / 3;

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

        private void AnalyzeRaycastTargets()
        {
            totalRaycastableUnityUI_Text = FindObjectsOfType<Text>().Where(t => t.raycastTarget).Count();
            totalRaycastableUnityUI_TMP_UGUI = FindObjectsOfType<TextMeshProUGUI>().Where(t => t.raycastTarget).Count();
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

        private static void DisableRaycastTargetAll<T>() where T : Graphic
        {
            DisableRaycastTarget(FindObjectsOfType<T>());
        }

        private static void DisableRaycastTarget(Graphic[] elements)
        {
            if (elements == null)
            {
                return;
            }

            for (int i = 0; i < elements.Length; i++)
            {
                elements[i].raycastTarget = false;
            }
        }

        private void FindShaders()
        {
            replacementShader = StandardShaderUtility.MrtkStandardShader;
            unityStandardShader = Shader.Find("Standard");
            errorShader = Shader.Find("Hidden/InternalErrorShader");
        }

        private bool IsHololensTargeted()
        {
            return PerfTarget == PerformanceTarget.AR_Headsets && MixedRealityOptimizeUtils.IsBuildTargetUWP();
        }

        #endregion

        #region Render Helpers

        private static void RenderViewAssetButton(UnityEngine.Object asset)
        {
            if (GUILayout.Button(ViewAssetLabel, EditorStyles.miniButton, GUILayout.Width(42f)))
            {
                Selection.activeObject = asset;
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

        private Texture GetTitleIcon(bool isValid)
        {
            return isValid ? InspectorUIUtility.SuccessIcon : InspectorUIUtility.WarningIcon;
        }

        #endregion
    }
}