// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using HoloToolkit.Unity.InputModule;
using UnityEngine;

namespace HoloToolkit.Unity
{
    /// <summary>
    /// Lightweight game object placement
    /// </summary>
    public class TapToPlaceScene : MonoBehaviour, IPointerHandler
    {
        public float DistanceFromHead = 1.0f;

        public bool Placing = true;

        private Quaternion initialRotation;

        public void SetPlacing(bool placing)
        {
            Placing = placing;
        }

        private void OnEnable()
        {
            initialRotation = transform.rotation;
        }

        private void Update()
        {
            if (Placing)
            {
                var cameraTransform = CameraCache.Main.transform;
                var headPosition = cameraTransform.position;
                var forward = cameraTransform.forward;
                var scenePosition = headPosition + DistanceFromHead * forward;

                var facingRotation = cameraTransform.localRotation * initialRotation;
                //only yaw
                facingRotation.x = 0;
                facingRotation.z = 0;

                transform.position = scenePosition;
                transform.rotation = facingRotation;
            }

            if (Input.GetMouseButtonDown(0))
            {
                Placing = false;
            }
        }

        public void OnPointerUp(PointerEventData eventData) { }

        public void OnPointerDown(PointerEventData eventData) { }

        public void OnPointerClicked(PointerEventData eventData)
        {
            Placing = false;
        }
    }
}