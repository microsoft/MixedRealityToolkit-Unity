// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    public class SpherePointerGrabPoint : MonoBehaviour
    {
        [SerializeField]
        private SpherePointerVisual pointerVisual;
        [SerializeField]
        private Mesh grabPointMesh = null;
        [SerializeField]
        private Material grabPointMaterial = null;
        [SerializeField]
        private float scale = 1f;

        private Matrix4x4 pointMatrix;

        private void OnEnable()
        {
            if (pointerVisual == null)
            {
                pointerVisual = GetComponent<SpherePointerVisual>();
                if (pointerVisual == null)
                {
                    enabled = false;
                }
            }
        }

        private void LateUpdate()
        {
            if (pointerVisual.TetherVisualsEnabled)
            {
                pointMatrix = Matrix4x4.TRS(pointerVisual.TetherEndPoint.position, pointerVisual.TetherEndPoint.rotation, Vector3.one * scale);
                Graphics.DrawMesh(grabPointMesh, pointMatrix, grabPointMaterial, pointerVisual.gameObject.layer);
            }
        }
    }
}