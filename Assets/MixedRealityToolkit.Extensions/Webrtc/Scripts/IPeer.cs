// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using System;
using Microsoft.MixedReality.Toolkit.Extensions.WebRTC.Delegates;

namespace Microsoft.MixedReality.Toolkit.Extensions.WebRTC
{
    public interface IPeer : IPeerInteraction
    {
        /// <summary>
        /// Event that occurs when the local data channel is ready
        /// </summary>
        event Action LocalDataChannelReady;

        /// <summary>
        /// Event that occurs when data is available from the remote, sent via the data channel
        /// </summary>
        event Action<string> DataFromDataChannelReady;

        /// <summary>
        /// Event that occurs when a native failure occurs
        /// </summary>
        event Action<string> FailureMessage;

        /// <summary>
        /// Event that occurs when the audio bus is ready
        /// </summary>
        event AudioBusReadyHandler AudioBusReady;

        /// <summary>
        /// Event that occurs when a local frame is ready
        /// </summary>
        event I420FrameReadyHandler LocalI420FrameReady;

        /// <summary>
        /// Event that occurs when a remote frame is ready
        /// </summary>
        event I420FrameReadyHandler RemoteI420FrameReady;

        /// <summary>
        /// Event that occurs when a local SDP is ready to transmit
        /// </summary>
        event Action<string, string> LocalSdpReadytoSend;

        /// <summary>
        /// Event that occurs when a local ice candidate is ready to transmit
        /// </summary>
        event Action<string, int, string> IceCandiateReadytoSend;
    }
}
