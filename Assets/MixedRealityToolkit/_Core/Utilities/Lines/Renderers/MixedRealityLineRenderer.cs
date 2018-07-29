// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Definitions.Lines;
using Microsoft.MixedReality.Toolkit.Internal.Extensions;
using Microsoft.MixedReality.Toolkit.Internal.Utilities.Lines.DataProviders;
using UnityEngine;
using UnityEngine.Rendering;

namespace Microsoft.MixedReality.Toolkit.Internal.Utilities.Lines.Renderers
{
    [RequireComponent(typeof(LineRenderer))]
    [RequireComponent(typeof(BaseMixedRealityLineDataProvider))]
    public class MixedRealityLineRenderer : BaseMixedRealityLineRenderer
    {
        private const string DefaultLineShader = "Particles/Alpha Blended";
        private const string DefaultLineShaderColor = "_TintColor";

        [Header("Mixed Reality Line Renderer Settings")]

        [SerializeField]
        [Tooltip("The material to use for the Unity MixedRealityLineRenderer (will be auto-generated if null)")]
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

        protected void OnEnable()
        {
            lineRenderer = gameObject.EnsureComponent<LineRenderer>();

            if (lineMaterial == null)
            {
                lineMaterial = new Material(Shader.Find(DefaultLineShader));
                lineMaterial.SetColor(DefaultLineShaderColor, Color.gray);
            }
        }

        private void Update()
        {
            if (isActiveAndEnabled)
            {
                UpdateLineRenderer();
            }
        }

        private void UpdateLineRenderer()
        {
            if (LineDataSource == null)
            {
                enabled = false;
                lineRenderer.enabled = false;
                return;
            }

            lineRenderer.enabled = LineDataSource.enabled;
            lineRenderer.positionCount = StepMode == StepMode.FromSource ? LineDataSource.PointCount : LineStepCount;

            if (positions == null || positions.Length != lineRenderer.positionCount)
            {
                positions = new Vector3[lineRenderer.positionCount];
            }

            for (int i = 0; i < positions.Length; i++)
            {
                if (StepMode == StepMode.FromSource)
                {
                    positions[i] = LineDataSource.GetPoint(i);
                }
                else
                {
                    float normalizedDistance = (1f / (LineStepCount - 1)) * i;
                    positions[i] = LineDataSource.GetPoint(normalizedDistance);
                }
            }

            // Set line renderer properties
            lineRenderer.loop = LineDataSource.Loops;
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
