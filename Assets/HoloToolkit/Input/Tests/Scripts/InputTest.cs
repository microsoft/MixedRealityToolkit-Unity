// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using UnityEngine.EventSystems;

namespace HoloToolkit.Unity.InputModule.Tests
{
    /// <summary>
    /// Test behaviour that simply prints out a message very time a supported event is received from the input module.
    /// This is used to make sure that the input module routes events appropriately to game objects.
    /// </summary>
    public class InputTest : MonoBehaviour, IInputHandler, IFocusable, ISourceStateHandler
    {
        public void OnPointerEnter(PointerEventData eventData)
        {
            
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            Debug.Log("OnPointerExit");
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Debug.LogFormat("OnPointerClick {0} taps", eventData.clickCount);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            Debug.Log("OnPointerDown");
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            Debug.Log("OnPointerUp");
        }

        public void OnInputUp(InputEventData eventData)
        {
            Debug.LogFormat("OnInputUp\r\nSource: {0}  SourceId: {1}", eventData.InputSource, eventData.SourceId);
        }

        public void OnInputDown(InputEventData eventData)
        {
            Debug.LogFormat("OnInputDown\r\nSource: {0}  SourceId: {1}", eventData.InputSource, eventData.SourceId);
        }

        public void OnInputClicked(InputEventData eventData)
        {
            Debug.LogFormat("OnInputClicked\r\nSource: {0}  SourceId: {1}", eventData.InputSource, eventData.SourceId);
        }

        public void OnFocusEnter()
        {
            Debug.Log("OnFocusEnter");
        }

        public void OnFocusExit()
        {
            Debug.Log("OnFocusExit");
        }

        public void OnSourceDetected(SourceStateEventData eventData)
        {
            Debug.LogFormat("OnSourceDetected\r\nSource: {0}  SourceId: {1}", eventData.InputSource, eventData.SourceId);
        }

        public void OnSourceLost(SourceStateEventData eventData)
        {
            Debug.LogFormat("OnSourceLost\r\nSource: {0}  SourceId: {1}", eventData.InputSource, eventData.SourceId);
        }
    }
}