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

        private readonly GUIContent pvCameraRenderingTitle = new GUIContent("Render from PV Camera (Align holograms)");
        private readonly GUIContent reprojectionMethodTitle = new GUIContent("HoloLens 2 Reprojection Method");

        private const string MRCDocURL = "https://docs.microsoft.com/windows/mixed-reality/mixed-reality-capture-for-developers#render-from-the-pv-camera-opt-in";
        private const string DepthReprojectionDocURL = "https://docs.microsoft.com/windows/mixed-reality/hologram-stability#reprojection";

        protected override void OnEnable()
        {
            base.OnEnable();

            renderFromPVCameraForMixedRealityCapture = serializedObject.FindProperty("renderFromPVCameraForMixedRealityCapture");
            reprojectionMethod = serializedObject.FindProperty("reprojectionMethod");
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
                    EditorGUILayout.LabelField("Mixed Reality Capture Settings (Experimental)", EditorStyles.boldLabel);
                    InspectorUIUtility.RenderDocumentationButton(MRCDocURL);
                }
                EditorGUILayout.HelpBox("Render from PV camera is supported in Unity 2018.4.13 and newer if using Unity 2018, and in Unity 2019.4.9f1 and newer if using Unity 2019. Enabling the feature on other versions may result in incorrect capture behavior.", MessageType.Info);
                EditorGUILayout.HelpBox("This doesn't work on XR SDK when we shipped this MRTK release. See this page for the latest information: https://github.com/microsoft/MixedRealityToolkit-Unity/issues/8707", MessageType.Info);
                EditorGUILayout.PropertyField(renderFromPVCameraForMixedRealityCapture, pvCameraRenderingTitle);

                EditorGUILayout.Space();
                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.LabelField("Depth Reprojection Settings", EditorStyles.boldLabel);
                    InspectorUIUtility.RenderDocumentationButton(DepthReprojectionDocURL);
                }
                EditorGUILayout.PropertyField(reprojectionMethod, reprojectionMethodTitle);

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
