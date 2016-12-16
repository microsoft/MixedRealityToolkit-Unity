// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;
using System.Collections.Generic;

namespace HoloToolkit.Unity.InputModule.Tests
{
    public class SphereKeywords : MonoBehaviour, ISpeechHandler
    {
        private new Renderer renderer;
        private MaterialPropertyBlock propertyBlock;

        protected virtual void Awake()
        {
            renderer = GetComponent<Renderer>();
            propertyBlock = new MaterialPropertyBlock();
    }

    public void ChangeColor(string color)
        {
            switch (color.ToLower())
            {
                case "red":
                    ChangeColor(Color.red);
                    break;
                case "blue":
                    ChangeColor(Color.blue);
                    break;
                case "green":
                    ChangeColor(Color.green);
                    break;
            }
        }

        public void OnSpeechKeywordRecognized(SpeechKeywordRecognizedEventData eventData)
        {
            ChangeColor(eventData.RecognizedText);
        }

        private void ChangeColor(Color color)
        {
            renderer.GetPropertyBlock(propertyBlock);
            propertyBlock.SetColor("_Color", color);
            renderer.SetPropertyBlock(propertyBlock);
        }
    }
}
