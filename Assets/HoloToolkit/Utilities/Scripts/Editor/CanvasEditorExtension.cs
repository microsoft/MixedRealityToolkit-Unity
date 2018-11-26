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
        private const string DialogText = "Hi there, we noticed that you've changed this canvas to use WorldSpace.\n\n" +
                                          "In order for the InputManager to work properly with uGUI raycasting we'd like to update this canvas' " +
                                          "WorldCamera to use the FocusManager's UIRaycastCamera.\n";

        private Canvas canvas;

        private void OnEnable()
        {
            canvas = (Canvas)target;
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            base.OnInspectorGUI();

            // We will only ask if we have a focus manager in our scene.
            if (EditorGUI.EndChangeCheck() && FocusManager.Instance)
            {
                FocusManager.AssertIsInitialized();
                bool removeHelper = false;

                // Update the world camera if we need to.
                if (canvas.isRootCanvas && canvas.renderMode == RenderMode.WorldSpace && canvas.worldCamera != FocusManager.Instance.UIRaycastCamera)
                {
                    if (EditorUtility.DisplayDialog("Attention!", DialogText, "OK", "Cancel"))
                    {
                        canvas.worldCamera = FocusManager.Instance.UIRaycastCamera;
                    }
                    else
                    {
                        removeHelper = true;
                    }
                }

                // Add the Canvas Helper if we need it.
                if (canvas.isRootCanvas && canvas.renderMode == RenderMode.WorldSpace && canvas.worldCamera == FocusManager.Instance.UIRaycastCamera)
                {
                    var helper = canvas.gameObject.EnsureComponent<CanvasHelper>();
                    helper.Canvas = canvas;
                }

                // Reset the world canvas if we need to.
                if (canvas.isRootCanvas && canvas.renderMode != RenderMode.WorldSpace && canvas.worldCamera == FocusManager.Instance.UIRaycastCamera)
                {
                    // Sets it back to MainCamera default.
                    canvas.worldCamera = null;
                    removeHelper = true;
                }

                // Remove the helper if we don't need it.
                if (removeHelper)
                {
                    // Remove the helper if needed.
                    var helper = canvas.GetComponent<CanvasHelper>();
                    if (helper != null)
                    {
                        DestroyImmediate(helper);
                    }
                }
            }
        }
    }
}
