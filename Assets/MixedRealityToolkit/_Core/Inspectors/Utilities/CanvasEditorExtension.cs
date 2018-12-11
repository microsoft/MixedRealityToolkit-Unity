// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Extensions;
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

        private bool hasUtility = false;

        private static bool IsUtilityValid => MixedRealityToolkit.HasActiveProfile && MixedRealityToolkit.Instance.ActiveProfile.IsInputSystemEnabled && MixedRealityToolkit.InputSystem?.FocusProvider != null;

        private void OnEnable()
        {
            if (!MixedRealityToolkit.IsInitialized || !MixedRealityPreferences.ShowCanvasUtilityPrompt) { return; }

            canvas = (Canvas)target;

            var utility = canvas.GetComponent<CanvasUtility>();

            hasUtility = utility != null;

            if (hasUtility && !IsUtilityValid ||
                !hasUtility && IsUtilityValid)
            {
                UpdateCanvasSettings();
            }
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            base.OnInspectorGUI();

            if (EditorGUI.EndChangeCheck() &&
                MixedRealityToolkit.IsInitialized &&
                MixedRealityPreferences.ShowCanvasUtilityPrompt)
            {
                UpdateCanvasSettings();
            }
        }

        private void UpdateCanvasSettings()
        {
            bool removeUtility = false;

            // Update the world camera if we need to.
            if (IsUtilityValid &&
                canvas.isRootCanvas &&
                canvas.renderMode == RenderMode.WorldSpace &&
                canvas.worldCamera != MixedRealityToolkit.InputSystem.FocusProvider.UIRaycastCamera)
            {
                var selection = EditorUtility.DisplayDialogComplex("Attention!", DialogText, "OK", "Cancel", "Dismiss Forever");
                switch (selection)
                {
                    case 0:
                        canvas.worldCamera = MixedRealityToolkit.InputSystem.FocusProvider.UIRaycastCamera;
                        break;
                    case 1:
                        removeUtility = true;
                        break;
                    case 2:
                        MixedRealityPreferences.ShowCanvasUtilityPrompt = false;
                        removeUtility = true;
                        break;
                }
            }

            // Add the Canvas Helper if we need it.
            if (IsUtilityValid &&
                canvas.isRootCanvas &&
                canvas.renderMode == RenderMode.WorldSpace &&
                canvas.worldCamera == MixedRealityToolkit.InputSystem.FocusProvider.UIRaycastCamera)
            {
                var helper = canvas.gameObject.EnsureComponent<CanvasUtility>();
                helper.Canvas = canvas;
            }

            // Reset the world canvas if we need to.
            if (IsUtilityValid &&
                canvas.isRootCanvas &&
                canvas.renderMode != RenderMode.WorldSpace &&
                canvas.worldCamera == MixedRealityToolkit.InputSystem.FocusProvider.UIRaycastCamera)
            {
                // Sets it back to MainCamera default.
                canvas.worldCamera = null;
                removeUtility = true;
            }

            var utility = canvas.GetComponent<CanvasUtility>();

            // Remove the helper if we don't need it.
            if (removeUtility || !IsUtilityValid)
            {
                if (utility != null)
                {
                    canvas.worldCamera = null;
                    EditorApplication.delayCall += () => DestroyImmediate(utility);
                }

                hasUtility = false;
            }
            else
            {
                if (canvas.renderMode == RenderMode.WorldSpace)
                {
                    Debug.Assert(utility != null);
                    hasUtility = true;
                }
            }
        }
    }
}