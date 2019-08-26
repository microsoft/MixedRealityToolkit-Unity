// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(RectTransform))]
public class RectTransformCubeScaler : MonoBehaviour
{
    [SerializeField]
    [Range(0.0f, 1.0f)]
    [Tooltip("0 means set localScale.z to Min(x, y).  1 means set localScale z to Max(x, y)")]
    private float minOrMaxLerpFactor = 0.0f;

    [SerializeField]
    [Range(0.0f, 1.0f)]
    [Tooltip("0 means set localScale.z to x.  1 means set localScale z to y")]
    private float xOrYLerpFactor = 0.0f;

    [SerializeField]
    [Range(0.0f, 1.0f)]
    private float minMaxOrXYLerpFactor = 0.0f;
    [Tooltip("0 means set localScale.z to value derived from min/max lerp.  1 means set localScale.z to value derived from x/y lerp")]

    private RectTransform rectTransform;
    private Vector2 prevRectSize = default;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void Update()
    {
        var size = rectTransform.rect.size;

        if (prevRectSize != size)
        {
            prevRectSize = size;

            var min = Mathf.Min(size.x, size.y);
            var max = Mathf.Max(size.x, size.y);
            var minOrMax = Mathf.Lerp(min, max, minOrMaxLerpFactor);
            var xOrY = Mathf.Lerp(size.x, size.y, xOrYLerpFactor);

            var z = Mathf.Lerp(minOrMax, xOrY, minMaxOrXYLerpFactor);

            this.transform.localScale = new Vector3(size.x, size.y, z);
        }
    }
}
