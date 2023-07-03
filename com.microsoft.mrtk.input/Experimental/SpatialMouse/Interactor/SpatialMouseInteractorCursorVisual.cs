// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.XR.Interaction.Toolkit;

namespace Microsoft.MixedReality.Toolkit.Input.Experimental
{
    /// <summary>
    /// The cursor visual for a spatial mouse interactor. This behavior takes care of
    /// positioning the cursor and hiding it when the mouse is not in use. 
    /// </summary>
    /// <remarks>
    /// This is an experimental feature. This class is early in the cycle, it has 
    /// been labeled as experimental to indicate that it is still evolving, and 
    /// subject to change over time. Parts of the MRTK, such as this class, appear 
    /// to have a lot of value even if the details haven’t fully been fleshed out. 
    /// For these types of features, we want the community to see them and get 
    /// value out of them early enough so to provide feedback. 
    /// </remarks>
    public class SpatialMouseInteractorCursorVisual : BaseReticleVisual
    {
        /// <summary>
        /// The ray interactor which this visual represents.
        /// </summary>
        [field: SerializeField, Experimental, Tooltip("The ray interactor which this visual represents.")]
        public SpatialMouseInteractor mouseInteractor;


        [SerializeField]
        [Tooltip("The default distance of the reticle (cursor)")]
        private float defaultDistance = 1f;
        
        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected virtual void OnEnable()
        {
            mouseInteractor.selectEntered.AddListener(LocateTargetHitPoint);

            Application.onBeforeRender += OnBeforeRenderCursor;
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected virtual void OnDisable()
        {
            mouseInteractor.selectEntered.RemoveListener(LocateTargetHitPoint);

            Application.onBeforeRender -= OnBeforeRenderCursor;
        }

        private Vector3 targetLocalHitPoint;
        private Vector3 targetLocalHitNormal;
        private Transform hitTargetTransform;

        // reusable lists of the points returned by the XRRayInteractor
        Vector3[] rayPositions;
        int rayPositionsCount = -1;

        // reusable vectors for determining the raycast hit data
        private Vector3 reticlePosition;
        private Vector3 reticleNormal;
        private int endPositionInLine;

        private void LocateTargetHitPoint(SelectEnterEventArgs args)
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

            // Sanity check.
            if (rayPositions == null ||
                rayPositions.Length == 0 ||
                rayPositionsCount == 0 ||
                rayPositionsCount > rayPositions.Length)
            {
                return;
            }

            // Record relevant data about the hit point.
            if (raycastResult.HasValue && isUIHitClosest)
            {
                hitTargetTransform = raycastResult.Value.gameObject.transform;
                targetLocalHitPoint = hitTargetTransform.InverseTransformPoint(raycastResult.Value.worldPosition);
                targetLocalHitNormal = hitTargetTransform.InverseTransformDirection(raycastResult.Value.worldNormal);
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
            }
        }

        private void OnBeforeRenderCursor()
        {
            if (Reticle == null) { return; }

            // Hide the cursor if the mouse isn't in use
            if (!mouseInteractor.IsInUse)
            {
                Reticle.SetActive(false);
                return;
            }

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

            // If the mouse is selecting an interactable, then position the cursor based on the target transform
            if (mouseInteractor.interactablesSelected.Count > 0)
            {
                reticlePosition = hitTargetTransform.TransformPoint(targetLocalHitPoint);
            }
            // otherwise, try getting reticlePosition from the ray hit or set it a default distance from the user
            else if (!mouseInteractor.TryGetHitInfo(out reticlePosition, out reticleNormal, out endPositionInLine, out bool isValidTarget))
            {
                reticlePosition = mouseInteractor.rayOriginTransform.position + mouseInteractor.rayOriginTransform.forward * defaultDistance;
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
