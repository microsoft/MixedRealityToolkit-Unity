// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using HoloToolkit.Unity;

/// <summary>
/// Controls object appearance by changing its color when selected
/// and returning it to its original color when a different object is selected
/// </summary>
public class ChangeColorOnSelect : MonoBehaviour, IInteractable
{
    [Tooltip("Object color changes to this when focused.")]
    [SerializeField]
    private Color selectedColor = Color.red;

    private Material materialInstance;

    private Color originalColor;

    private void Start()
    {
        materialInstance = GetComponent<Renderer>().material;
        originalColor = materialInstance.color;
    }

    private void OnEnable()
    {
        GestureManager.Instance.OnTap += CheckOnTap;
    }

    private void OnDisable ()
    {
        GestureManager.Instance.OnTap -= CheckOnTap;
    }

    private void CheckOnTap(GameObject tappedObject)
    {
        if (tappedObject != gameObject)
        {
            materialInstance.color = originalColor;
        }
    }

    public void OnTap()
    {
        materialInstance.color = selectedColor;
    }

    public void OnGazeEnter() { }

    public void OnGazeExit() { }

    public void OnSelectObject()
    {
        materialInstance.color = selectedColor;
    }

    public void OnClearSelection()
    {
        materialInstance.color = originalColor;
    }
}