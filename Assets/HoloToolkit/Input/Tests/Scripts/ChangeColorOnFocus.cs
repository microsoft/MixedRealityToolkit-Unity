// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using HoloToolkit.Unity;
using UnityEngine;

/// <summary>
/// Controls object appearance by changing its color when focused.
/// </summary>
public class ChangeColorOnFocus : MonoBehaviour, IInteractable
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

    public void OnTap () { }

    public void OnGazeEnter()
    {
        materialInstance.color = focusedColor;
    }

    public void OnGazeExit()
    {
        materialInstance.color = originalColor;
    }
}