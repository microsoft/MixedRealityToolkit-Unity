// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using HoloToolkit.Unity.InputModule;

namespace HoloToolkit.Unity.Examples
{
    public class LoFiFilterSelection : MonoBehaviour, IInputClickHandler
    {
        [SerializeField]
        [Tooltip("Material used when the emitter is set to Narrow Band Telephony")]
        private Material narrowBandTelephony;

        [SerializeField]
        [Tooltip("Material used when the emitter is set to AM Radio")]
        private Material amRadio;

        [SerializeField]
        [Tooltip("Material used when the emitter is set to Full Range")]
        private Material fullRange;

        [SerializeField]
        [Tooltip("Material used when the emitter is set to an unknown quality")]
        private Material unknownQuality;

        private AudioLoFiEffect loFiEffect;

        private void Start()
        {
            // Get the attached AudioLoFiEffect script.
            loFiEffect = gameObject.GetComponent<AudioLoFiEffect>();
            if (loFiEffect == null)
            {
                Debug.LogError("LoFiFilterSelection requires an AudioLoFiEffect to be attached to the game object.");
            }

            // Set the material of the emitter object to match that of the
            // initial AudioLoFiEffect.SourceQuality value.
            SetEmitterMaterial(loFiEffect.SourceQuality);
        }

        /// <summary>
        /// Changes the AudioLoFiEffect source quality on click.
        /// </summary>
        /// <param name="data">Not used by this implementation.</param>
        public void OnInputClicked(InputClickedEventData data)
        {
            // Make sure we found an AudioLoFiEffect script.
            if (loFiEffect == null)
            {
                return;
            }

            // Get the current source quality setting.
            AudioLoFiSourceQuality sourceQuality = loFiEffect.SourceQuality;

            // Increment the source quality.
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

            // Update the emitter material to match the new source quality.
            SetEmitterMaterial(sourceQuality);

            // Update the source quality.
            loFiEffect.SourceQuality = sourceQuality;
        }

        private void SetEmitterMaterial(AudioLoFiSourceQuality sourceQuality)
        {
            Material emitterMaterial = unknownQuality;

            // Determine the material for the emitter based on the source quality.
            switch (sourceQuality)
            {
                case AudioLoFiSourceQuality.NarrowBandTelephony:
                    emitterMaterial = narrowBandTelephony;
                    break;

                case AudioLoFiSourceQuality.AmRadio:
                    emitterMaterial = amRadio;
                    break;

                case AudioLoFiSourceQuality.FullRange:
                    emitterMaterial = fullRange;
                    break;
            }

            // Set the material on the emitter.
            gameObject.GetComponent<Renderer>().sharedMaterial = emitterMaterial;
        }
    }
}
