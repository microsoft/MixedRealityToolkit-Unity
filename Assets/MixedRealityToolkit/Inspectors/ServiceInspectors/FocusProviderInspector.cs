// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using Microsoft.MixedReality.Toolkit.Input;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Editor
{
    [MixedRealityServiceInspectorAttribute(typeof(FocusProvider))]
    public class FocusProviderInspector : IMixedRealityServiceInspector
    {
        private static readonly Color enabledColor = GUI.backgroundColor;
        private static readonly Color disabledColor = Color.Lerp(enabledColor, Color.clear, 0.5f);

        public bool AlwaysDrawSceneGUI { get { return false; } }

        public void DrawInspectorGUI(object target)
        {
            IMixedRealityFocusProvider focusProvider = (IMixedRealityFocusProvider)target;

            EditorGUILayout.LabelField("Pointers", EditorStyles.boldLabel);

            int numPointersFound = 0;
            foreach (IMixedRealityPointer pointer in focusProvider.GetPointers<IMixedRealityPointer>())
            {
                GUI.color = pointer.IsInteractionEnabled ? enabledColor : disabledColor;

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.LabelField(pointer.PointerName);
                EditorGUILayout.Toggle("Interaction Enabled", pointer.IsInteractionEnabled);
                EditorGUILayout.Toggle("Focus Locked", pointer.IsFocusLocked);

                IMixedRealityFocusHandler focusTarget = pointer.FocusTarget;
                if (focusTarget == null)
                {
                    EditorGUILayout.ObjectField("Focus Target", null, typeof(Component), true);
                }
                else
                {
                    EditorGUILayout.Toggle("Has focus target", true);

                    Component focusTargetObject = focusTarget as Component;
                    if (focusTargetObject != null)
                    {
                        EditorGUILayout.ObjectField("Focus Target", focusTargetObject, typeof(Component), true);
                    }
                    else
                    {
                        EditorGUILayout.LabelField("Focus Target" + focusTarget.GetType().Name);
                    }
                }

                EditorGUILayout.EndVertical();

                numPointersFound++;
            }

            if (numPointersFound == 0)
            {
                EditorGUILayout.LabelField("(None found)", EditorStyles.miniLabel);
            }

            GUI.color = enabledColor;
        }

        public void DrawSceneGUI(object target, SceneView sceneView) { }

        public void DrawGizmos(object target) { }
    }
}