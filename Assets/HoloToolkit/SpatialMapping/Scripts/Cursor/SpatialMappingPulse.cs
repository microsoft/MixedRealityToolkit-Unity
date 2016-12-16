// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace HoloToolkit.Unity.SpatialMapping
{
    public class SpatialMappingPulse : MonoBehaviour
    {
        private Renderer cachedRenderer;
        private MaterialPropertyBlock matPropBlock;

        private void Awake()
        {
            matPropBlock = new MaterialPropertyBlock();
            cachedRenderer = GetComponent<Renderer>();
        }

        private void DoPulse(Color color)
        {
            // Get the current value of the material properties in the renderer.
            cachedRenderer.GetPropertyBlock(matPropBlock);
            // Assign our new value.
            matPropBlock.SetColor("_Color", color);
            // Apply the edited values to the renderer.
            cachedRenderer.SetPropertyBlock(matPropBlock);
        }
    }
}