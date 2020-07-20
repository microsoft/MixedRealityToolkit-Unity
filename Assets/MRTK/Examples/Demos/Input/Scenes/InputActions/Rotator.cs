// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos
{
    // Silly script that applies a rotation about the up axis on demand.
    [AddComponentMenu("Scripts/MRTK/Examples/Rotator")]
    public class Rotator : MonoBehaviour
    {
        public float angle = 45f;

        public void Rotate()
        {
            transform.Rotate(0, angle, 0);
        }
    }
}