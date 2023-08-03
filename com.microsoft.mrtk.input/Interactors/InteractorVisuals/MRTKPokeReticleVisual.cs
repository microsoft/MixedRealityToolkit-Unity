// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Unity.Profiling;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// The reticle visual for a poke interactor. This behavior takes care of
    /// showing the reticle when the interactor is hovering over a target.
    /// </summary>
    public class MRTKPokeReticleVisual : BaseReticleVisual
    {
        [SerializeField]
        [Tooltip("The interactor which this visual represents.")]
        private PokeInteractor pokeInteractor;

        [SerializeField]
        [Tooltip("The GameObject which holds the proximity light for the reticle")]
        private GameObject proximityLight;

        /// <summary>
        /// A Unity event function that is called when the script component has been enabled.
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            Application.onBeforeRender += UpdateReticle;
        }

        /// <summary>
        /// A Unity event function that is called when the script component has been disabled.
        /// </summary>
        protected virtual void OnDisable()
        {
            UpdateReticle();
            Application.onBeforeRender -= UpdateReticle;
        }

        private static readonly ProfilerMarker UpdateReticlePerfMarker =
            new ProfilerMarker("[MRTK] MRTKPokeReticleVisual.UpdateReticle");

        [BeforeRenderOrder(XRInteractionUpdateOrder.k_BeforeRenderLineVisual)]
        private void UpdateReticle()
        {
            using (UpdateReticlePerfMarker.Auto())
            {
                if (Reticle != null)
                {
                    Reticle.SetActive(pokeInteractor.enabled && pokeInteractor.isHoverActive);

                    // TODO: Ideally we'd want the ReticleVisualScript to be responsible for all aspects of a visuals appearance.
                    // This is because it would allow us to freely use different reticle icons without changing other behavior.
                    // However, until the ReticleMagnetism class is revisited and potentially incorporated into this class, we'll
                    // leave all pose related responsibilities to the ReticleMagnetism component.
                    // reticle.transform.SetPositionAndRotation(pokeInteractor.PokeTrajectory.End, pokeInteractor.attachTransform.rotation);

                    // The proximity light should only be active when the reticle is
                    if (proximityLight != null)
                    {
                        proximityLight.SetActive(Reticle.activeSelf);
                    }
                }

                // If the reticle is an IReticleVisual, have the reticle update based on selectedness
                if (Visual != null)
                {
                    Visual.UpdateVisual(new ReticleVisualUpdateArgs(pokeInteractor, Reticle.transform.position, Reticle.transform.forward));
                }
            }
        }
    }
}
