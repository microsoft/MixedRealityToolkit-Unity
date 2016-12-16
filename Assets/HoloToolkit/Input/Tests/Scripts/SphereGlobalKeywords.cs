// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;

namespace HoloToolkit.Unity.InputModule.Tests
{
    public class SphereGlobalKeywords : MonoBehaviour, ISpeechHandler
    {
        private MaterialPropertyBlock propertyBlock;

        private void Start()
        {
            propertyBlock = new MaterialPropertyBlock();
        }

        public void OnSpeechKeywordRecognized(SpeechKeywordRecognizedEventData eventData)
        {
            switch (eventData.RecognizedText.ToLower())
            {
                case "reset all":
                    foreach (Renderer renderer in GetComponentsInChildren<Renderer>())
                    {
                        ChangeColor(renderer, Color.white);
                    }
                    break;
            }
        }

        private void ChangeColor(Renderer renderer, Color color)
        {
            renderer.GetPropertyBlock(propertyBlock);
            propertyBlock.SetColor("_Color", color);
            renderer.SetPropertyBlock(propertyBlock);
        }
    }
}
