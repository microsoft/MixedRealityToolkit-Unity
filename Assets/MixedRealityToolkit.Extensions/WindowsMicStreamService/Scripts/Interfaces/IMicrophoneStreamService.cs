// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Microsoft.MixedReality.Toolkit.Experimental.Audio
{
    /// <summary>
    /// todo
    /// </summary>
    public interface IMicrophoneStreamService : IMixedRealityExtensionService
    {
        /// <summary>
        /// todo
        /// </summary>
        float Gain
        { get; set; }

        /// <summary>
        /// todo
        /// </summary>
        int InitializeMicrophone(/* todo */);

        /// <summary>
        /// todo
        /// </summary>
        int Pause();

        /// <summary>
        /// todo
        /// </summary>
        int Resume();

        /// <summary>
        /// todo
        /// </summary>
        int StartRecording(/* todo */);

        /// <summary>
        /// todo
        /// </summary>
        int StartStream(/* todo */);

        /// <summary>
        /// todo
        /// </summary>
        int StopRecording(/* todo */);

        /// <summary>
        /// todo
        /// </summary>
        int StopStream(/* todo */);

        /// <summary>
        /// todo
        /// </summary>
        int UninitializeMicrophone();
    }
}
