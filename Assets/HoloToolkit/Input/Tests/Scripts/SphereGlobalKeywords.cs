﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;

namespace HoloToolkit.Unity.InputModule.Tests
{
    public class SphereGlobalKeywords : MonoBehaviour, ISpeechHandler
    {
        public void Start()
        {
            InputManager.Instance.AddGlobalListener(gameObject);
        }

        public void OnPhraseRecognized(PhraseRecognizedEventData eventData)
        {
            switch (eventData.Text.ToLower())
            {
                case "reset all":
                    foreach (Renderer renderer in GetComponentsInChildren<Renderer>())
                    { 
                        renderer.material.color = Color.gray;
                    }
                    break;
            }
        }
    }
}
