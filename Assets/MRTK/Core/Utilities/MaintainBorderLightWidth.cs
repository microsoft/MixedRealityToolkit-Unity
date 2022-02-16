﻿// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities
{
    /// <summary>
    /// Utility component to keep the border light width a constant size no matter the
    /// object scale. This component should be used in conjunction with the
    /// "MixedRealityToolkit/Standard" shader "_BorderLight" feature.
    /// </summary>
    [RequireComponent(typeof(Renderer))]
    [AddComponentMenu("Scripts/MRTK/Core/MaintainBorderLightWidth")]
    public class MaintainBorderLightWidth : MonoBehaviour
    {
        private Renderer targetRenderer = null;
        private MaterialPropertyBlock properties = null;
        private static readonly int BorderWidthID = Shader.PropertyToID("_BorderWidth");
        private float initialBorderWidth = 1.0f;
        private Vector3 initialScale = Vector3.one;
        private Vector3 prevScale;

        private void Start()
        {
            // Cache the initial border width state.
            targetRenderer = GetComponent<Renderer>();
            properties = new MaterialPropertyBlock();

            initialBorderWidth = targetRenderer.sharedMaterial.GetFloat(BorderWidthID);
            initialScale = transform.lossyScale;
            prevScale = initialScale;

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
            if (prevScale != transform.lossyScale && targetRenderer != null)
            {
                prevScale = transform.lossyScale;

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

                // Multiply the initial border width by the amount need to maintain its value at the new scale.
                var scalePercentage = minScale / initialScale[minAxis];

                if (scalePercentage.Equals(0.0f))
                {
                    scalePercentage = 1.0f;
                }

                targetRenderer.GetPropertyBlock(properties);
                properties.SetFloat(BorderWidthID, initialBorderWidth / scalePercentage);
                targetRenderer.SetPropertyBlock(properties);
            }
        }
    }
}