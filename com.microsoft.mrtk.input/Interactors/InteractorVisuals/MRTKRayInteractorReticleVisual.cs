using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Microsoft.MixedReality.Toolkit.Input
{
    public class MRTKRayInteractorReticleVisual : MonoBehaviour, IXRCustomReticleProvider
    {
        [SerializeField]
        [Tooltip("The interactor which this visual represents.")]
        private XRRayInteractor rayInteractor;

        // reusable vectors for determining the raycast hit data
        private Vector3 reticlePosition;
        private Vector3 reticleNormal;

        [SerializeField]
        private GameObject baseReticle;

        // Staging area for custom reticles that interactors can attach to show unique visuals
        GameObject customReticle;
        bool customReticleAttached;

        public GameObject reticle => customReticleAttached ? customReticle : baseReticle;

        // Start is called before the first frame update
        void OnEnable()
        {
            Application.onBeforeRender += UpdateReticle;
            rayInteractor.selectEntered.AddListener(LocateTargetHitPoint);
        }

        protected void OnDisable()
        {
            UpdateReticle();
            Application.onBeforeRender -= UpdateReticle;
        }

        #region IXRCustomReticleProvider Implementation

        /// <inheritdoc />
        public bool AttachCustomReticle(GameObject reticleInstance)
        {
            if (!customReticleAttached)
            {
                if (baseReticle != null)
                {
                    baseReticle.SetActive(false);
                }
            }
            else
            {
                if (customReticle != null)
                {
                    customReticle.SetActive(false);
                }
            }

            customReticle = reticleInstance;
            if (customReticle != null)
            {
                customReticle.SetActive(true);
            }

            customReticleAttached = true;

            return true;
        }

        /// <inheritdoc />
        public bool RemoveCustomReticle()
        {
            if (customReticle != null)
            {
                customReticle.SetActive(false);
            }

            // If we have a standard reticle, re-enable that one.
            if (baseReticle != null)
            {
                baseReticle.SetActive(true);
            }

            customReticle = null;
            customReticleAttached = false;
            return false;
        }

        #endregion IXRCustomReticleProvider Implementation

        [BeforeRenderOrder(XRInteractionUpdateOrder.k_BeforeRenderLineVisual)]
        private void UpdateReticle()
        {
            bool showReticle = rayInteractor.hasHover || rayInteractor.hasSelection ||
                rayInteractor.enableUIInteraction && rayInteractor.TryGetCurrentUIRaycastResult(out _);

            if (showReticle)
            {
                if (rayInteractor.interactablesSelected.Count > 0)
                {
                    reticlePosition = hitTargetTransform.TransformPoint(targetLocalHitPoint);
                    reticleNormal = hitTargetTransform.TransformDirection(targetLocalHitNormal);
                    reticle.SetActive(true);
                }
                else
                {
                    bool rayHasHit = rayInteractor.TryGetHitInfo(out reticlePosition, out reticleNormal, out int _, out bool _);
                    reticle.SetActive(rayHasHit);
                }

                // Set the relevant reticle position/normal and ensure it's active.
                reticle.transform.position = reticlePosition;
                reticle.transform.forward = reticleNormal;
            }
            else
            {
                reticle.SetActive(false);
            }
        }

        private Vector3 targetLocalHitPoint;
        private Vector3 targetLocalHitNormal;
        private Transform hitTargetTransform;
        private void LocateTargetHitPoint(SelectEnterEventArgs args)
        {
            // If no hit, abort.
            if (!rayInteractor.TryGetCurrentRaycast(
                  out RaycastHit? raycastHit,
                  out _,
                  out UnityEngine.EventSystems.RaycastResult? raycastResult,
                  out _,
                  out bool isUIHitClosest))
            {
                return;
            }

            // Align the reticle with a UI hit if applicable
            if (raycastResult.HasValue && isUIHitClosest)
            {
                hitTargetTransform = raycastResult.Value.gameObject.transform;
                targetLocalHitPoint = hitTargetTransform.InverseTransformPoint(raycastResult.Value.worldPosition);
                targetLocalHitNormal = hitTargetTransform.InverseTransformDirection(raycastResult.Value.worldNormal);
            }
            // Otherwise, calcualte the reticle pose based on the raycast hit.
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
    }
}
