// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView.Compositor
{
    public class MicrophoneInput : MonoBehaviour
    {
        private const float MaxOffset = 1f;
        private const int DefaultMicrophoneFreq = 44100;

        [SerializeField]
        [Range(0, MaxOffset)]
        private float additionalDelay = 0;

        [SerializeField]
        private AudioSource microphoneAudioSource = null;

        private AudioSample[] samples;
        private int writeIndex;
        private int lastOffset;
        private float tickTime;
        private bool isInitialized;

        private struct AudioSample
        {
            public double dspTime;
            public float[] data;
        }

        private void Awake()
        {
            Reset();
        }

        private void Reset()
        {
            samples = null;
            isInitialized = false;
            writeIndex = 0;
            lastOffset = 0;
            tickTime = 0;
        }

        public bool StartMicrophone()
        {
            if (microphoneAudioSource == null)
            {
                Debug.LogWarning("No AudioSource for microphone audio was specified");
                return false;
            }

            if (Microphone.devices.Length == 0)
            {
                Debug.LogWarning("No connected microphones detected");
                return false;
            }

            int minFreq, maxFreq, reqFreq;
            Microphone.GetDeviceCaps(Microphone.devices[0], out minFreq, out maxFreq);
            reqFreq = Mathf.Clamp(DefaultMicrophoneFreq, minFreq, maxFreq);
            microphoneAudioSource.clip = Microphone.Start(Microphone.devices[0], true, 1, reqFreq);
            microphoneAudioSource.loop = true;

            // don't start playing the AudioSource until we have some data (else we get a weird doubleling of audio)
            StartCoroutine(StartAudioSourceCoroutine());

            AudioConfiguration currentConfiguration = AudioSettings.GetConfiguration();
            tickTime = (float)currentConfiguration.dspBufferSize / currentConfiguration.sampleRate;

            isInitialized = true;

            return true;
        }

        private IEnumerator StartAudioSourceCoroutine()
        {
            yield return null;
            while (Microphone.GetPosition(Microphone.devices[0]) <= 0)
            {
                yield return null;
            }
            microphoneAudioSource.Play();
        }

        private void OnAudioFilterRead(float[] data, int channels)
        {
            // if we haven't initialized or the additioanl delay is lower than the frequency with which we
            // can process data, then just do nothing
            if (!isInitialized || additionalDelay < tickTime)
            {
                return;
            }

            if (samples == null)
            {
                // initialize here once we have the data length rather than trying to calculate it based on 
                // enums in the AudioConfiguration class
                CreateSampleStorage(data.Length);
            }

            // write the lastest data to the next spot in our ring buffer
            double now = AudioSettings.dspTime;
            samples[writeIndex].dspTime = now;
            Array.Copy(data, samples[writeIndex].data, data.Length);

            // look back in the buffer to find the data for the correct time in the past
            // use the offset from the pevious frame as a hint (-1 so we can recover after hitches)
            int offset = Math.Max(lastOffset - 1, 0);
            int readIndex = AdjustIndex(writeIndex, -offset);
            while (now - samples[readIndex].dspTime < additionalDelay && offset < samples.Length)
            {
                readIndex = AdjustIndex(writeIndex, -(++offset));
            }
            lastOffset = offset;

            // copy the old data into this frames data
            Array.Copy(samples[readIndex].data, data, data.Length);

            writeIndex = AdjustIndex(writeIndex, 1);
        }

        private int AdjustIndex(int index, int offset)
        {
            return (index + samples.Length + offset) % samples.Length;
        }

        private void CreateSampleStorage(int dataSize)
        {
            samples = new AudioSample[(int)(MaxOffset / tickTime) + 1];
            for (int i = 0; i < samples.Length; i++)
            {
                samples[i] = new AudioSample();
                samples[i].data = new float[dataSize];
            }
        }
    }
}