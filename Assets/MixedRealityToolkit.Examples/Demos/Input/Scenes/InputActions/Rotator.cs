// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos
{
    // Silly script that applies a rotation about the up axis on demand.
    public class Rotator : MonoBehaviour
    {
        public float angle = 45f;

        public void Rotate()
        {
            transform.Rotate(0, angle, 0);
        }
    }
}