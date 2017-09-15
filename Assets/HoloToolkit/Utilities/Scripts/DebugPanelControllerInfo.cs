// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.WSA.Input;

namespace HoloToolkit.Unity
{
    public class DebugPanelControllerInfo : MonoBehaviour
    {
        private class ControllerState
        {
            public InteractionSourceHandedness Handedness;
            public Vector3 PointerPosition;
            public Quaternion PointerRotation;
            public Vector3 GripPosition;
            public Quaternion GripRotation;
            public bool Grasped;
            public bool MenuPressed;
            public bool SelectPressed;
            public float SelectPressedAmount;
            public bool ThumbstickPressed;
            public Vector2 ThumbstickPosition;
            public bool TouchpadPressed;
            public bool TouchpadTouched;
            public Vector2 TouchpadPosition;
        }

        // Text display label game objects
        [SerializeField]
        public GameObject leftInfoTextPointerPosition;
        public GameObject leftInfoTextPointerRotation;
        public GameObject leftInfoTextGripPosition;
        public GameObject leftInfoTextGripRotation;
        public GameObject leftInfoTextGripGrasped;
        public GameObject leftInfoTextMenuPressed;
        public GameObject leftInfoTextTriggerPressed;
        public GameObject leftInfoTextTriggerPressedAmount;
        public GameObject leftInfoTextThumbstickPressed;
        public GameObject leftInfoTextThumbstickPosition;
        public GameObject leftInfoTextTouchpadPressed;
        public GameObject leftInfoTextTouchpadTouched;
        public GameObject leftInfoTextTouchpadPosition;
        public GameObject rightInfoTextPointerPosition;
        public GameObject rightInfoTextPointerRotation;
        public GameObject rightInfoTextGripPosition;
        public GameObject rightInfoTextGripRotation;
        public GameObject rightInfoTextGripGrasped;
        public GameObject rightInfoTextMenuPressed;
        public GameObject rightInfoTextTriggerPressed;
        public GameObject rightInfoTextTriggerPressedAmount;
        public GameObject rightInfoTextThumbstickPressed;
        public GameObject rightInfoTextThumbstickPosition;
        public GameObject rightInfoTextTouchpadPressed;
        public GameObject rightInfoTextTouchpadTouched;
        public GameObject rightInfoTextTouchpadPosition;

        private Dictionary<uint, ControllerState> controllers;

        private void Awake()
        {
            controllers = new Dictionary<uint, ControllerState>();

#if UNITY_WSA
            InteractionManager.InteractionSourceDetected += InteractionManager_InteractionSourceDetected;

            InteractionManager.InteractionSourceLost += InteractionManager_InteractionSourceLost;
            InteractionManager.InteractionSourceUpdated += InteractionManager_InteractionSourceUpdated;
#endif
        }

        private void Start()
        {
            if (DebugPanel.Instance != null)
            {
                DebugPanel.Instance.RegisterExternalLogCallback(GetControllerInfo);
            }
        }

        private void InteractionManager_InteractionSourceDetected(InteractionSourceDetectedEventArgs obj)
        {
            Debug.LogFormat("{0} {1} Detected", obj.state.source.handedness, obj.state.source.kind);

            if (obj.state.source.kind == InteractionSourceKind.Controller && !controllers.ContainsKey(obj.state.source.id))
            {
                controllers.Add(obj.state.source.id, new ControllerState { Handedness = obj.state.source.handedness });
            }
        }

        private void InteractionManager_InteractionSourceLost(InteractionSourceLostEventArgs obj)
        {
            Debug.LogFormat("{0} {1} Lost", obj.state.source.handedness, obj.state.source.kind);

            controllers.Remove(obj.state.source.id);
        }

        private void InteractionManager_InteractionSourceUpdated(InteractionSourceUpdatedEventArgs obj)
        {
            ControllerState controllerState;
            if (controllers.TryGetValue(obj.state.source.id, out controllerState))
            {
                obj.state.sourcePose.TryGetPosition(out controllerState.PointerPosition, InteractionSourceNode.Pointer);
                obj.state.sourcePose.TryGetRotation(out controllerState.PointerRotation, InteractionSourceNode.Pointer);
                obj.state.sourcePose.TryGetPosition(out controllerState.GripPosition, InteractionSourceNode.Grip);
                obj.state.sourcePose.TryGetRotation(out controllerState.GripRotation, InteractionSourceNode.Grip);

                controllerState.Grasped = obj.state.grasped;
                controllerState.MenuPressed = obj.state.menuPressed;
                controllerState.SelectPressed = obj.state.selectPressed;
                controllerState.SelectPressedAmount = obj.state.selectPressedAmount;
                controllerState.ThumbstickPressed = obj.state.thumbstickPressed;
                controllerState.ThumbstickPosition = obj.state.thumbstickPosition;
                controllerState.TouchpadPressed = obj.state.touchpadPressed;
                controllerState.TouchpadTouched = obj.state.touchpadTouched;
                controllerState.TouchpadPosition = obj.state.touchpadPosition;
            }
        }

        private string GetControllerInfo()
        {
            string toReturn = "";
            foreach (ControllerState controllerState in controllers.Values)
            {
                // Debug message
                toReturn += string.Format("Hand: {0}\nPointer: Position: {1} Rotation: {2}\n" +
                                          "Grip: Position: {3} Rotation: {4}\nGrasped: {5} " +
                                          "MenuPressed: {6}\nSelect: Pressed: {7} PressedAmount: {8}\n" +
                                          "Thumbstick: Pressed: {9} Position: {10}\nTouchpad: Pressed: {11} " +
                                          "Touched: {12} Position: {13}\n\n",
                                          controllerState.Handedness, controllerState.PointerPosition, controllerState.PointerRotation.eulerAngles,
                                          controllerState.GripPosition, controllerState.GripRotation.eulerAngles, controllerState.Grasped,
                                          controllerState.MenuPressed, controllerState.SelectPressed, controllerState.SelectPressedAmount,
                                          controllerState.ThumbstickPressed, controllerState.ThumbstickPosition, controllerState.TouchpadPressed,
                                          controllerState.TouchpadTouched, controllerState.TouchpadPosition);

                // Text label display
                if(controllerState.Handedness.ToString().Equals("Left"))
                {
                    leftInfoTextPointerPosition.GetComponent<TextMesh>().text = controllerState.Handedness.ToString();
                    leftInfoTextPointerRotation.GetComponent<TextMesh>().text = controllerState.PointerRotation.ToString();
                    leftInfoTextGripPosition.GetComponent<TextMesh>().text = controllerState.GripPosition.ToString();
                    leftInfoTextGripRotation.GetComponent<TextMesh>().text = controllerState.GripRotation.ToString();
                    leftInfoTextGripGrasped.GetComponent<TextMesh>().text = controllerState.Grasped.ToString();
                    leftInfoTextMenuPressed.GetComponent<TextMesh>().text = controllerState.MenuPressed.ToString();
                    leftInfoTextTriggerPressed.GetComponent<TextMesh>().text = controllerState.SelectPressed.ToString();
                    leftInfoTextTriggerPressedAmount.GetComponent<TextMesh>().text = controllerState.SelectPressedAmount.ToString();
                    leftInfoTextThumbstickPressed.GetComponent<TextMesh>().text = controllerState.ThumbstickPressed.ToString();
                    leftInfoTextThumbstickPosition.GetComponent<TextMesh>().text = controllerState.ThumbstickPosition.ToString();
                    leftInfoTextTouchpadPressed.GetComponent<TextMesh>().text = controllerState.TouchpadPressed.ToString();
                    leftInfoTextTouchpadTouched.GetComponent<TextMesh>().text = controllerState.TouchpadTouched.ToString();
                    leftInfoTextTouchpadPosition.GetComponent<TextMesh>().text = controllerState.TouchpadPosition.ToString();
                }
                else if (controllerState.Handedness.ToString().Equals("Right"))
                {
                    rightInfoTextPointerPosition.GetComponent<TextMesh>().text = controllerState.PointerPosition.ToString();
                    rightInfoTextPointerRotation.GetComponent<TextMesh>().text = controllerState.PointerRotation.ToString();
                    rightInfoTextGripPosition.GetComponent<TextMesh>().text = controllerState.GripPosition.ToString();
                    rightInfoTextGripRotation.GetComponent<TextMesh>().text = controllerState.GripRotation.ToString();
                    rightInfoTextGripGrasped.GetComponent<TextMesh>().text = controllerState.Grasped.ToString();
                    rightInfoTextMenuPressed.GetComponent<TextMesh>().text = controllerState.MenuPressed.ToString();
                    rightInfoTextTriggerPressed.GetComponent<TextMesh>().text = controllerState.SelectPressed.ToString();
                    rightInfoTextTriggerPressedAmount.GetComponent<TextMesh>().text = controllerState.SelectPressedAmount.ToString();
                    rightInfoTextThumbstickPressed.GetComponent<TextMesh>().text = controllerState.ThumbstickPressed.ToString();
                    rightInfoTextThumbstickPosition.GetComponent<TextMesh>().text = controllerState.ThumbstickPosition.ToString();
                    rightInfoTextTouchpadPressed.GetComponent<TextMesh>().text = controllerState.TouchpadPressed.ToString();
                    rightInfoTextTouchpadTouched.GetComponent<TextMesh>().text = controllerState.TouchpadTouched.ToString();
                    rightInfoTextTouchpadPosition.GetComponent<TextMesh>().text = controllerState.TouchpadPosition.ToString();
                }
            }
            return toReturn.Substring(0, Math.Max(0, toReturn.Length - 2));
        }
    }
}