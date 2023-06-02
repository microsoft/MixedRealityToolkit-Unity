// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
#if SPATIAL_PRESENT
using Microsoft.MixedReality.Toolkit.SpatialManipulation;
#endif

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// The reticle visual for a ray interactor. This behavior takes care of
    /// aligning the reticle with a surface hit by the ray interactor.
    /// </summary>
    [AddComponentMenu("MRTK/Input/MRTK Ray Reticle Visual")]
    [DefaultExecutionOrder(XRInteractionUpdateOrder.k_BeforeRenderLineVisual)]
    public class MRTKRayReticleVisual : BaseReticleVisual
    {
        [SerializeField]
        [Tooltip("The root of the reticle visuals")]
        private Transform reticleRoot;

        [SerializeField]
        [Tooltip("The interactor which this visual represents.")]
        private XRRayInteractor rayInteractor;

        [SerializeField]
        [Tooltip("The GameObject which holds the proximity light for the reticle")]
        private GameObject proximityLight;

        [SerializeField]
        [Tooltip("Should a reticle appear on all surfaces hit by the interactor or interactables only?")]
        private ReticleVisibilitySettings visibilitySettings;

        // reusable vectors for determining the raycast hit data
        private Vector3 reticlePosition;
        private Vector3 reticleNormal;

        /// <summary>
        /// Determines whether a reticle should appear on all surfaces hit by the interactor or interactables only
        /// </summary>
        public ReticleVisibilitySettings VisibilitySettings
        {
            get => visibilitySettings;
            set => visibilitySettings = value;
        }

        protected void OnEnable()
        {
            rayInteractor.selectEntered.AddListener(LocateTargetHitPoint);
            rayInteractor.hoverEntered.AddListener(LocateTargetHoverPoint);

            // If no custom reticle root is specified, just use the interactor's transform.
            if (reticleRoot == null)
            {
                reticleRoot = transform;
            }
            UpdateReticle();
        }

        protected void OnDisable()
        {
            rayInteractor.selectEntered.RemoveListener(LocateTargetHitPoint);
            rayInteractor.hoverEntered.RemoveListener(LocateTargetHoverPoint);

            ReticleSetActive(false);
        }

        private void Update()
        {
            UpdateReticle();
        }

        private static readonly ProfilerMarker UpdateReticlePerfMarker = new ProfilerMarker("[MRTK] MRTKRayReticleVisual.UpdateReticle");

        private void UpdateReticle()
        {
            using (UpdateReticlePerfMarker.Auto())
            {
                if (Reticle != null)
                {
                    bool showReticle = VisibilitySettings == ReticleVisibilitySettings.AllValidSurfaces || rayInteractor.hasHover || rayInteractor.hasSelection ||
                        rayInteractor.enableUIInteraction && rayInteractor.TryGetCurrentUIRaycastResult(out _);

                    if (showReticle)
                    {
                        if (rayInteractor.interactablesSelected.Count > 0)
                        {
                            reticlePosition = hitTargetTransform.TransformPoint(targetLocalHitPoint);
                            reticleNormal = hitTargetTransform.TransformDirection(targetLocalHitNormal);
                            ReticleSetActive(true);
                        }
                        else
                        {
                            bool rayHasHit = rayInteractor.TryGetHitInfo(out reticlePosition, out reticleNormal, out int _, out bool _);
                            ReticleSetActive(rayHasHit);
                        }

                        // If we have a reticle, set its position and rotation.
                        if (reticleRoot != null)
                        {
                            reticleRoot.transform.SetPositionAndRotation(reticlePosition, Quaternion.LookRotation(reticleNormal, Vector3.up));
                        }

                        // If the reticle is an IVariableSelectReticle, have the reticle update based on selectedness
                        if (VariableReticle != null)
                        {
                            if (rayInteractor is IVariableSelectInteractor variableSelectInteractor)
                            {
                                VariableReticle.UpdateVisuals(variableSelectInteractor.SelectProgress);
                            }
                            else
                            {
                                VariableReticle.UpdateVisuals(rayInteractor.isSelectActive ? 1 : 0);
                            }
                        }

                        if (customReticle != null)
                        {
#if SPATIAL_PRESENT
                            SpatialManipulationReticle customVariableReticle = Reticle.GetComponent<SpatialManipulationReticle>();
                            if (customVariableReticle)
                            {
                                if (rayInteractor.interactablesSelected.Count > 0)
                                {
                                    customReticle.transform.position = reticlePosition;
                                    customVariableReticle.UpdateRotation();
                                }
                                else if (rayInteractor.interactablesHovered.Count > 0)
                                {
                                    customReticle.transform.SetPositionAndRotation(reticlePosition, Quaternion.LookRotation(reticleNormal, Vector3.up));
                                    customVariableReticle.UpdateReticle(reticleNormal, hoverTargetTransform);
                                }
                            }
                            else
                            {
                                customReticle.transform.SetPositionAndRotation(reticlePosition, Quaternion.LookRotation(reticleNormal, Vector3.up));
                            }
#endif
                        }
                    }
                    else
                    {
                        ReticleSetActive(false);
                    }
                }
            }
        }

        private void ReticleSetActive(bool value)
        {
            Reticle.SetActive(value);

            // The proximity light should only be active when the reticle is
            if (proximityLight != null)
            {
                proximityLight.SetActive(value);
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
            // Otherwise, calculate the reticle pose based on the raycast hit.
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
                    targetLocalHitNormal = hitTargetTransform.InverseTransformDirection(raycastHit.Value.normal);
                }
            }
        }

        private Transform hoverTargetTransform;
        private void LocateTargetHoverPoint(HoverEnterEventArgs args)
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
                hoverTargetTransform = raycastResult.Value.gameObject.transform;
            }
            // Otherwise, calculate the reticle pose based on the raycast hit.
            else if (raycastHit.HasValue)
            {
                // In the case of affordances/handles, we can stick the ray right on to the handle.
                if (args.interactableObject is ISnapInteractable snappable)
                {
                    hoverTargetTransform = snappable.HandleTransform;
                }
                else
                {
                    hoverTargetTransform = raycastHit.Value.collider.transform;
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
