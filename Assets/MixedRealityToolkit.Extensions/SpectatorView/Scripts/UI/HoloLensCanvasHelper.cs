// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.SpectatorView.UI
{
    public class HoloLensCanvasHelper : MonoBehaviour
    {
        [SerializeField] bool _bindToMainCamera = false;
        [SerializeField] float _zDistance = 1.0f;

        private void OnEnable()
        {
            var canvas = gameObject.GetComponentInChildren<Canvas>();

#if UNITY_WSA
            canvas.renderMode = RenderMode.WorldSpace;
            if (_bindToMainCamera)
            {
                // Set the canvas in front of the camera
                canvas.transform.position = new Vector3(0, 0, _zDistance);
                canvas.transform.rotation = Quaternion.identity;
                canvas.transform.parent = Camera.main.transform;
            }
            else
            {
                // Content thats interactable likely shouldn't be bound to a static position relative to the camera
                canvas.transform.position = Camera.main.transform.position + Camera.main.transform.forward * _zDistance;
                canvas.transform.rotation = Camera.main.transform.rotation;
            }
#else
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
#endif
        }
    }
}
