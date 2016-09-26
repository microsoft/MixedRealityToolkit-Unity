// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

public class SphereKeywords : MonoBehaviour
{
    private Material materialInstance;

    private void Start()
    {
        materialInstance = GetComponent<Renderer>().material;
    }

    public void ChangeColor(string color)
    {
        if (color.Equals("red"))
        {
            materialInstance.color = Color.red;
        }
        else if (color.Equals("blue"))
        {
            materialInstance.color = Color.blue;
        }
        else if (color.Equals("green"))
        {
            materialInstance.color = Color.green;
        }
    }
}