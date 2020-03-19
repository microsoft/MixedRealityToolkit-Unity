// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using UnityEngine.Rendering;
using System.Collections;
using System;

namespace Microsoft.MixedReality.Toolkit.Utilities
{
    /// <summary>
    /// Implements Unity's built in line renderer component, and applies the line data to it.
    /// </summary>
    [RequireComponent(typeof(LineRenderer))]
    [AddComponentMenu("Scripts/MRTK/Core/MixedRealityLineRenderer")]
    public class MixedRealityLineRenderer : BaseMixedRealityLineRenderer
    {
        [Header("Mixed Reality Line Renderer Settings")]

        [SerializeField]
        [Tooltip("The material to use for the Unity MixedRealityLineRenderer.")]
        private Material lineMaterial = null;

        public Material LineMaterial
        {
            get => lineMaterial;
            set => lineMaterial = value;
        }

        [SerializeField]
        private bool roundedEdges = true;

        public bool RoundedEdges
        {
            get => roundedEdges;
            set => roundedEdges = value;
        }

        [SerializeField]
        private bool roundedCaps = true;

        public bool RoundedCaps
        {
            get => roundedCaps;
            set => roundedCaps = value;
        }

        [SerializeField]
        [Tooltip("Sets whether the pointer line will animate to a lower brightness level after a hand/controller is recognized.")]
        private bool fadeLineBrightnessOnEnable = true;

        /// <summary>
        /// Sets whether the ray line will animate to a lower brightness level on enable of this component
        /// </summary>
        public bool FadeLineBrightnessOnEnable
        {
            get => fadeLineBrightnessOnEnable;
            set => fadeLineBrightnessOnEnable = value;
        }

        [SerializeField, Range(0f, 1f)]
        [Tooltip("The amount the pointer line will fade if FadeLineBrightnessOnEnable is true.")]
        private float fadeLinePercentage = 0.55f;

        /// <summary>
        /// The amount the pointer line will fade if FadeLineBrightnessOnEnable is true"
        /// </summary>
        public float FadeLinePercentage
        {
            get => fadeLinePercentage;
            set => fadeLinePercentage = value;
        }

        [SerializeField]
        [Tooltip("The length of the animation if fadeLineBrightnessOnEnable is true.")]
        private float fadeLineAnimationTime = 0.65f;

        /// <summary>
        /// The amount the pointer line will fade if fadeLineBrightnessOnEnable is true"
        /// </summary>
        public float FadeLineAnimationTime
        {
            get => fadeLineAnimationTime;
            set => fadeLineAnimationTime = value;
        }

        [SerializeField]
        [HideInInspector]
        private LineRenderer lineRenderer = null;

        [Header("Texture Tiling")]
        [SerializeField]
        [Tooltip("Tiles the material on the line renderer by world length. Use if you want the texture size to remain constant regardless of a line's length.")]
        private bool tileMaterialByWorldLength = false;
        private MaterialPropertyBlock tilingPropertyBlock;
        private Vector4 tilingPropertyVector = Vector4.one;

        [SerializeField]
        private float tileMaterialScale = 1f;

        private Vector3[] positions;

        private Coroutine fadeLine = null;
        private GradientAlphaKey[] cachedKeys;

        private void OnEnable()
        {
            lineRenderer = gameObject.EnsureComponent<LineRenderer>();

            if (LineMaterial == null)
            {
                LineMaterial = lineRenderer.sharedMaterial;
            }

            // mafinc - Start the line renderer off disabled (invisible), we'll enable it
            // when we have enough data for it to render properly.
            if (lineRenderer != null)
            {
                lineRenderer.enabled = false;
            }

            if (LineMaterial == null)
            {
                Debug.LogError("MixedRealityLineRenderer needs a material.");
                enabled = false;
            }

            if (FadeLineBrightnessOnEnable && Application.isPlaying)
            {
                fadeLine = StartCoroutine(FadeLine(FadeLinePercentage, FadeLineAnimationTime));
            }
        }

        private void OnDisable()
        {
            lineRenderer.enabled = false;

            if (fadeLine != null)
            {
                StopCoroutine(fadeLine);
                fadeLine = null;
            }
        }

        /// <inheritdoc />
        protected override void UpdateLine()
        {
            if (LineDataSource == null)
            {
                enabled = false;
                lineRenderer.enabled = false;
                return;
            }

            lineRenderer.enabled = lineDataSource.enabled;
            lineRenderer.positionCount = StepMode == StepMode.FromSource ? lineDataSource.PointCount : LineStepCount;

            if (positions == null || positions.Length != lineRenderer.positionCount)
            {
                positions = new Vector3[lineRenderer.positionCount];
            }

            for (int i = 0; i < positions.Length; i++)
            {
                if (StepMode == StepMode.FromSource)
                {
                    positions[i] = lineDataSource.GetPoint(i);
                }
                else
                {
                    float normalizedDistance = GetNormalizedPointAlongLine(i);
                    positions[i] = lineDataSource.GetPoint(normalizedDistance);
                }
            }

            // Set line renderer properties
            lineRenderer.loop = lineDataSource.Loops;
            lineRenderer.numCapVertices = roundedCaps ? 8 : 0;
            lineRenderer.numCornerVertices = roundedEdges ? 8 : 0;
            lineRenderer.useWorldSpace = true;
            lineRenderer.startWidth = 1;
            lineRenderer.endWidth = 1;
            lineRenderer.startColor = Color.white;
            lineRenderer.endColor = Color.white;
            lineRenderer.sharedMaterial = lineMaterial;
            lineRenderer.widthCurve = LineWidth;
            lineRenderer.widthMultiplier = WidthMultiplier;
            lineRenderer.colorGradient = LineColor;
            lineRenderer.shadowCastingMode = ShadowCastingMode.Off;
            lineRenderer.lightProbeUsage = LightProbeUsage.Off;

            // Set positions
            lineRenderer.positionCount = positions.Length;
            lineRenderer.SetPositions(positions);

            // Update texture tiling, if applicable
            if (tileMaterialByWorldLength)
            {
                if (tilingPropertyBlock == null)
                {
                    tilingPropertyBlock = new MaterialPropertyBlock();
                }

                tilingPropertyVector.x = lineDataSource.UnClampedWorldLength * tileMaterialScale;
                tilingPropertyBlock.SetVector("_MainTex_ST", tilingPropertyVector);
                lineRenderer.SetPropertyBlock(tilingPropertyBlock);
            }
            else
            {
                if (tilingPropertyBlock != null)
                {
                    tilingPropertyBlock.Clear();
                    lineRenderer.SetPropertyBlock(tilingPropertyBlock);
                }
            }
        }

        private IEnumerator FadeLine(float targetAlphaPercentage, float animationLength)
        {
            float currentTime = 0f;

            if (cachedKeys == null)
            {
                cachedKeys = LineColor.alphaKeys;
            }

            GradientAlphaKey[] fadedKeys = new GradientAlphaKey[cachedKeys.Length];
            Array.Copy(cachedKeys, fadedKeys, cachedKeys.Length);
            float startAlpha = 1f;

            while (currentTime != animationLength)
            {
                currentTime += Time.deltaTime;

                if (currentTime > animationLength)
                {
                    currentTime = animationLength;
                }

                float percentageComplete = currentTime / animationLength;

                float scalar = Mathf.Lerp(startAlpha, targetAlphaPercentage, percentageComplete);

                for (int i = 0; i < fadedKeys.Length; i++)
                {
                    fadedKeys[i].alpha = cachedKeys[i].alpha * scalar;
                }

                LineColor.alphaKeys = fadedKeys;

                yield return null;
            }

            fadeLine = null;
        }
    }
}
