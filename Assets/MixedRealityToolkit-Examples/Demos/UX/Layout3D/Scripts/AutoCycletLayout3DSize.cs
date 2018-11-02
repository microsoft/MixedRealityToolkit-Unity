// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.SDK.UX.Layout3D
{
    public class AutoCycletLayout3DSize : MonoBehaviour
    {
        public float AnimationTime = 3;

        private void Start()
        {
            StartCoroutine(Timer());
        }
        
        private IEnumerator Timer()
        {
            yield return new WaitForSeconds(AnimationTime);

            CycleLayout3DPixelSize cycle = GetComponent<CycleLayout3DPixelSize>();

            if (cycle)
            {
                cycle.Next();
            }

            StartCoroutine(Timer());
        }
    }
}
