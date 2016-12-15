// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using UnityEngine.UI;

namespace HoloToolkit.Unity.InputModule.Tests
{
    public class DisplaySpeechKeywords : MonoBehaviour
    {
        public SpeechInputSource speechInputSource;
        public Text textPanel;

        void Start()
        {
            if (speechInputSource == null || textPanel == null)
            {
                Debug.Log("Please check the variables in the Inspector on DisplaySpeechKeywords.cs on" + name + ".");
                return;
            }

            textPanel.text = "Try saying:\n";
            foreach (SpeechInputSource.KeywordAndKeyCode item in speechInputSource.Keywords)
            {
                textPanel.text += item.Keyword + "\n";
            }
        }
    }
}