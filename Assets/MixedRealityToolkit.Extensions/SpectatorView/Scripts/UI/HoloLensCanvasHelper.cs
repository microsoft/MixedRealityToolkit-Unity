// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView.UI
{
    /// <summary>
    /// Helper class for placing a Unity canvas relative to a HoloLens user
    /// </summary>
    public class HoloLensCanvasHelper : MonoBehaviour
    {
        /// <summary>
        /// Check if you want the Unity Canvas to be placed outward at a specific distance from the main camera.
        /// </summary>
        [Tooltip("Check if you want the Unity Canvas to be placed outward at a specific distance from the main camera.")]
        [SerializeField]
        protected bool _bindToMainCamera = false;

        /// <summary>
        /// If BindToMainCamera is set to true, the canvas will be placed at this distance (in meters) in front of the user.
        /// </summary>
        [Tooltip("If BindToMainCamera is set to true, the canvas will be placed at this distance (in meters) in front of the user.")]
        [SerializeField]
        protected float _zDistance = 1.0f;

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
