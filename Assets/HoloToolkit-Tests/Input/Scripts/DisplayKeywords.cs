// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;
using UnityEngine.UI;

namespace HoloToolkit.Unity.InputModule.Tests
{
    [Obsolete("See DisplaySpeechKeywords for updated usage.")]
    public class DisplayKeywords : MonoBehaviour
    {
        public KeywordManager keywordManager;
        public Text textPanel;

        private void Start()
        {
            if (keywordManager == null || textPanel == null)
            {
                Debug.Log("Please check the variables in the Inspector on DisplayKeywords.cs on" + name + ".");
                return;
            }

            textPanel.text = "Try saying:\n";
            foreach (KeywordManager.KeywordAndResponse k in keywordManager.KeywordsAndResponses)
            {
                textPanel.text += k.Keyword + "\n";
            }
        }
    }
}