// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using HoloToolkit.Unity.InputModule;
using UnityEditor;
using UnityEngine;

namespace HoloToolkit.Unity
{
    /// <summary>
    /// Helper class to assign the UIRaycastCamera when creating a new canvas object and assigning the world space render mode.
    /// </summary>
    [CustomEditor(typeof(Canvas))]
    public class CanvasEditorExtension : Editor
    {
        private Canvas canvas;
        private bool userPermission;

        private void OnEnable()
        {
            canvas = (Canvas)target;
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            base.OnInspectorGUI();

            // We will only ask if we have a focus manager in our scene.
            if (EditorGUI.EndChangeCheck() && FocusManager.IsInitialized)
            {
                if (canvas.isRootCanvas && canvas.renderMode == RenderMode.WorldSpace && canvas.worldCamera != FocusManager.Instance.UIRaycastCamera)
                {
                    userPermission = EditorUtility.DisplayDialog("Attention!",
                        "Hi there, we noticed that you've changed this canvas to use WorldSpace.\n\n" +
                        "In order for the InputManager to work properly with uGUI raycasting we'd like to update this canvas' " +
                        "WorldCamera to use the FocusManager's UIRaycastCamera.\n",
                        "OK", "Cancel");

                    if (userPermission)
                    {
                        canvas.worldCamera = FocusManager.Instance.UIRaycastCamera;
                    }
                }
                else
                {
                    // Sets it back to MainCamera default
                    canvas.worldCamera = null;
                }
            }
        }
    }
}
