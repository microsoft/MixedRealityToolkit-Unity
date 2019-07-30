// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Microsoft.MixedReality.Toolkit.Experimental.Audio
{
    /// <summary>
    /// The values that may be returned by <see cref="WindowsMicrophoneStream"/> methpds.
    /// </summary>
    public enum WindowsMicrophoneStreamErrorCode
    {
        /// <summary>
        /// The method succeeded.
        /// </summary>
        Success = 0,

        /// <summary>
        /// The microphone has already been initialized.
        /// </summary>
        AlreadyRunning = -10,

        /// <summary>
        /// A microphone does not appear to be configured on this system.
        /// </summary>
        NoAudioDevice,

        /// <summary>
        /// A microphone does not appear to be configured on this system.
        /// </summary>
        NoInputDevice,

        /// <summary>
        /// The microphone stream is currently recording.
        /// </summary>
        AlreadyRecording,

        /// <summary>
        /// A microphone is not connected or the stream has not been initialized.
        /// </summary>
        GraphDoesNotExist,

        /// <summary>
        /// The microphone appears to be misconfigured. Pleae try setting different mono/stereo options in the operating system settings.
        /// </summary>
        ChannelCountMismatch,

        /// <summary>
        /// Unable to create a file in the Music Library. Please ensure the proper permissions are configured.
        /// </summary>
        FileCreationPermissionError,

        /// <summary>
        /// The device has recently been started and has not produced enough data. Please try again at a later time.
        /// </summary>
        NotEnoughData,

        /// <summary>
        /// Unable to access the microphone. Please ensure the proper capabilies are configured.
        /// </summary>
        NeedMicCapabilityEnabled
    }
}
