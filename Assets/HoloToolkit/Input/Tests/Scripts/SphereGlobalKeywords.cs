// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;

namespace HoloToolkit.Unity.InputModule.Tests
{
    public class SphereGlobalKeywords : MonoBehaviour, ISpeechHandler
    {
        private void Start()
        {
            InputManager.Instance.AddGlobalListener(gameObject);
        }

        public void OnSpeechKeywordRecognized(SpeechKeywordRecognizedEventData eventData)
        {
            switch (eventData.RecognizedText.ToLower())
            {
                case "reset all":
                    foreach (Renderer renderer in GetComponentsInChildren<Renderer>())
                    {
                        Material material = renderer.material;
                        material.color = Color.gray;
                        Resources.UnloadAsset(material);
                    }
                    break;
            }
        }
    }
}
