using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities
{
    /// <summary>
    /// A line renderer that renders lines in a similar manner to MeshLineRenderer,
    /// but includes "Major" and "Minor" meshes, where the major and minor meshes
    /// are drawn at configurable intervals. MultiMeshLineRenderer can be used to
    /// render rulers, tickmarks, or other repeating patterns.
    /// </summary>
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
        [Tooltip("How many line steps to skip before a major mesh is drawn")]
        [Range(0, 20)]
        public int majorLineStepSkip = 0;

        [SerializeField]
        [Tooltip("How many line steps to skip before a minor mesh is drawn")]
        [Range(0, 20)]
        public int minorLineStepSkip = 0;

        private bool IsInitialized
        {
            get
            {
                if (lineMaterial != null && majorLineMesh != null)
                    return true;

                Debug.Assert(majorLineMesh != null, "Missing assigned line mesh.");
                Debug.Assert(lineMaterial != null, "Missing assigned line material.");
                return false;
            }
        }
        private List<Matrix4x4> majorMeshTransforms = new List<Matrix4x4>();
        private List<Matrix4x4> minorMeshTransforms = new List<Matrix4x4>();

        protected virtual void OnEnable()
        {
            if (!IsInitialized)
            {
                enabled = false;
                return;
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

                for (int i = 0; i < LineStepCount; i++)
                {
                    float normalizedDistance = GetNormalizedPointAlongLine(i);

                    if(i % majorLineStepSkip == 0)
                    {
                        majorMeshTransforms.Add(Matrix4x4.TRS(LineDataSource.GetPoint(normalizedDistance), LineDataSource.GetRotation(normalizedDistance), Vector3.one * GetWidth(normalizedDistance)));
                    } else if(i % minorLineStepSkip == 0)
                    {
                        minorMeshTransforms.Add(Matrix4x4.TRS(LineDataSource.GetPoint(normalizedDistance), LineDataSource.GetRotation(normalizedDistance), Vector3.one * GetWidth(normalizedDistance)));
                    }

                }

                Graphics.DrawMeshInstanced(majorLineMesh, 0, lineMaterial, majorMeshTransforms);

                if(minorLineMesh)
                {
                    Graphics.DrawMeshInstanced(minorLineMesh, 0, lineMaterial, minorMeshTransforms);
                }
                    
            }
        }
    }
}
    
