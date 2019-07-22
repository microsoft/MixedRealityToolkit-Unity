// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Experimental.Audio
{
    /// <summary>
    /// Configuration profile settings for setting up boundary visualizations.
    /// </summary>
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Profiles/Windows Microphone Stream Service Profile", fileName = "WindowsMicrophoneStreamServiceProfile", order = (int)100)]
    [MixedRealityServiceProfile(typeof(IMicrophoneStreamService))]
    // todo [DocLink("https://microsoft.github.io/MixedRealityToolkit-Unity/Documentation/Boundary/BoundarySystemGettingStarted.html")]
    public class WindowsMicrophoneStreamServiceProfile : BaseMixedRealityProfile
    {
        [SerializeField]
        [Tooltip("How should the microphone stream behave at startup?")]
        private AutoStartBehavior startupBehavior = AutoStartBehavior.AutoStart;

        /// <summary>
        /// Indicates how should the microphone stream behave at startup.
        /// </summary>
        public AutoStartBehavior StartupBehavior => startupBehavior;

        [SerializeField]
        [Tooltip("Should the microphone stream be played through the default audio device?")]
        private bool localPlayback = true;

        /// <summary>
        /// Indicates whether or not the microphone stream should be played through the default audio device.
        /// </summary>
        public bool LocalPlayback => localPlayback;

        [SerializeField]
        [Tooltip("Should the microphone data be preserved? Note: Low frame rates can result in significant audio lag.")]
        private bool keepData = false;

        /// <summary>
        /// Should the microphone data be preserved?
        /// </summary>
        /// <remarks>
        /// Low frame rates can result in significant audio lag. Enabling this setting is generally unsuitable
        /// for real-time voice chat scenarios.
        /// </remarks>
        public bool KeepData => keepData;

        [SerializeField]
        [Tooltip("The desired microphone stream.")]
        private WindowsMicrophoneStreamType streamType = WindowsMicrophoneStreamType.HighQualityVoice;

        /// <summary>
        /// The desired microphone stream type (ex: High Quality Voice).
        /// </summary>
        public WindowsMicrophoneStreamType StreamType => streamType;

        [SerializeField]
        [Tooltip("The volume level of the microphone sound.")]
        private float inputGain = 1.0f; // todo: is there a good range to use here? 

        /// <summary>
        /// The volume level of the microphone sound.
        /// </summary>
        /// <remarks>
        /// Gain values are between 0 (silent) and 1 (full volume), inclusive.
        /// </remarks>
        public float InputGain => inputGain;
    }
} 