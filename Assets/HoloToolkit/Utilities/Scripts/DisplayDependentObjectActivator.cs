// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace HoloToolkit.Unity
{
    public class DisplayDependentObjectActivator : MonoBehaviour
    {
        [SerializeField] protected bool OpaqueDisplay = true;

        [SerializeField] protected bool TransparentDisplay = false;

        protected void Awake()
        {
#if UNITY_WSA && UNITY_2017_2_OR_NEWER
            if ((UnityEngine.XR.WSA.HolographicSettings.IsDisplayOpaque && !OpaqueDisplay) ||
               (!UnityEngine.XR.WSA.HolographicSettings.IsDisplayOpaque && !TransparentDisplay))
            {
                gameObject.SetActive(false);
            }
#endif
        }
    }
}