// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;

namespace Microsoft.MixedReality.Toolkit.Subsystems
{
    /// <summary>
    /// Describes the reasons for a dictation event.
    /// </summary>
    public enum DictationEventReason
    {
        /// <summary>
        /// The dictation session is successfully completed.
        /// </summary>
        Complete = 0,

        /// <summary>
        /// Audio problems caused recognition to fail.
        /// </summary>
        AudioQualityFailure,

        /// <summary>
        /// User canceled recognition session or the application is paused.
        /// </summary>
        Canceled,

        /// <summary>
        /// A timeout due to extended silence or poor audio caused recognition to fail.
        /// </summary>
        TimeoutExceeded,

        /// <summary>
        /// An extended pause, or excessive processing time, caused recognition to fail.
        /// </summary>
        PauseLimitExceeded,

        /// <summary>
        /// Network problems caused recognition to fail.
        /// </summary>
        NetworkFailure,

        /// <summary>
        /// Lack of a microphone caused recognition to fail.
        /// </summary>
        MicrophoneUnavailable,

        /// <summary>
        /// Indicates an authentication error.
        /// </summary>
        AuthenticationFailure,

        /// <summary>
        /// Indicates that one or more recognition parameters are invalid or the audio format is not supported.
        /// </summary>
        BadRequest,

        /// <summary>
        /// Indicates that the current usage (e.g. parallel request number, quota) exceeds the limit for a cloud-based service.
        /// </summary>
        UsageLimitMet,

        /// <summary>
        /// Indicates a failure happened on the remote server.
        /// </summary>
        RemoteFailure,

        /// <summary>
        /// An unknown problem caused recognition to fail.
        /// </summary>
        UnknownFailure = 254,

        /// <summary>
        /// The reason for the raised event is unknown.
        /// </summary>
        Unknown = 255
    }
}
