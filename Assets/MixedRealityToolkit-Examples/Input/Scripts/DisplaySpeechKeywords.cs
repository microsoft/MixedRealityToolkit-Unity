// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using MixedRealityToolkit.InputModule.InputSources;
using UnityEngine;

namespace MixedRealityToolkit.Examples.InputModule
{
    public class DisplaySpeechKeywords : MonoBehaviour
    {
        public SpeechInputSource SpeechInputSource;
        public TextMesh TextMesh;

        private void Start()
        {
            if (SpeechInputSource == null || TextMesh == null)
            {
                Debug.Log("Please check the variables in the Inspector on DisplaySpeechKeywords.cs on" + name + ".");
                return;
            }

            TextMesh.text = "Try saying:\n";
            for (var i = 0; i < SpeechInputSource.Keywords.Length; i++)
            {
                TextMesh.text += " " + SpeechInputSource.Keywords[i].Keyword + "\n";
            }
        }
    }
}
