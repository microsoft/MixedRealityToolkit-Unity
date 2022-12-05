// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit
{
    /// <summary>
    /// Automatically configure an audio source to use a mixer-based
    /// spatializer, such as the Microsoft Spatializer
    /// <see cref="https://github.com/microsoft/spatialaudio-unity"/>.
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class ConfigureSpaitializationSettings : MonoBehaviour
    {
        private void Start()
        {
            AudioSource audioSource = GetComponent<AudioSource>();

            audioSource.spatialize = true;
            audioSource.spatializePostEffects = true;
            audioSource.spatialBlend = 1f;

            audioSource.rolloffMode = AudioRolloffMode.Logarithmic;
            audioSource.minDistance = 1f;
            audioSource.maxDistance = 45f;

            MRTKProfile profile = MRTKProfile.Instance;
            if (profile != null)
            {
                audioSource.outputAudioMixerGroup = profile.SpatializationMixer;
            }
        }
    }
}
