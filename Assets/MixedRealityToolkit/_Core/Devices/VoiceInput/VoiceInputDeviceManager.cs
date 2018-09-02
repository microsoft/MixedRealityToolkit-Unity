// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Definitions.Devices;
using Microsoft.MixedReality.Toolkit.Core.Definitions.Utilities;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.Devices;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.InputSystem;
using Microsoft.MixedReality.Toolkit.Core.Managers;
using System;

namespace Microsoft.MixedReality.Toolkit.Core.Devices.VoiceInput
{
    // TODO - Implement
    public class VoiceInputDeviceManager : BaseDeviceManager
    {
        public VoiceInputDeviceManager(string name, uint priority) : base(name, priority) { }

        /// <summary>
        /// Current Speech Input Source.
        /// </summary>
        public IMixedRealitySpeechController SpeechInputController { get; private set; }

#if UNITY_STANDALONE_WIN || UNITY_WSA || UNITY_EDITOR_WIN
        /// <inheritdoc />
        public override void Initialize()
        {
            base.Initialize();
            if (MixedRealityManager.Instance.ActiveProfile.IsSpeechCommandsEnabled)
            {
                var inputSource = InputSystem?.RequestNewGenericInputSource($"Speech Recognizer", null);
                SpeechInputController = Activator.CreateInstance(MixedRealityManager.Instance.ActiveProfile.SpeechCommandsProfile.SpeechSystemType, TrackingState.NotTracked, Handedness.None, inputSource, null) as IMixedRealitySpeechController;

                SpeechInputController.Initialize();
            }
        }
#else
            // TODO: Implement on other platforms
#endif // UNITY_STANDALONE_WIN || UNITY_WSA || UNITY_EDITOR_WIN

        /// <inheritdoc />
        public override void Destroy()
        {
            SpeechInputController?.Dispose();
            base.Destroy();
        }
    }
}