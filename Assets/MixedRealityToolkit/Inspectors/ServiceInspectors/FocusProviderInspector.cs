// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using Microsoft.MixedReality.Toolkit.Input;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Editor
{
    [MixedRealityServiceInspector(typeof(FocusProvider))]
    public class FocusProviderInspector : BaseMixedRealityServiceInspector
    {
        private static readonly Color enabledColor = GUI.backgroundColor;
        private static readonly Color disabledColor = Color.Lerp(enabledColor, Color.clear, 0.5f);

        public override void DrawInspectorGUI(object target)
        {
            IMixedRealityFocusProvider focusProvider = (IMixedRealityFocusProvider)target;

            EditorGUILayout.LabelField("Active Pointers", EditorStyles.boldLabel);

            int numPointersFound = 0;
            foreach (IMixedRealityPointer pointer in focusProvider.GetPointers<IMixedRealityPointer>())
            {
                GUI.color = pointer.IsInteractionEnabled ? enabledColor : disabledColor;

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.LabelField(pointer.PointerName);
                EditorGUILayout.Toggle("Interaction Enabled", pointer.IsInteractionEnabled);
                EditorGUILayout.Toggle("Focus Locked", pointer.IsFocusLocked);

                IPointerResult pointerResult = pointer.Result;
                if (pointerResult == null)
                {
                    EditorGUILayout.ObjectField("Focus Result", null, typeof(GameObject), true);
                }
                else
                {
                    EditorGUILayout.ObjectField("Focus Result", pointerResult.CurrentPointerTarget, typeof(GameObject), true);
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
    }
}