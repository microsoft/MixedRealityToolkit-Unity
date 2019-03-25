// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;

namespace Microsoft.MixedReality.Toolkit.Extensions.ScreenRecording
{
    public interface IRecordingService : IDisposable
    {
        void Initialize();
        bool IsInitialized();
        bool StartRecording();
        void StopRecording();
        bool IsRecordingAvailable();
        void ShowRecording();
    }
}

