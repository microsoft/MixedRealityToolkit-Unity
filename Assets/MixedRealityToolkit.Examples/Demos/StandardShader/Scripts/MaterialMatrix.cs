// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos.StandardShader
{
    /// <summary>
    /// Builds a matrix of spheres demonstrating a spectrum of two material properties.
    /// </summary>
    public class MaterialMatrix : MonoBehaviour
    {
        [SerializeField]
        private Material material = null;
        [SerializeField]
        private Mesh mesh = null;
        [SerializeField]
        [Range(2, 100)]
        private int dimension = 5;
        [SerializeField]
        [Range(0.0f, 10.0f)]
        private float positionOffset = 0.1f;
        [SerializeField]
        private string firstPropertyName = "_Metallic";
        [SerializeField]
        private string secondPropertyName = "_Glossiness";
        [SerializeField]
        private Vector3 localRotation = Vector3.zero;

        private void Awake()
        {
            BuildMatrix();
        }

        public void BuildMatrix()
        {
            List<Transform> children = transform.Cast<Transform>().ToList();

            for (int i = 0; i < children.Count; ++i)
            {
                Transform child = children[i];

                if (Application.isPlaying)
                {
                    Destroy(child.gameObject);
                }
                else
                {
                    DestroyImmediate(child.gameObject);
                }
            }

            if (material == null)
            {
                Debug.LogError("Failed to build material matrix due to missing material.");

                return;
            }

            float center = (dimension - 1) * positionOffset * -0.5f;
            Vector3 position = new Vector3(center, 0.0f, center);
            int firstPropertyId = Shader.PropertyToID(firstPropertyName);
            int secondPropertyId = Shader.PropertyToID(secondPropertyName);

            float firstProperty = 0.0f;
            float secondProperty = 0.0f;

            for (int i = 0; i < dimension; ++i)
            {
                for (int j = 0; j < dimension; ++j)
                {
                    GameObject element = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    element.name = "Element" + (i * dimension + j);
                    element.transform.parent = transform;
                    element.transform.localPosition = position;
                    element.transform.localRotation = Quaternion.Euler(localRotation);
                    position.x += positionOffset;

                    Material newMaterial = new Material(material);
                    newMaterial.SetFloat(firstPropertyId, firstProperty);
                    newMaterial.SetFloat(secondPropertyId, secondProperty);

                    Renderer _renderer = element.GetComponent<Renderer>();
                    MeshFilter meshFilter = element.GetComponent<MeshFilter>();

                    if (Application.isPlaying == true)
                    {
                        _renderer.material = newMaterial;

                        if (mesh != null)
                        {
                            meshFilter.mesh = mesh;
                            Destroy(element.GetComponent<SphereCollider>());
                            element.AddComponent<MeshCollider>();
                        }
                    }
                    else
                    {
                        _renderer.sharedMaterial = newMaterial;

                        if (mesh != null)
                        {
                            meshFilter.mesh = mesh;
                            DestroyImmediate(element.GetComponent<SphereCollider>());
                            element.AddComponent<MeshCollider>();
                        }
                    }

                    firstProperty += 1.0f / (dimension - 1);
                }

                position.x = center;
                position.z += positionOffset;

                firstProperty = 0.0f;
                secondProperty += 1.0f / (dimension - 1);
            }
        }
    }
}
