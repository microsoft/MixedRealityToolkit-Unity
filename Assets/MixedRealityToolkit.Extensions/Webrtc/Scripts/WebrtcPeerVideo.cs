using System;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Extensions.Webrtc.Marshalling;

namespace Microsoft.MixedReality.Toolkit.Extensions.Webrtc
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
        /// 
        /// TODO(bengreenier): This could be auto-bound with editor script
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
        /// Internal yuv data copy routine
        /// </summary>
        private void CopyYuvToBuffer(IntPtr dataY, IntPtr dataU, IntPtr dataV,
        int strideY, int strideU, int strideV,
        uint width, uint height, byte[] buffer)
        {
            unsafe
            {
                byte* ptrY = (byte*)dataY.ToPointer();
                byte* ptrU = (byte*)dataU.ToPointer();
                byte* ptrV = (byte*)dataV.ToPointer();
                int srcOffsetY = 0;
                int srcOffsetU = 0;
                int srcOffsetV = 0;
                int destOffset = 0;
                for (int i = 0; i < height; i++)
                {
                    srcOffsetY = i * strideY;
                    srcOffsetU = (i / 2) * strideU;
                    srcOffsetV = (i / 2) * strideV;
                    destOffset = i * (int)width * 4;
                    for (int j = 0; j < width; j += 2)
                    {
                        {
                            byte y = ptrY[srcOffsetY];
                            byte u = ptrU[srcOffsetU];
                            byte v = ptrV[srcOffsetV];
                            srcOffsetY++;
                            srcOffsetU++;
                            srcOffsetV++;
                            destOffset += 4;
                            buffer[destOffset] = y;
                            buffer[destOffset + 1] = u;
                            buffer[destOffset + 2] = v;
                            buffer[destOffset + 3] = 0xff;

                            // use same u, v values
                            byte y2 = ptrY[srcOffsetY];
                            srcOffsetY++;
                            destOffset += 4;
                            buffer[destOffset] = y2;
                            buffer[destOffset + 1] = u;
                            buffer[destOffset + 2] = v;
                            buffer[destOffset + 3] = 0xff;
                        }
                    }
                }
            }
        }
    }
}
