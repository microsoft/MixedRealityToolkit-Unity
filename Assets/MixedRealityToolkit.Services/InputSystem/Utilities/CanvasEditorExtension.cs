// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input.Utilities
{
    /// <summary>
    /// Helper class to get CanvasUtility onto Canvas objects.
    /// </summary>
    [CustomEditor(typeof(Canvas))]
    public class CanvasEditorExtension : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            Canvas canvas = (Canvas)target;

            CanvasUtility utility = canvas.GetComponent<CanvasUtility>();

            if (!utility)
            {
                EditorGUILayout.HelpBox("Canvases must have the CanvasUtility on them to support Mixed Reality Toolkit input.", MessageType.Error);
                if (GUILayout.Button("Add CanvasUtility"))
                {
                    Undo.AddComponent<CanvasUtility>(canvas.gameObject);
                }
            }

            base.OnInspectorGUI();
        }
    }
}

#endif