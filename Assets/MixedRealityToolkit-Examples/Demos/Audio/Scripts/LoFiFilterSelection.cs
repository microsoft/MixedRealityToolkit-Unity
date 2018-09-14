// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.EventDatum.Input;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.InputSystem.Handlers;
using Microsoft.MixedReality.Toolkit.SDK.Audio.Influencers;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos
{
    [RequireComponent(typeof(AudioLoFiEffect))]
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
            AudioLoFiSourceQualityType sourceQuality = loFiEffect.SourceQuality;

            // Select a new source quality setting.
            switch (sourceQuality)
            {
                case AudioLoFiSourceQualityType.NarrowBandTelephony:
                    sourceQuality = AudioLoFiSourceQualityType.AmRadio;
                    break;

                case AudioLoFiSourceQualityType.AmRadio:
                    sourceQuality = AudioLoFiSourceQualityType.FullRange;
                    break;

                case AudioLoFiSourceQualityType.FullRange:
                    sourceQuality = AudioLoFiSourceQualityType.NarrowBandTelephony;
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
        /// This script does not handle pointer up events.
        /// </summary>
        public void OnPointerUp(MixedRealityPointerEventData eventData)
        { }

        /// <summary>
        /// Sets the appropriate material based on the source quality setting.
        /// </summary>
        /// <param name="sourceQuality">The source quality used to determine the appropriate material.</param>
        private void SetEmitterMaterial(AudioLoFiSourceQualityType sourceQuality)
        {
            Material emitterMaterial = UnknownQuality;

            // Determine the material for the emitter based on the source quality.
            switch (sourceQuality)
            {
                case AudioLoFiSourceQualityType.NarrowBandTelephony:
                    emitterMaterial = NarrowBandTelephony;
                    break;

                case AudioLoFiSourceQualityType.AmRadio:
                    emitterMaterial = AmRadio;
                    break;

                case AudioLoFiSourceQualityType.FullRange:
                    emitterMaterial = FullRange;
                    break;
            }

            // Set the material on the emitter.
            objectRenderer.sharedMaterial = emitterMaterial;
        }
    }
}
