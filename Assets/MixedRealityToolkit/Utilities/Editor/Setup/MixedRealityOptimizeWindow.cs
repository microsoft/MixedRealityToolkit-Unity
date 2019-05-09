using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR;

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

        private Light[] sceneLights;

        private const uint kSize = 5;
        private long totalActivePolyCount, totalInActivePolyCount = 0;
        private GameObject[] LargestMeshes = new GameObject[kSize];
        private int[] LargestMeshSizes = new int[kSize];

        private Shader replacementShader;
        private Shader unityStandardShader;
        private Shader errorShader;
        private Sprite MRTKIcon;

        // Internal structure to easily search mesh polycounts in scene
        private struct MeshNode
        {
            public int polycount;
            public MeshFilter filter;
        }

        private enum PerformanceTarget
        {
            AR_Headsets,
            VR_Standalone,
            VR_Tethered
        };

        [SerializeField]
        private PerformanceTarget PerfTarget = PerformanceTarget.AR_Headsets;

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

            // TODO: This is broken?
            var results = AssetDatabase.FindAssets("t:sprite MRTK_Logo_Black");
            if (results.Length >= 1)
            {
                MRTKIcon = (Sprite)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(results[0]), typeof(Sprite));
            }
        }

        private GUIStyle GetIconButtonStyle()
        {
            var s = new GUIStyle();
            s.alignment = TextAnchor.MiddleLeft;
            s.fontStyle = FontStyle.Bold;
            s.fixedWidth = 24;
            var b = s.border;
            b.left = 0;
            b.top = 0;
            b.right = 0;
            b.bottom = 0;
            return s;
        }

        private void BuildTitle(string title, string url)
        {
            // Section Title
            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
            EditorGUILayout.LabelField(title, EditorStyles.boldLabel);
            if (GUILayout.Button(EditorGUIUtility.IconContent("_Help", "|Learn more"), GetIconButtonStyle()))
            {
                Application.OpenURL(url);
            }
            EditorGUILayout.EndHorizontal();
        }

        private void BuildSection(string title, string url, Action renderContent)
        {
            EditorGUILayout.BeginVertical();
                // Section Title
                BuildTitle(title, url);

                renderContent();
            EditorGUILayout.EndVertical();
        }

        private bool test = false;
        private int selected = 0;
        private string[] items = { "Camera", "Experience", "Boundary" };
        private void OnGUI()
        {
            // TODO: Hide things if already set/optimized
            // TODO: Filter or look at current build target => Ie also enable depth buffer sharing for oculus?

            // TODO: Put in custom mixed reality toolkit logo
            //EditorGUILayout.LabelField(new GUIContent("Mixed Reality Toolkit Optimize Window", this.MRTKIcon), EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
                EditorGUILayout.LabelField(new GUIContent(this.MRTKIcon.texture), GUILayout.Height(48), GUILayout.MaxWidth(100));

            var s = new GUIStyle();
            s.alignment = TextAnchor.MiddleLeft;
            s.fixedHeight = 48;
            s.fontStyle = FontStyle.Bold;
            EditorGUILayout.LabelField("Mixed Reality Toolkit Optimize Window", s);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.LabelField("This tool automates the process of initializing your project, currently open scene, and material assets to recommended settings for Mixed Reality");
            EditorGUILayout.Space();

            // TODO: insert enum drop down or some kind of toggle select*

            PerfTarget = (PerformanceTarget)EditorGUILayout.EnumPopup("Performance Target", this.PerfTarget);

            if (!PlayerSettings.virtualRealitySupported)
            {
                EditorGUILayout.HelpBox("Current build target does not have virtual reality support enabled", MessageType.Warning);
            }

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            showProjectOptimizations = EditorGUILayout.Foldout(showProjectOptimizations, "Project Optimizations");
            if (showProjectOptimizations)
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    if (PlayerSettings.stereoRenderingPath != StereoRenderingPath.Instancing)
                    {
                        BuildSection("Single Pass Instanced Rendering", "www.google.com", () =>
                            {
                                EditorGUILayout.LabelField("Long explanation of how single pass rendering works here", EditorStyles.label);
                                singlePassInstanced = EditorGUILayout.ToggleLeft("Set Single Pass Instanced Rendering", singlePassInstanced);
                            });
                    }

                    BuildSection("Quality Settings", "www.google.com", () =>
                    {
                        EditorGUILayout.LabelField("Long explanation of how single pass rendering works here", EditorStyles.label);
                        setQualityLevel = EditorGUILayout.ToggleLeft("Set Quality Level to Lowest", setQualityLevel);
                    });

                    BuildSection("Depth Buffer Sharing", "www.google.com", () =>
                    {
                        EditorGUILayout.LabelField("Long explanation of how single pass rendering works here", EditorStyles.label);
                        enableDepthBufferSharing = EditorGUILayout.BeginToggleGroup("Enable Depth Buffer Sharing", enableDepthBufferSharing);
                        enable16BitDepthBuffer = EditorGUILayout.ToggleLeft("Set Depth Buffer to 16-bit", enable16BitDepthBuffer);
                        // TODO: Set camera to far plane of 100m? Or have this automatically in scene configure?
                        EditorGUILayout.EndToggleGroup();
                    });

                    // TODO: Change size of button? Put on right
                    if (GUILayout.Button("Optimize Project"))
                    {
                        OptimizeProject();
                    }
                }
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();

            GUILayout.BeginVertical("Box");
            showSceneOptimizations = EditorGUILayout.Foldout(showSceneOptimizations, "Scene Optimizations");
            if (showSceneOptimizations)
            {
                EditorGUILayout.LabelField("This section will provide controls and information for the currently opened & active scene.", EditorStyles.label);

                BuildSection("Lighting Settings", "www.google.com", () =>
                {
                    EditorGUILayout.LabelField("Long explanation of how single pass rendering works here", EditorStyles.label);

                    disableRealtimeGlobalIllumination = EditorGUILayout.ToggleLeft("Disable Realtime Global Illumination", disableRealtimeGlobalIllumination);
                    disableBakedGlobalIllumination = EditorGUILayout.ToggleLeft("Disable Baked Global Illumination", disableBakedGlobalIllumination);

                    if (GUILayout.Button("Optimize Lighting"))
                    {
                        OptimizeScene();
                    }
                });

                BuildSection("Live Scene Analysis", "www.google.com", () =>
                {
                    if (GUILayout.Button("Analyze Scene"))
                    {
                        AnalyzeScene();
                    }

                    // TODO: Finish this per platform?
                    /*
                    if (sceneLights != null && sceneLights.Length > 1)
                    {
                        EditorGUILayout.LabelField("Long explanation of how single pass rendering works here", EditorStyles.label);

                        sceneLights
                    }*/

                    // Number of lights
                    // Shadows/MSAA?
                    // Mesh polycount
                });
            }
            GUILayout.EndVertical();

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
                        //UndoMaterials();
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

        private void AnalyzeScene()
        {
            this.sceneLights = FindObjectsOfType<Light>();

            // TODO: Consider searching for particle renderers count?

            var filters = FindObjectsOfType<MeshFilter>();
            List<MeshNode> meshes = new List<MeshNode>();
            foreach(var f in filters)
            {
                int count = f.mesh.triangles.Length / 3;

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

            meshes.OrderByDescending(s => s.polycount);
            for(int i = 0; i < kSize; i++)
            {
                this.LargestMeshSizes[i] = 0;
                this.LargestMeshes[i] = null;
                if (i < meshes.Count)
                {
                    this.LargestMeshSizes[i] = meshes[i].polycount;
                    this.LargestMeshes[i] = meshes[i].filter.gameObject;
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