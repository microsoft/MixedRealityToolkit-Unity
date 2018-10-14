// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Extensions;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.InputSystem;
using Microsoft.MixedReality.Toolkit.Core.Services;
using Microsoft.MixedReality.Toolkit.Core.Utilities;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Inspectors.Utilities
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

        private void OnEnable()
        {
            canvas = (Canvas)target;

            if (MixedRealityOrchestrator.HasActiveProfile &&
                MixedRealityOrchestrator.Instance.ActiveProfile.IsInputSystemEnabled)
            {
                CheckCanvasSettings();
            }
        }

        public override void OnInspectorGUI()
        {
            if (!MixedRealityOrchestrator.HasActiveProfile ||
                !MixedRealityOrchestrator.Instance.ActiveProfile.IsInputSystemEnabled)
            {
                base.OnInspectorGUI();
                return;
            }

            if (MixedRealityOrchestrator.Instance.ActiveProfile.IsInputSystemEnabled && MixedRealityOrchestrator.InputSystem == null)
            {
                EditorGUILayout.HelpBox("No Input System Profile found in the Mixed Reality Orchestrator's Active Profile.", MessageType.Error);
                base.OnInspectorGUI();
                return;
            }

            EditorGUI.BeginChangeCheck();
            base.OnInspectorGUI();

            // We will only ask if we have a focus manager in our scene.
            if (EditorGUI.EndChangeCheck())
            {
                CheckCanvasSettings();
            }
        }

        private void CheckCanvasSettings()
        {
            bool removeHelper = false;

            // Update the world camera if we need to.
            if (canvas.isRootCanvas && canvas.renderMode == RenderMode.WorldSpace && canvas.worldCamera != MixedRealityOrchestrator.InputSystem.FocusProvider.UIRaycastCamera)
            {
                if (EditorUtility.DisplayDialog("Attention!", DialogText, "OK", "Cancel"))
                {
                    canvas.worldCamera = MixedRealityOrchestrator.InputSystem.FocusProvider.UIRaycastCamera;
                }
                else
                {
                    removeHelper = true;
                }
            }

            // Add the Canvas Helper if we need it.
            if (canvas.isRootCanvas && canvas.renderMode == RenderMode.WorldSpace && canvas.worldCamera == MixedRealityOrchestrator.InputSystem.FocusProvider.UIRaycastCamera)
            {
                var helper = canvas.gameObject.EnsureComponent<CanvasUtility>();
                helper.Canvas = canvas;
            }

            // Reset the world canvas if we need to.
            if (canvas.isRootCanvas && canvas.renderMode != RenderMode.WorldSpace && canvas.worldCamera == MixedRealityOrchestrator.InputSystem.FocusProvider.UIRaycastCamera)
            {
                // Sets it back to MainCamera default.
                canvas.worldCamera = null;
                removeHelper = true;
            }

            // Remove the helper if we don't need it.
            if (removeHelper)
            {
                // Remove the helper if needed.
                var helper = canvas.GetComponent<CanvasUtility>();
                if (helper != null)
                {
                    DestroyImmediate(helper);
                }
            }
        }
    }
}