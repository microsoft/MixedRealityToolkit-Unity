// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Internal.Utilities
{
    /// <summary>
    /// Utility component to animate and visualize a clipping plane that can be used with 
    /// the "MixedRealityToolkit/Standard" shader "_ClippingPlane" feature.
    /// </summary>
    [ExecuteInEditMode]
    public class ClipPlane : MonoBehaviour
    {
        [Tooltip("The renderer(s) that should be affected by the clip plane.")]
        [SerializeField]
        private Renderer[] renderers = null;

        /// <summary>
        /// The renderer(s) that should be affected by the clip plane.
        /// </summary>
        public Renderer[] Renderers => renderers;

        private int clipPlaneID;
        private MaterialPropertyBlock materialPropertyBlock;

        private const string clippingPlaneKeyword = "_CLIPPING_PLANE";
        private const string clippingPlaneProperty = "_ClippingPlane";
        private Dictionary<Material, bool> modifiedMaterials = new Dictionary<Material, bool>();
        private List<Material> allocatedMaterials = new List<Material>();

        private void OnValidate()
        {
            ToggleClippingPlanes(true);
        }

        private void OnEnable()
        {
            Initialize();
            UpdatePlanePosition();
            ToggleClippingPlanes(true);
        }

        private void OnDisable()
        {
            UpdatePlanePosition();
            ToggleClippingPlanes(false);
        }

        private void Update()
        {
            if (Application.isPlaying || !Application.isEditor)
            {
                return;
            }

            Initialize();
            UpdatePlanePosition();
        }

        private void LateUpdate()
        {
            UpdatePlanePosition();
        }

        private void OnDrawGizmosSelected()
        {
            if (!enabled)
            {
                return;
            }

            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawLine(Vector3.zero, Vector3.up * 0.5f);
            Gizmos.DrawWireCube(Vector3.zero, new Vector3(1.0f, 0.0f, 1.0f));
        }

        private void OnDestroy()
        {
            if (renderers == null)
            {
                return;
            }

            for (int i = 0; i < renderers.Length; ++i)
            {
                Renderer renderer = renderers[i];

                if (!renderer)
                {
                    continue;
                }

                Material material = GetMaterial(renderer);

                if (material)
                {
                    bool clippingPlaneOn;

                    if (modifiedMaterials.TryGetValue(material, out clippingPlaneOn))
                    {
                        ToggleClippingPlaneKeyword(material, clippingPlaneOn);
                    }
                }
            }

            for (int i = 0; i < allocatedMaterials.Count; ++i)
            {
                Destroy(allocatedMaterials[i]);
            }
        }

        private void Initialize()
        {
            clipPlaneID = Shader.PropertyToID("_ClipPlane");
            materialPropertyBlock = new MaterialPropertyBlock();
        }

        private void UpdatePlanePosition()
        {
            if (renderers == null)
            {
                return;
            }

            Vector3 up = transform.up;
            Vector4 plane = new Vector4(up.x, up.y, up.z, Vector3.Dot(up, transform.position));

            for (int i = 0; i < renderers.Length; ++i)
            {
                Renderer renderer = renderers[i];

                if (!renderer)
                {
                    continue;
                }

                renderer.GetPropertyBlock(materialPropertyBlock);
                materialPropertyBlock.SetVector(clipPlaneID, plane);
                renderer.SetPropertyBlock(materialPropertyBlock);
            }
        }

        private void ToggleClippingPlanes(bool clippingPlaneOn)
        {
            if (renderers == null)
            {
                return;
            }

            for (int i = 0; i < renderers.Length; ++i)
            {
                Renderer renderer = renderers[i];

                if (!renderer)
                {
                    continue;
                }

                Material material = GetMaterial(renderer);

                if (material)
                {
                    // Cache the initial keyword state of the material.
                    if (!modifiedMaterials.ContainsKey(material))
                    {
                        modifiedMaterials[material] = material.IsKeywordEnabled(clippingPlaneKeyword);
                    }

                    ToggleClippingPlaneKeyword(material, clippingPlaneOn);
                }
            }
        }

        private Material GetMaterial(Renderer renderer)
        {
            if (Application.isEditor && !Application.isPlaying)
            {
                return renderer.sharedMaterial;
            }
            else
            {
                Material material = renderer.material;

                if (!allocatedMaterials.Contains(material))
                {
                    allocatedMaterials.Add(material);
                }

                return material;
            }
        }

        private void ToggleClippingPlaneKeyword(Material material, bool clippingPlaneOn)
        {
            if (clippingPlaneOn)
            {
                material.EnableKeyword(clippingPlaneKeyword);
                material.SetFloat(clippingPlaneProperty, 1.0f);
            }
            else
            {
                material.DisableKeyword(clippingPlaneKeyword);
                material.SetFloat(clippingPlaneProperty, 0.0f);
            }
        }
    }
}
