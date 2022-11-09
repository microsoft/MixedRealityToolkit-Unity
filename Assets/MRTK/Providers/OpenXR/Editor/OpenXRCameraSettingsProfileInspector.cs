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
        private const string DepthReprojectionDocURL = "https://docs.microsoft.com/windows/mixed-reality/hologram-stability#reprojection";

        private static readonly GUIContent ReprojectionMethodTitle = new GUIContent("HoloLens 2 Reprojection Method");

#if MSFT_OPENXR
        private static GUIContent mrcSettingsButtonContent = null;
#endif

        private SerializedProperty reprojectionMethod;

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

                EditorGUILayout.HelpBox("Render from PV camera is now enabled by default when running with the Mixed Reality OpenXR Plugin. " +
                    "It can be turned off from the \"Mixed Reality Features\" settings in the OpenXR plug-in settings. " +
                    "Look for \"Disable First Person Observer\".", MessageType.Info);

#if MSFT_OPENXR
                mrcSettingsButtonContent ??= new GUIContent()
                {
                    image = EditorGUIUtility.IconContent("Settings").image,
                    text = " OpenXR plug-in settings",
                };

                using (new EditorGUILayout.HorizontalScope())
                {
                    GUILayout.FlexibleSpace();
                    // The settings button should always be enabled.
                    using (new GUIEnabledWrapper())
                    {
                        if (GUILayout.Button(mrcSettingsButtonContent, EditorStyles.miniButton, GUILayout.MaxWidth(250f)))
                        {
                            SettingsService.OpenProjectSettings("Project/XR Plug-in Management/OpenXR");
                        }
                    }
                    GUILayout.FlexibleSpace();
                }
#endif

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
