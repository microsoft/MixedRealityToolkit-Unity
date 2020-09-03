// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.﻿

using Microsoft.MixedReality.Toolkit.Input;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Editor
{
    [MixedRealityServiceInspector(typeof(IMixedRealityFocusProvider))]
    public class FocusProviderInspector : BaseMixedRealityServiceInspector
    {
        private static readonly Color enabledColor = GUI.backgroundColor;
        private static readonly Color disabledColor = Color.Lerp(enabledColor, Color.clear, 0.5f);

        public override void DrawInspectorGUI(object target)
        {
            IMixedRealityFocusProvider focusProvider = (IMixedRealityFocusProvider)target;

            EditorGUILayout.LabelField("Active Pointers", EditorStyles.boldLabel);

            if (!Application.isPlaying)
            {
                EditorGUILayout.HelpBox("Pointers will be populated once you enter play mode.", MessageType.Info);
                return;
            }

            bool pointerFound = false;
            foreach (IMixedRealityPointer pointer in focusProvider.GetPointers<IMixedRealityPointer>())
            {
                GUI.color = pointer.IsInteractionEnabled ? enabledColor : disabledColor;

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.LabelField(pointer.PointerName);
                EditorGUILayout.Toggle("Interaction Enabled", pointer.IsInteractionEnabled);
                EditorGUILayout.Toggle("Focus Locked", pointer.IsFocusLocked);
                EditorGUILayout.ObjectField("Focus Result", pointer.Result?.CurrentPointerTarget, typeof(GameObject), true);
                EditorGUILayout.EndVertical();

                pointerFound = true;
            }

            if (!pointerFound)
            {
                EditorGUILayout.LabelField("(None found)", EditorStyles.miniLabel);
            }

            GUI.color = enabledColor;
        }
    }
}