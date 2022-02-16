// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Editor;
using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.WindowsMixedReality.Editor
{
    [CustomEditor(typeof(WindowsMixedRealityCameraSettingsProfile))]
    public class WindowsMixedRealityCameraSettingsProfileInspector : BaseMixedRealityToolkitConfigurationProfileInspector
    {
        private const string ProfileTitle = "Windows Mixed Reality Camera Settings";
        private const string ProfileDescription = "";

        private SerializedProperty renderFromPVCameraForMixedRealityCapture;
        private SerializedProperty reprojectionMethod;
        private SerializedProperty readingModeEnabled;

        private static readonly GUIContent PVCameraRenderingTitle = new GUIContent("Render from PV Camera (Align holograms)");
        private static readonly GUIContent ReprojectionMethodTitle = new GUIContent("HoloLens 2 Reprojection Method");

        private const string MRCDocURL = "https://docs.microsoft.com/windows/mixed-reality/mixed-reality-capture-for-developers#render-from-the-pv-camera-opt-in";
        private const string DepthReprojectionDocURL = "https://docs.microsoft.com/windows/mixed-reality/hologram-stability#reprojection";
        private const string ReadingModeDocURL = "https://docs.microsoft.com/en-us/hololens/hololens2-display#what-improvements-are-coming-that-will-improve-hololens-2-image-quality";

        protected override void OnEnable()
        {
            base.OnEnable();

            renderFromPVCameraForMixedRealityCapture = serializedObject.FindProperty("renderFromPVCameraForMixedRealityCapture");
            reprojectionMethod = serializedObject.FindProperty("reprojectionMethod");
            readingModeEnabled = serializedObject.FindProperty("readingModeEnabled");
        }

        public override void OnInspectorGUI()
        {
            RenderProfileHeader(ProfileTitle, ProfileDescription, target);

            using (new EditorGUI.DisabledGroupScope(IsProfileLock((BaseMixedRealityProfile)target)))
            {
                serializedObject.Update();

                EditorGUILayout.Space();
                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.LabelField("Mixed Reality Capture Settings", EditorStyles.boldLabel);
                    InspectorUIUtility.RenderDocumentationButton(MRCDocURL);
                }

                EditorGUILayout.HelpBox("On legacy XR, render from PV camera is supported in Unity 2018.4.35f1 and newer if using Unity 2018 and Unity 2019.4.26f1 and newer if using Unity 2019.", MessageType.Info);
                EditorGUILayout.HelpBox("On Windows XR Plugin, render from PV camera is supported in versions 2.8.0, 4.5.0, and 5.3.0 (and newer in each respective major version).", MessageType.Info);
                EditorGUILayout.PropertyField(renderFromPVCameraForMixedRealityCapture, PVCameraRenderingTitle);

                EditorGUILayout.Space();
                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.LabelField("Depth Reprojection Settings", EditorStyles.boldLabel);
                    InspectorUIUtility.RenderDocumentationButton(DepthReprojectionDocURL);
                }
                EditorGUILayout.PropertyField(reprojectionMethod, ReprojectionMethodTitle);

                EditorGUILayout.Space();
                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.LabelField("Reading Mode Settings", EditorStyles.boldLabel);
                    InspectorUIUtility.RenderDocumentationButton(ReadingModeDocURL);
                }
                EditorGUILayout.PropertyField(readingModeEnabled);

                serializedObject.ApplyModifiedProperties();
            }
        }

        protected override bool IsProfileInActiveInstance()
        {
            var profile = target as BaseMixedRealityProfile;

            return MixedRealityToolkit.IsInitialized && profile != null &&
                   MixedRealityToolkit.Instance.HasActiveProfile &&
                   MixedRealityToolkit.Instance.ActiveProfile.CameraProfile != null &&
                   MixedRealityToolkit.Instance.ActiveProfile.CameraProfile.SettingsConfigurations != null &&
                   MixedRealityToolkit.Instance.ActiveProfile.CameraProfile.SettingsConfigurations.Any(s => s.SettingsProfile == profile);
        }
    }
}
