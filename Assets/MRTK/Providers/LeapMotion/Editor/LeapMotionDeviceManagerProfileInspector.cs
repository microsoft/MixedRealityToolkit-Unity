// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.﻿

using Microsoft.MixedReality.Toolkit.Editor;
using Microsoft.MixedReality.Toolkit.LeapMotion.Input;
using Microsoft.MixedReality.Toolkit.LeapMotion.Utilities;
using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using System.Linq;
using UnityEditor;
using UnityEngine;

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

        protected SerializedProperty leapVRDeviceOffsetMode;
        protected SerializedProperty leapVRDeviceOffsetY;
        protected SerializedProperty leapVRDeviceOffsetZ;
        protected SerializedProperty leapVRDeviceOffsetTiltX;
        protected SerializedProperty leapVRDeviceOrigin;

        protected SerializedProperty enterPinchDistance;
        protected SerializedProperty exitPinchDistance;

        private const string leapDocURL = "https://microsoft.github.io/MixedRealityToolkit-Unity/Documentation/CrossPlatform/LeapMotionMRTK.html";

        // Used for setting the leapVRDeviceOrigin object reference value
        Transform leapVRDeviceOriginTransform;

        protected override void OnEnable()
        {
            base.OnEnable();

            instance = target as LeapMotionDeviceManagerProfile;
            leapControllerOrientation = serializedObject.FindProperty("leapControllerOrientation");
            leapControllerOffset = serializedObject.FindProperty("leapControllerOffset");

            leapVRDeviceOffsetMode = serializedObject.FindProperty("leapVRDeviceOffsetMode");
            leapVRDeviceOffsetY = serializedObject.FindProperty("leapVRDeviceOffsetY");
            leapVRDeviceOffsetZ = serializedObject.FindProperty("leapVRDeviceOffsetZ");
            leapVRDeviceOffsetTiltX = serializedObject.FindProperty("leapVRDeviceOffsetTiltX");
            leapVRDeviceOrigin = serializedObject.FindProperty("leapVRDeviceOrigin");

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
                    else if (instance.LeapControllerOrientation == LeapControllerOrientation.Headset)
                    {
                        // Allow selection of the LeapVRDeviceOffsetMode if the LeapControllerOrientation is Headset
                        EditorGUILayout.PropertyField(leapVRDeviceOffsetMode);

                        if (leapVRDeviceOffsetMode.enumValueIndex == (int)LeapVRDeviceOffsetMode.ManualHeadOffset)
                        {
                            // Display the properties for editing the head offset 
                            EditorGUILayout.PropertyField(leapVRDeviceOffsetY);
                            EditorGUILayout.PropertyField(leapVRDeviceOffsetZ);
                            EditorGUILayout.PropertyField(leapVRDeviceOffsetTiltX);   
                        }
                        else if (leapVRDeviceOffsetMode.enumValueIndex == (int)LeapVRDeviceOffsetMode.Transform)
                        {
                            // Display the transform property 
                            // EditorGUILayout.PropertyField() did not allow the setting the transform property in editor 
                            leapVRDeviceOriginTransform = EditorGUILayout.ObjectField("Leap VR Device Origin", leapVRDeviceOrigin.objectReferenceValue, typeof(Transform), true) as Transform;

                            instance.LeapVRDeviceOrigin = leapVRDeviceOriginTransform;
                        }
                    }

                    // Display pinch thresholds
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
