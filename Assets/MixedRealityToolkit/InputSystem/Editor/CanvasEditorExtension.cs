// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.InputSystem.Focus;
using Microsoft.MixedReality.Toolkit.InputSystem.Utilities;
using Microsoft.MixedReality.Toolkit.Internal.Extensions;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces;
using Microsoft.MixedReality.Toolkit.Internal.Managers;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.InputSystem.EditorScript
{
    /// <summary>
    /// Helper class to assign the UIRaycastCamera when creating a new canvas object and assigning the world space render mode.
    /// </summary>
    [CustomEditor(typeof(Canvas))]
    public class CanvasEditorExtension : Editor
    {
        private const string DialogText = "Hi there, we noticed that you've changed this canvas to use WorldSpace.\n\n" +
                                          "In order for the InputManager to work properly with uGUI raycasting we'd like to update this canvas' " +
                                          "WorldCamera to use the FocusProvider's UIRaycastCamera.\n";

        private Canvas canvas;
        private IMixedRealityInputSystem inputSystem;

        private void OnEnable()
        {
            canvas = (Canvas)target;
            inputSystem = MixedRealityManager.Instance.GetManager<IMixedRealityInputSystem>();
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            base.OnInspectorGUI();

            // We will only ask if we have a focus manager in our scene.
            if (EditorGUI.EndChangeCheck())
            {
                bool removeHelper = false;

                // Update the world camera if we need to.
                if (canvas.isRootCanvas && canvas.renderMode == RenderMode.WorldSpace && canvas.worldCamera != inputSystem.FocusProvider.UIRaycastCamera)
                {
                    if (EditorUtility.DisplayDialog("Attention!", DialogText, "OK", "Cancel"))
                    {
                        canvas.worldCamera = inputSystem.FocusProvider.UIRaycastCamera;
                    }
                    else
                    {
                        removeHelper = true;
                    }
                }

                // Add the Canvas Helper if we need it.
                if (canvas.isRootCanvas && canvas.renderMode == RenderMode.WorldSpace && canvas.worldCamera == inputSystem.FocusProvider.UIRaycastCamera)
                {
                    var helper = canvas.gameObject.EnsureComponent<CanvasHelper>();
                    helper.Canvas = canvas;
                }

                // Reset the world canvas if we need to.
                if (canvas.isRootCanvas && canvas.renderMode != RenderMode.WorldSpace && canvas.worldCamera == inputSystem.FocusProvider.UIRaycastCamera)
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