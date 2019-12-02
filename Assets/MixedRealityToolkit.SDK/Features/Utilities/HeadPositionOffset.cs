// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities
{
    /// <summary>
    /// Set the content around the camera height
    /// </summary>
    [AddComponentMenu("Scripts/MRTK/SDK/HeadPositionOffset")]
    public class HeadPositionOffset : MonoBehaviour
    {
        public Vector3 HeadOffset = new Vector3(0, 0, 1f);

        private bool started = false;

        private void Start()
        {
            transform.position = CameraCache.Main.transform.position + HeadOffset;
            started = true;
        }

        private void OnEnable()
        {
            if (started)
            {
                transform.position = CameraCache.Main.transform.position + HeadOffset;
            }
        }
    }
}
