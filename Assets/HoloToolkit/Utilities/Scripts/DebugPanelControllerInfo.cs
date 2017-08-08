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
            public InteractionSourceHandType HandType;
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
            InteractionManager.OnSourceDetected += InteractionManager_OnSourceDetected;

            InteractionManager.OnSourceLost += InteractionManager_OnSourceLost;
            InteractionManager.OnSourceUpdated += InteractionManager_OnSourceUpdated;
#endif
        }

        private void Start()
        {
            if (DebugPanel.Instance != null)
            {
                DebugPanel.Instance.RegisterExternalLogCallback(GetControllerInfo);
            }
        }

        private void InteractionManager_OnSourceDetected(SourceDetectedEventArgs obj)
        {
            Debug.LogFormat("{0} {1} Detected", obj.state.handType, obj.state.source.kind);

            if (obj.state.source.kind == InteractionSourceKind.Controller)
            {
                controllers.Add(obj.state.source.id, new ControllerState { HandType = obj.state.handType });
            }
        }

        private void InteractionManager_OnSourceLost(SourceLostEventArgs obj)
        {
            Debug.LogFormat("{0} {1} Lost", obj.state.handType, obj.state.source.kind);

            controllers.Remove(obj.state.source.id);
        }

        private void InteractionManager_OnSourceUpdated(SourceUpdatedEventArgs obj)
        {
            ControllerState controllerState;
            if (controllers.TryGetValue(obj.state.source.id, out controllerState))
            {
                obj.state.properties.location.pointer.TryGetPosition(out controllerState.PointerPosition);
                obj.state.properties.location.pointer.TryGetRotation(out controllerState.PointerRotation);
                obj.state.properties.location.grip.TryGetPosition(out controllerState.GripPosition);
                obj.state.properties.location.grip.TryGetRotation(out controllerState.GripRotation);

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
                toReturn += string.Format("Hand: {0}\nPointerPosition: {1}\nPointerRotation: {2}\n" +
                                          "GripPosition: {3}\nGripRotation: {4}\nGrasped: {5}\n" +
                                          "MenuPressed: {6}\nSelectPressed: {7}\nSelectPressedAmount: {8}\n" +
                                          "ThumbstickPressed: {9}\nThumbstickPosition: {10}\nTouchpadPressed: {11}\n" +
                                          "TouchpadTouched: {12}\nTouchpadPosition: {13}\n\n",
                                          controllerState.HandType, controllerState.PointerPosition, controllerState.PointerRotation,
                                          controllerState.GripPosition, controllerState.GripRotation, controllerState.Grasped,
                                          controllerState.MenuPressed, controllerState.SelectPressed, controllerState.SelectPressedAmount,
                                          controllerState.ThumbstickPressed, controllerState.ThumbstickPosition, controllerState.TouchpadPressed,
                                          controllerState.TouchpadTouched, controllerState.TouchpadPosition);
            }
            return toReturn.Substring(0, Math.Max(0, toReturn.Length - 2));
        }
    }
}