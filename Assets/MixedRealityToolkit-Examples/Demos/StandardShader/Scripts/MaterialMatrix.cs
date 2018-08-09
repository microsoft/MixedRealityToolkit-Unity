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
        [Range(2, 100)]
        private int dimension = 5;
        [SerializeField]
        [Range(0.0f, 10.0f)]
        private float positionOffset = 0.1f;
        [SerializeField]
        private string firstPropertyName = "_Metallic";
        [SerializeField]
        private string secondPropertyName = "_Glossiness";

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

            Vector3 position = Vector3.zero;
            int firstPropertyId = Shader.PropertyToID(firstPropertyName);
            int secondPropertyId = Shader.PropertyToID(secondPropertyName);

            float firstProperty = 0.0f;
            float secondProperty = 0.0f;

            for (int i = 0; i < dimension; ++i)
            {
                for (int j = 0; j < dimension; ++j)
                {
                    GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    sphere.name = "Sphere" + (i * dimension + j);
                    sphere.transform.parent = transform;
                    sphere.transform.localPosition = position;
                    position.x += 1.0f + positionOffset;

                    Material newMaterial = new Material(material);
                    newMaterial.SetFloat(firstPropertyId, firstProperty);
                    newMaterial.SetFloat(secondPropertyId, secondProperty);

                    Renderer renderer = sphere.GetComponent<Renderer>();

                    if (Application.isPlaying)
                    {
                        renderer.material = newMaterial;
                    }
                    else
                    {
                        renderer.sharedMaterial = newMaterial;
                    }

                    firstProperty += 1.0f / (dimension - 1);
                }

                position.x = 0.0f;
                position.z += 1.0f + positionOffset;

                firstProperty = 0.0f;
                secondProperty += 1.0f / (dimension - 1);
            }
        }
    }
}
