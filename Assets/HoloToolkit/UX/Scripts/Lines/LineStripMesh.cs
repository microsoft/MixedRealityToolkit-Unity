// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using HoloToolkit.Unity;
using System.Collections.Generic;
using UnityEngine;

namespace MRTK.UX
{
    public class LineStripMesh : LineRendererBase
    {
        [Header("Strip Mesh Settings")]
        public Material LineMaterial;

        public Vector3 Forward;

        [SerializeField]
        private float uvOffset = 0f;

        private Mesh stripMesh;
        private MeshRenderer stripMeshRenderer;
        private Material lineMatInstance;
        private List<Vector3> positions = new List<Vector3>();
        private List<Vector3> forwards = new List<Vector3>();
        private List<Color> colors = new List<Color>();
        private List<float> widths = new List<float>();

        private GameObject meshRendererGameObject;

        protected override void OnEnable()
        {
            base.OnEnable();

            if (LineMaterial == null)
            {
                Debug.LogError("Line material cannot be null.");
                enabled = false;
                return;
            }

            lineMatInstance = new Material(LineMaterial);

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

            MeshFilter stripMeshFilter = stripMeshRenderer.GetComponent<MeshFilter>();
            if (stripMeshFilter == null)
                stripMeshFilter = stripMeshRenderer.gameObject.AddComponent<MeshFilter>();

            stripMeshFilter.sharedMesh = stripMesh;
        }

        private void OnDisable()
        {
            if (lineMatInstance != null)
                GameObject.Destroy(lineMatInstance);

            if (meshRendererGameObject != null)
            {
                GameObject.Destroy(meshRendererGameObject);
                stripMeshRenderer = null;
            }
        }

        public void Update()
        {
            if (!source.enabled)
            {
                stripMeshRenderer.enabled = false;
                return;
            }

            stripMeshRenderer.enabled = true;
            positions.Clear();
            forwards.Clear();
            colors.Clear();
            widths.Clear();

            for (int i = 0; i <= NumLineSteps; i++)
            {
                float normalizedDistance = (1f / (NumLineSteps - 1)) * i;
                positions.Add(source.GetPoint(normalizedDistance));
                colors.Add(GetColor(normalizedDistance));
                widths.Add(GetWidth(normalizedDistance));
                forwards.Add(source.GetRotation(normalizedDistance) * Vector3.down);
            }

            GenerateStripMesh(positions, colors, widths, uvOffset, forwards, stripMesh);
        }

        public static void GenerateStripMesh(List<Vector3> positionList, List<Color> colorList, List<float> thicknessList, float uvOffsetLocal, List<Vector3> forwardList, Mesh mesh)
        {
            // TODO store these as local variables to reduce allocations
            Vector3[] vertices = new Vector3[positionList.Count * 2];
            Color[] colors = new Color[colorList.Count * 2];
            Vector2[] uvs = new Vector2[positionList.Count * 2];

            for (int x = 0; x < positionList.Count; x++)
            {
                Vector3 forward = forwardList[x / 2];
                Vector3 right = Vector3.Cross(forward, Vector3.up).normalized;
                float thickness = thicknessList[x / 2] / 2;
                vertices[2 * x] = positionList[x] - right * thickness;
                vertices[2 * x + 1] = positionList[x] + right * thickness;
                colors[2 * x] = colorList[x];
                colors[2 * x + 1] = colorList[x];

                float uv = uvOffsetLocal;
                if (x == positionList.Count - 1 && x > 1)
                {
                    float dist_last = (positionList[x - 2] - positionList[x - 1]).magnitude;
                    float dist_cur = (positionList[x] - positionList[x - 1]).magnitude;
                    uv += 1 - dist_cur / dist_last;
                }

                uvs[2 * x] = new Vector2(0, x - uv);
                uvs[2 * x + 1] = new Vector2(1, x - uv);
            }

            int numTriangles = ((positionList.Count * 2 - 2) * 3);
            int[] triangles = new int[numTriangles];
            int j = 0;
            for (int i = 0; i < positionList.Count * 2 - 3; i += 2, j++)
            {
                triangles[i * 3] = j * 2;
                triangles[i * 3 + 1] = j * 2 + 1;
                triangles[i * 3 + 2] = j * 2 + 2;

                triangles[i * 3 + 3] = j * 2 + 1;
                triangles[i * 3 + 4] = j * 2 + 3;
                triangles[i * 3 + 5] = j * 2 + 2;
            }

            mesh.Clear();
            mesh.vertices = vertices;
            mesh.uv = uvs;
            mesh.triangles = triangles;
            mesh.colors = colors;
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();
        }

#if UNITY_EDITOR
        [UnityEditor.CustomEditor(typeof(LineStripMesh))]
        public class CustomEditor : MRTKEditor { }
#endif
    }
}