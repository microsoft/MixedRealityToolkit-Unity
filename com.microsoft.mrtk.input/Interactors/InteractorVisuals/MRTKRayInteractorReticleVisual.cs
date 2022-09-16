using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Microsoft.MixedReality.Toolkit.Input
{
    public class MRTKRayInteractorReticleVisual : MRTKBaseReticleVisual
    {
        [SerializeField]
        [Tooltip("The interactor which this visual represents.")]
        private XRRayInteractor rayInteractor;

        // reusable vectors for determining the raycast hit data
        private Vector3 reticlePosition;
        private Vector3 reticleNormal;

        [SerializeField]
        private GameObject baseReticle;

        [SerializeField]
        [Tooltip("Should a reticle appear on all surfaces or interactables only?")]
        private ReticleVisibilitySettings visibilitySettings;

        public ReticleVisibilitySettings VisibilitySettings
        {
            get
            {
                return visibilitySettings;
            }
            set
            {
                visibilitySettings = value;
            }
        }

        protected void OnEnable()
        {
            rayInteractor.selectEntered.AddListener(LocateTargetHitPoint);
            Application.onBeforeRender += UpdateReticle;
        }

        protected void OnDisable()
        {
            UpdateReticle();
            Application.onBeforeRender -= UpdateReticle;
        }

        [BeforeRenderOrder(XRInteractionUpdateOrder.k_BeforeRenderLineVisual)]
        private void UpdateReticle()
        {
            bool showReticle = VisibilitySettings == ReticleVisibilitySettings.AllValidSurfaces || rayInteractor.hasHover || rayInteractor.hasSelection ||
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

        public enum ReticleVisibilitySettings
        {
            InteractablesOnly,
            AllValidSurfaces
        }
    }
}
