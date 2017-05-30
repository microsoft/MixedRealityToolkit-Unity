// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;

namespace HoloToolkit.Unity.InputModule.Tests
{
    /// <summary>
    /// This class implements IInputClickHandler to handle the tap gesture.
    /// It increases the scale of the object when tapped.
    /// </summary>
    public class TapResponder : MonoBehaviour, IInputClickHandler, ISourceStateHandler
    {
        GameObject controller;

        private void Start()
        {
            controller = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            controller.transform.localScale = Vector3.one * 0.02f;
            controller.SetActive(false);
        }

        public void OnInputClicked(InputClickedEventData eventData)
        {
            // Increase the scale of the object just as a response.
            gameObject.transform.localScale += 0.05f * gameObject.transform.localScale;

            SupportedInputInfo info = eventData.InputSource.GetSupportedInputInfo(eventData.SourceId);

            if (info == SupportedInputInfo.Position)
            {
                Vector3 position;
                if (eventData.InputSource.TryGetPosition(eventData.SourceId, out position))
                {
                    controller.transform.localPosition = position;
                    controller.SetActive(true);
                }
            }

            eventData.Use(); // Mark the event as used, so it doesn't fall through to other handlers.
        }

        public void OnSourceDetected(SourceStateEventData eventData)
        {
            Vector3 position;
            if (eventData.InputSource.TryGetPosition(eventData.SourceId, out position))
            {
                controller.transform.localPosition = position;
                controller.SetActive(true);
            }

            eventData.Use(); // Mark the event as used, so it doesn't fall through to other handlers.
        }

        public void OnSourceLost(SourceStateEventData eventData)
        {
            controller.SetActive(false);

            eventData.Use(); // Mark the event as used, so it doesn't fall through to other handlers.
        }
    }
}