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
        public Renderer[] Renderers;

        private int clipPlaneID;
        private MaterialPropertyBlock materialPropertyBlock;

        private void Awake()
        {
            Initialize();
            UpdatePlanePosition();
        }

#if UNITY_EDITOR
        private void Update()
        {
            Initialize();
#else
        private void LateUpdate()
        {
#endif
            UpdatePlanePosition();
        }

        private void UpdatePlanePosition()
        {
            Vector3 up = transform.up;
            Vector4 plane = new Vector4(up.x, up.y, up.z, Vector3.Dot(up, transform.position));

            foreach (Renderer renderer in Renderers)
            {
                if (!renderer)
                {
                    continue;
                }

                renderer.GetPropertyBlock(materialPropertyBlock);
                materialPropertyBlock.SetVector(clipPlaneID, plane);
                renderer.SetPropertyBlock(materialPropertyBlock);
            }
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

        private void Initialize()
        {
            clipPlaneID = Shader.PropertyToID("_ClipPlane");
            materialPropertyBlock = new MaterialPropertyBlock();
        }
    }
}
