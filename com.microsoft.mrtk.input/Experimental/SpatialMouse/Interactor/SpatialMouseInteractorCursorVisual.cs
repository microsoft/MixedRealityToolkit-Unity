// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.XR.Interaction.Toolkit;

namespace Microsoft.MixedReality.Toolkit.Input
{
    public class SpatialMouseInteractorCursorVisual : BaseReticleVisual
    {
        [SerializeField, Experimental]
        [Tooltip("The ray interactor which this visual represents.")]
        private SpatialMouseInteractor mouseInteractor;


        [SerializeField]
        [Tooltip("The default distance of the reticle (cursor)")]
        private float defaultDistance = 2.0f;
        
        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected void OnEnable()
        {
            mouseInteractor.selectEntered.AddListener(LocateTargetHitPoint);

            Application.onBeforeRender += OnBeforeRenderCursor;
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected void OnDisable()
        {
            mouseInteractor.selectEntered.RemoveListener(LocateTargetHitPoint);

            Application.onBeforeRender -= OnBeforeRenderCursor;
        }

        private Vector3 targetLocalHitPoint;
        private Vector3 targetLocalHitNormal;
        private float hitDistance;
        private Transform hitTargetTransform;

        // reusable lists of the points returned by the XRRayInteractor
        Vector3[] rayPositions;
        int rayPositionsCount = -1;

        // reusable vectors for determining the raycast hit data
        private Vector3 reticlePosition;
        private Vector3 reticleNormal;
        private int endPositionInLine;

        public void LocateTargetHitPoint(SelectEnterEventArgs args)
        {
            // If no hit, abort.
            if (!mouseInteractor.TryGetCurrentRaycast(
                out RaycastHit? raycastHit,
                out _,
                out UnityEngine.EventSystems.RaycastResult? raycastResult,
                out _,
                out bool isUIHitClosest))
            {
                return;
            }

            // Record relevant data about the hit point.
            if (raycastResult.HasValue && isUIHitClosest)
            {
                hitTargetTransform = raycastResult.Value.gameObject.transform;
                targetLocalHitPoint = hitTargetTransform.InverseTransformPoint(raycastResult.Value.worldPosition);
                targetLocalHitNormal = hitTargetTransform.InverseTransformDirection(raycastResult.Value.worldNormal);
                hitDistance = (raycastResult.Value.worldPosition - rayPositions[0]).magnitude;
            }
            else if (raycastHit.HasValue)
            {
                // In the case of affordances/handles, we can stick the ray right on to the handle.
                if (args.interactableObject is ISnapInteractable snappable)
                {
                    hitTargetTransform = snappable.HandleTransform;
                    targetLocalHitPoint = Vector3.zero;
                    targetLocalHitNormal = Vector3.up;
                }
                else
                {
                    hitTargetTransform = raycastHit.Value.collider.transform;
                    targetLocalHitPoint = hitTargetTransform.InverseTransformPoint(raycastHit.Value.point);
                    targetLocalHitNormal = hitTargetTransform.InverseTransformPoint(raycastHit.Value.normal);
                }

                hitDistance = (raycastHit.Value.point - rayPositions[0]).magnitude;
            }
        }

        private void OnBeforeRenderCursor()
        {
            if (Reticle == null) { return; }

            // Hide the cursor if the mouse isn't in use
            if (!mouseInteractor.IsInUse)
            {
                UnityEngine.Debug.Log("MouseNotUsed");
                Reticle.SetActive(false);
                return;
            }
            UnityEngine.Debug.Log("MouseRendering");

            // Get all the line sample points
            if (!mouseInteractor.GetLinePoints(ref rayPositions, out rayPositionsCount))
            {
                return;
            }

            // Sanity check.
            if (rayPositions == null ||
                rayPositions.Length == 0 ||
                rayPositionsCount == 0 ||
                rayPositionsCount > rayPositions.Length)
            {
                return;
            }
            
            if (mouseInteractor.interactablesSelected.Count > 0)
            {
                reticlePosition = hitTargetTransform.TransformPoint(targetLocalHitPoint);
            }
            else if (!mouseInteractor.TryGetHitInfo(out reticlePosition, out reticleNormal, out endPositionInLine, out bool isValidTarget))
            {
                reticlePosition = mouseInteractor.rayOriginTransform.position + mouseInteractor.rayOriginTransform.forward;
            }

            // Mouse cursor should always face the user
            reticleNormal = -mouseInteractor.rayOriginTransform.forward;

            // Set the relevant reticle position/normal and ensure it's active.
            Reticle.transform.position = reticlePosition;
            Reticle.transform.forward = reticleNormal;

            // If the reticle is an IVariableSelectReticle, have the reticle update based on selectedness
            if (VariableReticle != null)
            {
                VariableReticle.UpdateVisuals(new VariableReticleUpdateArgs(mouseInteractor, reticlePosition, reticleNormal));
            }

            if (Reticle.activeSelf == false)
            {
                Reticle.SetActive(true);
            }
        }
    }
}
