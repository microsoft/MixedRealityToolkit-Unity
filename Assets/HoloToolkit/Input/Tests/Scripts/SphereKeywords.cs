// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using HoloToolkit.Unity;
using UnityEngine;

public class SphereKeywords : Interactable
{
    protected override void KeywordRecognized(string keyword)
    {
        if (keyword.Equals("red"))
        {
            GetComponent<Renderer>().material.color = Color.red;
        }
        else if (keyword.Equals("blue"))
        {
            GetComponent<Renderer>().material.color = Color.blue;
        }
        else if (keyword.Equals("green"))
        {
            GetComponent<Renderer>().material.color = Color.green;
        }
    }
}