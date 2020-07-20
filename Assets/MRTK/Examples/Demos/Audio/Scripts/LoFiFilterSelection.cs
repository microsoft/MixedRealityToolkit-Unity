// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Audio;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos
{
    [RequireComponent(typeof(AudioLoFiEffect))]
    [AddComponentMenu("Scripts/MRTK/Examples/LoFiFilterSelection")]
    public class LoFiFilterSelection : MonoBehaviour, IMixedRealityPointerHandler
    {
        [Tooltip("Material used when the emitter is set to Narrow Band Telephony")]
        [SerializeField]
        private Material NarrowBandTelephony = null;

        [Tooltip("Material used when the emitter is set to AM Radio")]
        [SerializeField]
        private Material AmRadio = null;

        [Tooltip("Material used when the emitter is set to Full Range")]
        [SerializeField]
        private Material FullRange = null;

        [Tooltip("Material used when the emitter is set to an unknown quality")]
        [SerializeField]
        private Material UnknownQuality = null;

        private AudioLoFiEffect loFiEffect = null;
        // Component.renderer has been deprecated. It is safe to hide it and reuse the name.
        private Renderer objectRenderer = null;

        private void Start()
        {
            // Get the attached AudioLoFiEffect script.
            loFiEffect = gameObject.GetComponent<AudioLoFiEffect>();

            // Get the renderer.
            objectRenderer = gameObject.GetComponent<Renderer>();

            // Set the material of the emitter object to match that of the
            // initial AudioLoFiEffect.SourceQuality value.
            SetEmitterMaterial(loFiEffect.SourceQuality);
        }

        /// <summary>
        /// When the user clicks the pointer (select button) or air-taps on the object, 
        /// change the filter setting and the material.
        /// </summary>
        public void OnPointerClicked(MixedRealityPointerEventData eventData)
        {
            // Only proceed if the effect script is attached.
            if (loFiEffect == null) { return; }

            // Get the current source quality setting.
            AudioLoFiSourceQuality sourceQuality = loFiEffect.SourceQuality;

            // Select a new source quality setting.
            switch (sourceQuality)
            {
                case AudioLoFiSourceQuality.NarrowBandTelephony:
                    sourceQuality = AudioLoFiSourceQuality.AmRadio;
                    break;

                case AudioLoFiSourceQuality.AmRadio:
                    sourceQuality = AudioLoFiSourceQuality.FullRange;
                    break;

                case AudioLoFiSourceQuality.FullRange:
                    sourceQuality = AudioLoFiSourceQuality.NarrowBandTelephony;
                    break;
            }

            // Update the material to match the new source quality.
            SetEmitterMaterial(sourceQuality);

            // Update the source quality.
            loFiEffect.SourceQuality = sourceQuality;
        }

        /// <summary>
        /// This script does not handle pointer down events.
        /// </summary>
        public void OnPointerDown(MixedRealityPointerEventData eventData)
        { }

        /// <summary>
        /// This script does not handle pointer dragged events.
        /// </summary>
        public void OnPointerDragged(MixedRealityPointerEventData eventData) { }

        /// <summary>
        /// This script does not handle pointer up events.
        /// </summary>
        public void OnPointerUp(MixedRealityPointerEventData eventData)
        { }

        /// <summary>
        /// Sets the appropriate material based on the source quality setting.
        /// </summary>
        /// <param name="sourceQuality">The source quality used to determine the appropriate material.</param>
        private void SetEmitterMaterial(AudioLoFiSourceQuality sourceQuality)
        {
            Material emitterMaterial = UnknownQuality;

            // Determine the material for the emitter based on the source quality.
            switch (sourceQuality)
            {
                case AudioLoFiSourceQuality.NarrowBandTelephony:
                    emitterMaterial = NarrowBandTelephony;
                    break;

                case AudioLoFiSourceQuality.AmRadio:
                    emitterMaterial = AmRadio;
                    break;

                case AudioLoFiSourceQuality.FullRange:
                    emitterMaterial = FullRange;
                    break;
            }

            // Set the material on the emitter.
            objectRenderer.sharedMaterial = emitterMaterial;
        }
    }
}
