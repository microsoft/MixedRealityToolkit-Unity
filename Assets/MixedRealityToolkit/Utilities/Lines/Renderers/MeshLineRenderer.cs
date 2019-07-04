// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities
{
    /// <summary>
    /// Creates instances of a mesh along the line
    /// </summary>
    public class MeshLineRenderer : BaseMixedRealityLineRenderer
    {
        [Header("Instanced Mesh Settings")]

        [SerializeField]
        private Mesh lineMesh = null;

        public Mesh LineMesh
        {
            get { return lineMesh; }
            set
            {
                enabled = false;
                lineMesh = value;
                enabled = true;
            }
        }

        [SerializeField]
        private Material lineMaterial = null;

        public Material LineMaterial
        {
            get { return lineMaterial; }
            set
            {
                enabled = false;
                lineMaterial = value;
                enabled = true;
            }
        }

        [SerializeField]
        private string colorProperty = "_Color";

        [SerializeField]
        [Tooltip("How many line steps to skip before a mesh is drawn")]
        [Range(0, 10)]
        private int lineStepSkip = 0;

        [SerializeField]
        private bool useVertexColors = true;

        public string ColorProperty
        {
            get { return colorProperty; }
            set
            {
                enabled = false;
                colorProperty = value;

                if (!lineMaterial.HasProperty(value))
                {
                    Debug.LogError($"Unable to find the property {value} for the line material");
                    return;
                }

                enabled = true;
            }
        }

        private bool IsInitialized
        {
            get
            {
                if (lineMaterial != null && lineMesh != null && lineMaterial.HasProperty(colorProperty))
                    return true;

                Debug.Assert(lineMesh != null, "Missing assigned line mesh.");
                Debug.Assert(lineMaterial != null, "Missing assigned line material.");
                Debug.Assert((lineMaterial != null && lineMaterial.HasProperty(colorProperty)), $"Unable to find the property \"{colorProperty}\" for the line material");
                return false;
            }
        }

        private int colorId;
        private List<Vector4> colorValues = new List<Vector4>();
        private List<Matrix4x4> meshTransforms = new List<Matrix4x4>();
        private MaterialPropertyBlock linePropertyBlock;

        protected virtual void OnEnable()
        {
            if (!IsInitialized)
            {
                enabled = false;
                return;
            }

            if (linePropertyBlock == null)
            {
                linePropertyBlock = new MaterialPropertyBlock();
            }

            lineMaterial.enableInstancing = true;
        }

        protected override void UpdateLine()
        {
            if (!Application.isPlaying)
            {   // This check is only necessary in edit mode.
                if (!IsInitialized)
                {
                    enabled = false;
                    return;
                }
            }

            if (LineDataSource.enabled)
            {
                meshTransforms.Clear();
                colorValues.Clear();
                linePropertyBlock.Clear();

                int skipCount = 0;

                for (int i = 0; i < LineStepCount; i++)
                {
                    if (lineStepSkip > 0)
                    {
                        skipCount++;
                        if (skipCount < lineStepSkip)
                            continue;

                        skipCount = 0;
                    }

                    float normalizedDistance = GetNormalizedPointAlongLine(i);
                    colorValues.Add(GetColor(normalizedDistance));
                    meshTransforms.Add(Matrix4x4.TRS(LineDataSource.GetPoint(normalizedDistance), LineDataSource.GetRotation(normalizedDistance), Vector3.one * GetWidth(normalizedDistance)));
                }

                if (useVertexColors)
                {
                    colorId = Shader.PropertyToID(colorProperty);
                    linePropertyBlock.SetVectorArray(colorId, colorValues);
                }

                Graphics.DrawMeshInstanced(lineMesh, 0, lineMaterial, meshTransforms, linePropertyBlock);
            }
        }
    }
}