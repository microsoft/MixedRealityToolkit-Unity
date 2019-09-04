// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Microsoft.MixedReality.Toolkit.Input.Utilities
{
    /// <summary>
    /// Helper class for setting up canvases for use in the MRTK.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Canvas))]
    public class CanvasUtility : MonoBehaviour, IMixedRealityPointerHandler
    {
        private Vector3 previousHitPosition;
        private RectTransform rectTransform;

        public void OnPointerClicked(MixedRealityPointerEventData eventData) {}

        public void OnPointerDown(MixedRealityPointerEventData eventData)
        {
            previousHitPosition = eventData.Pointer.Result.Details.Point;
        }

        public void OnPointerDragged(MixedRealityPointerEventData eventData)
        {
            if (canvas.)
        }

        public void OnPointerUp(MixedRealityPointerEventData eventData)
        {
        }

        private void Start()
        {
            rectTransform = GetComponent<RectTransform>();
            Canvas canvas = GetComponent<Canvas>();
            Debug.Assert(canvas != null);

            if (canvas.worldCamera == null)
            {
                Debug.Assert(CoreServices.InputSystem?.FocusProvider?.UIRaycastCamera != null, this);
                canvas.worldCamera = CoreServices.InputSystem?.FocusProvider?.UIRaycastCamera;

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
