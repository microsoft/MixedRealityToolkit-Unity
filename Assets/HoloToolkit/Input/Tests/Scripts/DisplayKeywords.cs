// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using HoloToolkit.Unity;
using UnityEngine;
using UnityEngine.UI;

public class DisplayKeywords : MonoBehaviour
{
    public KeywordManager keywordManager;
    public Text textPanel;

    void Start()
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