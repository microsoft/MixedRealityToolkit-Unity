// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace MixedRealityToolkit.Utilities
{
    /// <summary>
    /// A MonoBehaviour which automatically scales an object for better usability across different devices.
    /// </summary>
    public class UsabilityScaler : MonoBehaviour
    {
        private Vector3 baseScale;

        private void OnEnable()
        {
            baseScale = transform.localScale;
            float usabilityScaleFactor = UsabilityUtilities.GetUsabilityScaleFactor(Camera.main);

            transform.localScale = (baseScale * usabilityScaleFactor);
        }

        private void OnDisable()
        {
            transform.localScale = baseScale;
        }
    }
}
