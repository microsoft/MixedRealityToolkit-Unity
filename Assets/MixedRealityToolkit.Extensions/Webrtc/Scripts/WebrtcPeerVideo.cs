// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using System;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Extensions.WebRTC.Marshalling;

namespace Microsoft.MixedReality.Toolkit.Extensions.WebRTC
{
    /// <summary>
    /// Provides video frame hooks for Webrtc
    /// </summary>
    public class WebrtcPeerVideo : MonoBehaviour
    {
        /// <summary>
        /// Instance of a local video player which will have frame data written to it
        /// </summary>
        [Tooltip("Video player instance that local stream data will be written to")]
        public WebRtcVideoPlayer LocalPlayer;

        /// <summary>
        /// Instance of a remote video player which will have frame data written to it
        /// </summary>
        [Tooltip("Video player instance that remote stream data will be written to")]
        public WebRtcVideoPlayer RemotePlayer;

        /// <summary>
        /// The underlying peer
        /// </summary>
        private IPeer peer = null;

        /// <summary>
        /// Internal queue used for storing and processing local video frames
        /// </summary>
        private FrameQueue localFrameQueue = new FrameQueue(3);

        /// <summary>
        /// Internal queue used for storing and processing remote video frames
        /// </summary>
        private FrameQueue remoteFrameQueue = new FrameQueue(5);

        /// <summary>
        /// Initialization handler
        /// </summary>
        /// <remarks>
        /// This is expected to be bound to <see cref="Webrtc.OnInitialized"/> via the UnityEditor
        /// </remarks>
        public void OnInitialized()
        {
            // cache the peer.
            this.peer = this.GetComponent<Webrtc>().Peer;

            // setup the video players frame queue references.
            this.LocalPlayer.FrameQueue = localFrameQueue;
            this.RemotePlayer.FrameQueue = remoteFrameQueue;

            // setup the peer events.
            this.peer.LocalI420FrameReady += Peer_LocalI420FrameReady;
            this.peer.RemoteI420FrameReady += Peer_RemoteI420FrameReady;
        }

        /// <summary>
        /// Unity Engine OnDestroy() hook
        /// </summary>
        /// <remarks>
        /// https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnDestroy.html
        /// </remarks>
        private void OnDestroy()
        {
            // cleanup, if necessary
            if (this.peer != null)
            {
                // cleanup the peer events.
                this.peer.LocalI420FrameReady -= Peer_LocalI420FrameReady;
                this.peer.RemoteI420FrameReady -= Peer_RemoteI420FrameReady;

                // cleanup the peer.
                this.peer = null;
            }
        }

        private void Peer_LocalI420FrameReady(IntPtr dataY, IntPtr dataU, IntPtr dataV, IntPtr dataA, int strideY, int strideU, int strideV, int strideA, uint width, uint height)
        {
            FramePacket packet = localFrameQueue.GetDataBufferWithoutContents((int)(width * height * 4));
            if (packet == null)
            {
                return;
            }
            CopyYuvToBuffer(dataY, dataU, dataV, strideY, strideU, strideV, width, height, packet.Buffer);
            packet.width = (int)width;
            packet.height = (int)height;
            localFrameQueue.Push(packet);
        }

        private void Peer_RemoteI420FrameReady(IntPtr dataY, IntPtr dataU, IntPtr dataV, IntPtr dataA, int strideY, int strideU, int strideV, int strideA, uint width, uint height)
        {
            FramePacket packet = remoteFrameQueue.GetDataBufferWithoutContents((int)(width * height * 4));
            if (packet == null)
            {
                return;
            }
            CopyYuvToBuffer(dataY, dataU, dataV, strideY, strideU, strideV, width, height, packet.Buffer);
            packet.width = (int)width;
            packet.height = (int)height;
            remoteFrameQueue.Push(packet);
        }

        /// <summary>
        /// Internal yuv data copy routine for I420A frame-data
        /// </summary>
        /// <remarks>
        /// https://docs.microsoft.com/en-us/windows/desktop/medfound/about-yuv-video
        /// https://chromium.googlesource.com/libyuv/libyuv/+/refs/heads/master/docs/formats.md
        /// https://www.fourcc.org/pixel-format/yuv-i420/
        /// 
        /// As VLC gracefully puts it (from https://wiki.videolan.org/YUV):
        /// The two chroma planes (blue and red projections) are sub-sampled in both the horizontal and vertical dimensions by a factor of 2.
        /// That is to say, for a 2x2 square of pixels, there are 4 Y samples but only 1 U sample and 1 V sample. 
        /// </remarks>
        private void CopyYuvToBuffer(IntPtr dataY, IntPtr dataU, IntPtr dataV,
        int strideY, int strideU, int strideV,
        uint width, uint height, byte[] buffer)
        {
            unsafe
            {
                // each data pointer is an array of pixel values
                byte* ptrY = (byte*)dataY.ToPointer();
                byte* ptrU = (byte*)dataU.ToPointer();
                byte* ptrV = (byte*)dataV.ToPointer();

                int srcOffsetY = 0;
                int srcOffsetU = 0;
                int srcOffsetV = 0;
                int destOffset = 0;

                // we walk the height of the frame
                for (int i = 0; i < height; i++)
                {
                    // reading data (accounting for stride) from the native buffers
                    srcOffsetY = i * strideY;
                    srcOffsetU = (i / 2) * strideU;
                    srcOffsetV = (i / 2) * strideV;
                    destOffset = i * (int)width * 4;

                    // we walk the width of the frame
                    for (int j = 0; j < width; j += 2)
                    {
                        // read the src yuv data (first y read)
                        byte y = ptrY[srcOffsetY];
                        byte u = ptrU[srcOffsetU];
                        byte v = ptrV[srcOffsetV];

                        srcOffsetY++;
                        srcOffsetU++;
                        srcOffsetV++;
                        destOffset += 4;

                        // write them out to the buffer
                        buffer[destOffset] = y;
                        buffer[destOffset + 1] = u;
                        buffer[destOffset + 2] = v;

                        // max alpha value
                        buffer[destOffset + 3] = 0xff;

                        // y advances (second y read)
                        byte y2 = ptrY[srcOffsetY];

                        srcOffsetY++;
                        destOffset += 4;

                        // second write, but same u,v values
                        buffer[destOffset] = y2;
                        buffer[destOffset + 1] = u;
                        buffer[destOffset + 2] = v;

                        // max alpha value
                        buffer[destOffset + 3] = 0xff;
                    }
                }
            }
        }
    }
}
