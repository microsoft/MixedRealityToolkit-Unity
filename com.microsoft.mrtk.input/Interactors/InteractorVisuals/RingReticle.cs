// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// A ring-like reticle that expands/contracts.
    /// </summary>
    [AddComponentMenu("MRTK/Input/Ring Reticle")]
    internal class RingReticle : MonoBehaviour, IVariableReticle
    {
        [SerializeField]
        [Tooltip("The amount of smoothing to apply to the reticle's grow/shrink effect.")]
        private float animationSmoothing = 0.000001f;

        /// <summary>
        /// The amount of smoothing to apply to the reticle's grow/shrink effect.
        /// </summary>
        public float AnimationSmoothing
        {
            get => animationSmoothing;
            set => animationSmoothing = value;
        }

        [SerializeField]
        [Tooltip("Should the ring fade when the value is small?")]
        private bool fadeEnabled = false;

        /// <summary>
        /// Should the ring fade when the value is small?
        /// </summary>
        public bool FadeEnabled
        {
            get => fadeEnabled;
            set
            {
                fadeEnabled = value;
                InitializeFadeBehavior(fadeEnabled);
            }
        }

        private MaterialPropertyBlock propertyBlock;

        private float smoothedValue = 0.0f;

        private MeshRenderer reticleRenderer;

        private float previousNearFadeValue;

        void Awake()
        {
            propertyBlock = new MaterialPropertyBlock();

            if (TryGetComponent(out reticleRenderer))
            {
                // Cache the previous near fade value so we know what to restore to
                // if we want to re-enable fading after disabling it (through InitializeShaderFadeEnabled).
                previousNearFadeValue = reticleRenderer.material.GetFloat("_Fade_Near_Distance_");
            }
        }

        protected void OnEnable()
        {
            InitializeFadeBehavior(fadeEnabled);

            // Set the initial size of the reticle.
            SetReticleShrink(0);
        }

        /// <inheritdoc />
        public void UpdateVisuals(float value)
        {
            if (reticleRenderer == null || Mathf.Approximately(value, smoothedValue)) { return; }

            smoothedValue = Smoothing.SmoothTo(smoothedValue, value, animationSmoothing, Time.deltaTime);
            SetReticleShrink(smoothedValue);
        }

        /// Extracts values from VariableReticleArgs to call UpdateVisuals
        public void UpdateVisuals(VariableReticleUpdateArgs args)
        {
            if (args.Interactor is XRRayInteractor rayInteractor)
            {
                if (rayInteractor is IVariableSelectInteractor variableSelectInteractor)
                {
                    UpdateVisuals(variableSelectInteractor.SelectProgress);
                }
                else
                {
                    UpdateVisuals(rayInteractor.isSelectActive ? 1 : 0);
                }
            }
        }

        private void SetReticleShrink(float value)
        {
            reticleRenderer.GetPropertyBlock(propertyBlock);
            propertyBlock.SetFloat("_Proximity_Distance_", 1.0f - value);
            reticleRenderer.SetPropertyBlock(propertyBlock);
        }

        // Sets the shader to either use fading or not. If not,
        // it disables fading by setting the near fade value to 1.0f, which
        // makes the shader never fade (far fade always set to 1.0f).
        private void InitializeFadeBehavior(bool shouldFade)
        {
            if (reticleRenderer == null) { return; }

            reticleRenderer.GetPropertyBlock(propertyBlock);
            propertyBlock.SetFloat("_Fade_Near_Distance_", shouldFade ? previousNearFadeValue : 1.0f);
            reticleRenderer.SetPropertyBlock(propertyBlock);
        }
    }
}
