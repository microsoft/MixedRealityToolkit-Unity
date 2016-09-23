// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using HoloToolkit.Unity;
using UnityEngine;

public class SphereKeywords : Interactable
{
    private Material materialInstance;

    private void Start()
    {
        materialInstance = GetComponent<Renderer>().material;
    }

    protected override void KeywordRecognized(string keyword)
    {
        if (keyword.Equals("red"))
        {
            materialInstance.color = Color.red;
        }
        else if (keyword.Equals("blue"))
        {
            materialInstance.color = Color.blue;
        }
        else if (keyword.Equals("green"))
        {
            materialInstance.color = Color.green;
        }
    }
}