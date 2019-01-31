// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Microsoft.MixedReality.Toolkit.Core.Utilities.Lines.Renderers
{
    /// <summary>
    /// Creates instances of a mesh along the line
    /// </summary>
    public class MeshLineRenderer : BaseMixedRealityLineRenderer
    {
        private const string InvisibleShaderName = "MixedRealityToolkit/InvisibleShader";

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
                Debug.Assert(lineMesh != null, "Missing assigned line mesh.");
                Debug.Assert(lineMaterial != null, "Missing assigned line material.");
                Debug.Assert(lineMaterial.HasProperty(colorProperty), $"Unable to find the property \"{colorProperty}\" for the line material");
                return lineMaterial != null && lineMesh != null && lineMaterial.HasProperty(colorProperty);
            }
        }

        #region Command buffer properties

        private readonly Dictionary<Camera, CommandBuffer> cameras = new Dictionary<Camera, CommandBuffer>();

        private int colorId;
        private Vector4[] colorValues;
        private Matrix4x4[] meshTransforms;
        private bool executeCommandBuffer = false;
        private MaterialPropertyBlock linePropertyBlock;

        #endregion

        #region OnWillRenderObject helpers

        private readonly Vector3[] meshVertices = new Vector3[3];

        [SerializeField]
        [HideInInspector]
        private Material renderedMaterial = null;

        [SerializeField]
        [HideInInspector]
        private Mesh renderedMesh = null;

        private MeshRenderer meshRenderer;

        #endregion OnWillRenderObject helpers

        private void OnValidate()
        {
            if (!IsInitialized)
            {
                enabled = false;
                return;
            }

            if (renderedMaterial == null)
            {
                // Create an 'invisible' material so the mesh doesn't show up pink
                renderedMaterial = new Material(Shader.Find(InvisibleShaderName));
            }

            if (renderedMesh == null)
            {
                // create a simple 1-triangle mesh to ensure OnWillRenderObject is always called.
                renderedMesh = new Mesh
                {
                    vertices = meshVertices,
                    triangles = new[] { 0, 1, 2 }
                };
            }
        }

        protected virtual void OnEnable()
        {
            if (!IsInitialized)
            {
                enabled = false;
                return;
            }

            if (linePropertyBlock == null)
            {
                lineMaterial.enableInstancing = true;
                linePropertyBlock = new MaterialPropertyBlock();
                colorId = Shader.PropertyToID(colorProperty);
            }

            if (meshRenderer == null)
            {
                // HACK: OnWillRenderObject won't be called unless there's a renderer attached and its bounds are visible.
                meshRenderer = gameObject.AddComponent<MeshRenderer>();
                meshRenderer.receiveShadows = false;
                meshRenderer.shadowCastingMode = ShadowCastingMode.Off;
                meshRenderer.lightProbeUsage = LightProbeUsage.Off;
                meshRenderer.motionVectorGenerationMode = MotionVectorGenerationMode.ForceNoMotion;
                meshRenderer.sharedMaterial = renderedMaterial;

                var meshFilter = gameObject.AddComponent<MeshFilter>();
                meshFilter.sharedMesh = renderedMesh;
            }
        }

        private void Update()
        {
            executeCommandBuffer = false;

            if (LineDataSource.enabled)
            {
                if (meshTransforms == null || meshTransforms.Length != LineStepCount)
                {
                    meshTransforms = new Matrix4x4[LineStepCount];
                }

                if (colorValues == null || colorValues.Length != LineStepCount)
                {
                    colorValues = new Vector4[LineStepCount];
                    linePropertyBlock.Clear();
                }

                for (int i = 0; i < LineStepCount; i++)
                {
                    float normalizedDistance = (1f / (LineStepCount - 1)) * i;
                    colorValues[i] = GetColor(normalizedDistance);
                    meshTransforms[i] = Matrix4x4.TRS(LineDataSource.GetPoint(normalizedDistance), LineDataSource.GetRotation(normalizedDistance), Vector3.one * GetWidth(normalizedDistance));
                }

                linePropertyBlock.SetVectorArray(colorId, colorValues);

                executeCommandBuffer = true;
            }
        }

        private void OnDisable()
        {
            foreach (KeyValuePair<Camera, CommandBuffer> bufferCamera in cameras)
            {
                if (bufferCamera.Key != null)
                {
                    bufferCamera.Key.RemoveCommandBuffer(CameraEvent.AfterForwardOpaque, bufferCamera.Value);
                }
            }

            cameras.Clear();
        }

        private void OnWillRenderObject()
        {
            Camera currentCamera = Camera.current;
            CommandBuffer buffer;

            if (!cameras.TryGetValue(currentCamera, out buffer))
            {
                buffer = new CommandBuffer { name = $"LineDataProvider Mesh Renderer {currentCamera.name}" };
                currentCamera.AddCommandBuffer(CameraEvent.AfterForwardOpaque, buffer);
                cameras.Add(currentCamera, buffer);
            }

            buffer.Clear();

            if (executeCommandBuffer)
            {
                buffer.DrawMeshInstanced(lineMesh, 0, lineMaterial, 0, meshTransforms, meshTransforms.Length, linePropertyBlock);
            }
        }

        private void LateUpdate()
        {
            // Update our helper mesh so OnWillRenderObject will be called
            meshVertices[0] = transform.InverseTransformPoint(LineDataSource.GetPoint(0.0f)); // - transform.position;
            meshVertices[1] = transform.InverseTransformPoint(LineDataSource.GetPoint(0.5f)); // - transform.position;
            meshVertices[2] = transform.InverseTransformPoint(LineDataSource.GetPoint(1.0f)); // - transform.position;
            renderedMesh.vertices = meshVertices;
            renderedMesh.RecalculateBounds();
        }
    }
}