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

            if (obj.state.source.kind == InteractionSourceKind.Controller)
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
            }
            return toReturn.Substring(0, Math.Max(0, toReturn.Length - 2));
        }
    }
}