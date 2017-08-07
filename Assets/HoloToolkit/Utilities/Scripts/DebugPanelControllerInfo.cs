// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

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
            public Vector3 Position;
            public Quaternion Rotation;
        }

        private Dictionary<uint, ControllerState> controllers;

        private void Awake()
        {
            controllers = new Dictionary<uint, ControllerState>();

#if UNITY_WSA
            InteractionManager.OnSourceDetected += InteractionManager_OnSourceDetected;

            InteractionManager.OnSourceLost += InteractionManager_OnSourceLost;
            InteractionManager.OnSourceUpdated += InteractionManager_OnSourceUpdated;

            InteractionManager.OnSourcePressed += InteractionManager_OnSourcePressed;
            InteractionManager.OnSourceReleased += InteractionManager_OnSourceReleased;
#endif
        }

        private void Start()
        {
            if (DebugPanel.Instance != null)
            {
                DebugPanel.Instance.RegisterExternalLogCallback(GetControllerInfo);
            }
        }

        private void OnDestroy()
        {
            if (DebugPanel.Instance != null)
            {
                DebugPanel.Instance.UnregisterExternalLogCallback(GetControllerInfo);
            }
        }

        private void InteractionManager_OnSourceDetected(SourceDetectedEventArgs obj)
        {
            Debug.LogFormat("{0} {1} Detected", obj.state.handType, obj.state.source.kind);

            if (obj.state.source.kind == InteractionSourceKind.Controller)
            {
                controllers.Add(obj.state.source.id, new ControllerState { HandType = obj.state.handType, Position = Vector3.zero, Rotation = Quaternion.identity });
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
                obj.state.properties.location.pointer.TryGetPosition(out controllerState.Position);
                obj.state.properties.location.pointer.TryGetRotation(out controllerState.Rotation);
            }
        }

        private void InteractionManager_OnSourcePressed(SourcePressedEventArgs obj)
        {
            Debug.LogFormat("{0} {1} {2} Pressed", obj.state.handType, obj.state.source.kind, obj.pressType);
        }

        private void InteractionManager_OnSourceReleased(SourceReleasedEventArgs obj)
        {
            Debug.LogFormat("{0} {1} {2} Released", obj.state.handType, obj.state.source.kind, obj.pressType);
        }

        private string GetControllerInfo()
        {
            string toReturn = "";
            foreach (ControllerState controllerState in controllers.Values)
            {
                toReturn += string.Format("Hand: {0} Position: {1} Rotation: {2}\n", controllerState.HandType, controllerState.Position, controllerState.Rotation);
            }
            return toReturn;
        }
    }
}