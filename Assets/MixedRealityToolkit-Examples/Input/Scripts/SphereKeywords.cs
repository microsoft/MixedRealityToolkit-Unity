// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using MixedRealityToolkit.InputModule.EventData;
using MixedRealityToolkit.InputModule.InputHandlers;
using UnityEngine;

namespace MixedRealityToolkit.Examples.InputModule
{
    [RequireComponent(typeof(Renderer))]
    public class SphereKeywords : MonoBehaviour, ISpeechHandler
    {
        private Material cachedMaterial;
        private Color defaultColor;

        private void Awake()
        {
            cachedMaterial = GetComponent<Renderer>().material;
            defaultColor = cachedMaterial.color;
        }

        public void ChangeColor(string color)
        {
            switch (color.ToLower())
            {
                case "red":
                    cachedMaterial.SetColor("_Color", Color.red);
                    break;
                case "blue":
                    cachedMaterial.SetColor("_Color", Color.blue);
                    break;
                case "green":
                    cachedMaterial.SetColor("_Color", Color.green);
                    break;
            }
        }

        public void ResetColor()
        {
            cachedMaterial.SetColor("_Color", defaultColor);
        }

        public void OnSpeechKeywordRecognized(SpeechEventData eventData)
        {
            ChangeColor(eventData.RecognizedText);
        }

        private void OnDestroy()
        {
            DestroyImmediate(cachedMaterial);
        }
    }
}