// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Unity.Profiling;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using static Microsoft.MixedReality.Toolkit.Input.XRRayInteractorExtensions;

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
        /// Determines whether a reticle should appear on all surfaces hit by the interactor or interactables only.
        /// </summary>
        public ReticleVisibilitySettings VisibilitySettings
        {
            get => visibilitySettings;
            set => visibilitySettings = value;
        }

        /// <summary>
        /// A Unity event function that is called when the script component has been enabled.
        /// </summary>
        private void OnEnable()
        {
            rayInteractor.selectEntered.AddListener(LocateTargetHitPoint);

            // If no custom reticle root is specified, just use the interactor's transform.
            if (reticleRoot == null)
            {
                reticleRoot = transform;
            }
            UpdateReticle();
        }

        /// <summary>
        /// A Unity event function that is called when the script component has been disabled.
        /// </summary>
        private void OnDisable()
        {
            rayInteractor.selectEntered.RemoveListener(LocateTargetHitPoint);

            ReticleSetActive(false);
        }

        /// <summary>
        /// A Unity event function that is called every frame, if this object is enabled.
        /// </summary>
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
                            reticlePosition = selectedHitDetails.HitTargetTransform.TransformPoint(selectedHitDetails.TargetLocalHitPoint);
                            reticleNormal = selectedHitDetails.HitTargetTransform.TransformDirection(selectedHitDetails.TargetLocalHitNormal);
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
                            if (reticleNormal != Vector3.zero)
                            {
                                reticleRoot.transform.SetPositionAndRotation(reticlePosition, Quaternion.LookRotation(reticleNormal, Vector3.up));
                            }
                            else
                            {
                                reticleRoot.transform.position = reticlePosition;
                            }
                        }

                        // If the reticle is an IVariableSelectReticle, have the reticle update based on selectedness
                        if (VariableReticle != null)
                        {
                            VariableReticle.UpdateVisuals(new VariableReticleUpdateArgs(rayInteractor, reticlePosition, reticleNormal));
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

        private TargetHitDetails selectedHitDetails = new TargetHitDetails();

        /// <summary>
        /// Used to locate and lock the raycast hit data on a select
        /// </summary>
        private void LocateTargetHitPoint(SelectEnterEventArgs args)
        {
            rayInteractor.TryLocateTargetHitPoint(args.interactableObject, out selectedHitDetails);
        }

        /// <summary>
        /// An enumeration used to control when a <see cref="Microsoft.MixedReality.Toolkit.Input.MRTKRayReticleVisual">MRTKRayReticleVisual</see> 
        /// is visible. 
        /// </summary>
        public enum ReticleVisibilitySettings
        {
            /// <summary>
            /// The reticle is only visible when it intersects an interactable object.
            /// </summary> 
            InteractablesOnly,

            /// <summary>
            /// The reticle is visible anytime it intersects with another object, regardless of it being interactable.
            /// </summary>
            AllValidSurfaces
        }
    }
}
