// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using HoloToolkit.Unity;

/// <summary>
/// Responds to gesture manager's tap event named OnSelect
/// </summary>
public class GestureResponder : MonoBehaviour
{
    private void OnEnable()
    {
        GestureManager.Instance.OnTap += OnTap;
    }

    private void OnDisable()
    {
        GestureManager.Instance.OnTap -= OnTap;
    }

    // Responds to the gesture manager's "TappedEvent"
    private void OnTap(GameObject tappedGameObject)
    {
        // Check to make sure we've tapped on the object this script is attached to
        if (tappedGameObject == gameObject)
        {
            PlaneTargetGroupPicker.Instance.PickNewTarget();
        }
    }
}