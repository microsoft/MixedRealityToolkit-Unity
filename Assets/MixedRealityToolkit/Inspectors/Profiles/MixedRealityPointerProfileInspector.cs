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

        private static bool showPointerProperties = true;
        private SerializedProperty pointingExtent;
        private SerializedProperty pointingRaycastLayerMasks;
        private static bool showPointerOptionProperties = true;
        private SerializedProperty pointerOptions;
        private ReorderableList pointerOptionList;
        private static bool showPointerDebugProperties = true;
        private SerializedProperty debugDrawPointingRays;
        private SerializedProperty debugDrawPointingRayColors;
        private static bool showGazeProperties = true;
        private SerializedProperty gazeCursorPrefab;
        private SerializedProperty gazeProviderType;
        private SerializedProperty showCursorWithEyeGaze;
        private SerializedProperty pointerMediator;

        private int currentlySelectedPointerOption = -1;

        protected override void OnEnable()
        {
            base.OnEnable();

            if (!MixedRealityInspectorUtility.CheckMixedRealityConfigured(false))
            {
                return;
            }

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
            RenderMixedRealityToolkitLogo();
            if (!MixedRealityInspectorUtility.CheckMixedRealityConfigured())
            {
                return;
            }

            if (DrawBacktrackProfileButton("Back to Input Profile", MixedRealityToolkit.Instance.ActiveProfile.InputSystemProfile))
            {
                return;
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Pointer Profile", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("Pointers attach themselves onto controllers as they are initialized.", MessageType.Info);
            EditorGUILayout.Space();

            CheckProfileLock(target);
            serializedObject.Update();
            currentlySelectedPointerOption = -1;

            EditorGUILayout.Space();
            showPointerProperties = EditorGUILayout.Foldout(showPointerProperties, "Pointer Settings", true);
            if (showPointerProperties)
            {
                using (new EditorGUI.IndentLevelScope())
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

                    EditorGUILayout.Space();
                    showPointerDebugProperties = EditorGUILayout.Foldout(showPointerDebugProperties, "Debug Settings", true);
                    if (showPointerDebugProperties)
                    {
                        using (new EditorGUI.IndentLevelScope())
                        {
                            EditorGUILayout.PropertyField(debugDrawPointingRays);
                            EditorGUILayout.PropertyField(debugDrawPointingRayColors, true);
                        }
                    }
                }
            }

            EditorGUILayout.Space();
            showGazeProperties = EditorGUILayout.Foldout(showGazeProperties, "Gaze Settings", true);
            if (showGazeProperties)
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    EditorGUILayout.HelpBox("The gaze provider uses the default settings above, but further customization of the gaze can be done on the Gaze Provider.", MessageType.Info);

                    EditorGUILayout.Space();
                    EditorGUILayout.PropertyField(gazeCursorPrefab);
                    EditorGUILayout.PropertyField(gazeProviderType);
                    EditorGUILayout.PropertyField(showCursorWithEyeGaze);

                    EditorGUILayout.Space();
                    if (GUILayout.Button("Customize Gaze Provider Settings"))
                    {
                        Selection.activeObject = CameraCache.Main.gameObject;
                    }
                }
            }
            
            serializedObject.ApplyModifiedProperties();
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