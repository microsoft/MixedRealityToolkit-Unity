// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace HoloToolkit.Unity.InputModule
{
    /// <summary>
    /// This script keeps track of the GameObjects for each button on the controller.
    /// It also keeps track of the animation Transforms in order to properly animate according to user input.
    /// </summary>
    public class ControllerInfo : MonoBehaviour
    {
        public GameObject home;
        public Transform homePressed;
        public Transform homeUnpressed;
        public GameObject menu;
        public Transform menuPressed;
        public Transform menuUnpressed;
        public GameObject grasp;
        public Transform graspPressed;
        public Transform graspUnpressed;
        public GameObject thumbstickPress;
        public Transform thumbstickPressed;
        public Transform thumbstickUnpressed;
        public GameObject thumbstickX;
        public Transform thumbstickXMin;
        public Transform thumbstickXMax;
        public GameObject thumbstickY;
        public Transform thumbstickYMin;
        public Transform thumbstickYMax;
        public GameObject select;
        public Transform selectPressed;
        public Transform selectUnpressed;
        public GameObject touchpadPress;
        public Transform touchpadPressed;
        public Transform touchpadUnpressed;
        public GameObject touchpadPressX;
        public Transform touchpadPressXMin;
        public Transform touchpadPressXMax;
        public GameObject touchpadPressY;
        public Transform touchpadPressYMin;
        public Transform touchpadPressYMax;
        public GameObject touchpadTouchX;
        public Transform touchpadTouchXMin;
        public Transform touchpadTouchXMax;
        public GameObject touchpadTouchY;
        public Transform touchpadTouchYMin;
        public Transform touchpadTouchYMax;
        public GameObject touchpadTouchVisualizer;

        // These bools and doubles are used to determine if a button's state has changed.
        public bool wasGrasped;
        public bool wasMenuPressed;
        public bool wasThumbstickPressed;
        public bool wasTouchpadPressed;
        public bool wasTouchpadTouched;
        public double lastThumbstickX;
        public double lastThumbstickY;
        public double lastTouchpadX;
        public double lastTouchpadY;
        public double lastSelectPressedValue;

        public void LoadInfo(Transform[] childTransforms, GameObject touchpadTouchedOverride, Shader shader)
        {
            foreach (Transform child in childTransforms)
            {
                switch (child.name.ToLower())
                {
                    case "pressed":
                        switch (child.parent.name.ToLower())
                        {
                            case "home":
                                homePressed = child;
                                break;
                            case "menu":
                                menuPressed = child;
                                break;
                            case "grasp":
                                graspPressed = child;
                                break;
                            case "select":
                                selectPressed = child;
                                break;
                            case "thumbstick_press":
                                thumbstickPressed = child;
                                break;
                            case "touchpad_press":
                                touchpadPressed = child;
                                break;
                        }
                        break;
                    case "unpressed":
                        switch (child.parent.name.ToLower())
                        {
                            case "home":
                                homeUnpressed = child;
                                break;
                            case "menu":
                                menuUnpressed = child;
                                break;
                            case "grasp":
                                graspUnpressed = child;
                                break;
                            case "select":
                                selectUnpressed = child;
                                break;
                            case "thumbstick_press":
                                thumbstickUnpressed = child;
                                break;
                            case "touchpad_press":
                                touchpadUnpressed = child;
                                break;
                        }
                        break;
                    case "value":
                        switch (child.parent.name.ToLower())
                        {
                            case "home":
                                home = child.gameObject;
                                break;
                            case "menu":
                                menu = child.gameObject;
                                break;
                            case "grasp":
                                grasp = child.gameObject;
                                break;
                            case "select":
                                select = child.gameObject;
                                break;
                            case "thumbstick_press":
                                thumbstickPress = child.gameObject;
                                break;
                            case "thumbstick_x":
                                thumbstickX = child.gameObject;
                                break;
                            case "thumbstick_y":
                                thumbstickY = child.gameObject;
                                break;
                            case "touchpad_press":
                                touchpadPress = child.gameObject;
                                break;
                            case "touchpad_press_x":
                                touchpadPressX = child.gameObject;
                                break;
                            case "touchpad_press_y":
                                touchpadPressY = child.gameObject;
                                break;
                            case "touchpad_touch_x":
                                touchpadTouchX = child.gameObject;
                                break;
                            case "touchpad_touch_y":
                                touchpadTouchY = child.gameObject;
                                break;
                        }
                        break;
                    case "min":
                        switch (child.parent.name.ToLower())
                        {
                            case "thumbstick_x":
                                thumbstickXMin = child;
                                break;
                            case "thumbstick_y":
                                thumbstickYMin = child;
                                break;
                            case "touchpad_press_x":
                                touchpadPressXMin = child;
                                break;
                            case "touchpad_press_y":
                                touchpadPressYMin = child;
                                break;
                            case "touchpad_touch_x":
                                touchpadTouchXMin = child;
                                break;
                            case "touchpad_touch_y":
                                touchpadTouchYMin = child;
                                break;
                        }
                        break;
                    case "max":
                        switch (child.parent.name.ToLower())
                        {
                            case "thumbstick_x":
                                thumbstickXMax = child;
                                break;
                            case "thumbstick_y":
                                thumbstickYMax = child;
                                break;
                            case "touchpad_press_x":
                                touchpadPressXMax = child;
                                break;
                            case "touchpad_press_y":
                                touchpadPressYMax = child;
                                break;
                            case "touchpad_touch_x":
                                touchpadTouchXMax = child;
                                break;
                            case "touchpad_touch_y":
                                touchpadTouchYMax = child;
                                break;
                        }
                        break;
                    case "touch":
                        GameObject touchVisualizer;
                        if (touchpadTouchedOverride != null)
                        {
                            touchVisualizer = Instantiate(touchpadTouchedOverride);
                        }
                        else
                        {
                            touchVisualizer = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                            touchVisualizer.transform.localScale = new Vector3(0.0025f, 0.0025f, 0.0025f);
                            touchVisualizer.GetComponent<Renderer>().material.shader = shader;
                        }
                        Destroy(touchVisualizer.GetComponent<Collider>());
                        touchVisualizer.transform.parent = child;
                        touchVisualizer.transform.localPosition = Vector3.zero;
                        touchVisualizer.transform.localRotation = Quaternion.identity;
                        touchVisualizer.SetActive(false);
                        touchpadTouchVisualizer = touchVisualizer;
                        break;
                }
            }
        }
    }

}