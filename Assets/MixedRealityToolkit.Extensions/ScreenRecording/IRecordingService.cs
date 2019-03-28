// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.ScreenRecording
{
    /// <summary>
    /// An interface implemented by classes that perform screen recording
    /// </summary>
    public interface IRecordingService : IDisposable
    {
        /// <summary>
        /// Initializes the screen recording service
        /// </summary>
        void Initialize();

        /// <summary>
        /// True if the screen recording service has completed initialization and is ready for use
        /// </summary>
        /// <returns></returns>
        bool IsInitialized();

        /// <summary>
        /// Starts screen recording
        /// </summary>
        /// <returns></returns>
        bool StartRecording();

        /// <summary>
        /// Stops screen recording
        /// </summary>
        void StopRecording();

        /// <summary>
        /// True if a screen recording has been taken during the current application session
        /// </summary>
        /// <returns></returns>
        bool IsRecordingAvailable();

        /// <summary>
        /// Shows the last captured screen recording from the current application session
        /// </summary>
        void ShowRecording();
    }
}

