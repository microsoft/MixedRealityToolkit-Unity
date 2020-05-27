// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities
{
    /// <summary>
    /// Draws a strip of polygons along the line
    /// </summary>
    [AddComponentMenu("Scripts/MRTK/Core/StripMeshLineRenderer")]
    public class StripMeshLineRenderer : BaseMixedRealityLineRenderer
    {
        [Header("Strip Mesh Settings")]

        [SerializeField]
        private Material lineMaterial = null;
        [SerializeField]
        private float uvOffset = 0f;

        [SerializeField]
        [HideInInspector]
        private MeshRenderer stripMeshRenderer;
        [SerializeField]
        [HideInInspector]
        private GameObject meshRendererGameObject;

        private Mesh stripMesh;
        private Material lineMatInstance;

        private readonly List<Vector3> positions = new List<Vector3>();
        private readonly List<Vector3> forwards = new List<Vector3>();
        private readonly List<Color> colors = new List<Color>();
        private readonly List<float> widths = new List<float>();

        private static Vector3[] stripMeshVertices = null;
        private static Color[] stripMeshColors = null;
        private static Vector2[] stripMeshUvs = null;
        private static int[] stripMeshTriangles = null;

        private void OnEnable()
        {
            if (lineMaterial == null)
            {
                Debug.LogError("LineDataProvider material cannot be null.");
                enabled = false;
                return;
            }

            lineMatInstance = new Material(lineMaterial);

            // Create a mesh
            if (stripMesh == null)
            {
                stripMesh = new Mesh();
            }

            if (stripMeshRenderer == null)
            {
                meshRendererGameObject = new GameObject("Strip Mesh Renderer");
                stripMeshRenderer = meshRendererGameObject.AddComponent<MeshRenderer>();
            }

            stripMeshRenderer.sharedMaterial = lineMatInstance;
            stripMeshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            stripMeshRenderer.receiveShadows = false;
            stripMeshRenderer.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;

            var stripMeshFilter = stripMeshRenderer.EnsureComponent<MeshFilter>();
            stripMeshFilter.sharedMesh = stripMesh;
        }

        private void OnDisable()
        {
            if (lineMatInstance != null)
            {
                if (Application.isEditor)
                {
                    DestroyImmediate(lineMatInstance);
                }
                else
                {
                    Destroy(lineMatInstance);
                }
            }

            if (meshRendererGameObject != null)
            {
                if (Application.isEditor)
                {
                    DestroyImmediate(meshRendererGameObject);
                }
                else
                {
                    Destroy(meshRendererGameObject);
                }
                stripMeshRenderer = null;
            }
        }

        /// <inheritdoc />
        protected override void UpdateLine()
        {
            if (stripMeshRenderer == null)
            {
                Debug.LogError("Strip mesh renderer has been destroyed - disabling");
                enabled = false;
            }

            if (!LineDataSource.enabled)
            {
                stripMeshRenderer.enabled = false;
                return;
            }

            stripMeshRenderer.enabled = true;
            positions.Clear();
            forwards.Clear();
            colors.Clear();
            widths.Clear();
            
            for (int i = 0; i <= LineStepCount; i++)
            {
                float normalizedDistance = GetNormalizedPointAlongLine(i);
                positions.Add(LineDataSource.GetPoint(normalizedDistance));
                colors.Add(GetColor(normalizedDistance));
                widths.Add(GetWidth(normalizedDistance));
                forwards.Add(LineDataSource.GetVelocity(normalizedDistance));
            }

            GenerateStripMesh(positions, colors, widths, uvOffset, forwards, stripMesh, LineDataSource.LineTransform.up);
        }

        public static void GenerateStripMesh(List<Vector3> positionList, List<Color> colorList, List<float> thicknessList, float uvOffsetLocal, List<Vector3> forwardList, Mesh mesh, Vector3 up)
        {
            int vertexCount = positionList.Count * 2;
            int colorCount = colorList.Count * 2;
            int uvCount = positionList.Count * 2;

            if (stripMeshVertices == null || stripMeshVertices.Length != vertexCount)
            {
                stripMeshVertices = new Vector3[vertexCount];
            }

            if (stripMeshColors == null || stripMeshColors.Length != colorCount)
            {
                stripMeshColors = new Color[colorCount];
            }

            if (stripMeshUvs == null || stripMeshUvs.Length != uvCount)
            {
                stripMeshUvs = new Vector2[uvCount];
            }

            for (int x = 0; x < positionList.Count; x++)
            {
                int index = (int)(x * 0.5f);
                Vector3 forward = forwardList[index];
                Vector3 right = Vector3.Cross(forward, up).normalized;
                float thickness = thicknessList[index] * 0.5f;
                stripMeshVertices[2 * x] = positionList[x] - right * thickness;
                stripMeshVertices[2 * x + 1] = positionList[x] + right * thickness;
                stripMeshColors[2 * x] = colorList[x];
                stripMeshColors[2 * x + 1] = colorList[x];

                float uv = uvOffsetLocal;

                if (x == positionList.Count - 1 && x > 1)
                {
                    float distLast = (positionList[x - 2] - positionList[x - 1]).magnitude;
                    float distCur = (positionList[x] - positionList[x - 1]).magnitude;
                    uv += 1 - distCur / distLast;
                }

                stripMeshUvs[2 * x] = new Vector2(0, x - uv);
                stripMeshUvs[2 * x + 1] = new Vector2(1, x - uv);
            }

            int numTriangles = ((positionList.Count * 2 - 2) * 3);

            if (stripMeshTriangles == null || stripMeshTriangles.Length != numTriangles)
            {
                stripMeshTriangles = new int[numTriangles];
            }

            int j = 0;

            for (int i = 0; i < positionList.Count * 2 - 3; i += 2, j++)
            {
                stripMeshTriangles[i * 3] = j * 2;
                stripMeshTriangles[i * 3 + 1] = j * 2 + 1;
                stripMeshTriangles[i * 3 + 2] = j * 2 + 2;

                stripMeshTriangles[i * 3 + 3] = j * 2 + 1;
                stripMeshTriangles[i * 3 + 4] = j * 2 + 3;
                stripMeshTriangles[i * 3 + 5] = j * 2 + 2;
            }

            mesh.Clear();
            mesh.vertices = stripMeshVertices;
            mesh.uv = stripMeshUvs;
            mesh.triangles = stripMeshTriangles;
            mesh.colors = stripMeshColors;
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();
        }
    }
}