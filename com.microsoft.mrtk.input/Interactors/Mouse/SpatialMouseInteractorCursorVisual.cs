// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.XR.Interaction.Toolkit;

namespace Microsoft.MixedReality.Toolkit.Input
{
    public class SpatialMouseInteractorCursorVisual : MonoBehaviour, IXRCustomReticleProvider
    {
        [SerializeField]
        [Tooltip("The ray interactor which this visual represents.")]
        private SpatialMouseInteractor mouseInteractor;

        [SerializeField]
        [Tooltip("The reticle (cursor)")]
        private GameObject reticle;

        [SerializeField]
        [Tooltip("The default distance of the reticle (cursor)")]
        private float defaultDistance = 2.0f;

        /// <summary>
        /// The reticle (cursor).
        /// </summary>
        public GameObject Reticle
        {
            get => reticle;
            set
            {
                if (reticle != value)
                {
                    reticle = value;
                }
            }
        }

        // If an interactable requests a custom reticle, it'll be referenced here.
        private GameObject customReticle;
        
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

        /// <summary>
        /// Used internally to determine if the ray we are visualizing hit an object or not.
        /// </summary>
        private bool rayHasHit;

        // reusable lists of the points returned by the XRRayInteractor
        Vector3[] rayPositions;
        int rayPositionsCount = -1;

        // reusable vectors for determining the raycast hit data
        private Vector3 reticlePosition;
        private Vector3 reticleNormal;
        private float reticleDistance;
        private int endPositionInLine;

        protected void Start()
        {
            reticleDistance = defaultDistance;
        }

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

        #region IXRCustomReticleProvider Implementation

        /// <inheritdoc />
        public bool AttachCustomReticle(GameObject reticleInstance)
        {
            // If we don't already have a custom reticle,
            // disable our standard reticle.
            if (customReticle == null)
            {
                if (reticle != null)
                {
                    reticle.SetActive(false);
                }
            }
            else if (customReticle != null)
            {
                // Otherwise, disable our current custom reticle.
                customReticle.SetActive(false);
            }

            // Install the new custom reticle.
            customReticle = reticleInstance;
            if (customReticle != null)
            {
                customReticle.SetActive(true);
            }
            return false;
        }

        /// <inheritdoc />
        public bool RemoveCustomReticle()
        {
            if (customReticle != null)
            {
                customReticle.SetActive(false);
            }

            // If we have a standard reticle, re-enable that one.
            if (reticle != null)
            {
                reticle.SetActive(true);
            }

            customReticle = null;
            return false;
        }

        #endregion IXRCustomReticleProvider Implementation

        private void OnBeforeRenderCursor()
        {
            // Grab the reticle we're currently using
            GameObject reticleToUse = customReticle != null ? customReticle : reticle;

            if (reticleToUse == null) { return; }

            // Hide the cursor is the mouse isn't in use
            if (!mouseInteractor.IsInUse)
            {
                reticleToUse.SetActive(false);
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
            
            if (mouseInteractor.interactablesSelected.Count > 0)
            {
                reticlePosition = hitTargetTransform.TransformPoint(targetLocalHitPoint);
                reticleDistance = Vector3.Distance(mouseInteractor.rayOriginTransform.position, reticlePosition);
                rayHasHit = true;
            }
            else
            {
                if (mouseInteractor.TryGetHitInfo(out reticlePosition, out reticleNormal, out endPositionInLine, out bool isValidTarget))
                {
                    if (isValidTarget && endPositionInLine > 0 && endPositionInLine < rayPositionsCount)
                    {
                        reticleDistance = Vector3.Distance(mouseInteractor.rayOriginTransform.position, reticlePosition);
                        rayHasHit = true;
                    }
                    else
                    {
                        rayHasHit = false;
                    }
                }
                else
                {
                    reticlePosition = mouseInteractor.rayOriginTransform.position + mouseInteractor.rayOriginTransform.forward * reticleDistance;
                    rayHasHit = false;
                }
            }

            // Mouse cursor should always face the user
            reticleNormal = -mouseInteractor.rayOriginTransform.forward;

            // Set the relevant reticle position/normal and ensure it's active.
            reticleToUse.transform.position = reticlePosition;
            reticleToUse.transform.forward = reticleNormal;

            if (reticleToUse.activeSelf == false)
            {
                reticleToUse.SetActive(true);
            }
        }
    }
}
