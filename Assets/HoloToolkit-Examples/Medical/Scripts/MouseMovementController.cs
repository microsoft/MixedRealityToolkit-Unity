// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#define INPUT_RDP_WORKAROUND
using UnityEngine;

namespace HoloToolkit.Unity
{
    public class MouseMovementController : MonoBehaviour
    {
        public Vector2 PanSpeed = new Vector2(60.0f, 60.0f);
        public Vector2 RotateSpeed = new Vector2(120.0f, 120.0f);
        public float ZoomSpeed = 450.0f;
        public Transform TargetTransform;

#if INPUT_RDP_WORKAROUND
        private Vector2 mousePositionLast;
#endif
        void Update()
        {
            var sensitivityScale = Input.GetKey(KeyCode.LeftShift) ? 0.1f : 1.0f;
            var finalScale = Time.deltaTime * sensitivityScale;

#if INPUT_RDP_WORKAROUND
            var mousePosition = Input.mousePosition;
            var axisDelta = new Vector3(mousePosition.x - mousePositionLast.x,
                                        mousePosition.y - mousePositionLast.y,
                                        Input.GetAxis("Mouse ScrollWheel"));

            axisDelta *= 0.1f; //default Unity mouse sensitivity
            mousePositionLast = mousePosition;
#else
            var axisDelta = new Vector3(Input.GetAxis("Mouse X"),
                                        Input.GetAxis("Mouse Y"),
                                        Input.GetAxis("Mouse ScrollWheel"));
#endif

            this.TargetTransform.Translate(0.0f, 0.0f, Input.GetAxis("Mouse ScrollWheel") * this.ZoomSpeed * finalScale);

            //mouse middle button - pan
            if (Input.GetMouseButton(2))
            {
                this.TargetTransform.Translate(-axisDelta.x * this.PanSpeed.x * finalScale,
                                               -axisDelta.y * this.PanSpeed.y * finalScale,
                                                0.0f);
            }

            //mouse right button - rotate
            if (Input.GetMouseButton(1))
            {
                this.TargetTransform.eulerAngles += new Vector3(-axisDelta.y * RotateSpeed.y * finalScale,
                                                                 axisDelta.x * RotateSpeed.x * finalScale,
                                                                 0.0f);
            }
        }
    }
}