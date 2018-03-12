// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using UnityEngine;

namespace MixedRealityToolkit.Common
{
    /// <summary>
    /// Utility component to animate and visualize a clipping plane that can be used with 
    /// the "MixedRealityToolkit/Standard" shader "_ClippingPlane" feature.
    /// </summary>
    [ExecuteInEditMode]
    public class ClipPlane : MonoBehaviour
    {
        [SerializeField]
        private Renderer[] renderers = null;

        private int clipPlaneID;
        private Material[] materials;
        private MaterialPropertyBlock materialPropertyBlock;

        private void OnEnable()
        {
            Initialize();
            UpdatePlanePosition();
            ToggleClippingPlane(true);
        }

        private void OnDisable()
        {
            UpdatePlanePosition();
            ToggleClippingPlane(false);
        }

#if UNITY_EDITOR
        private void Update()
        {
            if (Application.isPlaying)
            {
                return;
            }

            Initialize();
            UpdatePlanePosition();
        }
#endif

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
            if (materials != null)
            {
                foreach (Material material in materials)
                {
                    if (Application.isPlaying)
                    {
                        Destroy(material);
                    }
                }

                materials = null;
            }
        }

        private void Initialize()
        {
            clipPlaneID = Shader.PropertyToID("_ClipPlane");

            materials = new Material[renderers.Length];

            for (int i = 0; i < renderers.Length; ++i)
            {
                if (Application.isPlaying)
                {
                    materials[i] = renderers[i].material;
                }
                else
                {
                    materials[i] = renderers[i].sharedMaterial;
                }
            }

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

            foreach (Renderer renderer in renderers)
            {
                if (renderer == null)
                {
                    continue;
                }

                renderer.GetPropertyBlock(materialPropertyBlock);
                materialPropertyBlock.SetVector(clipPlaneID, plane);
                renderer.SetPropertyBlock(materialPropertyBlock);
            }
        }

        private void ToggleClippingPlane(bool isClippingPlaneOn)
        {
            if (materials == null)
            {
                return;
            }

            foreach (Material material in materials)
            {
                if (material == null)
                {
                    continue;
                }

                const string clippingPlaneKeyword = "_CLIPPING_PLANE";

                if (isClippingPlaneOn)
                {
                    if (!material.IsKeywordEnabled(clippingPlaneKeyword))
                    {
                        material.EnableKeyword(clippingPlaneKeyword);
                    }
                }
                else
                {
                    if (material.IsKeywordEnabled(clippingPlaneKeyword))
                    {
                        material.DisableKeyword(clippingPlaneKeyword);
                    }
                }
            }
        }
    }
}
