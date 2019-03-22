// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
namespace Microsoft.MixedReality.Toolkit.Extensions.WebRTC
{
    public interface IPeerInteraction
    {
        /// <summary>
        /// Close the peer connection
        /// </summary>
        void ClosePeerConnection();

        /// <summary>
        /// Get the unique id
        /// </summary>
        /// <returns>unique id</returns>
        int GetUniqueId();

        /// <summary>
        /// Add a stream
        /// </summary>
        /// <param name="audioOnly">flag indicating if the stream should only contain audio</param>
        void AddStream(bool audioOnly);

        /// <summary>
        /// Add a data channel
        /// </summary>
        void AddDataChannel();

        /// <summary>
        /// Create an sdp offer
        /// </summary>
        void CreateOffer();

        /// <summary>
        /// Create an sdp answer
        /// </summary>
        void CreateAnswer();

        /// <summary>
        /// Send data via the created data channel
        /// </summary>
        /// <remarks>
        /// Must call <see cref="AddDataChannel"/> first
        /// </remarks>
        /// <param name="data">data to send</param>
        void SendDataViaDataChannel(string data);

        /// <summary>
        /// Adjust audio controls
        /// </summary>
        /// <param name="isMute">is the stream muted</param>
        /// <param name="isRecord">is the stream recording data</param>
        void SetAudioControl(bool isMute, bool isRecord);

        /// <summary>
        /// Set the remote stream description
        /// </summary>
        /// <remarks>
        /// Known valid types are "answer" and "offer"
        /// </remarks>
        /// <param name="type">description type</param>
        /// <param name="sdp">sdp data</param>
        void SetRemoteDescription(string type, string sdp);

        /// <summary>
        /// Add an ice candidate
        /// </summary>
        /// <param name="candidate">sdp candidate data</param>
        /// <param name="sdpMlineindex">sdp mline index</param>
        /// <param name="sdpMid">sdp mid</param>
        void AddIceCandidate(string candidate, int sdpMlineindex, string sdpMid);
    }
}