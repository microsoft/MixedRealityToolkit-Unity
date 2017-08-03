// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace HoloToolkit.Unity
{
    /// <summary>
    /// Keeps a game object within configured boudns
    /// </summary>
    [RequireComponent(typeof(Transform))]
    public class RestrictToBounds : MonoBehaviour
    {
        public Vector3 Min;
        public Vector3 Max;

        void Update()
        {
            this.transform.localPosition = MathExtensions.ClampComponentwise(this.transform.localPosition, this.Min, this.Max);
        }
    }
}