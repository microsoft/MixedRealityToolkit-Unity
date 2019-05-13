// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Service to record input into an internal buffer and export as a file when requested.
    /// </summary>
    [MixedRealityDataProvider(
        typeof(IMixedRealityInputSystem),
        (SupportedPlatforms)(-1), // Supported on all platforms
        "Input Recording Service",
        "Profiles/DefaultMixedRealityInputRecordingProfile.asset",
        "MixedRealityToolkit.SDK")]
    public class InputRecordingService :
        BaseInputDeviceManager,
        IMixedRealityInputRecordingService
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="registrar">The <see cref="IMixedRealityServiceRegistrar"/> instance that loaded the data provider.</param>
        /// <param name="inputSystem">The <see cref="Microsoft.MixedReality.Toolkit.Input.IMixedRealityInputSystem"/> instance that receives data from this provider.</param>
        /// <param name="inputSystemProfile">The input system configuration profile.</param>
        /// <param name="name">Friendly name of the service.</param>
        /// <param name="priority">Service priority. Used to determine order of instantiation.</param>
        /// <param name="profile">The service's configuration profile.</param>
        public InputRecordingService(
            IMixedRealityServiceRegistrar registrar,
            IMixedRealityInputSystem inputSystem,
            MixedRealityInputSystemProfile inputSystemProfile,
            string name = null,
            uint priority = DefaultPriority,
            BaseMixedRealityProfile profile = null) : base(registrar, inputSystem, inputSystemProfile, name, priority, profile) { }

        public bool IsRecording { get; private set; }

        private InputAnimation recordingBuffer = null;

        /// Size of the recording buffer
        private float recordingBufferLength = 30.0f;

        /// <inheritdoc />
        public float RecordingBufferLength
        {
            get { return recordingBufferLength; }
            set
            {
                recordingBufferLength = Mathf.Max(value, 0.0f);
                PruneBuffer();
            }
        }

        /// <inheritdoc />
        public void StartRecording()
        {
            IsRecording = true;
        }

        /// <inheritdoc />
        public void StopRecording()
        {
            IsRecording = false;
        }

        /// <inheritdoc />
        public void DiscardRecordedInput()
        {
            recordingBuffer.Clear();
        }

        /// <inheritdoc />
        public void ExportRecordedInput(Ray eyeRay, bool appendTimestamp = true)
        {
            // TODO
        }

        /// Discard keyframes before the cutoff time.
        private void PruneBuffer()
        {
            recordingBuffer.CutoffBeforeTime(Time.time - recordingBufferLength);
        }
    }
}
