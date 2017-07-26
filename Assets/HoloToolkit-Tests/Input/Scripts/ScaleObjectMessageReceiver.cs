// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace HoloToolkit.Unity.InputModule.Tests
{
    public class ScaleObjectMessageReceiver : MonoBehaviour
    {
        private const float DefaultSizeFactor = 2.0f;

        [Tooltip("Size multiplier to use when scaling the object up and down.")]
        public float SizeFactor = DefaultSizeFactor;

        private void Start()
        {
            if (SizeFactor <= 0.0f)
            {
                SizeFactor = DefaultSizeFactor;
            }
        }

        public void OnMakeBigger()
        {
            Vector3 scale = transform.localScale;
            scale *= SizeFactor;
            transform.localScale = scale;
        }

        public void OnMakeSmaller()
        {
            Vector3 scale = transform.localScale;
            scale /= SizeFactor;
            transform.localScale = scale;
        }
    }
}