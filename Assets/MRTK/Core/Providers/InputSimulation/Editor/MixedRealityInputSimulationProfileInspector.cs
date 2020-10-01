// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.﻿

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
        private SerializedProperty isHandsFreeInputEnabled;

        private SerializedProperty mouseLookSpeed;
        private SerializedProperty mouseRotationSensitivity;
        private SerializedProperty mouseLookButton;
        private SerializedProperty mouseLookToggle;
        private SerializedProperty isControllerLookInverted;
        private SerializedProperty cameraOriginOffset;
        private SerializedProperty currentControlMode;
        private SerializedProperty fastControlKey;
        private SerializedProperty controlSlowSpeed;
        private SerializedProperty controlFastSpeed;
        private SerializedProperty moveHorizontal;
        private SerializedProperty moveVertical;
        private SerializedProperty moveUpDown;
        private SerializedProperty mouseX;
        private SerializedProperty mouseY;
        private SerializedProperty mouseScroll;
        private SerializedProperty lookHorizontal;
        private SerializedProperty lookVertical;

        private SerializedProperty defaultControllerSimulationMode;

        private SerializedProperty defaultEyeGazeSimulationMode;

        private SerializedProperty toggleLeftControllerKey;
        private SerializedProperty toggleRightControllerKey;
        private SerializedProperty controllerHideTimeout;
        private SerializedProperty leftControllerManipulationKey;
        private SerializedProperty rightControllerManipulationKey;
        private SerializedProperty mouseControllerRotationSpeed;
        private SerializedProperty controllerRotateButton;

        private SerializedProperty defaultHandGesture;
        private SerializedProperty leftMouseHandGesture;
        private SerializedProperty middleMouseHandGesture;
        private SerializedProperty rightMouseHandGesture;
        private SerializedProperty handGestureAnimationSpeed;

        private SerializedProperty defaultControllerDistance;
        private SerializedProperty controllerDepthMultiplier;
        private SerializedProperty controllerJitterAmount;

        private SerializedProperty motionControllerTriggerKey;
        private SerializedProperty motionControllerGrabKey;
        private SerializedProperty motionControllerMenuKey;

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
            isHandsFreeInputEnabled = serializedObject.FindProperty("isHandsFreeInputEnabled");

            mouseLookSpeed = serializedObject.FindProperty("mouseLookSpeed");
            mouseRotationSensitivity = serializedObject.FindProperty("mouseRotationSensitivity");
            mouseLookButton = serializedObject.FindProperty("mouseLookButton");
            mouseLookToggle = serializedObject.FindProperty("mouseLookToggle");
            isControllerLookInverted = serializedObject.FindProperty("isControllerLookInverted");
            cameraOriginOffset = serializedObject.FindProperty("cameraOriginOffset");
            currentControlMode = serializedObject.FindProperty("currentControlMode");
            fastControlKey = serializedObject.FindProperty("fastControlKey");
            controlSlowSpeed = serializedObject.FindProperty("controlSlowSpeed");
            controlFastSpeed = serializedObject.FindProperty("controlFastSpeed");
            moveHorizontal = serializedObject.FindProperty("moveHorizontal");
            moveVertical = serializedObject.FindProperty("moveVertical");
            moveUpDown = serializedObject.FindProperty("moveUpDown");
            mouseX = serializedObject.FindProperty("mouseX");
            mouseY = serializedObject.FindProperty("mouseY");
            mouseScroll = serializedObject.FindProperty("mouseScroll");
            lookHorizontal = serializedObject.FindProperty("lookHorizontal");
            lookVertical = serializedObject.FindProperty("lookVertical");

            defaultEyeGazeSimulationMode = serializedObject.FindProperty("defaultEyeGazeSimulationMode");
            defaultControllerSimulationMode = serializedObject.FindProperty("defaultControllerSimulationMode");

            toggleLeftControllerKey = serializedObject.FindProperty("toggleLeftControllerKey");
            toggleRightControllerKey = serializedObject.FindProperty("toggleRightControllerKey");
            controllerHideTimeout = serializedObject.FindProperty("controllerHideTimeout");
            leftControllerManipulationKey = serializedObject.FindProperty("leftControllerManipulationKey");
            rightControllerManipulationKey = serializedObject.FindProperty("rightControllerManipulationKey");
            mouseControllerRotationSpeed = serializedObject.FindProperty("mouseControllerRotationSpeed");
            controllerRotateButton = serializedObject.FindProperty("controllerRotateButton");

            defaultHandGesture = serializedObject.FindProperty("defaultHandGesture");
            leftMouseHandGesture = serializedObject.FindProperty("leftMouseHandGesture");
            middleMouseHandGesture = serializedObject.FindProperty("middleMouseHandGesture");
            rightMouseHandGesture = serializedObject.FindProperty("rightMouseHandGesture");
            handGestureAnimationSpeed = serializedObject.FindProperty("handGestureAnimationSpeed");

            holdStartDuration = serializedObject.FindProperty("holdStartDuration");
            navigationStartThreshold = serializedObject.FindProperty("navigationStartThreshold");

            defaultControllerDistance = serializedObject.FindProperty("defaultControllerDistance");
            controllerDepthMultiplier = serializedObject.FindProperty("controllerDepthMultiplier");
            controllerJitterAmount = serializedObject.FindProperty("controllerJitterAmount");

            motionControllerTriggerKey = serializedObject.FindProperty("motionControllerTriggerKey");
            motionControllerGrabKey = serializedObject.FindProperty("motionControllerGrabKey");
            motionControllerMenuKey = serializedObject.FindProperty("motionControllerMenuKey");
        }

        public override void OnInspectorGUI()
        {
            if (!RenderProfileHeader(ProfileTitle, ProfileDescription, target, true, BackProfileType.Input))
            {
                return;
            }

            serializedObject.Update();

            using (new EditorGUI.DisabledGroupScope(IsProfileLock((BaseMixedRealityProfile)target)))
            {
                EditorGUILayout.PropertyField(indicatorsPrefab);

                EditorGUILayout.Space();
                EditorGUILayout.BeginVertical("Label");
                EditorGUILayout.PropertyField(mouseRotationSensitivity);
                EditorGUILayout.PropertyField(mouseX);
                EditorGUILayout.PropertyField(mouseY);
                EditorGUILayout.PropertyField(mouseScroll);
                EditorGUILayout.PropertyField(doublePressTime);
                EditorGUILayout.PropertyField(isHandsFreeInputEnabled);
                EditorGUILayout.EndVertical();

                EditorGUILayout.PropertyField(isCameraControlEnabled);
                {
                    EditorGUILayout.BeginVertical("Label");
                    using (new EditorGUI.DisabledGroupScope(!isCameraControlEnabled.boolValue))
                    {
                        EditorGUILayout.PropertyField(mouseLookSpeed);
                        EditorGUILayout.PropertyField(mouseLookButton);
                        EditorGUILayout.PropertyField(mouseLookToggle);
                        EditorGUILayout.PropertyField(isControllerLookInverted);
                        EditorGUILayout.PropertyField(cameraOriginOffset);
                        EditorGUILayout.PropertyField(currentControlMode);
                        EditorGUILayout.PropertyField(fastControlKey);
                        EditorGUILayout.PropertyField(controlSlowSpeed);
                        EditorGUILayout.PropertyField(controlFastSpeed);
                        EditorGUILayout.PropertyField(moveHorizontal);
                        EditorGUILayout.PropertyField(moveVertical);
                        EditorGUILayout.PropertyField(moveUpDown);
                        EditorGUILayout.PropertyField(lookHorizontal);
                        EditorGUILayout.PropertyField(lookVertical);

                        EditorGUILayout.EndVertical();
                    }
                }

                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(defaultEyeGazeSimulationMode);

                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(defaultControllerSimulationMode);
                {
                    EditorGUILayout.BeginVertical("Label");

                    EditorGUILayout.PropertyField(toggleLeftControllerKey);
                    EditorGUILayout.PropertyField(toggleRightControllerKey);
                    EditorGUILayout.PropertyField(controllerHideTimeout);
                    EditorGUILayout.PropertyField(leftControllerManipulationKey);
                    EditorGUILayout.PropertyField(rightControllerManipulationKey);
                    EditorGUILayout.PropertyField(mouseControllerRotationSpeed);
                    EditorGUILayout.PropertyField(controllerRotateButton);
                    EditorGUILayout.Space();

                    EditorGUILayout.PropertyField(defaultControllerDistance);
                    EditorGUILayout.PropertyField(controllerDepthMultiplier);
                    EditorGUILayout.PropertyField(controllerJitterAmount);
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

                    EditorGUILayout.PropertyField(motionControllerTriggerKey);
                    EditorGUILayout.PropertyField(motionControllerGrabKey);
                    EditorGUILayout.PropertyField(motionControllerMenuKey);
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
