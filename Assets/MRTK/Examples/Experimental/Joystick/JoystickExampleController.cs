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
        GameObject JoystickVisual = null;

        [SerializeField]
        GameObject GrabberVisual = null;

        [SerializeField]
        bool ShowGrabberVisual = true;

        [SerializeField]
        [Range(1, 20)]
        int ReboundSpeed = 5;

        [SerializeField]
        [Range(50, 300)]
        int SensitivityLeftRight = 100;

        [SerializeField]
        [Range(50, 300)]
        int SensitivityForwardBack = 150;

        [SerializeField]
        [Range(0.01f, 1f)]
        float MultiplierMove = 0.03f;

        [SerializeField]
        [Range(0.1f, 3.0f)]
        float MultiplierRotate = 1.1f;

        [SerializeField]
        [Range(0.1f, 3.0f)]
        float MultiplierScale = 0.4f;

        Vector3 startPosition;
        Vector3 joystickGrabberPosition;
        Vector3 joystickVisualRotation;
        const int joystickVisualMaxRotation = 80;
        bool isDragging = false;

        string JoystickMode = "Move";

        private void Start()
        {
            startPosition = GrabberVisual.transform.position;
            if(GrabberVisual != null)
            {
                GrabberVisual.GetComponent<MeshRenderer>().enabled = ShowGrabberVisual;
            }
        }
        void Update()
        {
            if (!isDragging)
            {
                // when dragging stops, move joystick back to idle
                if(GrabberVisual != null)
                {
                    GrabberVisual.transform.position = Vector3.Lerp(GrabberVisual.transform.position, startPosition, Time.deltaTime * ReboundSpeed);
                }
            }
            calculateJoystickRotation();
            applyJoystickValues();
        }
        void calculateJoystickRotation()
        {
            joystickGrabberPosition = GrabberVisual.transform.position - startPosition;
            // Left Right
            joystickVisualRotation.z = Mathf.Clamp(-joystickGrabberPosition.x * SensitivityLeftRight,-joystickVisualMaxRotation, joystickVisualMaxRotation);
            // Forward Back
            joystickVisualRotation.x = Mathf.Clamp(joystickGrabberPosition.z * SensitivityForwardBack,-joystickVisualMaxRotation, joystickVisualMaxRotation);
            if (JoystickVisual != null)
            {
                JoystickVisual.transform.localRotation = Quaternion.Euler(joystickVisualRotation);
            }
        }
        void applyJoystickValues()
        {
            if(ObjectToManipulate != null)
            {
                if (JoystickMode == "Move")
                {
                    ObjectToManipulate.transform.position += (joystickGrabberPosition * MultiplierMove);
                }
                else if (JoystickMode == "Rotate")
                {
                    Vector3 newRotation = ObjectToManipulate.transform.rotation.eulerAngles;
                    // only take the horizontal axis from the joystick
                    newRotation.y += (joystickGrabberPosition.x * MultiplierRotate);
                    newRotation.x = 0;
                    newRotation.z = 0;
                    ObjectToManipulate.transform.rotation = Quaternion.Euler(newRotation);
                }
                else if (JoystickMode == "Scale")
                {
                    Vector3 newScale = new Vector3(joystickGrabberPosition.x, joystickGrabberPosition.x, joystickGrabberPosition.x) * MultiplierScale;
                    //TODO: Clamp above Minimum_Scale
                    ObjectToManipulate.transform.localScale += newScale;
                }
            }
            if (DebugText != null)
            {
                DebugText.text = joystickGrabberPosition.ToString();
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
