// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using Microsoft.MixedReality.Toolkit.Internal.Definitions.InputSystem;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Inspectors.Profiles
{
    [CustomEditor(typeof(MixedRealityPointerProfile))]
    public class MixedRealityPointerProfileInspector : MixedRealityBaseConfigurationProfileInspector
    {
        private static readonly GUIContent ControllerTypeContent = new GUIContent("Controller Type", "The type of Controller this pointer will attach itself to at runtime.");

        private SerializedProperty pointerOptions;
        private ReorderableList pointerOptionList;

        private int currentlySelectedPointerOption = -1;

        private void OnEnable()
        {
            if (!CheckMixedRealityManager(false))
            {
                return;
            }

            pointerOptions = serializedObject.FindProperty("pointerOptions");

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
            EditorGUILayout.LabelField("Pointer Options", EditorStyles.boldLabel);

            if (!CheckMixedRealityManager())
            {
                return;
            }

            EditorGUILayout.HelpBox("Pointers attach themselves onto controllers as they are initialized.", MessageType.Info);
            EditorGUILayout.Space();

            serializedObject.Update();
            currentlySelectedPointerOption = -1;
            pointerOptionList.DoLayoutList();
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
            var pointerOption = pointerOptions.GetArrayElementAtIndex(pointerOptions.arraySize - 1);
            var controllerType = pointerOption.FindPropertyRelative("controllerType");
            var referenceType = controllerType.FindPropertyRelative("reference");
            referenceType.stringValue = string.Empty;
            var handedness = pointerOption.FindPropertyRelative("handedness");
            handedness.enumValueIndex = 0;
            var prefab = pointerOption.FindPropertyRelative("pointerPrefab");
            prefab.objectReferenceValue = null;
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