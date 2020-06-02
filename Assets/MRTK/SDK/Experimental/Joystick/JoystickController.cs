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
    public class JoystickController : MonoBehaviour
    {
        public GameObject ObjectToManipulate;
        public TextMeshPro DebugText;

        [SerializeField]
        [Tooltip("The joystick mesh that gets rotated when this control is interacted with.")]
        GameObject JoystickVisual = null;

        [SerializeField]
        [Tooltip("The mesh + collider object that gets dragged and controls the joystick visual rotation.")]
        GameObject GrabberVisual = null;

        [SerializeField]
        [Tooltip("Toggles on / off the GrabberVisual's mesh renderer because it can be dragged away from the joystick visual, it kind of breaks the illusion of pushing / pulling a lever.")]
        bool ShowGrabberVisual = true;

        [SerializeField]
        [Tooltip("The speed at which the JoystickVisual and GrabberVisual move / rotate back to a neutral position.")]
        [Range(1, 20)]
        int ReboundSpeed = 5;

        [SerializeField]
        [Tooltip("How sensitive the joystick reacts to dragging left / right. Customize this value to get the right feel for your scenario.")]
        [Range(50, 300)]
        int SensitivityLeftRight = 100;

        [SerializeField]
        [Tooltip("How sensitive the joystick reacts to pushing / pulling. Customize this value to get the right feel for your scenario.")]
        [Range(50, 300)]
        int SensitivityForwardBack = 150;

        public enum JoystickMode { Move, Rotate, Scale }
        public JoystickMode mode = JoystickMode.Move;

        [SerializeField]
        [Tooltip("The distance multiplier for joystick input. Customize this value to get the right feel for your scenario.")]
        [Range(0.01f, 1f)]
        float MultiplierMove = 0.03f;

        [SerializeField]
        [Tooltip("The rotation multiplier for joystick input. Customize this value to get the right feel for your scenario.")]
        [Range(0.1f, 3.0f)]
        float MultiplierRotate = 1.1f;

        [SerializeField]
        [Tooltip("The scale multiplier for joystick input. Customize this value to get the right feel for your scenario.")]
        [Range(0.001f, 0.1f)]
        float MultiplierScale = 0.01f;

        Vector3 startPosition;
        Vector3 joystickGrabberPosition;
        Vector3 joystickVisualRotation;
        const int joystickVisualMaxRotation = 80;
        bool isDragging = false;


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
                if (mode == JoystickMode.Move)
                {
                    ObjectToManipulate.transform.position += (joystickGrabberPosition * MultiplierMove);
                    if (DebugText != null)
                    {
                        DebugText.text = ObjectToManipulate.transform.position.ToString();
                    }
                }
                else if (mode == JoystickMode.Rotate)
                {
                    Vector3 newRotation = ObjectToManipulate.transform.rotation.eulerAngles;
                    // only take the horizontal axis from the joystick
                    newRotation.y += (joystickGrabberPosition.x * MultiplierRotate);
                    newRotation.x = 0;
                    newRotation.z = 0;
                    ObjectToManipulate.transform.rotation = Quaternion.Euler(newRotation);
                    if (DebugText != null)
                    {
                        DebugText.text = ObjectToManipulate.transform.rotation.eulerAngles.ToString();
                    }
                }
                else if (mode == JoystickMode.Scale)
                {
                    //TODO: Clamp above zero
                    Vector3 newScale = new Vector3(joystickGrabberPosition.x, joystickGrabberPosition.x, joystickGrabberPosition.x) * MultiplierScale;
                    ObjectToManipulate.transform.localScale += newScale;
                    if (DebugText != null)
                    {
                        DebugText.text = ObjectToManipulate.transform.localScale.ToString();
                    }
                }
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
            mode = JoystickMode.Move;
        }
        public void JoystickMode_Rotate()
        {
            mode = JoystickMode.Rotate;
        }
        public void JoystickMode_Scale()
        {
            mode = JoystickMode.Scale;
        }
    }
}
