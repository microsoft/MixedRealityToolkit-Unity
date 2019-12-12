// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// TODO: Troy - comment
    /// </summary>
    [AddComponentMenu("Scripts/MRTK/SDK/ShellHandRayPointer")]
    public class ShellHandRayPointer : LinePointer
    {
        [Header("Shell Pointer Settings")]

        [SerializeField]
        [Tooltip("Used when a focus target exists, or when select is pressed")]
        private Material lineMaterialSelected = null;

        [SerializeField]
        [Tooltip("Used when a no focus target exists and select is not pressed")]
        private Material lineMaterialNoTarget = null;

        [Header("Inertia Settings")]
        [SerializeField]
        private BezierInertia inertia;

        [Tooltip("Where to place the first control point of the bezier curve")]
        [SerializeField]
        [Range(0f, 0.5f)]
        private float startPointLerp = 0.33f;

        [SerializeField]
        [Tooltip("Where to place the second control point of the bezier curve")]
        [Range(0.5f, 1f)]
        private float endPointLerp = 0.66f;

        private bool wasSelectPressed = false;

        protected override void OnEnable()
        {
            base.OnEnable();

            inertia = gameObject.EnsureComponent<BezierInertia>();
        }

        /// <inheritdoc />
        public override void OnPostSceneQuery()
        {
            base.OnPostSceneQuery();

            if (!LineBase.enabled) 
            {
                return;
            }

            if (wasSelectPressed != IsSelectPressed)
            {
                wasSelectPressed = IsSelectPressed;

                var currentMaterial = IsSelectPressed ? lineMaterialSelected : lineMaterialNoTarget;

                for (int i = 0; i < LineRenderers.Length; i++)
                {
                    var lineRenderer = LineRenderers[i] as MixedRealityLineRenderer;
                    lineRenderer.LineMaterial = currentMaterial;

                    // TODO: Troy - lineRenderer.LineStepCount change?
                }
            }
        }

        protected override void PreUpdateLineRenderers()
        {
            base.PreUpdateLineRenderers();

            bool isFocusedLock = IsFocusLocked && IsTargetPositionLockedOnFocusLock;

            inertia.enabled = !isFocusedLock;

            if (isFocusedLock)
            {
                float distance = Result != null ? Result.Details.RayDistance : DefaultPointerExtent;
                Vector3 startPoint = LineBase.FirstPoint;

                // Project forward based on pointer direction to get an 'expected' position of the first control point
                Vector3 expectedPoint = startPoint + Rotation * Vector3.forward * distance;

                // Lerp between the expected position and the expected point
                LineBase.SetPoint(1, Vector3.Lerp(startPoint, expectedPoint, startPointLerp));

                // Get our next 'expected' position by lerping between the expected point and the end point
                // The result will be a line that starts moving in the pointer's direction then bends towards the target
                expectedPoint = Vector3.Lerp(expectedPoint, LineBase.LastPoint, endPointLerp);

                LineBase.SetPoint(2, Vector3.Lerp(startPoint, expectedPoint, endPointLerp));
            }
        }

    }
}