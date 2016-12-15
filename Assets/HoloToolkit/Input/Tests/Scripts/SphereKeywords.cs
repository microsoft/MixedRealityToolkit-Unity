// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;
using System.Collections.Generic;

namespace HoloToolkit.Unity.InputModule.Tests
{
    public class SphereKeywords : MonoBehaviour, ISpeechHandler
    {
        MeshFilter meshFilter;

        protected virtual void Start()
        {
            meshFilter = GetComponent<MeshFilter>();
        }

        public void ChangeColor(string color)
        {
            switch (color.ToLower())
            {
                case "red":
                    meshFilter.ChangeColor(Color.red);
                    break;
                case "blue":
                    meshFilter.ChangeColor(Color.blue);
                    break;
                case "green":
                    meshFilter.ChangeColor(Color.green);
                    break;
            }
        }

        public void OnSpeechKeywordRecognized(SpeechKeywordRecognizedEventData eventData)
        {
            ChangeColor(eventData.RecognizedText);
        }
    }
}
