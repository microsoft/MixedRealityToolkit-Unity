// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.IO;
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
            BaseMixedRealityProfile profile = null) : base(registrar, inputSystem, inputSystemProfile, name, priority, profile)
        {}

        /// <summary>
        /// Return the service profile and ensure that the type is correct.
        /// </summary>
        public MixedRealityInputRecordingProfile InputRecordingProfile
        {
            get
                {
                var profile = ConfigurationProfile as MixedRealityInputRecordingProfile;
                if (!profile)
                {
                    Debug.LogError("Profile for Input Recording Service must be a MixedRealityInputRecordingProfile");
                }
                return profile;
            }
        }

        /// <summary>
        /// Service has been enabled.
        /// </summary>
        public bool IsEnabled { get; private set; } = false;

        /// <inheritdoc />
        public bool IsRecording { get; private set; } = false;

        private bool useBufferTimeLimit = true;
        /// <inheritdoc />
        public bool UseBufferTimeLimit
        {
            get { return useBufferTimeLimit; }
            set
            {
                useBufferTimeLimit = value;
                if (useBufferTimeLimit)
                {
                    PruneBuffer();
                }
            }
        }

        private float recordingBufferTimeLimit = 30.0f;
        /// <inheritdoc />
        public float RecordingBufferTimeLimit
        {
            get { return recordingBufferTimeLimit; }
            set
            {
                recordingBufferTimeLimit = Mathf.Max(value, 0.0f);
                if (useBufferTimeLimit)
                {
                    PruneBuffer();
                }
            }
        }

        private InputAnimation recordingBuffer = null;

        /// <inheritdoc />
        public override void Enable()
        {
            IsEnabled = true;
            recordingBuffer = new InputAnimation();
        }

        /// <inheritdoc />
        public override void Disable()
        {
            IsEnabled = false;
            recordingBuffer = null;
        }

        /// <inheritdoc />
        public void StartRecording(bool useTimeLimit = false)
        {
            IsRecording = true;
            UseBufferTimeLimit = useTimeLimit;
        }

        /// <inheritdoc />
        public void StartRecording(float bufferTimeLimit)
        {
            IsRecording = true;
            UseBufferTimeLimit = true;
            RecordingBufferTimeLimit = bufferTimeLimit;
            PruneBuffer();
        }

        /// <inheritdoc />
        public void StopRecording()
        {
            IsRecording = false;
        }

        /// <inheritdoc />
        public override void LateUpdate()
        {
            if (IsEnabled)
            {
                if (IsRecording)
                {
                    if (UseBufferTimeLimit)
                    {
                        PruneBuffer();
                    }

                    InputAnimationRecordingUtils.RecordKeyframe(recordingBuffer, Time.time, InputRecordingProfile);
                }
            }
        }

        /// <inheritdoc />
        public void DiscardRecordedInput()
        {
            if (IsEnabled)
            {
                recordingBuffer.Clear();
            }
        }

        /// <inheritdoc />
        public void ExportRecordedInput()
        {
            if (IsEnabled)
            {
                var profile = InputRecordingProfile;
                string filename;
                if (profile.AppendTimestamp)
                {
                    filename = String.Format("{0}-{1}.{2}", profile.OutputFilename, DateTime.UtcNow.ToString("yyyyMMdd-HHmmss"), InputAnimationConverterUtils.Extension);
                }
                else
                {
                    filename = profile.OutputFilename;
                }

                ExportRecordedInput(filename);
            }
        }

        /// <inheritdoc />
        public void ExportRecordedInput(string filename)
        {
            if (IsEnabled)
            {
                string path = Path.Combine(Application.persistentDataPath, filename);

                try
                {
                    using (Stream fileStream = File.Open(path, FileMode.Create))
                    {
                        recordingBuffer.ToStream(fileStream);
                    }
                }
                catch (IOException ex)
                {
                    Debug.LogWarning(ex.Message);
                }
            }
        }

        /// Discard keyframes before the cutoff time.
        private void PruneBuffer()
        {
            recordingBuffer.CutoffBeforeTime(Time.time - RecordingBufferTimeLimit);
        }
    }
}
