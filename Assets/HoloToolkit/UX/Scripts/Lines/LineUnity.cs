// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections;
using UnityEngine;

namespace HoloToolkit.Unity.UX
{
    [UseWith(typeof(LineBase))]
    public class LineUnity : LineRendererBase
    {
        const string DefaultLineShader = "Particles/Alpha Blended";
        const string DefaultLineShaderColor = "_TintColor";

        [Header("LineUnity Settings")]
        [Tooltip("The material to use for the Unity LineRenderer (will be auto-generated if null)")]
        public Material LineMaterial;

        public bool RoundedEdges = true;
        public bool RoundedCaps = true;

        [SerializeField]
        private UnityEngine.LineRenderer lineRenderer;

        private Vector3[] positions;

        protected void OnEnable()
        {
            // If we haven't specified a line renderer
            if (lineRenderer == null)
            {
                // Get or create one that's attached to this gameObject
                lineRenderer = gameObject.EnsureComponent<UnityEngine.LineRenderer>();
            }

            if (LineMaterial == null)
            {
                LineMaterial = new Material(Shader.Find(DefaultLineShader));
                LineMaterial.SetColor(DefaultLineShaderColor, Color.white);
            }

            StartCoroutine(UpdateLineUnity());
        }

        private IEnumerator UpdateLineUnity()
        {
            while (isActiveAndEnabled)
            {
                if (!source.enabled)
                {
                    lineRenderer.enabled = false;
                }
                else
                {
                    lineRenderer.enabled = true;

                    switch (StepMode)
                    {
                        case StepModeEnum.FromSource:
                            lineRenderer.positionCount = source.NumPoints;
                            if (positions == null || positions.Length != source.NumPoints)
                            {
                                positions = new Vector3[source.NumPoints];
                            }
                            for (int i = 0; i < positions.Length; i++)
                            {
                                positions[i] = source.GetPoint(i);
                            }
                            break;

                        case StepModeEnum.Interpolated:
                            lineRenderer.positionCount = NumLineSteps;
                            if (positions == null || positions.Length != NumLineSteps)
                            {
                                positions = new Vector3[NumLineSteps];
                            }
                            for (int i = 0; i < positions.Length; i++)
                            {
                                float normalizedDistance = (1f / (NumLineSteps - 1)) * i;
                                positions[i] = source.GetPoint(normalizedDistance);
                            }
                            break;
                    }

                    // Set line renderer properties
                    lineRenderer.loop = source.Loops;
                    lineRenderer.numCapVertices = RoundedCaps ? 8 : 0;
                    lineRenderer.numCornerVertices = RoundedEdges ? 8 : 0;
                    lineRenderer.useWorldSpace = true;
                    lineRenderer.startWidth = 1;
                    lineRenderer.endWidth = 1;
                    lineRenderer.startColor = Color.white;
                    lineRenderer.endColor = Color.white;
                    lineRenderer.sharedMaterial = LineMaterial;
                    lineRenderer.widthCurve = LineWidth;
                    lineRenderer.widthMultiplier = WidthMultiplier;
                    lineRenderer.colorGradient = LineColor;
                    lineRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                    lineRenderer.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
                    // Set positions
                    lineRenderer.positionCount = positions.Length;
                    lineRenderer.SetPositions(positions);
                }
                yield return null;
            }
        }

#if UNITY_EDITOR
        [UnityEditor.CustomEditor(typeof(LineUnity))]
        public class CustomEditor : MRTKEditor { }
#endif

    }
}
