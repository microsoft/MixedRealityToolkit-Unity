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
    /// Plays back input animation via the input simulation system.
    /// </summary>
    [MixedRealityDataProvider(
        typeof(IMixedRealityInputSystem),
        (SupportedPlatforms)(-1), // Supported on all platforms
        "Input Playback Service")]
    public class InputPlaybackService :
        BaseInputSimulationService,
        IMixedRealityInputPlaybackService,
        IMixedRealityEyeGazeDataProvider
    {
        /// <summary>
        /// Invoked when playback begins or resumes
        /// </summary>
        public event Action OnPlaybackStarted;
        /// <summary>
        /// Invoked when playback stops
        /// </summary>
        public event Action OnPlaybackStopped;
        /// <summary>
        /// Invoked when playback is paused
        /// </summary>
        public event Action OnPlaybackPaused;

        private bool isPlaying = false;
        /// <inheritdoc />
        public bool IsPlaying => isPlaying;

        /// <inheritdoc />
        public bool CheckCapability(MixedRealityCapability capability)
        {
            switch (capability)
            {
                case MixedRealityCapability.ArticulatedHand:
                    return true;
                case MixedRealityCapability.GGVHand:
                    return true;
            }

            return false;
        }

        public bool SmoothEyeTracking { get; set; }

        /// <summary>
        /// Duration of the played animation.
        /// </summary>
        public float Duration => (animation != null ? animation.Duration : 0.0f);

        private float localTime = 0.0f;
        /// <inheritdoc />
        public float LocalTime
        {
            get { return localTime; }
            set
            {
                localTime = value;
                Evaluate();
            }
        }

        /// <summary>
        /// Pose data for the left hand.
        /// </summary>
        public SimulatedHandData HandDataLeft { get; } = new SimulatedHandData();

        /// <summary>
        /// Pose data for the right hand.
        /// </summary>
        public SimulatedHandData HandDataRight { get; } = new SimulatedHandData();

        private InputAnimation animation = null;
        /// <inheritdoc />
        public InputAnimation Animation
        {
            get { return animation; }
            set
            {
                animation = value;
                Evaluate();
            }
        }

        public IMixedRealityEyeSaccadeProvider SaccadeProvider => null;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="registrar">The <see cref="IMixedRealityServiceRegistrar"/> instance that loaded the data provider.</param>
        /// <param name="inputSystem">The <see cref="Microsoft.MixedReality.Toolkit.Input.IMixedRealityInputSystem"/> instance that receives data from this provider.</param>
        /// <param name="name">Friendly name of the service.</param>
        /// <param name="priority">Service priority. Used to determine order of instantiation.</param>
        /// <param name="profile">The service's configuration profile.</param>
        [System.Obsolete("This constructor is obsolete (registrar parameter is no longer required) and will be removed in a future version of the Microsoft Mixed Reality Toolkit.")]
        public InputPlaybackService(
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
        public InputPlaybackService(
            IMixedRealityInputSystem inputSystem,
            string name = null,
            uint priority = DefaultPriority,
            BaseMixedRealityProfile profile = null) : base(inputSystem, name, priority, profile)
        { }

        /// <inheritdoc />
        public void Play()
        {
            if (animation == null || isPlaying)
            {
                return;
            }

            isPlaying = true;
            OnPlaybackStarted?.Invoke();
        }

        /// <inheritdoc />
        public void Stop()
        {
            if (!isPlaying)
            {
                return;
            }

            localTime = 0.0f;
            isPlaying = false;
            OnPlaybackStopped?.Invoke();
            Evaluate();
            RemoveControllerDevice(Handedness.Left);
            RemoveControllerDevice(Handedness.Right);
        }

        /// <inheritdoc />
        public void Pause()
        {
            if (!isPlaying)
            {
                return;
            }

            isPlaying = false;
            OnPlaybackPaused?.Invoke();
        }

        /// <inheritdoc />
        public override void Update()
        {
            if (isPlaying)
            {
                localTime += Time.deltaTime;

                if (localTime < Duration)
                {
                    Evaluate();
                }
                else
                {
                    Stop();
                }
            }
        }

        /// <inheritdoc />
        public bool LoadInputAnimation(string filepath)
        {
            if (filepath.Length > 0)
            {
                try
                {
                    using (FileStream fs = new FileStream(filepath, FileMode.Open))
                    {
                        animation = InputAnimation.FromStream(fs);
                        Debug.Log($"Loaded input animation from {filepath}");
                        Evaluate();

                        return true;
                    }
                }
                catch (IOException ex)
                {
                    Debug.LogError(ex.Message);
                    animation = null;
                }
            }

            return false;
        }

        /// <inheritdoc />
        public async Task<bool> LoadInputAnimationAsync(string filepath)
        {
            if (filepath.Length > 0)
            {
                try
                {
                    using (FileStream fs = new FileStream(filepath, FileMode.Open))
                    {
                        animation = await InputAnimation.FromStreamAsync(fs);
                        Debug.Log($"Loaded input animation from {filepath}");
                        Evaluate();

                        return true;
                    }
                }
                catch (IOException ex)
                {
                    Debug.LogError(ex.Message);
                    animation = null;
                }
            }

            return false;
        }

        /// Evaluate the animation and update the simulation service to apply input.
        private void Evaluate()
        {
            if (animation == null)
            {
                localTime = 0.0f;
                isPlaying = false;

                return;
            }

            if (animation.HasCameraPose && CameraCache.Main)
            {
                var cameraPose = animation.EvaluateCameraPose(localTime);
                CameraCache.Main.transform.SetPositionAndRotation(cameraPose.Position, cameraPose.Rotation);
            }

            if (animation.HasHandData)
            {
                EvaluateHandData(HandDataLeft, Handedness.Left);
                EvaluateHandData(HandDataRight, Handedness.Right);
            }

            if (animation.HasEyeGaze)
            {
                EvaluateEyeGaze();
            }
        }

        private void EvaluateHandData(SimulatedHandData handData, Handedness handedness)
        {
            animation.EvaluateHandState(localTime, handedness, out bool isTracked, out bool isPinching);

            if (handData.Update(isTracked, isPinching,
                (MixedRealityPose[] joints) =>
                {
                    for (int i = 0; i < ArticulatedHandPose.JointCount; ++i)
                    {
                        joints[i] = animation.EvaluateHandJoint(localTime, handedness, (TrackedHandJoint)i);
                    }
                }))
            {
                UpdateControllerDevice(ControllerSimulationMode.ArticulatedHand, handedness, handData);
            }
        }

        private void EvaluateEyeGaze()
        {
            var ray = animation.EvaluateEyeGaze(localTime);

            Service?.EyeGazeProvider?.UpdateEyeTrackingStatus(this, true);
            Service?.EyeGazeProvider?.UpdateEyeGaze(this, ray, DateTime.UtcNow);
        }
    }
}
