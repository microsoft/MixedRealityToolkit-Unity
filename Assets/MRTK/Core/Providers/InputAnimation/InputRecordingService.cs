// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.IO;
using System.Threading.Tasks;
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

        /// <summary>
        /// Invoked when recording begins
        /// </summary>
        public event Action OnRecordingStarted;
        /// <summary>
        /// Invoked when recording ends
        /// </summary>
        public event Action OnRecordingStopped;

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

        // Start time of recording if buffer is unlimited.
        // Nullable to determine when time needs to be reset.
        private float? unlimitedRecordingStartTime = null;
        /// <summary>
        /// Start time of recording.
        /// </summary>
        public float StartTime
        {
            get
            {
                if (unlimitedRecordingStartTime.HasValue)
                {
                    if (useBufferTimeLimit)
                    {
                        return Mathf.Max(unlimitedRecordingStartTime.Value, EndTime - recordingBufferTimeLimit);
                    }
                    else
                    {
                        return unlimitedRecordingStartTime.Value;
                    }
                }
                
                return EndTime;
            }
        }
        
        /// <summary>
        /// End time of recording.
        /// </summary>
        public float EndTime { get; private set; }

        /// <summary>
        /// The profile used for recording.
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
            set => ConfigurationProfile = value;
        }

        private float frameRate;
        private float frameInterval;
        private float nextFrame;
        private InputRecordingBuffer recordingBuffer = null;
        private IMixedRealityEyeGazeProvider eyeGazeProvider;

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
            BaseMixedRealityProfile profile = null) : this( inputSystem, name, priority, profile)
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

        /// <inheritdoc />
        public override void Enable()
        {
            base.Enable();
            recordingBuffer = new InputRecordingBuffer();
        }

        /// <inheritdoc />
        public override void Disable()
        {
            base.Disable();
            recordingBuffer = null;
            ResetStartTime();
        }

        /// <inheritdoc />
        public void StartRecording()
        {
            eyeGazeProvider = CoreServices.InputSystem.EyeGazeProvider;
            IsRecording = true;
            frameRate = InputRecordingProfile.FrameRate;
            frameInterval = 1f / frameRate;
            nextFrame = Time.time + frameInterval;
            
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
            if (IsEnabled && IsRecording && Time.time > nextFrame)
            {
                EndTime = Time.time;
                nextFrame += frameInterval * (Mathf.Floor((Time.time - nextFrame) * frameRate) + 1f);
                
                if (UseBufferTimeLimit)
                {
                    PruneBuffer();
                }

                RecordKeyframe();
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

        /// <inheritdoc />
        public string SaveInputAnimation(string directory = null) => SaveInputAnimation(InputAnimationSerializationUtils.GetOutputFilename(), directory);
        
        /// <inheritdoc />
        public string SaveInputAnimation(string filename, string directory)
        {
            if (IsEnabled)
            {
                string path = Path.Combine(directory ?? Application.persistentDataPath, filename);

                try
                {
                    using (Stream fileStream = File.Open(path, FileMode.Create))
                    {
                        PruneBuffer();
                        
                        var animation = InputAnimation.FromRecordingBuffer(recordingBuffer, InputRecordingProfile);
                        
                        Debug.Log($"Recording buffer saved to animation");
                        animation.ToStream(fileStream, 0f);
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
        
        /// <inheritdoc />
        public Task<string> SaveInputAnimationAsync(string directory = null) => SaveInputAnimationAsync(InputAnimationSerializationUtils.GetOutputFilename(), directory);

        /// <inheritdoc />
        public async Task<string> SaveInputAnimationAsync(string filename, string directory)
        {
            if (IsEnabled)
            {
                string path = Path.Combine(directory ?? Application.persistentDataPath, filename);

                try
                {
                    using (Stream fileStream = File.Open(path, FileMode.Create))
                    {
                        PruneBuffer();

                        var animation = await Task.Run(() => InputAnimation.FromRecordingBuffer(recordingBuffer, InputRecordingProfile));
                        
                        Debug.Log($"Recording buffer saved to animation");

                        await animation.ToStreamAsync(fileStream, 0f);
                        
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

        /// <summary>
        /// Record a keyframe at the given time for the main camera and tracked input devices.
        /// </summary>
        private void RecordKeyframe()
        {
            float time = Time.time;
            var profile = InputRecordingProfile;

            recordingBuffer.NewKeyframe(time);

            if (profile.RecordHandData)
            {
                RecordInputHandData(Handedness.Left);
                RecordInputHandData(Handedness.Right);
            }

            MixedRealityPose cameraPose;
            
            if (profile.RecordCameraPose && CameraCache.Main)
            {
                cameraPose = new MixedRealityPose(CameraCache.Main.transform.position, CameraCache.Main.transform.rotation);
                recordingBuffer.SetCameraPose(cameraPose);
            }
            else
            {
                cameraPose = new MixedRealityPose(Vector3.zero, Quaternion.identity);
            }

            if (profile.RecordEyeGaze)
            {
                if (eyeGazeProvider != null)
                {
                    recordingBuffer.SetGazeRay(eyeGazeProvider.LatestEyeGaze);
                }
                else 
                {
                    recordingBuffer.SetGazeRay(new Ray(cameraPose.Position, cameraPose.Forward));
                }
            }
        }

        /// <summary>
        /// Record a keyframe at the given time for a hand with the given handedness it is tracked.
        /// </summary>
        private void RecordInputHandData(Handedness handedness)
        {
            float time = Time.time;
            var profile = InputRecordingProfile;

            var hand = HandJointUtils.FindHand(handedness);
            if (hand == null)
            {
                recordingBuffer.SetHandState(handedness, false, false);

                return;
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

            recordingBuffer.SetHandState(handedness, isTracked, isPinching);

            if (isTracked)
            {
                for (int i = 0; i < jointCount; ++i)
                {
                    if (hand.TryGetJoint((TrackedHandJoint)i, out MixedRealityPose jointPose))
                    {
                        recordingBuffer.SetJointPose(handedness, (TrackedHandJoint)i, jointPose);
                    }
                }
            }
        }

        /// Discard keyframes before the cutoff time.
        private void PruneBuffer()
        {
            recordingBuffer.RemoveBeforeTime(StartTime);
        }
    }
}
