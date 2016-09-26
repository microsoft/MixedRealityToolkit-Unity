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
        switch(color.ToLower())
        {
            case "red":
                materialInstance.color = Color.red;
                break;
            case "blue":
                materialInstance.color = Color.blue;
                break;
            case "green":
                materialInstance.color = Color.green;
                break;
        }
    }
}