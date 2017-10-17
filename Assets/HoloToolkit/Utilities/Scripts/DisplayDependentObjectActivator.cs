// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

#if UNITY_WSA && UNITY_2017_2_OR_NEWER
using UnityEngine.XR.WSA;
#endif

public class DisplayDependentObjectActivator : MonoBehaviour
{
    [SerializeField]
    protected bool OpaqueDisplay = true;

    [SerializeField]
    protected bool TransparentDisplay = false;

    protected void Awake()
    {
#if UNITY_WSA && UNITY_2017_2_OR_NEWER
        if ((HolographicSettings.IsDisplayOpaque && !OpaqueDisplay) ||
            (!HolographicSettings.IsDisplayOpaque && !TransparentDisplay))
        {
            gameObject.SetActive(false);
        }
#endif
    }
}
