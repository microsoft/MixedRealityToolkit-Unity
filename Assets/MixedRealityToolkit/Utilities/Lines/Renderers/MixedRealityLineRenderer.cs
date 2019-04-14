// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using UnityEngine.Rendering;

namespace Microsoft.MixedReality.Toolkit.Utilities
{
    /// <summary>
    /// Implements Unity's built in line renderer component, and applies the line data to it.
    /// </summary>
    [RequireComponent(typeof(LineRenderer))]
    public class MixedRealityLineRenderer : BaseMixedRealityLineRenderer
    {
        [Header("Mixed Reality Line Renderer Settings")]

        [SerializeField]
        [Tooltip("The material to use for the Unity MixedRealityLineRenderer.")]
        private Material lineMaterial = null;

        public Material LineMaterial
        {
            get { return lineMaterial; }
            set { lineMaterial = value; }
        }

        [SerializeField]
        private bool roundedEdges = true;

        public bool RoundedEdges
        {
            get { return roundedEdges; }
            set { roundedEdges = value; }
        }

        [SerializeField]
        private bool roundedCaps = true;

        public bool RoundedCaps
        {
            get { return roundedCaps; }
            set { roundedCaps = value; }
        }

        [SerializeField]
        [HideInInspector]
        private LineRenderer lineRenderer = null;

        private Vector3[] positions;

        private void OnEnable()
        {
            lineRenderer = gameObject.EnsureComponent<LineRenderer>();

            if (lineMaterial == null)
            {
                lineMaterial = lineRenderer.sharedMaterial;
            }

            // mafinc - Start the line renderer off disabled (invisible), we'll enable it
            // when we have enough data for it to render properly.
            if (lineRenderer != null)
            {
                lineRenderer.enabled = false;
            }

            if (lineMaterial == null)
            {
                Debug.LogError("MixedRealityLineRenderer needs a material.");
                enabled = false;
            }
        }

        private void OnDisable()
        {
            lineRenderer.enabled = false;
        }

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
        }
    }
}
