using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// The reticle visual for a ray interactor. This behavior takes care of
    /// aligning the reticle with a surface hit by the ray interactor.
    /// </summary>
    [AddComponentMenu("MRTK/Input/MRTK Ray Reticle Visual")]
    public class MRTKRayReticleVisual : BaseReticleVisual
    {
        [SerializeField]
        [Tooltip("The interactor which this visual represents.")]
        private XRRayInteractor rayInteractor;

        // reusable vectors for determining the raycast hit data
        private Vector3 reticlePosition;
        private Vector3 reticleNormal;
        private IVariableReticle variableReticle;

        [SerializeField]
        [Tooltip("Should a reticle appear on all surfaces hit by the interactor or interactables only?")]
        private ReticleVisibilitySettings visibilitySettings;

        /// <summary>
        /// Determines whether a reticle should appear on all surfaces hit by the interactor or interactables only
        /// </summary>
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

            variableReticle = reticle?.GetComponentInChildren<IVariableReticle>();
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

                // Additional controls to have the reticle update based on selectedness if it is an IVariableSelectReticle
                if (variableReticle != null)
                {
                    if(rayInteractor is IVariableSelectInteractor variableSelectInteractor)
                    {
                        variableReticle.UpdateVisuals(variableSelectInteractor.SelectProgress);
                    }
                    else
                    {
                        variableReticle.UpdateVisuals(rayInteractor.isSelectActive ? 1 : 0);
                    }
                }
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
