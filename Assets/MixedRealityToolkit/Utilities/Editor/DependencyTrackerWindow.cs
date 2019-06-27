// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

using Object = UnityEngine.Object;

namespace Microsoft.MixedReality.Toolkit.Utilities.Editor
{
    public class DependencyTrackerWindow : EditorWindow
    {
        private readonly string[] assetsWithDependencies =
        {
            ".unity",
            ".prefab",
            ".asset",
            ".mat",
            ".anim",
            ".controller"
        };

        private const string windowTitle = "Dependency Tracker";
        private const string guidPrefix = "guid: ";
        private const string nullGuid = "00000000000000000000000000000000";
        private const int guidCharacterCount = 32;
        private const int maxDepth = 8;

        private Object assetSelection = null;
        private Vector2 scrollPosition = Vector2.zero;
        private float assetGraphRefreshTime = 0.0f;

        private class AssetGraphNode
        {
            public string guid = null;
            public HashSet<AssetGraphNode> assetsThisDependsOn = new HashSet<AssetGraphNode>();
            public HashSet<AssetGraphNode> assetsDependentOnThis = new HashSet<AssetGraphNode>();
        }

        private Dictionary<string, AssetGraphNode> dependencyGraph = new Dictionary<string, AssetGraphNode>();

        [MenuItem("Mixed Reality Toolkit/Utilities/Dependency Tracker", false, 3)]
        private static void ShowWindow()
        {
            var window = GetWindow<DependencyTrackerWindow>();
            window.titleContent = new GUIContent(windowTitle);
            window.minSize = new Vector2(380.0f, 700.0f);
            window.RefreshAssetGraph();
            window.Show();
        }

        private void OnGUI()
        {
            GUILayout.Label("Asset Selection", EditorStyles.boldLabel);

            assetSelection = EditorGUILayout.ObjectField("Asset", assetSelection, typeof(Object), false);

            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.Label(string.Format("The dependency graph contains {0} assets and took {1:0.00} seconds to build.", dependencyGraph.Count, assetGraphRefreshTime));

                if (GUILayout.Button("Refresh"))
                {
                    RefreshAssetGraph();
                }
            }
            EditorGUILayout.EndHorizontal();

            GUILayout.Label("Dependency Graph", EditorStyles.boldLabel);

            var file = AssetDatabase.GetAssetPath(assetSelection);
            AssetGraphNode node;

            if (dependencyGraph.TryGetValue(AssetDatabase.AssetPathToGUID(file), out node))
            {
                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
                {
                    GUILayout.Label("This asset depends on:", EditorStyles.boldLabel);

                    if (node.assetsThisDependsOn.Count != 0)
                    {
                        foreach (var guid in node.assetsThisDependsOn)
                        {
                            DrawDependency(guid, 0);
                        }
                    }
                    else
                    {
                        GUILayout.Label("Nothing.");
                    }

                    EditorGUILayout.Separator();

                    GUILayout.Label("Assets that depend on this:", EditorStyles.boldLabel);

                    if (node.assetsDependentOnThis.Count != 0)
                    {
                        foreach (var guid in node.assetsDependentOnThis)
                        {
                            DrawDependencyRecurse(guid, 0);
                            GUILayout.Space(8);
                        }
                    }
                    else
                    {
                        GUILayout.Label("Nothing, you could consider deleting it if it isn't loaded programmatically.");
                    }
                }
                EditorGUILayout.EndScrollView();
            }
            else
            {
                if (IsAsset(file))
                {
                    GUILayout.Box(string.Format("Failed to find data for {0} try refreshing the dependency graph.", file), EditorStyles.helpBox, new GUILayoutOption[0]);
                }
                else
                {
                    GUILayout.Box("Please select an asset above to see the dependency graph.", EditorStyles.helpBox, new GUILayoutOption[0]);
                }
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

        private void RefreshAssetGraph()
        {
            // TODO, check if text serialization is on.

            var beginTime = DateTime.UtcNow;

            dependencyGraph.Clear();

            string metaExtension = ".meta";
            string[] metaFiles = Directory.GetFiles(Application.dataPath, "*" + metaExtension, SearchOption.AllDirectories);

            for (int i = 0; i < metaFiles.Length; ++i)
            {
                EditorUtility.DisplayProgressBar(windowTitle, "Building dependency graph...", (float)i / metaFiles.Length);

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

                AssetGraphNode node = EnsureNode(guid);

                // Check if this asset can have dependencies.
                var extension = Path.GetExtension(file).ToLowerInvariant();

                if (Array.IndexOf(assetsWithDependencies, extension) >= 0)
                {
                    var dependencies = GetDependenciesInFile(file);

                    // Add linkage between this node and it's dependencies and the dependency and this node.
                    foreach (var dependency in dependencies)
                    {
                        node.assetsThisDependsOn.Add(EnsureNode(dependency));
                        EnsureNode(dependency).assetsDependentOnThis.Add(node);
                    }
                }
            }

            EditorUtility.ClearProgressBar();

            assetGraphRefreshTime = (float)((DateTime.UtcNow - beginTime).TotalSeconds);
        }

        private AssetGraphNode EnsureNode(string guid)
        {
            AssetGraphNode node;

            if (!dependencyGraph.TryGetValue(guid, out node))
            {
                node = new AssetGraphNode();
                node.guid = guid;
                dependencyGraph.Add(guid, node);
            }

            return node;
        }

        private static bool IsAsset(string file)
        {
            return File.Exists(file);
        }

        private static string GetGuidFromMeta(string file)
        {
            string[] lines = File.ReadAllLines(file);

            foreach (var line in lines)
            {
                if (line.StartsWith(guidPrefix))
                {
                    return line.Split(' ')[1];
                }
            }

            return null;
        }

        private static List<string> GetDependenciesInFile(string file)
        {
            var guids = new List<string>();

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

            return guids;
        }

        private static bool IsGuidValid(string guid)
        {
            return !string.IsNullOrEmpty(guid) && guid != nullGuid;
        }

        private static void DrawDependency(AssetGraphNode node, int depth)
        {
            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.Space(depth * 16);

                var path = AssetDatabase.GUIDToAssetPath(node.guid);

                if (GUILayout.Button("Select", GUILayout.ExpandWidth(false)))
                {
                    Selection.objects = new Object[]  { AssetDatabase.LoadMainAssetAtPath(path) };
                }

                GUILayout.Label(path, GUILayout.ExpandWidth(false));
            }
            EditorGUILayout.EndHorizontal();
        }

        private static int DrawDependencyRecurse(AssetGraphNode node, int depth)
        {
            // To avoid infinite recursion of circular dependencies stop at a max depth.
            if (depth == maxDepth)
            {
                // TODO, notify max depth hit.
                return depth;
            }

            DrawDependency(node, depth);

            foreach (var dependency in node.assetsDependentOnThis)
            {
                DrawDependencyRecurse(dependency, depth + 1);
            }

            return depth;
        }
    }
}
