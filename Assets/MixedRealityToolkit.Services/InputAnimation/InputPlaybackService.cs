// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.IO;
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
        BaseInputDeviceManager,
        IMixedRealityInputPlaybackService
    {
        private static readonly int jointCount = Enum.GetNames(typeof(TrackedHandJoint)).Length;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="registrar">The <see cref="IMixedRealityServiceRegistrar"/> instance that loaded the data provider.</param>
        /// <param name="inputSystem">The <see cref="Microsoft.MixedReality.Toolkit.Input.IMixedRealityInputSystem"/> instance that receives data from this provider.</param>
        /// <param name="name">Friendly name of the service.</param>
        /// <param name="priority">Service priority. Used to determine order of instantiation.</param>
        /// <param name="profile">The service's configuration profile.</param>
        public InputPlaybackService(
            IMixedRealityServiceRegistrar registrar,
            IMixedRealityInputSystem inputSystem,
            string name = null,
            uint priority = DefaultPriority,
            BaseMixedRealityProfile profile = null) : base(registrar, inputSystem, name, priority, profile)
        {}

#if UNITY_EDITOR
        private IInputSimulationService inputSimService = null;
        private IInputSimulationService InputSimService
        {
            get
            {
                if (inputSimService == null)
                {
                    if (MixedRealityServiceRegistry.TryGetService<IMixedRealityInputSystem>(out IMixedRealityInputSystem inputSystem))
                    {
                        inputSimService = (inputSystem as IMixedRealityDataProviderAccess).GetDataProvider<IInputSimulationService>();
                    }
                }
                return inputSimService;
            }
        }
#endif

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

        /// <summary>
        /// Duration of the played animation.
        /// </summary>
        public float Duration => (animation != null ? animation.Duration : 0.0f);

        private bool isPlaying = false;
        /// <inheritdoc />
        public bool IsPlaying => isPlaying;

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

        /// <inheritdoc />
        public void Play()
        {
            if (animation != null)
            {
                SetPlaying(true);
            }
        }

        /// <inheritdoc />
        public void Stop()
        {
            SetPlaying(false);
            localTime = 0.0f;

            Evaluate();
        }

        /// <inheritdoc />
        public void Pause()
        {
            SetPlaying(false);
        }

        /// <inheritdoc />
        public override void Update()
        {
            if (isPlaying)
            {
                localTime += Time.deltaTime;

                if (localTime >= Duration)
                {
                    localTime = 0.0f;
                    SetPlaying(false);
                }

                Evaluate();
            }
        }

        private void SetPlaying(bool playing)
        {
            isPlaying = playing;

#if UNITY_EDITOR
            if (InputSimService != null)
            {
                // Disable user input while playing animation
                InputSimService.UserInputEnabled = !isPlaying;
            }
#endif
        }

        /// Evaluate the animation and update the simulation service to apply input.
        private void Evaluate()
        {
            if (animation == null)
            {
                SetPlaying(false);
                localTime = 0.0f;
                return;
            }

            if (CameraCache.Main)
            {
                var cameraPose = animation.EvaluateCameraPose(localTime);
                CameraCache.Main.transform.SetPositionAndRotation(cameraPose.Position, cameraPose.Rotation);
            }

#if UNITY_EDITOR
            if (InputSimService != null)
            {
                EvaluateHandData(InputSimService.HandDataLeft, Handedness.Left);
                EvaluateHandData(InputSimService.HandDataRight, Handedness.Right);
            }
#endif
        }

#if UNITY_EDITOR
        private void EvaluateHandData(SimulatedHandData handData, Handedness handedness)
        {
            animation.EvaluateHandState(localTime, handedness, out bool isTracked, out bool isPinching);

            handData.Update(isTracked, isPinching,
                (MixedRealityPose[] joints) =>
                {
                    for (int i = 0; i < jointCount; ++i)
                    {
                        joints[i] = animation.EvaluateHandJoint(localTime, handedness, (TrackedHandJoint)i);
                    }
                });
        }
#endif

        /// <inheritdoc />
        public bool LoadInputAnimation(string filepath)
        {
            if (filepath.Length > 0)
            {
                try
                {
                    using (FileStream fs = new FileStream(filepath, FileMode.Open))
                    {
                        animation = new InputAnimation();
                        animation.FromStream(fs);

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
    }
}
