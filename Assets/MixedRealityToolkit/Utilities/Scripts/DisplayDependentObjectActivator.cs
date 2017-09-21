// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using UnityEngine.XR.WSA;

public class DisplayDependentObjectActivator : MonoBehaviour
{
    [SerializeField]
    protected bool OpaqueDisplay = true;

    [SerializeField]
    protected bool TransparentDisplay = false;

    protected void Awake()
    {
        if ((HolographicSettings.IsDisplayOpaque && !OpaqueDisplay) ||
            (!HolographicSettings.IsDisplayOpaque && !TransparentDisplay))
        {
            gameObject.SetActive(false);
        }
    }
}
