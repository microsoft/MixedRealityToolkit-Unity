// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
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

        private const string guidPrefix = "guid: ";
        private const int guidCharacterCount = 32;
        private const int maxDepth = 8;

        private Object assetSelection = null;
        private Vector2 scrollPosition = Vector2.zero;
        private float assetGraphRefreshTime = 0.0f;

        private class AssetGraphNode
        {
            public string guid = null;
            public HashSet<string> assetsThisDependsOn = new HashSet<string>();
            public HashSet<string> assetsDependentOnThis = new HashSet<string>();
        }

        private Dictionary<string, AssetGraphNode> assetGraph = new Dictionary<string, AssetGraphNode>();

        [MenuItem("Mixed Reality Toolkit/Utilities/Dependency Tracker", false, 3)]
        private static void ShowWindow()
        {
            var window = GetWindow<DependencyTrackerWindow>();
            window.titleContent = new GUIContent("Dependency Tracker");
            window.minSize = new Vector2(380.0f, 700.0f);
            window.RefreshAssetGraph();
            window.Show();
        }

        private void OnGUI()
        {
            GUILayout.Label("Asset Selection", EditorStyles.boldLabel);

            assetSelection = EditorGUILayout.ObjectField("Asset", assetSelection, typeof(Object), false);

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            {
                GUILayout.Label("Asset Graph", EditorStyles.boldLabel);

                string assetSelectionGuid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(assetSelection));
                AssetGraphNode node;

                if (assetGraph.TryGetValue(assetSelectionGuid, out node))
                {
                    GUILayout.Label("This asset depends on:", EditorStyles.boldLabel);

                    foreach (var guid in node.assetsThisDependsOn)
                    {
                        DrawDependency(guid, 0);
                    }

                    EditorGUILayout.Separator();

                    GUILayout.Label("Assets that depend on this:", EditorStyles.boldLabel);

                    foreach (var guid in node.assetsDependentOnThis)
                    {
                        DrawDependencyRecurse(guid, 0);
                        GUILayout.Space(8);
                    }
                }
                else
                {
                    //TODO, prompt to refresh.
                }
            }

            EditorGUILayout.EndScrollView();
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
            // TODO, progress bar.

            var beginTime = DateTime.UtcNow;

            assetGraph.Clear();

            string metaExtension = ".meta";
            string[] metaFiles = Directory.GetFiles(Application.dataPath, "*" + metaExtension, SearchOption.AllDirectories);

            foreach (var metaFile in metaFiles)
            {
                var file = metaFile.Substring(0, metaFile.Length - metaExtension.Length);

                if (!IsAsset(file))
                {
                    continue;
                }

                string guid = GetGuidFromMeta(metaFile); // TODO AssetDatabase.AssetPathToGUID?

                // Ignore files without a guid.
                if (string.IsNullOrEmpty(guid))
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
                        node.assetsThisDependsOn.Add(dependency);
                        EnsureNode(dependency).assetsDependentOnThis.Add(guid);
                    }
                }
            }

            assetGraphRefreshTime = (float)((DateTime.Now - beginTime).TotalSeconds);
        }

        private bool IsAsset(string file)
        {
            return File.Exists(file);
        }

        private AssetGraphNode EnsureNode(string guid)
        {
            AssetGraphNode node;

            if (!assetGraph.TryGetValue(guid, out node))
            {
                node = new AssetGraphNode();
                node.guid = guid;
                assetGraph.Add(guid, node);
            }

            return node;
        }

        private string GetGuidFromMeta(string file)
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

        private List<string> GetDependenciesInFile(string file)
        {
            var guids = new List<string>();

            foreach (var line in File.ReadAllLines(file))
            {
                var index = line.IndexOf(guidPrefix);

                if (index > 0)
                {
                    var guid = line.Substring(index + guidPrefix.Length, guidCharacterCount);

                    if (!guids.Contains(guid))
                    {
                        guids.Add(guid);
                    }
                }
            }

            return guids;
        }

        private void DrawDependency(string guid, int depth)
        {
            EditorGUILayout.BeginHorizontal();

            {
                GUILayout.Space(depth * 16);

                var path = AssetDatabase.GUIDToAssetPath(guid);

                if (GUILayout.Button("Select", GUILayout.ExpandWidth(false)))
                {
                    Selection.objects = new Object[] 
                    {
                        AssetDatabase.LoadMainAssetAtPath(path)
                    };
                }

                GUILayout.Label(path, GUILayout.ExpandWidth(false));
            }

            EditorGUILayout.EndHorizontal();
        }

        private int DrawDependencyRecurse(string guid, int depth)
        {
            // To avoid infinite recursion of circular dependencies stop at a max depth.
            if (depth == maxDepth)
            {
                // TODO, notify max depth hit.
                return depth;
            }

            DrawDependency(guid, depth);

            AssetGraphNode node;

            if (assetGraph.TryGetValue(guid, out node))
            {
                foreach (var dependency in node.assetsDependentOnThis)
                {
                    DrawDependencyRecurse(dependency, depth + 1); 
                }
            }

            return depth;
        }
    }
}
