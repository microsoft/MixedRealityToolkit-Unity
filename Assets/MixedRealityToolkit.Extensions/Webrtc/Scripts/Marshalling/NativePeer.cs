// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using AOT;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.MixedReality.Toolkit.Extensions.WebRTC.Delegates;

namespace Microsoft.MixedReality.Toolkit.Extensions.WebRTC.Marshalling
{
    /// <summary>
    /// A peer, backed by a native-library implementation
    /// </summary>
    public class NativePeer : IPeer
    {
        /// <summary>
        /// Internal marshalling definitions to the native-library
        /// </summary>
        private static class NativeMethods
        {
#if UNITY_ANDROID
            // note: android uses a different lib name
            // see https://webrtc.googlesource.com/src/+/refs/heads/master/examples/unityplugin/ANDROID_INSTRUCTION
            private const string dllPath = "libjingle_peerconnection_so";
#elif UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN || UNITY_WSA
            private const string dllPath = "webrtc_unity_plugin";
#endif

            #region Synchronous calls

            [DllImport(dllPath, CallingConvention = CallingConvention.Cdecl)]
            public static extern int CreatePeerConnection(string[] turnUrls,
                int noOfUrls,
                string username,
                string credential);

            [DllImport(dllPath, CallingConvention = CallingConvention.Cdecl)]
            public static extern bool ClosePeerConnection(int peerConnectionId);

            [DllImport(dllPath, CallingConvention = CallingConvention.Cdecl)]
            public static extern bool AddStream(int peerConnectionId,
                bool audioOnly);

            [DllImport(dllPath, CallingConvention = CallingConvention.Cdecl)]
            public static extern bool AddDataChannel(int peerConnectionId);

            [DllImport(dllPath, CallingConvention = CallingConvention.Cdecl)]
            public static extern bool CreateOffer(int peerConnectionId);

            [DllImport(dllPath, CallingConvention = CallingConvention.Cdecl)]
            public static extern bool CreateAnswer(int peerConnectionId);

            [DllImport(dllPath, CallingConvention = CallingConvention.Cdecl)]
            public static extern bool SendDataViaDataChannel(int peerConnectionId,
                string data);

            [DllImport(dllPath, CallingConvention = CallingConvention.Cdecl)]
            public static extern bool SetAudioControl(int peerConnectionId,
                bool isMute,
                bool isRecord);

            [DllImport(dllPath, CallingConvention = CallingConvention.Cdecl)]
            public static extern bool SetRemoteDescription(int peerConnectionId,
                string type,
                string sdp);

            [DllImport(dllPath, CallingConvention = CallingConvention.Cdecl)]
            public static extern bool AddIceCandidate(int peerConnectionId,
                string sdp,
                int sdpMlineindex,
                string sdpMid);

            #endregion

            #region Asynchronous calls

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void LocalDataChannelReady(IntPtr userData);

            [DllImport(dllPath, CallingConvention = CallingConvention.Cdecl)]
            public static extern bool RegisterOnLocalDataChannelReady(int peerConnectionId,
                LocalDataChannelReady callback,
                IntPtr userData);

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void DataFromDataChannelReady(string data,
                IntPtr userData);

            [DllImport(dllPath, CallingConvention = CallingConvention.Cdecl)]
            public static extern bool RegisterOnDataFromDataChannelReady(int peerConnectionId,
                DataFromDataChannelReady callback,
                IntPtr userData);

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void FailureMessage(string msg,
                IntPtr userData);

            [DllImport(dllPath, CallingConvention = CallingConvention.Cdecl)]
            public static extern bool RegisterOnFailure(int peerConnectionId,
                FailureMessage callback,
                IntPtr userData);

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void AudioBusReady(IntPtr data,
                int bitsPerSample,
                int sampleRate,
                int numberOfChannels,
                int numberOfFrames,
                IntPtr userData);

            [DllImport(dllPath, CallingConvention = CallingConvention.Cdecl)]
            public static extern bool RegisterOnAudioBusReady(int peerConnectionId,
                AudioBusReady callback,
                IntPtr userData);

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void I420FrameReady(IntPtr dataY,
                IntPtr dataU,
                IntPtr dataV,
                IntPtr dataA,
                int strideY,
                int strideU,
                int strideV,
                int strideA,
                uint width,
                uint height,
                IntPtr userData);

            [DllImport(dllPath, CallingConvention = CallingConvention.Cdecl)]
            public static extern bool RegisterOnLocalI420FrameReady(int peerConnectionId,
                I420FrameReady callback,
                IntPtr userData);

            [DllImport(dllPath, CallingConvention = CallingConvention.Cdecl)]
            public static extern bool RegisterOnRemoteI420FrameReady(int peerConnectionId,
                I420FrameReady callback,
                IntPtr userData);

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void LocalSdpReadytoSend(string type,
                string sdp,
                IntPtr userData);

            [DllImport(dllPath, CallingConvention = CallingConvention.Cdecl)]
            public static extern bool RegisterOnLocalSdpReadytoSend(int peerConnectionId,
                LocalSdpReadytoSend callback,
                IntPtr userData);

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void IceCandiateReadytoSend(string sdp,
                int sdpMlineIndex,
                string sdpMid,
                IntPtr userData);

            [DllImport(dllPath, CallingConvention = CallingConvention.Cdecl)]
            public static extern bool RegisterOnIceCandiateReadytoSend(int peerConnectionId,
                IceCandiateReadytoSend callback,
                IntPtr userData);

            #endregion
        }

        /// <summary>
        /// Internal static callback definitions to facilitate marshalling callbacks back from the native-library
        /// </summary>
        private static class NativeCallbacks
        {
            [MonoPInvokeCallback(typeof(NativeMethods.LocalDataChannelReady))]
            public static void LocalDataChannelReady_Global(IntPtr userData)
            {
                var handle = GCHandle.FromIntPtr(userData);
                var peer = handle.Target as NativePeer;

                if (peer.LocalDataChannelReady != null)
                {
                    peer.LocalDataChannelReady();
                }
            }

            [MonoPInvokeCallback(typeof(NativeMethods.DataFromDataChannelReady))]
            public static void DataFromDataChannelReady_Global(string data, IntPtr userData)
            {
                var handle = GCHandle.FromIntPtr(userData);
                var peer = handle.Target as NativePeer;

                if (peer.DataFromDataChannelReady != null)
                {
                    peer.DataFromDataChannelReady(data);
                }
            }

            [MonoPInvokeCallback(typeof(NativeMethods.FailureMessage))]
            public static void FailureMessage_Global(string msg, IntPtr userData)
            {
                var handle = GCHandle.FromIntPtr(userData);
                var peer = handle.Target as NativePeer;

                if (peer.FailureMessage != null)
                {
                    peer.FailureMessage(msg);
                }
            }

            [MonoPInvokeCallback(typeof(NativeMethods.AudioBusReady))]
            public static void AudioBusReady_Global(IntPtr data,
                int bitsPerSample,
                int sampleRate,
                int numberOfChannels,
                int numberOfFrames,
                IntPtr userData)
            {
                var handle = GCHandle.FromIntPtr(userData);
                var peer = handle.Target as NativePeer;

                if (peer.AudioBusReady != null)
                {
                    peer.AudioBusReady(data,
                        bitsPerSample,
                        sampleRate,
                        numberOfChannels,
                        numberOfFrames);
                }
            }
            
            [MonoPInvokeCallback(typeof(NativeMethods.I420FrameReady))]
            public static void LocalI420FrameReady_Global(IntPtr dataY,
                IntPtr dataU,
                IntPtr dataV,
                IntPtr dataA,
                int strideY,
                int strideU,
                int strideV,
                int strideA,
                uint width,
                uint height,
                IntPtr userData)
            {
                var handle = GCHandle.FromIntPtr(userData);
                var peer = handle.Target as NativePeer;

                if (peer.LocalI420FrameReady != null)
                {
                    peer.LocalI420FrameReady(dataY,
                        dataU,
                        dataV,
                        dataA,
                        strideY,
                        strideU,
                        strideV,
                        strideA,
                        width,
                        height);
                }
            }

            [MonoPInvokeCallback(typeof(NativeMethods.I420FrameReady))]
            public static void RemoteI420FrameReady_Global(IntPtr dataY,
                IntPtr dataU,
                IntPtr dataV,
                IntPtr dataA,
                int strideY,
                int strideU,
                int strideV,
                int strideA,
                uint width,
                uint height,
                IntPtr userData)
            {
                var handle = GCHandle.FromIntPtr(userData);
                var peer = handle.Target as NativePeer;

                if (peer.RemoteI420FrameReady != null)
                {
                    peer.RemoteI420FrameReady(dataY,
                        dataU,
                        dataV,
                        dataA,
                        strideY,
                        strideU,
                        strideV,
                        strideA,
                        width,
                        height);
                }
            }

            [MonoPInvokeCallback(typeof(NativeMethods.LocalSdpReadytoSend))]
            public static void LocalSdpReadytoSend_Global(string type,
                string sdp,
                IntPtr userData)
            {
                var handle = GCHandle.FromIntPtr(userData);
                var peer = handle.Target as NativePeer;

                if (peer.LocalSdpReadytoSend != null)
                {
                    peer.LocalSdpReadytoSend(type,
                        sdp);
                }
            }

            [MonoPInvokeCallback(typeof(NativeMethods.IceCandiateReadytoSend))]
            public static void IceCandiateReadytoSend_Global(string sdp,
                int sdpMlineIndex,
                string sdpMid,
                IntPtr userData)
            {
                var handle = GCHandle.FromIntPtr(userData);
                var peer = handle.Target as NativePeer;

                if (peer.IceCandiateReadytoSend != null)
                {
                    peer.IceCandiateReadytoSend(sdp,
                        sdpMlineIndex,
                        sdpMid);
                }
            }
        }
        
        /// <summary>
        /// Event that occurs when the local data channel is ready
        /// </summary>
        public event Action LocalDataChannelReady;

        /// <summary>
        /// Event that occurs when data is available from the remote, sent via the data channel
        /// </summary>
        public event Action<string> DataFromDataChannelReady;

        /// <summary>
        /// Event that occurs when a native failure occurs
        /// </summary>
        public event Action<string> FailureMessage;

        /// <summary>
        /// Event that occurs when the audio bus is ready
        /// </summary>
        public event AudioBusReadyHandler AudioBusReady;

        /// <summary>
        /// Event that occurs when a local frame is ready
        /// </summary>
        public event I420FrameReadyHandler LocalI420FrameReady;

        /// <summary>
        /// Event that occurs when a remote frame is ready
        /// </summary>
        public event I420FrameReadyHandler RemoteI420FrameReady;

        /// <summary>
        /// Event that occurs when a local SDP is ready to transmit
        /// </summary>
        public event Action<string, string> LocalSdpReadytoSend;

        /// <summary>
        /// Event that occurs when a local ice candidate is ready to transmit
        /// </summary>
        public event Action<string, int, string> IceCandiateReadytoSend;

        /// <summary>
        /// Internal peer id
        /// </summary>
        private int peerId;

        /// <summary>
        /// Internal pinned gc handle to <c>this</c>
        /// </summary>
        /// <remarks>
        /// this allows us to use il2cpp callbacks (which require static handlers) but still keep
        /// our instance-based interface on the managed side
        /// </remarks>
        private GCHandle peerHandle;
        
        public NativePeer(List<string> turnUrls, string username, string credential)
        {
            // create a gc-pinned handle to ourselves, to be passed as userdata in the callbacks
            // this allows us to use il2cpp callbacks (which require static handlers) but still keep
            // our instance-based interface on the managed side
            peerHandle = GCHandle.Alloc(this);

            peerId = NativeMethods.CreatePeerConnection(turnUrls.ToArray(),
                turnUrls.Count,
                username,
                credential);

            // register callbacks
            NativeMethods.RegisterOnLocalDataChannelReady(peerId,
                NativeCallbacks.LocalDataChannelReady_Global,
                GCHandle.ToIntPtr(peerHandle));
            NativeMethods.RegisterOnDataFromDataChannelReady(peerId,
                NativeCallbacks.DataFromDataChannelReady_Global,
                GCHandle.ToIntPtr(peerHandle));
            NativeMethods.RegisterOnFailure(peerId,
                NativeCallbacks.FailureMessage_Global,
                GCHandle.ToIntPtr(peerHandle));
            NativeMethods.RegisterOnAudioBusReady(peerId,
                NativeCallbacks.AudioBusReady_Global,
                GCHandle.ToIntPtr(peerHandle));
            NativeMethods.RegisterOnLocalI420FrameReady(peerId,
                NativeCallbacks.LocalI420FrameReady_Global,
                GCHandle.ToIntPtr(peerHandle));
            NativeMethods.RegisterOnRemoteI420FrameReady(peerId,
                NativeCallbacks.RemoteI420FrameReady_Global,
                GCHandle.ToIntPtr(peerHandle));
            NativeMethods.RegisterOnLocalSdpReadytoSend(peerId,
                NativeCallbacks.LocalSdpReadytoSend_Global,
                GCHandle.ToIntPtr(peerHandle));
            NativeMethods.RegisterOnIceCandiateReadytoSend(peerId,
                NativeCallbacks.IceCandiateReadytoSend_Global,
                GCHandle.ToIntPtr(peerHandle));
        }

        #region IPeer

        public void AddDataChannel()
        {
            NativeMethods.AddDataChannel(peerId);
        }

        public void AddIceCandidate(string sdp, int sdpMlineindex, string sdpMid)
        {
            NativeMethods.AddIceCandidate(peerId, sdp, sdpMlineindex, sdpMid);
        }

        public void AddStream(bool audioOnly)
        {
            NativeMethods.AddStream(peerId, audioOnly);
        }

        public void ClosePeerConnection()
        {
            NativeMethods.ClosePeerConnection(peerId);
        }

        public void CreateAnswer()
        {
            NativeMethods.CreateAnswer(peerId);
        }

        public void CreateOffer()
        {
            NativeMethods.CreateOffer(peerId);
        }

        /// <summary>
        /// Get the peer id 
        /// </summary>
        /// <returns>the peer id</returns>
        public int GetUniqueId()
        {
            return peerId;
        }

        public void SendDataViaDataChannel(string data)
        {
            NativeMethods.SendDataViaDataChannel(peerId, data);
        }

        public void SetAudioControl(bool isMute, bool isRecord)
        {
            NativeMethods.SetAudioControl(peerId, isMute, isRecord);
        }

        public void SetRemoteDescription(string type, string sdp)
        {
            NativeMethods.SetRemoteDescription(peerId, type, sdp);
        }

        #endregion
    }
}
