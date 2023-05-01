// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using PackageInfo = UnityEditor.PackageManager.PackageInfo;

namespace Microsoft.MixedReality.Toolkit.Editor
{
    /// <summary>
    /// Dialog that displays information about MRTK.
    /// </summary>
    internal class AboutMRTK : EditorWindow
    {
        private static readonly GUIContent WindowTitle = new GUIContent("About MRTK");
        private static readonly Vector2 WindowSizeWithoutLogo = new Vector2(600, 250);
        private static readonly Vector2 WindowSizeWithLogo = new Vector2(600, 320);

        private static AboutMRTK window = null;

        private static ListRequest packageListRequest = null;
        private static List<PackageInfo> installedPackages = new List<PackageInfo>();

        [MenuItem("Mixed Reality/MRTK3/About MRTK...", priority = int.MaxValue)]
        private static void Init()
        {
            installedPackages.Clear();

            packageListRequest = Client.List();
            EditorApplication.update += EditorUpdate;

            if (window != null)
            {
                window.ShowUtility();
                return;
            }

            window = GetWindow<AboutMRTK>();
            window.titleContent = WindowTitle;

            if (InspectorUIUtility.IsMixedRealityToolkitLogoAssetPresent())
            {
                window.minSize = WindowSizeWithLogo;
                window.maxSize = WindowSizeWithLogo;
            }
            else
            {
                window.minSize = WindowSizeWithoutLogo;
                window.maxSize = WindowSizeWithoutLogo;
            }
        }

        private static void EditorUpdate()
        {
            if (!packageListRequest.IsCompleted) { return; }

            if (packageListRequest.Status == StatusCode.Success)
            {
                installedPackages.AddRange(packageListRequest.Result);
            }

            EditorApplication.update -= EditorUpdate;
        }

        private Vector2 scrollPos;

        private void OnGUI()
        {
            if (window == null)
            {
                Init();
            }

            using (new EditorGUILayout.VerticalScope())
            {
                EditorGUILayout.Space(2);
                if (!InspectorUIUtility.RenderMixedRealityToolkitLogo())
                {
                    // Only add additional space if the text fallback is used in RenderMixedRealityToolkitLogo().
                    EditorGUILayout.Space(3);
                }
                EditorGUILayout.LabelField("Copyright (c) Microsoft Corporation. Licensed under the MIT License.", MRTKEditorStyles.LicenseStyle);
                EditorGUILayout.Space(12);

                if (packageListRequest != null && packageListRequest.IsCompleted == false)
                {
                    EditorGUILayout.Space(30);
                    EditorGUILayout.LabelField("Loading package information...", MRTKEditorStyles.LicenseStyle);
                }
                else
                {
                    StringBuilder sb = new StringBuilder();

                    foreach (PackageInfo packageInfo in installedPackages)
                    {
                        if (packageInfo.name.StartsWith("com.microsoft.mrtk") ||
                            packageInfo.name.StartsWith("com.microsoft.mixedreality"))
                        {
                            sb.AppendLine($"{packageInfo.name}: {packageInfo.version}");
                        }
                    }

                    using (new EditorGUILayout.HorizontalScope())
                    {
                        EditorGUILayout.LabelField("Package versions", EditorStyles.boldLabel);
                        using (new EditorGUI.DisabledGroupScope(sb.Length == 0))
                        {
                            if (GUILayout.Button(new GUIContent("Copy")))
                            {
                                GUIUtility.systemCopyBuffer = sb.ToString();
                            }
                        }
                    }
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        scrollPos = EditorGUILayout.BeginScrollView(
                            scrollPos,
                            GUILayout.Height(150));
                        using (new EditorGUI.DisabledGroupScope(true))
                        {
                            EditorGUILayout.TextArea(
                                sb.ToString(),
                                GUILayout.ExpandHeight(true));
                        }
                        EditorGUILayout.EndScrollView();
                    }
                }
            }
        }
    }
}
