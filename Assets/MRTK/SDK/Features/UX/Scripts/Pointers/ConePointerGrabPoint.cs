// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.ConePointerGrabPoint

using Unity.Profiling;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    [AddComponentMenu("Scripts/MRTK/SDK/ConePointerGrabPoint")]
    public class ConePointerGrabPoint : MonoBehaviour
    {
        [SerializeField]
        private ConePointerVisual pointerVisual;
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
                pointerVisual = GetComponent<ConePointerVisual>();
                if (pointerVisual == null)
                {
                    enabled = false;
                }
            }
        }

        private static readonly ProfilerMarker LateUpdatePerfMarker = new ProfilerMarker("[MRTK] ConePointerGrabPoint.LateUpdate");

        private void LateUpdate()
        {
            using (LateUpdatePerfMarker.Auto())
            {
                if (pointerVisual.TetherVisualsEnabled)
                {
                    pointMatrix = Matrix4x4.TRS(pointerVisual.TetherEndPoint.position, pointerVisual.TetherEndPoint.rotation, Vector3.one * scale);
                    Graphics.DrawMesh(grabPointMesh, pointMatrix, grabPointMaterial, pointerVisual.gameObject.layer);
                }
            }
        }
    }
}