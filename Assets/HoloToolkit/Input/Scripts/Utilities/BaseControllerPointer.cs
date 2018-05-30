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
    [Tooltip("Source transform for raycast origin - leave null to use default transform")]
    protected Transform RaycastOrigin;

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
