// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.IO;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Provides input recording into an internal buffer and exporting to files.
    /// </summary>
    [MixedRealityDataProvider(
        typeof(IMixedRealityInputSystem),
        (SupportedPlatforms)(-1), // Supported on all platforms
        "Input Recording Service",
        "Profiles/DefaultMixedRealityInputRecordingProfile.asset",
        "MixedRealityToolkit.SDK",
        true)]
    public class InputRecordingService :
        BaseInputDeviceManager,
        IMixedRealityInputRecordingService
    {
        private static readonly int jointCount = Enum.GetNames(typeof(TrackedHandJoint)).Length;

        public event Action OnRecordingStarted;
        public event Action OnRecordingStopped;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="registrar">The <see cref="IMixedRealityServiceRegistrar"/> instance that loaded the data provider.</param>
        /// <param name="inputSystem">The <see cref="Microsoft.MixedReality.Toolkit.Input.IMixedRealityInputSystem"/> instance that receives data from this provider.</param>
        /// <param name="name">Friendly name of the service.</param>
        /// <param name="priority">Service priority. Used to determine order of instantiation.</param>
        /// <param name="profile">The service's configuration profile.</param>
        [Obsolete("This constructor is obsolete (registrar parameter is no longer required) and will be removed in a future version of the Microsoft Mixed Reality Toolkit.")]
        public InputRecordingService(
            IMixedRealityServiceRegistrar registrar,
            IMixedRealityInputSystem inputSystem,
            string name = null,
            uint priority = DefaultPriority,
            BaseMixedRealityProfile profile = null) : this(inputSystem, name, priority, profile)
        {
            Registrar = registrar;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="inputSystem">The <see cref="Microsoft.MixedReality.Toolkit.Input.IMixedRealityInputSystem"/> instance that receives data from this provider.</param>
        /// <param name="name">Friendly name of the service.</param>
        /// <param name="priority">Service priority. Used to determine order of instantiation.</param>
        /// <param name="profile">The service's configuration profile.</param>
        public InputRecordingService(
            IMixedRealityInputSystem inputSystem,
            string name = null,
            uint priority = DefaultPriority,
            BaseMixedRealityProfile profile = null) : base(inputSystem, name, priority, profile)
        { }

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
                if (useBufferTimeLimit && !value)
                {
                    // Start at buffer limit when making buffer unlimited
                    unlimitedRecordingStartTime = StartTime;
                }

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

        // Start time of recording if buffer is unlimited.
        // Nullable to determine when time needs to be reset.
        private float? unlimitedRecordingStartTime = null;
        public float StartTime
        {
            get
            {
                if (unlimitedRecordingStartTime.HasValue)
                {
                    if (useBufferTimeLimit)
                    {
                        return Mathf.Max(unlimitedRecordingStartTime.Value, Time.time - recordingBufferTimeLimit);
                    }
                    else
                    {
                        return unlimitedRecordingStartTime.Value;
                    }
                }
                return Time.time;
            }
        }

        private void ResetStartTime()
        {
            if (IsRecording)
            {
                unlimitedRecordingStartTime = Time.time;
            }
            else
            {
                unlimitedRecordingStartTime = null;
            }
        }

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
            ResetStartTime();
        }

        /// <inheritdoc />
        public void StartRecording()
        {
            IsRecording = true;
            if (UseBufferTimeLimit)
            {
                PruneBuffer();
            }
            if (!unlimitedRecordingStartTime.HasValue)
            {
                unlimitedRecordingStartTime = Time.time;
            }

            OnRecordingStarted?.Invoke();
        }

        /// <inheritdoc />
        public void StopRecording()
        {
            IsRecording = false;

            OnRecordingStopped?.Invoke();
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

                    RecordKeyframe();
                }
            }
        }

        /// <inheritdoc />
        public void DiscardRecordedInput()
        {
            if (IsEnabled)
            {
                recordingBuffer.Clear();
                ResetStartTime();
            }
        }

        /// <summary>
        /// Record a keyframe at the given time for the main camera and tracked input devices.
        /// </summary>
        private void RecordKeyframe()
        {
            float time = Time.time;
            var profile = InputRecordingProfile;

            RecordInputHandData(Handedness.Left);
            RecordInputHandData(Handedness.Right);
            if (CameraCache.Main)
            {
                var cameraPose = new MixedRealityPose(CameraCache.Main.transform.position, CameraCache.Main.transform.rotation);
                recordingBuffer.AddCameraPoseKey(time, cameraPose, profile.CameraPositionThreshold, profile.CameraRotationThreshold);
            }
        }

        /// <summary>
        /// Record a keyframe at the given time for a hand with the given handedness it is tracked.
        /// </summary>
        private bool RecordInputHandData(Handedness handedness)
        {
            float time = Time.time;
            var profile = InputRecordingProfile;

            var hand = HandJointUtils.FindHand(handedness);
            if (hand == null)
            {
                recordingBuffer.AddHandStateKey(time, handedness, false, false);
                return false;
            }

            bool isTracked = (hand.TrackingState == TrackingState.Tracked);

            // Extract extra information from current interactions
            bool isPinching = false;
            for (int i = 0; i < hand.Interactions?.Length; i++)
            {
                var interaction = hand.Interactions[i];
                switch (interaction.InputType)
                {
                    case DeviceInputType.Select:
                        isPinching = interaction.BoolData;
                        break;
                }
            }

            recordingBuffer.AddHandStateKey(time, handedness, isTracked, isPinching);

            if (isTracked)
            {
                for (int i = 0; i < jointCount; ++i)
                {
                    if (hand.TryGetJoint((TrackedHandJoint)i, out MixedRealityPose jointPose))
                    {
                        recordingBuffer.AddHandJointKey(time, handedness, (TrackedHandJoint)i, jointPose, profile.JointPositionThreshold, profile.JointRotationThreshold);
                    }
                }
            }

            return true;
        }

        /// <inheritdoc />
        public string SaveInputAnimation(string directory = null)
        {
            return SaveInputAnimation(InputAnimationSerializationUtils.GetOutputFilename(), directory);
        }

        /// <inheritdoc />
        public string SaveInputAnimation(string filename, string directory = null)
        {
            if (IsEnabled)
            {
                string path = Path.Combine(directory ?? Application.persistentDataPath, filename);

                try
                {
                    using (Stream fileStream = File.Open(path, FileMode.Create))
                    {
                        PruneBuffer();
                        recordingBuffer.ToStream(fileStream, StartTime);
                        Debug.Log($"Recorded input animation exported to {path}");
                    }
                    return path;
                }
                catch (IOException ex)
                {
                    Debug.LogWarning(ex.Message);
                }
            }
            return "";
        }

        /// Discard keyframes before the cutoff time.
        private void PruneBuffer()
        {
            recordingBuffer.CutoffBeforeTime(StartTime);
        }
    }
}
