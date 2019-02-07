// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

using Microsoft.MixedReality.Tookit.Extension.SpectatorView.Interfaces;

namespace Microsoft.MixedReality.Toolkit.Extensions.SpectatorView.Recording
{
    public class RecordingServiceVisual : MonoBehaviour,
        IRecordingServiceVisual
    {
        // TODO - determine to show content and fire button presses
        public event RecordingStateChangedHandler RecordingStateChanged;

        private bool _recording = false;
    }
}
