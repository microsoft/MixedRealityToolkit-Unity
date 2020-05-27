// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using Unity.Profiling;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Implementation for default hand ray pointers shipped with MRTK. Primarily used with hands and motion controllers
    /// </summary>
    [AddComponentMenu("Scripts/MRTK/SDK/ShellHandRayPointer")]
    public class ShellHandRayPointer : LinePointer
    {
        [Header("Shell Pointer Settings")]

        [SerializeField]
        [Tooltip("Used when a focus target exists, or when select is pressed")]
        private Material lineMaterialSelected = null;

        [SerializeField]
        [Tooltip("Used when no focus target exists and select is not pressed")]
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

        [Header("Obsolete Settings")]

        [SerializeField]
        [Obsolete("Use lineMaterialSelected instead.")]
        private BaseMixedRealityLineRenderer lineRendererSelected = null;

        [SerializeField]
        [Obsolete("Use lineMaterialNoTarget instead.")]
        private BaseMixedRealityLineRenderer lineRendererNoTarget = null;

        /// <inheritdoc />
        protected override void Start()
        {
#pragma warning disable 0618
            if (lineRendererSelected != null && lineMaterialSelected == null)
            {
                Debug.LogWarning("Property lineRendererSelected is obsolete. Use lineMaterialSelected instead.");

                if (lineRendererSelected is MixedRealityLineRenderer lineRenderer)
                {
                    lineMaterialSelected = lineRenderer.LineMaterial;
                }
            }

            if (lineRendererNoTarget != null && lineMaterialNoTarget == null)
            {
                Debug.LogWarning("Property lineRendererNoTarget is obsolete. Use lineMaterialNoTarget instead.");

                if (lineRendererNoTarget is MixedRealityLineRenderer lineRenderer)
                {
                    lineMaterialNoTarget = lineRenderer.LineMaterial;
                }
            }
#pragma warning restore 0618

            base.Start();
        }

        /// <inheritdoc />
        protected override void OnEnable()
        {
            base.OnEnable();

            inertia = gameObject.EnsureComponent<BezierInertia>();
        }

        private static readonly ProfilerMarker OnPostSceneQueryPerfMarker = new ProfilerMarker("[MRTK] ShellHandRayPointer.OnPostSceneQuery");

        /// <inheritdoc />
        public override void OnPostSceneQuery()
        {
            using (OnPostSceneQueryPerfMarker.Auto())
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
                    }
                }
            }
        }

        private static readonly ProfilerMarker PreUpdateLineRenderersPerfMarker = new ProfilerMarker("[MRTK] ShellHandRayPointer.PreUpdateLineRenderers");

        protected override void PreUpdateLineRenderers()
        {
            using (PreUpdateLineRenderersPerfMarker.Auto())
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
}
