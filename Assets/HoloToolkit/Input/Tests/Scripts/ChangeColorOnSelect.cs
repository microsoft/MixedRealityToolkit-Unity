// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using HoloToolkit.Unity;

/// <summary>
/// Controls object appearance by changing its color when selected
/// and returning it to its original color when a different object is selected
/// </summary>
public class ChangeColorOnSelect : Interactable
{
    [Tooltip( "Object color changes to this when focused." )]
    [SerializeField]
    private Color selectedColor = Color.red;

    private Material materialInstance;

    private Color originalColor;

    private void Start()
    {
        materialInstance = GetComponent<Renderer>().material;
        originalColor = materialInstance.color;
    }

    protected override void OnTap(GameObject tappedGameObject)
    {
        // Check to make sure this is the game object we've tapped
        if (tappedGameObject == gameObject)
        {
            materialInstance.color = selectedColor;
        }
        else
        {
            materialInstance.color = originalColor;
        }
    }
}