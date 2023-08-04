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
    internal class RingReticle : MonoBehaviour, IReticleVisual, IVariableProgressReticle
    {
        [SerializeField]
        [Tooltip("The amount of smoothing to apply to the reticle's grow and shrink effect.")]
        private float animationSmoothing = 0.000001f;

        /// <summary>
        /// The amount of smoothing to apply to the reticle's grow and shrink effect.
        /// </summary>
        public float AnimationSmoothing
        {
            get => animationSmoothing;
            set => animationSmoothing = value;
        }

        [SerializeField]
        [Tooltip("Turn on or off the ring fade when the value is small.")]
        private bool fadeEnabled = false;

        /// <summary>
        /// Turn on or off the ring fade when the value is small.
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

        [SerializeField]
        [Tooltip("Turn on or off the handling of selection progress.")]
        private bool displaySelectionProgress = true;

        /// <summary>
        /// Turn on or off the handling of selection progress.
        /// </summary>
        public bool DisplaySelectionProgress
        {
            get => displaySelectionProgress;
            set => displaySelectionProgress = value;
        }

        private MaterialPropertyBlock propertyBlock;

        private float smoothedValue = 0.0f;

        private MeshRenderer reticleRenderer;

        private float previousNearFadeValue;

        /// <summary>
        /// A Unity event function that is called when an enabled script instance is being loaded.
        /// </summary>
        protected virtual void Awake()
        {
            propertyBlock = new MaterialPropertyBlock();

            if (TryGetComponent(out reticleRenderer))
            {
                // Cache the previous near fade value so we know what to restore to
                // if we want to re-enable fading after disabling it (through InitializeShaderFadeEnabled).
                previousNearFadeValue = reticleRenderer.material.GetFloat("_Fade_Near_Distance_");
            }
        }

        /// <summary>
        /// A Unity event function that is called when the script component has been enabled.
        /// </summary>
        protected void OnEnable()
        {
            InitializeFadeBehavior(fadeEnabled);

            // Set the initial size of the reticle.
            SetReticleShrink(0);
        }

        /// <inheritdoc />
        public void UpdateProgress(VariableProgressReticleUpdateArgs args)
        {
            UpdateReticleProgressVisual(args.Progress);
        }

        /// <inheritdoc />
        public void UpdateVisual(ReticleVisualUpdateArgs args)
        {
            if (displaySelectionProgress)
            {
                if (args.Interactor is IVariableSelectInteractor variableSelectInteractor)
                {
                    UpdateReticleProgressVisual(variableSelectInteractor.SelectProgress);
                }
                else if (args.Interactor is IXRSelectInteractor selectInteractor)
                {
                    UpdateReticleProgressVisual(selectInteractor.isSelectActive ? 1 : 0);
                }
            }
        }

        private void UpdateReticleProgressVisual(float progress)
        {
            if (reticleRenderer == null || Mathf.Approximately(progress, smoothedValue)) { return; }

            smoothedValue = Smoothing.SmoothTo(smoothedValue, progress, animationSmoothing, Time.deltaTime);
            SetReticleShrink(smoothedValue);
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
