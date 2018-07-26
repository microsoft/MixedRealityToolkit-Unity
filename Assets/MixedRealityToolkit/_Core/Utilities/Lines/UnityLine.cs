// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Definitions.Lines;
using Microsoft.MixedReality.Toolkit.Internal.Extensions;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Internal.Utilities.Lines
{
    public class UnityLine : LineRendererBase
    {
        private const string DefaultLineShader = "Particles/Alpha Blended";
        private const string DefaultLineShaderColor = "_TintColor";

        [Header("UnityLine Settings")]
        [SerializeField]
        [Tooltip("The material to use for the Unity LineRenderer (will be auto-generated if null)")]
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
        private LineRenderer lineRenderer;

        private Vector3[] positions;

        private void OnValidate()
        {
            lineRenderer = gameObject.EnsureComponent<LineRenderer>();
        }

        protected void OnEnable()
        {
            if (lineMaterial == null)
            {
                lineMaterial = new Material(Shader.Find(DefaultLineShader));
                lineMaterial.SetColor(DefaultLineShaderColor, Color.white);
            }
        }

        private void Update()
        {
            if (isActiveAndEnabled)
            {
                if (!Source.enabled)
                {
                    lineRenderer.enabled = false;
                }
                else
                {
                    lineRenderer.enabled = true;
                    lineRenderer.positionCount = StepMode == StepMode.FromSource ? Source.PointCount : LineStepCount;

                    if (positions == null || positions.Length != lineRenderer.positionCount)
                    {
                        positions = new Vector3[lineRenderer.positionCount];
                    }

                    for (int i = 0; i < positions.Length; i++)
                    {
                        if (StepMode == StepMode.FromSource)
                        {
                            positions[i] = Source.GetPoint(i);
                        }
                        else
                        {
                            float normalizedDistance = (1f / (LineStepCount - 1)) * i;
                            positions[i] = Source.GetPoint(normalizedDistance);
                        }
                    }

                    // Set line renderer properties
                    lineRenderer.loop = Source.Loops;
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
                    lineRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                    lineRenderer.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
                    // Set positions
                    lineRenderer.positionCount = positions.Length;
                    lineRenderer.SetPositions(positions);
                }
            }
        }
    }
}
