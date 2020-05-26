// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Microsoft.MixedReality.Toolkit.Experimental.Joystick
{
    /// <summary>
    /// Example script to demonstrate joystick control in sample scene
    /// </summary>
    public class JoystickExampleController : MonoBehaviour
    {
        public GameObject ObjectToManipulate;
        public TextMeshPro DebugText;

        [SerializeField]
        GameObject Grabber = null;

        [SerializeField]
        GameObject JoystickVisual = null;

        [SerializeField]
        float Rebound_Speed = 3;

        [SerializeField]
        float Sensitivity_LeftRight = 100;

        [SerializeField]
        float Sensitivity_ForwardBack = 150;

        [SerializeField]
        float Multiplier_Move = 0.03f;

        float Multiplier_Rotate = 1.1f;
        float Multiplier_Scale = 0.4f;

        Vector3 startPosition;
        Vector3 draggingObjectPosition;
        Vector3 joystickRotation;
        bool isDragging = false;

        string JoystickMode = "Move";

        private void Start()
        {
            startPosition = Grabber.transform.position;
        }
        void Update()
        {
            if (!isDragging)
            {
                // when dragging stops, move joystick back to idle
                if(Grabber != null)
                {
                    Grabber.transform.position = Vector3.Lerp(Grabber.transform.position, startPosition, Time.deltaTime * Rebound_Speed);
                }
            }
            calculateJoystickRotation();
            applyJoystickValues();
        }
        void calculateJoystickRotation()
        {
            draggingObjectPosition = Grabber.transform.position - startPosition;
            // Left Right
            joystickRotation.z = -draggingObjectPosition.x * Sensitivity_LeftRight;
            // Forward Back
            joystickRotation.x = draggingObjectPosition.z * Sensitivity_ForwardBack;
            if(JoystickVisual != null)
            {
                JoystickVisual.transform.localRotation = Quaternion.Euler(joystickRotation);
            }
        }
        void applyJoystickValues()
        {
            if(ObjectToManipulate != null)
            {
                if (JoystickMode == "Move")
                {
                    ObjectToManipulate.transform.position += (draggingObjectPosition * Multiplier_Move);
                }
                else if (JoystickMode == "Rotate")
                {
                    Vector3 newRotation = ObjectToManipulate.transform.rotation.eulerAngles;
                    // only take the horizontal axis from the joystick
                    newRotation.y += (draggingObjectPosition.x * Multiplier_Rotate);
                    newRotation.x = 0;
                    newRotation.z = 0;
                    ObjectToManipulate.transform.rotation = Quaternion.Euler(newRotation);
                }
                else if (JoystickMode == "Scale")
                {
                    Vector3 newScale = new Vector3(draggingObjectPosition.x, draggingObjectPosition.x, draggingObjectPosition.x) * Multiplier_Scale;
                    //TODO: Clamp above Minimum_Scale
                    ObjectToManipulate.transform.localScale += newScale;
                }
            }
            if(DebugText != null)
            {
                DebugText.text = draggingObjectPosition.ToString();
            }
        }
        public void StartDrag()
        {
            isDragging = true;
        }
        public void StopDrag()
        {
            isDragging = false;
        }
        public void JoystickMode_Move()
        {
            JoystickMode = "Move";
        }
        public void JoystickMode_Rotate()
        {
            JoystickMode = "Rotate";
        }
        public void JoystickMode_Scale()
        {
            JoystickMode = "Scale";
        }
    }
}
