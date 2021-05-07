// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Tests
{
    // For InputSystemGlobalListener
#pragma warning disable 0618
    [AddComponentMenu("Scripts/MRTK/Tests/TestInputFocusListener")]
    internal class TestInputFocusListener : MonoBehaviour, IMixedRealityFocusHandler, IMixedRealityPointerHandler, IMixedRealitySpeechHandler
    {
        // Parameters, which are set by child classes
        protected bool useObjectBasedRegistration = false;
        protected bool registerSpeechOnly = false;

        // Values changed by class to validate event receiving
        public int pointerDownCount = 0;
        public int pointerDraggedCount = 0;
        public int pointerUpCount = 0;
        public int pointerClickedCount = 0;

        public int focusGainedCount = 0;
        public int focusLostCount = 0;
        public List<string> speechCommandsReceived = new List<string>();

        protected void Start()
        {
            pointerDownCount = 0;
            pointerDraggedCount = 0;
            pointerUpCount = 0;
            pointerClickedCount = 0;

            focusGainedCount = 0;
            focusLostCount = 0;
            speechCommandsReceived = new List<string>();
        }

        public void OnPointerDown(MixedRealityPointerEventData eventData)
        {
            pointerDownCount++;
        }

        public void OnPointerDragged(MixedRealityPointerEventData eventData)
        {
            pointerDraggedCount++;
        }

        public void OnPointerUp(MixedRealityPointerEventData eventData)
        {
            pointerUpCount++;
        }

        public virtual void OnPointerClicked(MixedRealityPointerEventData eventData)
        {
            pointerClickedCount++;
        }

        public void OnSpeechKeywordRecognized(SpeechEventData eventData)
        {
            speechCommandsReceived.Add(eventData.Command.Keyword);
        }

        public void OnFocusEnter(FocusEventData eventData)
        {
            focusGainedCount++;
        }

        public void OnFocusExit(FocusEventData eventData)
        {
            focusLostCount++;
        }
    }
#pragma warning restore 0618
}