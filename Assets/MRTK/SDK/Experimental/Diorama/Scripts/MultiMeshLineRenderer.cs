using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities
{
    public class MultiMeshLineRenderer : BaseMixedRealityLineRenderer
    {
        [Header("Instanced Mesh Settings")]

        [SerializeField]
        private Mesh majorLineMesh = null;

        public Mesh MajorLineMesh
        {
            get { return majorLineMesh; }
            set
            {
                enabled = false;
                majorLineMesh = value;
                enabled = true;
            }
        }

        [SerializeField]
        private Mesh minorLineMesh = null;

        public Mesh MinorLineMesh
        {
            get { return minorLineMesh; }
            set
            {
                enabled = false;
                minorLineMesh = value;
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
        [Tooltip("How many line steps to skip before a major mesh is drawn")]
        [Range(0, 20)]
        public int majorLineStepSkip = 0;

        [SerializeField]
        [Tooltip("How many line steps to skip before a minor mesh is drawn")]
        [Range(0, 20)]
        public int minorLineStepSkip = 0;

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
                if (lineMaterial != null && majorLineMesh != null && lineMaterial.HasProperty(colorProperty))
                    return true;

                Debug.Assert(majorLineMesh != null, "Missing assigned line mesh.");
                Debug.Assert(lineMaterial != null, "Missing assigned line material.");
                Debug.Assert((lineMaterial != null && lineMaterial.HasProperty(colorProperty)), $"Unable to find the property \"{colorProperty}\" for the line material");
                return false;
            }
        }

        private int colorId;
        private List<Vector4> majorColorValues = new List<Vector4>();
        private List<Vector4> minorColorValues = new List<Vector4>();
        private List<Matrix4x4> majorMeshTransforms = new List<Matrix4x4>();
        private List<Matrix4x4> minorMeshTransforms = new List<Matrix4x4>();
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
                majorMeshTransforms.Clear();
                minorMeshTransforms.Clear();
                majorColorValues.Clear();
                minorColorValues.Clear();
                linePropertyBlock.Clear();

                for (int i = 0; i < LineStepCount; i++)
                {
                    float normalizedDistance = GetNormalizedPointAlongLine(i);


                    if(i % majorLineStepSkip == 0)
                    {
                        majorColorValues.Add(GetColor(normalizedDistance));
                        majorMeshTransforms.Add(Matrix4x4.TRS(LineDataSource.GetPoint(normalizedDistance), LineDataSource.GetRotation(normalizedDistance), Vector3.one * GetWidth(normalizedDistance)));
                    } else if(i % minorLineStepSkip == 0)
                    {
                        //minorColorValues.Add(GetColor(normalizedDistance));
                        minorMeshTransforms.Add(Matrix4x4.TRS(LineDataSource.GetPoint(normalizedDistance), LineDataSource.GetRotation(normalizedDistance), Vector3.one * GetWidth(normalizedDistance)));
                    }

                }

                if (useVertexColors)
                {
                    colorId = Shader.PropertyToID(colorProperty);
                    linePropertyBlock.SetVectorArray(colorId, majorColorValues);
                }
                Graphics.DrawMeshInstanced(majorLineMesh, 0, lineMaterial, majorMeshTransforms, linePropertyBlock);

                if(minorLineMesh)
                {
                    //if (useVertexColors)
                    //{
                    //    colorId = Shader.PropertyToID(colorProperty);
                    //    linePropertyBlock.Clear();
                    //    linePropertyBlock.SetVectorArray(colorId, minorColorValues);
                    //}
                    Graphics.DrawMeshInstanced(minorLineMesh, 0, lineMaterial, minorMeshTransforms, linePropertyBlock);
                }
                    
            }
        }
    }
}
    
