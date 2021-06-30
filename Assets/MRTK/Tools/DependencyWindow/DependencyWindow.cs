// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

using Object = UnityEngine.Object;

namespace Microsoft.MixedReality.Toolkit.Utilities.Editor
{
    public class DependencyWindow : EditorWindow
    {
        private int selectedToolbarIndex = 0;
        private Vector2 scrollPosition = Vector2.zero;

        // Asset Dependency Graph
        private Object assetSelection = null;
        private int maxDisplayDepth = 4;
        private float dependencyGraphRefreshTime = 0.0f;

        // Unreferenced Asset List
        private bool excludeUnityScenes = false;

        private class DependencyGraphNode
        {
            public string guid = null;
            public string path = null;
            public string extension = null;
            public Object targetObject = null;
            public HashSet<DependencyGraphNode> assetsThisDependsOn = new HashSet<DependencyGraphNode>();
            public HashSet<DependencyGraphNode> assetsDependentOnThis = new HashSet<DependencyGraphNode>();
        }

        private Dictionary<string, DependencyGraphNode> dependencyGraph = new Dictionary<string, DependencyGraphNode>();

        private const string windowTitle = "Dependency Window";
        private static readonly string[] toolbarTitles = { "Asset Dependency Graph", "Unreferenced Asset List" };
        private const string guidPrefix = "guid: ";
        private const int guidCharacterCount = 32;
        private static readonly string nullGuid = new string('0', guidCharacterCount);

        private readonly string[] assetsWithDependencies =
        {
            ".unity",
            ".prefab",
            ".asset",
            ".mat",
            ".anim",
            ".controller",
            ".playable",
        };

        private readonly string[] assetsWithMetaDependencies =
        {
            ".fbx",
        };

        private readonly string[] assetsWhichCanBeUnreferenced =
        {
            ".cs",
            ".asmdef",
            ".targets",
            ".pfx",
            ".md",
            ".txt",
            ".nuspec",
            ".yml",
            ".json",
            ".pdf",
        };

        private const string DependencyWindow_URL = "https://docs.microsoft.com/windows/mixed-reality/mrtk-unity/features/tools/dependency-window";

        [MenuItem("Mixed Reality/Toolkit/Utilities/Dependency Window", false, 3)]
        private static void ShowWindow()
        {
            var window = GetWindow<DependencyWindow>(typeof(SceneView));
            window.titleContent = new GUIContent(windowTitle, EditorGUIUtility.IconContent("d_EditCollider").image);
            window.minSize = new Vector2(580.0f, 380.0f);
            window.RefreshDependencyGraph();
            window.Show();
        }

        private void OnGUI()
        {
            // Don't allow the dependency window to be used when the editor is playing.
            if (EditorApplication.isPlaying || EditorApplication.isPaused)
            {
                GUI.enabled = false;
            }

            DrawHeader();
            DrawVerifyAssetSerializationMode();
            DrawDependencyGraphStatistics();

            int previousSelectedToolbarIndex = selectedToolbarIndex;
            selectedToolbarIndex = GUILayout.Toolbar(selectedToolbarIndex, toolbarTitles);

            if (previousSelectedToolbarIndex != selectedToolbarIndex)
            {
                scrollPosition = Vector2.zero;
            }

            EditorGUILayout.Space();

            if (selectedToolbarIndex == 0)
            {
                assetSelection = EditorGUILayout.ObjectField("Asset Selection", assetSelection, typeof(Object), false);
                maxDisplayDepth = EditorGUILayout.IntSlider("Max Display Depth", maxDisplayDepth, 1, 32);

                DrawDependencyGraphForSelection();
            }
            else if (selectedToolbarIndex == 1)
            {
                excludeUnityScenes = EditorGUILayout.Toggle("Exclude Unity Scenes", excludeUnityScenes);

                string tooltip = "Although certain asset types may not be directly referenced by other assets as tracked via Unity meta files, these assets may be utilized and/or necessary to a project in other ways.\n\nThus, this list of asset extensions are ignored and always excluded in the list below.\n\n";
                foreach (string extension in assetsWhichCanBeUnreferenced)
                {
                    tooltip += extension + "\n";
                }

                EditorGUILayout.LabelField(new GUIContent("Some asset types are always excluded", InspectorUIUtility.HelpIcon, tooltip));

                DrawUnreferencedAssetList();
            }
        }

        private void OnSelectionChange()
        {
            if (Selection.objects.Length != 0)
            {
                if (IsAsset(AssetDatabase.GetAssetPath(Selection.objects[0])))
                {
                    assetSelection = Selection.objects[0];
                    Repaint();
                }
            }
        }

        private static void DrawHeader()
        {
            MixedRealityInspectorUtility.RenderMixedRealityToolkitLogo();

            // Render Title
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("Mixed Reality Toolkit Dependency Window", EditorStyles.boldLabel);
                InspectorUIUtility.RenderDocumentationButton(DependencyWindow_URL);
            }

            EditorGUILayout.LabelField("This tool displays how assets reference and depend on each other. Dependencies are calculated by parsing guids within project YAML files, code dependencies are not considered.", EditorStyles.wordWrappedLabel);

            EditorGUILayout.Space();
        }

        private static void DrawVerifyAssetSerializationMode()
        {
            if (EditorSettings.serializationMode != SerializationMode.ForceText)
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.HelpBox("Dependencies can only be tracked with text assets. Please change the project serialization mode to \"Force Text\"", MessageType.Error);

                    if (GUILayout.Button("Fix Now"))
                    {
                        EditorSettings.serializationMode = SerializationMode.ForceText;
                    }
                }
            }
        }

        private void DrawDependencyGraphStatistics()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                if (dependencyGraph.Count == 0)
                {
                    GUILayout.Label(string.Format("The dependency graph contains 0 assets, please refresh the dependency graph.", dependencyGraph.Count), EditorStyles.boldLabel);
                }
                else
                {
                    GUILayout.Label(string.Format("The dependency graph contains {0:n0} assets and took {1:0.00} seconds to build.", dependencyGraph.Count, dependencyGraphRefreshTime));
                }

                if (GUILayout.Button("Refresh"))
                {
                    RefreshDependencyGraph();
                }
            }
            EditorGUILayout.Space();

            if (GUI.enabled)
            {
                GUI.enabled = dependencyGraph.Count != 0;
            }
        }

        private void DrawDependencyGraphForSelection()
        {
            EditorGUILayout.Space();

            EditorGUILayout.BeginVertical("Box");
            {
                EditorGUILayout.LabelField("Dependency Graph", EditorStyles.boldLabel);

                EditorGUILayout.LabelField(string.Empty, GUI.skin.horizontalSlider);

                var file = AssetDatabase.GetAssetPath(assetSelection);
                DependencyGraphNode node;

                if (dependencyGraph.TryGetValue(AssetDatabase.AssetPathToGUID(file), out node))
                {
                    scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
                    {
                        EditorGUILayout.LabelField("This asset depends on:", EditorStyles.boldLabel);

                        if (node.assetsThisDependsOn.Count != 0)
                        {
                            foreach (var dependency in node.assetsThisDependsOn)
                            {
                                DrawDependencyGraphNode(dependency);
                            }
                        }
                        else
                        {
                            EditorGUILayout.HelpBox("Nothing.", MessageType.Info);
                        }

                        EditorGUILayout.LabelField(string.Empty, GUI.skin.horizontalSlider);

                        EditorGUILayout.LabelField("Assets that depend on this:", EditorStyles.boldLabel);

                        if (node.assetsDependentOnThis.Count != 0)
                        {
                            foreach (var dependency in node.assetsDependentOnThis)
                            {
                                DrawDependencyGraphNodeRecurse(dependency, 0, maxDisplayDepth);
                            }
                        }
                        else
                        {
                            EditorGUILayout.HelpBox("Nothing, you could consider deleting this asset if it isn't referenced programmatically.", MessageType.Warning);
                        }
                    }
                    EditorGUILayout.EndScrollView();
                }
                else
                {
                    if (IsAsset(file))
                    {
                        EditorGUILayout.HelpBox(string.Format("Failed to find data for {0} try refreshing the dependency graph.", file), MessageType.Warning);
                    }
                    else
                    {
                        EditorGUILayout.HelpBox("Please select an asset above to see the dependency graph.", MessageType.Info);
                    }
                }
            }
            EditorGUILayout.EndVertical();
        }

        private void DrawUnreferencedAssetList()
        {
            EditorGUILayout.Space();

            EditorGUILayout.BeginVertical("Box");
            {
                EditorGUILayout.LabelField("Unreferenced Asset List", EditorStyles.boldLabel);

                var unreferencedAssetCount = 0;

                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
                {
                    foreach (var kvp in dependencyGraph)
                    {
                        var node = kvp.Value;

                        if (node.assetsDependentOnThis.Count != 0)
                        {
                            continue;
                        }

                        if (DoesNodeHaveExtension(node, assetsWhichCanBeUnreferenced))
                        {
                            continue;
                        }

                        if (excludeUnityScenes && DoesNodeHaveExtension(node, ".unity"))
                        {
                            continue;
                        }

                        DrawDependencyGraphNode(kvp.Value);

                        ++unreferencedAssetCount;
                    }
                }
                EditorGUILayout.EndScrollView();

                EditorGUILayout.LabelField(string.Format("{0:n0} assets are not referenced by any other asset.", unreferencedAssetCount));
            }
            EditorGUILayout.EndVertical();
        }

        private void RefreshDependencyGraph()
        {
            var beginTime = DateTime.UtcNow;

            dependencyGraph.Clear();

            // Get all meta files from the assets and package cache directories.
            const string metaExtension = ".meta";
            var metaFiles = new List<string>();
            metaFiles.AddRange(Directory.GetFiles(Application.dataPath, "*" + metaExtension, SearchOption.AllDirectories));
            metaFiles.AddRange(Directory.GetFiles(Path.Combine(Directory.GetParent(Application.dataPath).FullName, "Library\\PackageCache"), "*" + metaExtension, SearchOption.AllDirectories));
            metaFiles.AddRange(Directory.GetFiles(Path.Combine(Directory.GetParent(Application.dataPath).FullName, "Packages"), "*" + metaExtension, SearchOption.AllDirectories));

            for (int i = 0; i < metaFiles.Count; ++i)
            {
                var progress = (float)i / metaFiles.Count;

                if (EditorUtility.DisplayCancelableProgressBar(windowTitle, "Building dependency graph...", progress))
                {
                    break;
                }

                var metaFile = metaFiles[i];
                var file = metaFile.Substring(0, metaFile.Length - metaExtension.Length);

                if (!IsAsset(file))
                {
                    continue;
                }

                string guid = GetGuidFromMeta(metaFile);
                if (!IsGuidValid(guid))
                {
                    continue;
                }

                var node = EnsureNode(guid);

                List<string> dependencies = new List<string>();

                // Check if this asset can have dependencies.
                if (DoesNodeHaveExtension(node, assetsWithDependencies))
                {
                    dependencies.AddRange(GetDependenciesInFile(file));
                }

                // Some files have dependencies in their meta files.
                if (DoesNodeHaveExtension(node, assetsWithMetaDependencies))
                {
                    dependencies.AddRange(GetDependenciesInFile(metaFile));
                }

                // Add linkage between this node and its dependencies and the dependency and this node.
                foreach (var dependency in dependencies)
                {
                    node.assetsThisDependsOn.Add(EnsureNode(dependency));
                    EnsureNode(dependency).assetsDependentOnThis.Add(node);
                }
            }

            EditorUtility.ClearProgressBar();

            dependencyGraphRefreshTime = (float)((DateTime.UtcNow - beginTime).TotalSeconds);
        }

        private DependencyGraphNode EnsureNode(string guid)
        {
            DependencyGraphNode node;

            if (!dependencyGraph.TryGetValue(guid, out node))
            {
                node = new DependencyGraphNode();
                node.guid = guid;
                node.path = AssetDatabase.GUIDToAssetPath(guid);
                node.extension = Path.GetExtension(node.path).ToLowerInvariant();
                node.targetObject = AssetDatabase.LoadMainAssetAtPath(node.path);
                dependencyGraph.Add(guid, node);
            }

            return node;
        }

        private static bool IsAsset(string file)
        {
            return File.Exists(file);
        }

        private static bool DoesNodeHaveExtension(DependencyGraphNode node, string targetExtension)
        {
            if (!string.IsNullOrEmpty(node.extension))
            {
                return targetExtension.Equals(node.extension);
            }

            return false;
        }

        private static bool DoesNodeHaveExtension(DependencyGraphNode node, string[] targetExtensions)
        {
            if (!string.IsNullOrEmpty(node.extension))
            {
                return Array.IndexOf(targetExtensions, node.extension) >= 0;
            }

            return false;
        }

        private static bool IsGuidValid(string guid)
        {
            return !string.IsNullOrEmpty(guid) && guid != nullGuid
            && !string.IsNullOrEmpty(Path.GetExtension(AssetDatabase.GUIDToAssetPath(guid)));
        }

        private static string GetGuidFromMeta(string file)
        {
            try
            {
                string[] lines = File.ReadAllLines(file);

                foreach (var line in lines)
                {
                    if (line.StartsWith(guidPrefix))
                    {
                        return line.Split(' ')[1];
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }

            return null;
        }

        private static List<string> GetDependenciesInFile(string file)
        {
            var guids = new List<string>();

            try
            {
                foreach (var line in File.ReadAllLines(file))
                {
                    var index = line.IndexOf(guidPrefix);

                    if (index > 0)
                    {
                        var guid = line.Substring(index + guidPrefix.Length, guidCharacterCount);

                        if (IsGuidValid(guid) && !guids.Contains(guid))
                        {
                            guids.Add(guid);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }

            return guids;
        }

        private static void DrawDependencyGraphNode(DependencyGraphNode node)
        {
            DrawDependencyGraphNode(node, 0, int.MaxValue);
        }

        private static void DrawDependencyGraphNode(DependencyGraphNode node, int depth, int maxDepth)
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Space(depth * 8);

                if (depth != maxDepth)
                {
                    if (string.IsNullOrEmpty(node.path))
                    {
                        EditorGUILayout.LabelField(string.Format("Missing Asset: {0}", node.guid));
                    }
                    else
                    {
                        EditorGUILayout.ObjectField(string.Empty, node.targetObject, typeof(Object), false);
                    }
                }
                else
                {
                    EditorGUILayout.LabelField("Max display depth was exceeded...");
                }
            }
        }

        private static void DrawDependencyGraphNodeRecurse(DependencyGraphNode node, int depth, int maxDepth)
        {
            // To avoid infinite recursion with circular dependencies, stop at a max depth.
            if (depth != maxDepth)
            {
                DrawDependencyGraphNode(node, depth, maxDepth);

                foreach (var dependency in node.assetsDependentOnThis)
                {
                    DrawDependencyGraphNodeRecurse(dependency, depth + 1, maxDepth);

                    // Only draw the first child at max depth.
                    if ((depth + 1) == maxDepth)
                    {
                        break;
                    }
                }
            }
            else
            {
                DrawDependencyGraphNode(null, depth, maxDepth);
            }
        }
    }
}
