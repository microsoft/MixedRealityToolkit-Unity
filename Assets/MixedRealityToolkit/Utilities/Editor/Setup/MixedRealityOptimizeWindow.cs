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
        private Vector2 windowScrollPosition = new Vector2();
        private Vector2 discoveredMaterialsScrollPosition = new Vector2();


        // Scene Optimizations
        private static DateTime? lastAnalyzedTime = null;
        private Light[] sceneLights;
        private const uint kSize = 5;
        private long totalActivePolyCount, totalInActivePolyCount = 0;
        private long totalPolyCount
        {
            get
            {
                return totalActivePolyCount + totalInActivePolyCount;
            }
        }
        private string TotalPolyCountStr, TotalActivePolyCountStr, TotalInactivePolyCountStr;
        private GameObject[] LargestMeshes = new GameObject[kSize];
        private int[] LargestMeshSizes = new int[kSize];

        // Shader Optimizations
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
        private readonly string[] PerformanceTargetDescriptions = {
            "Suggest performance optimizations for AR devices with mobile class specifications",
            "Suggest performance optimizations for mobile VR devices with mobile class specifications",
            "Suggest performance optimizations for VR devices tethered to a PC" };

        private readonly int[] SceneLightCountMax = { 1, 2, 4 };

        [SerializeField]
        private PerformanceTarget PerfTarget = PerformanceTarget.AR_Headsets;

        private Sprite MRTKIcon;
        protected static GUIStyle TitleStyle = new GUIStyle()
        {
            fixedHeight = 48,
            fontStyle = FontStyle.Bold,
        };

        protected static GUIStyle HelpIconStyle = new GUIStyle()
        {
            fixedWidth = 24,
            border = new RectOffset(0, 0, 0, 0),
        };

        protected static GUIStyle BoldLargeFoldout = new GUIStyle(EditorStyles.foldout)
        {
            fontSize = 12,
            fontStyle = FontStyle.Bold,
        };

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

            var results = AssetDatabase.FindAssets("t:sprite MRTK_Logo_Black");
            if (results.Length >= 1)
            {
                MRTKIcon = (Sprite)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(results[0]), typeof(Sprite));
            }
        }

        private void OnGUI()
        {
            // TODO: Hide things if already set/optimized
            // TODO: Filter or look at current build target => Ie also enable depth buffer sharing for oculus?

            windowScrollPosition = EditorGUILayout.BeginScrollView(windowScrollPosition);

            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
                EditorGUILayout.LabelField(new GUIContent(this.MRTKIcon.texture), GUILayout.Height(48), GUILayout.MaxWidth(100));
                EditorGUILayout.LabelField("Mixed Reality Toolkit Optimize Window", TitleStyle);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.LabelField("This tool automates the process of updating your project, currently open scene, and material assets to recommended settings for Mixed Reality", EditorStyles.wordWrappedLabel);
            EditorGUILayout.Space();

            PerfTarget = (PerformanceTarget)EditorGUILayout.EnumPopup("Performance Target", this.PerfTarget);
            EditorGUILayout.HelpBox(PerformanceTargetDescriptions[(int)PerfTarget], MessageType.Info);

            if (!PlayerSettings.virtualRealitySupported)
            {
                EditorGUILayout.HelpBox("Current build target does not have virtual reality support enabled", MessageType.Warning);
            }

            GUILayout.BeginVertical("Box");
                showProjectOptimizations = EditorGUILayout.Foldout(showProjectOptimizations, "Project Optimizations", true, BoldLargeFoldout);
                if (showProjectOptimizations)
                {
                    using (new EditorGUI.IndentLevelScope())
                    {
                        BuildSection("Single Pass Instanced Rendering", "https://docs.unity3d.com/Manual/SinglePassInstancing.html", () =>
                        {
                            EditorGUILayout.LabelField("Long explanation of how single pass rendering works here", EditorStyles.wordWrappedLabel);
                            singlePassInstanced = EditorGUILayout.ToggleLeft("Set Single Pass Instanced Rendering", singlePassInstanced);
                        });

                        BuildSection("Quality Settings", "www.google.com", () =>
                        {
                            EditorGUILayout.LabelField("Long explanation of how single pass rendering works here", EditorStyles.wordWrappedLabel);
                            setQualityLevel = EditorGUILayout.ToggleLeft("Set Quality Level to Lowest", setQualityLevel);
                        });

                        BuildSection("Depth Buffer Sharing", "www.google.com", () =>
                        {
                            EditorGUILayout.LabelField("Long explanation of how single pass rendering works here", EditorStyles.wordWrappedLabel);
                            enableDepthBufferSharing = EditorGUILayout.BeginToggleGroup("Enable Depth Buffer Sharing", enableDepthBufferSharing);
                            enable16BitDepthBuffer = EditorGUILayout.ToggleLeft("Set Depth Buffer to 16-bit", enable16BitDepthBuffer);
                            // TODO: Set camera to far plane of 100m? Or have this automatically in scene configure?
                            EditorGUILayout.EndToggleGroup();
                        });

                        if (GUILayout.Button("Optimize Project"))
                        {
                            OptimizeProject();
                        }
                    }
                }
            GUILayout.EndVertical();
            EditorGUILayout.Space();

            GUILayout.BeginVertical("Box");
                showSceneOptimizations = EditorGUILayout.Foldout(showSceneOptimizations, "Scene Optimizations", true, BoldLargeFoldout);
                if (showSceneOptimizations)
                {
                    using (new EditorGUI.IndentLevelScope())
                    {
                        EditorGUILayout.LabelField("This section provides controls and performance information for the currently opened scene. Any optimizations performed are only for the active scene at any moment. Also be aware that changes made while in editor Play mode will not be saved.", EditorStyles.wordWrappedLabel);
                        
                        BuildSection("Lighting Settings", "https://docs.unity3d.com/Manual/GlobalIllumination.html", () =>
                        {
                            EditorGUILayout.LabelField("Global Illumination can produce great visual results but sometimes at great expense. Scene lighting settings can be found under Window > Rendering > Lighting Settings in Unity.", EditorStyles.wordWrappedLabel);

                            disableRealtimeGlobalIllumination = EditorGUILayout.ToggleLeft("Disable Realtime Global Illumination", disableRealtimeGlobalIllumination);

                            if (this.PerfTarget == PerformanceTarget.AR_Headsets)
                            {
                                disableBakedGlobalIllumination = EditorGUILayout.ToggleLeft("Disable Baked Global Illumination", disableBakedGlobalIllumination);
                            }

                            if (GUILayout.Button("Optimize Lighting"))
                            {
                                OptimizeScene();
                            }
                        });

                        BuildSection("Live Scene Analysis", null, () =>
                        {
                            EditorGUILayout.BeginHorizontal();
                                EditorGUILayout.LabelField(lastAnalyzedTime == null ? "Click analysis button for MRTK to scan your currently opened scene" : "Scanned " + GetRelativeTime(lastAnalyzedTime));

                                if (GUILayout.Button("Analyze Scene", GUILayout.Width(200f)))
                                {
                                    AnalyzeScene();
                                    lastAnalyzedTime = DateTime.UtcNow;
                                }
                            EditorGUILayout.EndHorizontal();
                            EditorGUILayout.Space();

                            if (lastAnalyzedTime != null)
                            {
                                // Lighting analysis
                                if (this.sceneLights != null && this.sceneLights.Length > SceneLightCountMax[(int)PerfTarget])
                                {
                                    EditorGUILayout.LabelField("You currently have " + this.sceneLights.Length + " lights in your scene. Consider reducing this");
                                }

                                if (this.PerfTarget == PerformanceTarget.AR_Headsets)
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

                                EditorGUILayout.BeginHorizontal();
                                    EditorGUILayout.LabelField(TotalPolyCountStr);
                                    EditorGUILayout.LabelField(TotalActivePolyCountStr);
                                    EditorGUILayout.LabelField(TotalInactivePolyCountStr);
                                EditorGUILayout.EndHorizontal();

                                for (int i = 0; i < LargestMeshes.Length; i++)
                                {
                                    if (LargestMeshes[i] != null)
                                    {
                                        EditorGUILayout.BeginHorizontal();
                                            EditorGUILayout.ObjectField("Num of Polygons: " + this.LargestMeshSizes[i].ToString("N0") + "    ", this.LargestMeshes[i], typeof(GameObject), true);
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
                }
            GUILayout.EndVertical();
            EditorGUILayout.Space();

            GUILayout.BeginVertical("Box");
                showShaderOptimizations = EditorGUILayout.Foldout(showShaderOptimizations, "Shader Optimizations", true, BoldLargeFoldout);
                if (showShaderOptimizations)
                {
                    using (new EditorGUI.IndentLevelScope())
                    {
                        EditorGUILayout.LabelField("The Unity standard shader is generally not performant or optimized for Mixed Reality development. The MRTK Standard shader can be a more performant option."
                        + "This tool allows developers to discover and automatically convert materials in their project to the replacement shader option below. It is recommended to utilize the MRTK Standard Shader."
                            , EditorStyles.wordWrappedLabel);
                        EditorGUILayout.Space();
                        
                        replacementShader = (Shader)EditorGUILayout.ObjectField("Replacement Shader", replacementShader, typeof(Shader), false);

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
                            EditorGUILayout.LabelField("Discovered " + discoveredMaterials.Count + " materials to convert");

                            EditorGUILayout.BeginHorizontal();
                                EditorGUILayout.LabelField("Current Shader", EditorStyles.boldLabel);
                                EditorGUILayout.LabelField("Material Asset", EditorStyles.boldLabel);
                            EditorGUILayout.EndHorizontal();

                            discoveredMaterialsScrollPosition = EditorGUILayout.BeginScrollView(discoveredMaterialsScrollPosition, GUILayout.Height(125));
                            for (int i = 0; i < discoveredMaterials.Count; i++)
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
                                        ConvertMaterial(this.discoveredMaterials[i]);
                                    }
                                EditorGUILayout.EndHorizontal();
                            }
                            EditorGUILayout.EndScrollView();
                        }
                    }
                }
            GUILayout.EndVertical();

            EditorGUILayout.EndScrollView();
        }

        private void AnalyzeScene()
        {
            this.sceneLights = FindObjectsOfType<Light>();

            // TODO: Consider searching for particle renderers count?

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
            for(int i = 0; i < kSize; i++)
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

            // TODO: Enable Depth Buffer Sharing may be enabled, but we would still like them to be at 16-bit
            if (enableDepthBufferSharing)
            {
                MixedRealityOptimizeUtils.SetDepthBufferSharing(enableDepthBufferSharing, enable16BitDepthBuffer);
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
                Material asset = AssetDatabase.LoadAssetAtPath<Material>(assetPath);

                if (CanConvertMaterial(asset))
                {
                    discoveredMaterials.Add(asset);
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
            return asset != null && ((asset.shader != replacementShader && (!onlyUnityShader || asset.shader == unityStandardShader))
                        || asset.shader == errorShader);
        }

        private static void BuildTitle(string title, string url)
        {
            // Section Title
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(title, EditorStyles.boldLabel);
            if (!string.IsNullOrEmpty(url))
            {
                if (GUILayout.Button(EditorGUIUtility.IconContent("_Help", "|Learn more"), HelpIconStyle))
                {
                    Application.OpenURL(url);
                }
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.LabelField(string.Empty, GUI.skin.horizontalSlider);
        }

        private static void BuildSection(string title, string url, Action renderContent)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                // Section Title
                BuildTitle(title, url);

                renderContent();
            EditorGUILayout.EndVertical();
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