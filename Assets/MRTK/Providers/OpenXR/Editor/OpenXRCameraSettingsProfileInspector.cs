// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Editor;
using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.XRSDK.OpenXR.Editor
{
    [CustomEditor(typeof(OpenXRCameraSettingsProfile))]
    public class OpenXRCameraSettingsProfileInspector : BaseMixedRealityToolkitConfigurationProfileInspector
    {
        private const string ProfileTitle = "OpenXR Camera Settings";
        private const string ProfileDescription = "";

        private SerializedProperty reprojectionMethod;

        private static readonly GUIContent ReprojectionMethodTitle = new GUIContent("HoloLens 2 Reprojection Method");

        private const string DepthReprojectionDocURL = "https://docs.microsoft.com/windows/mixed-reality/hologram-stability#reprojection";

        protected override void OnEnable()
        {
            base.OnEnable();
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
                    EditorGUILayout.LabelField("Depth Reprojection Settings", EditorStyles.boldLabel);
                    InspectorUIUtility.RenderDocumentationButton(DepthReprojectionDocURL);
                }
                EditorGUILayout.PropertyField(reprojectionMethod, ReprojectionMethodTitle);

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
