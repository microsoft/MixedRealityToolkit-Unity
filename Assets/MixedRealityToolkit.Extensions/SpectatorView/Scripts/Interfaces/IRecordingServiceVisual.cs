// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Microsoft.MixedReality.Tookit.Extension.SpectatorView.Interfaces
{
    public delegate void RecordingStateChangedHandler(bool recording); 

    public interface IRecordingServiceVisual
    {
        event RecordingStateChangedHandler RecordingStateChanged;
    }
}
