// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using Microsoft.MixedReality.Toolkit.Editor;
using Microsoft.MixedReality.Toolkit.LeapMotion.Input;
using Microsoft.MixedReality.Toolkit.LeapMotion.Utilities;
using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using System.Linq;
using UnityEditor;

namespace Microsoft.MixedReality.Toolkit.LeapMotion.Inspectors
{
    [CustomEditor(typeof(LeapMotionDeviceManagerProfile))]
    /// <summary>
    /// Custom inspector for the Leap Motion input data provider
    /// </summary>
    public class LeapMotionDeviceManagerProfileInspector : BaseMixedRealityToolkitConfigurationProfileInspector
    {
        protected const string ProfileTitle = "Leap Motion Controller Settings";
        protected const string ProfileDescription = "";

        protected LeapMotionDeviceManagerProfile instance;
        protected SerializedProperty leapControllerOrientation;
        protected SerializedProperty leapControllerOffset;
        protected SerializedProperty enterPinchDistance;
        protected SerializedProperty exitPinchDistance;

        private const string leapDocURL = "https://microsoft.github.io/MixedRealityToolkit-Unity/Documentation/CrossPlatform/LeapMotionMRTK.html";

        protected override void OnEnable()
        {
            base.OnEnable();

            instance = (LeapMotionDeviceManagerProfile)target;
            leapControllerOrientation = serializedObject.FindProperty("leapControllerOrientation");
            leapControllerOffset = serializedObject.FindProperty("leapControllerOffset");
            enterPinchDistance = serializedObject.FindProperty("enterPinchDistance");
            exitPinchDistance = serializedObject.FindProperty("exitPinchDistance");
        }

        /// <summary>
        /// Display the MRTK header for the profile and render custom properties
        /// </summary>
        public override void OnInspectorGUI()
        {
            RenderProfileHeader(ProfileTitle, ProfileDescription, target);

            RenderCustomInspector();
        }

        /// <summary>
        /// Render the custom properties for the Leap Motion profile
        /// </summary>
        public virtual void RenderCustomInspector()
        {
            using (new EditorGUI.DisabledGroupScope(IsProfileLock((BaseMixedRealityProfile)target)))
            {
                // Add the documentation help button
                using (new EditorGUILayout.HorizontalScope())
                {
                    // Draw an empty title to align the documentation button to the right
                    InspectorUIUtility.DrawLabel("", InspectorUIUtility.DefaultFontSize, InspectorUIUtility.ColorTint10);

                    InspectorUIUtility.RenderDocumentationButton(leapDocURL);
                }

                // Show warning if the leap core assets are not in the project
                if (!LeapMotionUtilities.IsLeapInProject)
                {
                    EditorGUILayout.HelpBox("The Leap Motion Core Assets could not be found in your project. For more information, visit the Leap Motion MRTK documentation.", MessageType.Error);
                }
                else
                {
                    serializedObject.Update();

                    EditorGUILayout.PropertyField(leapControllerOrientation);

                    if (instance.LeapControllerOrientation == LeapControllerOrientation.Desk)
                    {
                        EditorGUILayout.PropertyField(leapControllerOffset);
                    }

                    EditorGUILayout.PropertyField(enterPinchDistance);

                    EditorGUILayout.PropertyField(exitPinchDistance);

                    serializedObject.ApplyModifiedProperties();
                }
            }
        }

        protected override bool IsProfileInActiveInstance()
        {
            var profile = target as BaseMixedRealityProfile;
            return MixedRealityToolkit.IsInitialized && profile != null &&
                MixedRealityToolkit.Instance.ActiveProfile.InputSystemProfile != null &&
                MixedRealityToolkit.Instance.ActiveProfile.InputSystemProfile.DataProviderConfigurations != null &&
                MixedRealityToolkit.Instance.ActiveProfile.InputSystemProfile.DataProviderConfigurations.Any(s => profile == s.DeviceManagerProfile);
        }
    }
}
