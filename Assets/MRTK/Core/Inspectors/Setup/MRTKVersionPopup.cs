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
        private const string DefaultVersion = "0.0.0.0";
        private const string NotFoundMessage = "The version could not be read. This is most often due to (and expected when) using MRTK directly from the repo. If you're using an official distribution and seeing this message, please file a GitHub issue!";
        private static readonly Version MRTKVersion = typeof(MixedRealityToolkit).Assembly.GetName().Version;
        private static readonly bool FoundVersion = MRTKVersion.ToString() != DefaultVersion;
        private static readonly Vector2 WindowSize = new Vector2(300, 140);
        private static readonly Vector2 NotFoundWindowSize = new Vector2(300, 175);
        private static readonly GUIContent Title = new GUIContent("Mixed Reality Toolkit");
        private static MRTKVersionPopup window;

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
            window.maxSize = FoundVersion ? WindowSize : NotFoundWindowSize;
            window.minSize = FoundVersion ? WindowSize : NotFoundWindowSize;
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
                    EditorGUILayout.LabelField(FoundVersion ? $"Version {MRTKVersion}" : NotFoundMessage, EditorStyles.wordWrappedLabel);
                    GUILayout.FlexibleSpace();
                }
            }
        }
    }
}
