// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace HoloToolkit.Unity.InputModule.Tests
{
    [RequireComponent(typeof(Renderer))]
    public class SphereKeywords : MonoBehaviour, ISpeechHandler, IFocusable
    {
        private Material cachedMaterial;
        private Color defaultColor;
        private bool gazed = false;

        private void Awake()
        {
            if(GetComponent<Renderer>() != null) 
            {
                cachedMaterial = GetComponent<Renderer>().material;
                defaultColor = cachedMaterial.color;
            }
        }

        public void ChangeColor(string color)
        {
            if(!gazed) {
                return;
            }
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

        public void OnSpeechKeywordRecognized(SpeechKeywordRecognizedEventData eventData)
        {
            ChangeColor(eventData.RecognizedText);
        }

        public void OnFocusEnter() 
        {
            gazed = true;
        }

        public void OnFocusExit() 
        {
            gazed = false;
        }

        private void OnDestroy()
        {
            DestroyImmediate(cachedMaterial);
        }
    }
}