// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

public class FocusRaycastTestProxy : MonoBehaviour
{
    public LayerMask[] PrioritizedLayerMasks = null;
    public GameObject ExpectedHitObject = null;
    public BaseMixedRealityLineDataProvider RayLineData = null;
    public int LineCastResolution = 10;

    private void Awake()
    {
        RayLineData = RayLineData ?? GetComponent<BaseMixedRealityLineDataProvider>();
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(transform.position, 0.03f);
    }
}
