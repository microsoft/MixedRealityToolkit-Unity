// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;

namespace HoloToolkit.Unity.InputModule.Tests
{
    public class SphereKeywords : MonoBehaviour, ISpeechHandler
    {
        public void ChangeColor(string color)
        {
            Material material = GetComponent<Renderer>().material;

            switch (color.ToLower())
            {
                case "red":
                    material.color = Color.red;
                    break;
                case "blue":
                    material.color = Color.blue;
                    break;
                case "green":
                    material.color = Color.green;
                    break;
            }

            Resources.UnloadAsset(material);
        }

        public void OnSpeechKeywordRecognized(SpeechKeywordRecognizedEventData eventData)
        {
            ChangeColor(eventData.RecognizedText);
        }
    }
}
