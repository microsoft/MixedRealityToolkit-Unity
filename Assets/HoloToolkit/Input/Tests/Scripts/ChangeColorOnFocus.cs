// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using HoloToolkit.Unity;
using UnityEngine;

/// <summary>
/// Controls object appearance by changing its color when focused.
/// </summary>
public class ChangeColorOnFocus : Interactable
{
    [Tooltip("Object color changes to this when focused.")]
    [SerializeField]
    private Color focusedColor = Color.red;

    private Material materialInstance;

    private Color originalColor;

    private void Start()
    {
        materialInstance = GetComponent<Renderer>().material;
        originalColor = materialInstance.color;
    }

    protected override void OnGazeEnter(GameObject go)
    {
        //Check to make sure we're the game object our gaze has entered
        if (go == gameObject)
        {
            materialInstance.color = focusedColor;
        }
    }

    protected override void OnGazeExit(GameObject go)
    {
        //Check to make sure we're the game object our gaze has left
        if (go == gameObject)
        {
            materialInstance.color = originalColor;
        }
    }
}