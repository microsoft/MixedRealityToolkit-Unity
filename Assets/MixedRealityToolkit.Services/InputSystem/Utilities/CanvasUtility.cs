// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using UnityEngine.EventSystems;

namespace Microsoft.MixedReality.Toolkit.Input.Utilities
{
    /// <summary>
    /// Helper class for setting up canvases for use in the MRTK.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Canvas))]
    public class CanvasUtility : MonoBehaviour
    {
        private IMixedRealityInputSystem inputSystem = null;

        /// <summary>
        /// The active instance of the input system.
        /// </summary>
        private IMixedRealityInputSystem InputSystem => inputSystem ?? (inputSystem = CoreServices.InputSystem);

        private void Start()
        {
            Canvas canvas = GetComponent<Canvas>();
            Debug.Assert(canvas != null);

            if (canvas.worldCamera == null)
            {
                Debug.Assert(InputSystem?.FocusProvider?.UIRaycastCamera != null, this);
                canvas.worldCamera = InputSystem?.FocusProvider?.UIRaycastCamera;

                if (EventSystem.current == null)
                {
                    Debug.LogError("No EventSystem detected. UI events will not be propagated to Unity UI.");
                }
            }
            else
            {
                Debug.LogError("World Space Canvas should have no camera set to work properly with Mixed Reality Toolkit. At runtime, they'll get their camera set automatically.");
            }
        }
    }
}
