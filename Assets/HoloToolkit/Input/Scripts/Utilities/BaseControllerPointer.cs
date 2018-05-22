// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using HoloToolkit.Unity.InputModule;
using UnityEngine;

public abstract class BaseControllerPointer : AttachToController
{
    [SerializeField]
    [Range(0f, 360f)]
    protected float CurrentPointerOrientation;

    [SerializeField]
    [Range(0.5f, 50f)]
    private float extentOverride = 2f;

    [SerializeField]
    [Tooltip("Source transform for raycast origin - leave null to use default transform")]
    protected Transform RaycastOrigin;

    public float? ExtentOverride
    {
        get { return extentOverride; }
        set { extentOverride = value ?? FocusManager.Instance.GlobalPointingExtent; }
    }

    /// <summary>
    /// The Y orientation of the pointer target - used for touchpad rotation and navigation
    /// </summary>
    public virtual float PointerOrientation
    {
        get
        {
            return CurrentPointerOrientation + (RaycastOrigin != null ? RaycastOrigin.eulerAngles.y : transform.eulerAngles.y);
        }
        set
        {
            CurrentPointerOrientation = value;
        }
    }

    /// <summary>
    /// The forward direction of the targeting ray
    /// </summary>
    public virtual Vector3 PointerDirection
    {
        get { return RaycastOrigin != null ? RaycastOrigin.forward : transform.forward; }
    }
}
