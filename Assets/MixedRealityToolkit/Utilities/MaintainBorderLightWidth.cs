// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities
{
    /// <summary>
    /// Utility component to keep the border light width a constant size no mater the 
    /// object scale. This component should be used in conjunction with the 
    /// "MixedRealityToolkit/Standard" shader "_BorderLight" feature.
    /// </summary>
    [RequireComponent(typeof(Renderer))]
    public class MaintainBorderLightWidth : MonoBehaviour
    {
        private Renderer targetRenderer = null;
        private MaterialPropertyBlock properties = null;
        private int borderWidthID = 0;
        private float initialBorderWidth = 1.0f;
        private Vector3 initialScale = Vector3.one;

        private void Start()
        {
            // Cache the initial border width state.
            targetRenderer = GetComponent<Renderer>();
            properties = new MaterialPropertyBlock();
            borderWidthID = Shader.PropertyToID("_BorderWidth");
            initialBorderWidth = targetRenderer.sharedMaterial.GetFloat(borderWidthID);
            initialScale = transform.lossyScale;

            for (int i = 0; i < 3; ++i)
            {
                if (initialScale[i].Equals(0.0f))
                {
                    initialScale[i] = 1.0f;
                }
            }
        }

        private void LateUpdate()
        {
            if (targetRenderer != null)
            {
                // Find the axis with the smallest scale.
                var minAxis = 0;
                var minScale = float.MaxValue;

                for (int i = 0; i < 3; ++i)
                {
                    var scaleAbs = Mathf.Abs(transform.lossyScale[i]);

                    if (scaleAbs < minScale)
                    {
                        minAxis = i;
                        minScale = scaleAbs;
                    }
                }

                // Multiply the initial border width by the amount need to maintain it's value at the new scale.
                var scalePercentage = minScale / initialScale[minAxis];

                if (scalePercentage.Equals(0.0f))
                {
                    scalePercentage = 1.0f;
                }

                targetRenderer.GetPropertyBlock(properties);
                properties.SetFloat(borderWidthID, initialBorderWidth / scalePercentage);
                targetRenderer.SetPropertyBlock(properties);
            }
        }
    }
}