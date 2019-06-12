// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Editor;

namespace Microsoft.MixedReality.Toolkit.Input.Editor
{
    [CustomEditor(typeof(MixedRealityPointerProfile))]
    public class MixedRealityPointerProfileInspector : BaseMixedRealityToolkitConfigurationProfileInspector
    {
        private static readonly GUIContent ControllerTypeContent = new GUIContent("Controller Type", "The type of Controller this pointer will attach itself to at runtime.");

        private const string ProfileTitle = "Pointer Settings";
        private const string ProfileDescription = "Pointers attach themselves onto controllers as they are initialized.";

        private SerializedProperty pointingExtent;
        private SerializedProperty pointingRaycastLayerMasks;
        private static bool showPointerOptionProperties = true;
        private SerializedProperty pointerOptions;
        private ReorderableList pointerOptionList;
        private SerializedProperty debugDrawPointingRays;
        private SerializedProperty debugDrawPointingRayColors;
        private SerializedProperty gazeCursorPrefab;
        private SerializedProperty gazeProviderType;
        private SerializedProperty showCursorWithEyeGaze;
        private SerializedProperty pointerMediator;

        private int currentlySelectedPointerOption = -1;

        protected override void OnEnable()
        {
            base.OnEnable();

            pointingExtent = serializedObject.FindProperty("pointingExtent");
            pointingRaycastLayerMasks = serializedObject.FindProperty("pointingRaycastLayerMasks");
            pointerOptions = serializedObject.FindProperty("pointerOptions");
            debugDrawPointingRays = serializedObject.FindProperty("debugDrawPointingRays");
            debugDrawPointingRayColors = serializedObject.FindProperty("debugDrawPointingRayColors");
            gazeCursorPrefab = serializedObject.FindProperty("gazeCursorPrefab");
            gazeProviderType = serializedObject.FindProperty("gazeProviderType");
            showCursorWithEyeGaze = serializedObject.FindProperty("showCursorWithEyeGaze");
            pointerMediator = serializedObject.FindProperty("pointerMediator");

            pointerOptionList = new ReorderableList(serializedObject, pointerOptions, false, false, true, true)
            {
                elementHeight = EditorGUIUtility.singleLineHeight * 4
            };

            pointerOptionList.drawElementCallback += DrawPointerOptionElement;
            pointerOptionList.onAddCallback += OnPointerOptionAdded;
            pointerOptionList.onRemoveCallback += OnPointerOptionRemoved;
        }

        public override void OnInspectorGUI()
        {
            RenderProfileHeader(ProfileTitle, ProfileDescription, target, true, BackProfileType.Input);

            using (new GUIEnabledWrapper(!IsProfileLock((BaseMixedRealityProfile)target)))
            {
                serializedObject.Update();
                currentlySelectedPointerOption = -1;

                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Gaze Settings", EditorStyles.boldLabel);
                {
                    EditorGUILayout.Space();
                    EditorGUILayout.PropertyField(gazeCursorPrefab);
                    EditorGUILayout.PropertyField(gazeProviderType);
                    EditorGUILayout.Space();

                    if (MixedRealityEditorUtility.RenderIndentedButton("Customize Gaze Provider Settings"))
                    {
                        Selection.activeObject = CameraCache.Main.gameObject;
                    }
                }

                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Pointer Settings", EditorStyles.boldLabel);
                {
                    EditorGUILayout.PropertyField(pointingExtent);
                    EditorGUILayout.PropertyField(pointingRaycastLayerMasks, true);
                    EditorGUILayout.PropertyField(pointerMediator);

                    EditorGUILayout.Space();
                    showPointerOptionProperties = EditorGUILayout.Foldout(showPointerOptionProperties, "Pointer Options", true);
                    if (showPointerOptionProperties)
                    {
                        using (new EditorGUI.IndentLevelScope())
                        {
                            pointerOptionList.DoLayoutList();
                        }
                    }
                }

                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Debug Settings", EditorStyles.boldLabel);
                {
                    EditorGUILayout.PropertyField(debugDrawPointingRays);
                    EditorGUILayout.PropertyField(debugDrawPointingRayColors, true);
                }

                serializedObject.ApplyModifiedProperties();
            }
        }

        protected override bool IsProfileInActiveInstance()
        {
            var profile = target as BaseMixedRealityProfile;
            return MixedRealityToolkit.IsInitialized && profile != null &&
                   MixedRealityToolkit.Instance.ActiveProfile.InputSystemProfile != null &&
                   profile == MixedRealityToolkit.Instance.ActiveProfile.InputSystemProfile.PointerProfile;
        }

        private void DrawPointerOptionElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            if (isFocused)
            {
                currentlySelectedPointerOption = index;
            }

            bool lastMode = EditorGUIUtility.wideMode;
            EditorGUIUtility.wideMode = true;

            var halfFieldHeight = EditorGUIUtility.singleLineHeight * 0.25f;
            var controllerTypeRect = new Rect(rect.x, rect.y + halfFieldHeight, rect.width, EditorGUIUtility.singleLineHeight);
            var handednessControlRect = new Rect(rect.x, rect.y + halfFieldHeight * 6, rect.width, EditorGUIUtility.singleLineHeight);
            var pointerPrefabRect = new Rect(rect.x, rect.y + halfFieldHeight * 11, rect.width, EditorGUIUtility.singleLineHeight);

            var pointerOption = pointerOptions.GetArrayElementAtIndex(index);
            var controllerType = pointerOption.FindPropertyRelative("controllerType");
            var handedness = pointerOption.FindPropertyRelative("handedness");
            var prefab = pointerOption.FindPropertyRelative("pointerPrefab");

            EditorGUI.PropertyField(controllerTypeRect, controllerType, ControllerTypeContent);
            EditorGUI.PropertyField(handednessControlRect, handedness);
            EditorGUI.PropertyField(pointerPrefabRect, prefab);

            EditorGUIUtility.wideMode = lastMode;
        }

        private void OnPointerOptionAdded(ReorderableList list)
        {
            pointerOptions.arraySize += 1;
        }

        private void OnPointerOptionRemoved(ReorderableList list)
        {
            if (currentlySelectedPointerOption >= 0)
            {
                pointerOptions.DeleteArrayElementAtIndex(currentlySelectedPointerOption);
            }
        }
    }
}