// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace HoloToolkit.Unity.InputModule.Tests
{
    public class DisplaySpeechKeywords : MonoBehaviour
    {
        public SpeechInputSource speechInputSource;
        public TextMesh textMesh;

        private void Start()
        {
            if (speechInputSource == null || textMesh == null)
            {
                Debug.Log("Please check the variables in the Inspector on DisplaySpeechKeywords.cs on" + name + ".");
                return;
            }

            textMesh.text = "Try saying:\n";
            foreach (SpeechInputSource.KeywordAndKeyCode item in speechInputSource.Keywords)
            {
                textMesh.text += " " + item.Keyword + "\n";
            }
        }
    }
}