// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using Microsoft.MixedReality.Toolkit.Editor;
using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using System.Linq;
using UnityEditor;

namespace Microsoft.MixedReality.Toolkit.Input
{
    [CustomEditor(typeof(MixedRealityInputSimulationProfile))]
    public class MixedRealityInputSimulationProfileInspector : BaseMixedRealityToolkitConfigurationProfileInspector
    {
        private SerializedProperty indicatorsPrefab;
        private SerializedProperty doublePressTime;

        private SerializedProperty isCameraControlEnabled;

        private SerializedProperty extraMouseRotationScale;
        private SerializedProperty mouseRotationSensitivity;
        private SerializedProperty mouseLookButton;
        private SerializedProperty mouseLookToggle;
        private SerializedProperty isControllerLookInverted;
        private SerializedProperty currentControlMode;
        private SerializedProperty fastControlKey;
        private SerializedProperty controlSlowSpeed;
        private SerializedProperty controlFastSpeed;
        private SerializedProperty moveHorizontal;
        private SerializedProperty moveVertical;
        private SerializedProperty moveUpDown;
        private SerializedProperty mouseX;
        private SerializedProperty mouseY;
        private SerializedProperty lookHorizontal;
        private SerializedProperty lookVertical;
        private SerializedProperty rollCameraCWKey;
        private SerializedProperty rollCameraCCWKey;

        private SerializedProperty defaultHandSimulationMode;

        private SerializedProperty simulateEyePosition;

        private SerializedProperty toggleLeftHandKey;
        private SerializedProperty toggleRightHandKey;
        private SerializedProperty handHideTimeout;
        private SerializedProperty leftHandManipulationKey;
        private SerializedProperty rightHandManipulationKey;

        private SerializedProperty defaultHandGesture;
        private SerializedProperty leftMouseHandGesture;
        private SerializedProperty middleMouseHandGesture;
        private SerializedProperty rightMouseHandGesture;
        private SerializedProperty handGestureAnimationSpeed;

        private SerializedProperty defaultHandDistance;
        private SerializedProperty handDepthMultiplier;
        private SerializedProperty handJitterAmount;

        private SerializedProperty holdStartDuration;
        private SerializedProperty navigationStartThreshold;

        private const string ProfileTitle = "Input Simulation Settings";
        private const string ProfileDescription = "Settings for simulating input devices in the editor.";

        protected override void OnEnable()
        {
            base.OnEnable();

            indicatorsPrefab = serializedObject.FindProperty("indicatorsPrefab");
            doublePressTime = serializedObject.FindProperty("doublePressTime");

            isCameraControlEnabled = serializedObject.FindProperty("isCameraControlEnabled");

            extraMouseRotationScale = serializedObject.FindProperty("extraMouseRotationScale");
            mouseRotationSensitivity = serializedObject.FindProperty("mouseRotationSensitivity");
            mouseLookButton = serializedObject.FindProperty("mouseLookButton");
            mouseLookToggle = serializedObject.FindProperty("mouseLookToggle");
            isControllerLookInverted = serializedObject.FindProperty("isControllerLookInverted");
            currentControlMode = serializedObject.FindProperty("currentControlMode");
            fastControlKey = serializedObject.FindProperty("fastControlKey");
            controlSlowSpeed = serializedObject.FindProperty("controlSlowSpeed");
            controlFastSpeed = serializedObject.FindProperty("controlFastSpeed");
            moveHorizontal = serializedObject.FindProperty("moveHorizontal");
            moveVertical = serializedObject.FindProperty("moveVertical");
            moveUpDown = serializedObject.FindProperty("moveUpDown");
            mouseX = serializedObject.FindProperty("mouseX");
            mouseY = serializedObject.FindProperty("mouseY");
            lookHorizontal = serializedObject.FindProperty("lookHorizontal");
            lookVertical = serializedObject.FindProperty("lookVertical");
            rollCameraCWKey = serializedObject.FindProperty("rollCameraCWKey");
            rollCameraCCWKey = serializedObject.FindProperty("rollCameraCCWKey");

            defaultHandSimulationMode = serializedObject.FindProperty("defaultHandSimulationMode");

            simulateEyePosition = serializedObject.FindProperty("simulateEyePosition");

            toggleLeftHandKey = serializedObject.FindProperty("toggleLeftHandKey");
            toggleRightHandKey = serializedObject.FindProperty("toggleRightHandKey");
            handHideTimeout = serializedObject.FindProperty("handHideTimeout");
            leftHandManipulationKey = serializedObject.FindProperty("leftHandManipulationKey");
            rightHandManipulationKey = serializedObject.FindProperty("rightHandManipulationKey");

            defaultHandGesture = serializedObject.FindProperty("defaultHandGesture");
            leftMouseHandGesture = serializedObject.FindProperty("leftMouseHandGesture");
            middleMouseHandGesture = serializedObject.FindProperty("middleMouseHandGesture");
            rightMouseHandGesture = serializedObject.FindProperty("rightMouseHandGesture");
            handGestureAnimationSpeed = serializedObject.FindProperty("handGestureAnimationSpeed");

            holdStartDuration = serializedObject.FindProperty("holdStartDuration");
            navigationStartThreshold = serializedObject.FindProperty("navigationStartThreshold");

            defaultHandDistance = serializedObject.FindProperty("defaultHandDistance");
            handDepthMultiplier = serializedObject.FindProperty("handDepthMultiplier");
            handJitterAmount = serializedObject.FindProperty("handJitterAmount");
        }

        public override void OnInspectorGUI()
        {
            RenderProfileHeader(ProfileTitle, ProfileDescription, target, true, BackProfileType.Input);

            serializedObject.Update();

            using (new GUIEnabledWrapper(!IsProfileLock((BaseMixedRealityProfile)target)))
            {
                EditorGUILayout.PropertyField(indicatorsPrefab);

                EditorGUILayout.PropertyField(doublePressTime);

                EditorGUILayout.PropertyField(isCameraControlEnabled);
                {
                    EditorGUILayout.BeginVertical("Label");
                    using (new GUIEnabledWrapper(isCameraControlEnabled.boolValue))
                    {
                        EditorGUILayout.PropertyField(mouseRotationSensitivity);
                        EditorGUILayout.PropertyField(extraMouseRotationScale);
                        EditorGUILayout.PropertyField(mouseLookButton);
                        EditorGUILayout.PropertyField(mouseLookToggle);
                        EditorGUILayout.PropertyField(isControllerLookInverted);
                        EditorGUILayout.PropertyField(currentControlMode);
                        EditorGUILayout.PropertyField(fastControlKey);
                        EditorGUILayout.PropertyField(controlSlowSpeed);
                        EditorGUILayout.PropertyField(controlFastSpeed);
                        EditorGUILayout.PropertyField(moveHorizontal);
                        EditorGUILayout.PropertyField(moveVertical);
                        EditorGUILayout.PropertyField(moveUpDown);
                        EditorGUILayout.PropertyField(mouseX);
                        EditorGUILayout.PropertyField(mouseY);
                        EditorGUILayout.PropertyField(lookHorizontal);
                        EditorGUILayout.PropertyField(lookVertical);
                        EditorGUILayout.PropertyField(rollCameraCWKey);
                        EditorGUILayout.PropertyField(rollCameraCCWKey);

                        EditorGUILayout.EndVertical();
                    }
                }

                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(simulateEyePosition);

                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(defaultHandSimulationMode);
                {
                    EditorGUILayout.BeginVertical("Label");

                    EditorGUILayout.PropertyField(toggleLeftHandKey);
                    EditorGUILayout.PropertyField(toggleRightHandKey);
                    EditorGUILayout.PropertyField(handHideTimeout);
                    EditorGUILayout.PropertyField(leftHandManipulationKey);
                    EditorGUILayout.PropertyField(rightHandManipulationKey);
                    EditorGUILayout.Space();

                    EditorGUILayout.PropertyField(defaultHandGesture);
                    EditorGUILayout.PropertyField(leftMouseHandGesture);
                    EditorGUILayout.PropertyField(middleMouseHandGesture);
                    EditorGUILayout.PropertyField(rightMouseHandGesture);
                    EditorGUILayout.PropertyField(handGestureAnimationSpeed);
                    EditorGUILayout.Space();

                    EditorGUILayout.PropertyField(holdStartDuration);
                    EditorGUILayout.PropertyField(navigationStartThreshold);
                    EditorGUILayout.Space();

                    EditorGUILayout.PropertyField(defaultHandDistance);
                    EditorGUILayout.PropertyField(handDepthMultiplier);
                    EditorGUILayout.PropertyField(handJitterAmount);
                    EditorGUILayout.Space();

                    EditorGUILayout.EndVertical();
                }

                serializedObject.ApplyModifiedProperties();
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
