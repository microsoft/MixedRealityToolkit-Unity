// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Experimental.SurfacePulse
{
    /// <summary>
    /// Hide the game object on device.
    /// </summary>
    public class HideOnDevice : MonoBehaviour
    {
        void Start()
        {
            if (!Application.isEditor)
            {
                gameObject.SetActive(false);
            }
        }
    }
}