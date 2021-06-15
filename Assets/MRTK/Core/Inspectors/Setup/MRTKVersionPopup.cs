// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using System;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Editor
{
    internal class MRTKVersionPopup : EditorWindow
    {
        private static MRTKVersionPopup window;
        private static readonly Version MRTKVersion = typeof(MixedRealityToolkit).Assembly.GetName().Version;
        private static readonly Vector2 WindowSize = new Vector2(300, 150);
        private static readonly GUIContent Title = new GUIContent("Mixed Reality Toolkit");

        [MenuItem("Mixed Reality/Toolkit/Show version...", priority = int.MaxValue)]
        private static void Init()
        {
            if (window != null)
            {
                window.ShowUtility();
                return;
            }

            window = CreateInstance<MRTKVersionPopup>();
            window.titleContent = Title;
            window.maxSize = WindowSize;
            window.minSize = WindowSize;
            window.ShowUtility();
        }

        private void OnGUI()
        {
            using (new EditorGUILayout.VerticalScope())
            {
                MixedRealityInspectorUtility.RenderMixedRealityToolkitLogo();

                using (new EditorGUILayout.HorizontalScope())
                {
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.LabelField($"Version {MRTKVersion}", EditorStyles.wordWrappedLabel);
                    GUILayout.FlexibleSpace();
                }
            }
        }
    }
}
