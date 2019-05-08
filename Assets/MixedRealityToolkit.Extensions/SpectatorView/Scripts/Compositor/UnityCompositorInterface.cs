// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#if UNITY_EDITOR
using System;
using System.Runtime.InteropServices;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView.Compositor
{
    internal static class UnityCompositorInterface
    {
        [DllImport("UnityCompositorInterface")]
        public static extern int GetFrameWidth();

        [DllImport("UnityCompositorInterface")]
        public static extern int GetFrameHeight();

        [DllImport("UnityCompositorInterface")]
        public static extern bool SetHoloTexture(IntPtr holoTexture);

        [DllImport("UnityCompositorInterface")]
        public static extern void SetAlpha(float alpha);

        [DllImport("UnityCompositorInterface")]
        public static extern float GetAlpha();

        [DllImport("UnityCompositorInterface")]
        public static extern bool CreateUnityColorTexture(out IntPtr srv);

        [DllImport("UnityCompositorInterface")]
        public static extern bool CreateUnityHoloTexture(out IntPtr srv);

        [DllImport("UnityCompositorInterface")]
        public static extern bool SetMergedRenderTexture(IntPtr texturePtr);

        [DllImport("UnityCompositorInterface")]
        public static extern bool SetVideoRenderTexture(IntPtr texturePtr);

        [DllImport("UnityCompositorInterface")]
        public static extern bool SetOutputRenderTexture(IntPtr texturePtr);

        [DllImport("UnityCompositorInterface")]
        public static extern bool IsRecording();

        [DllImport("UnityCompositorInterface")]
        public static extern bool OutputYUV();

        [DllImport("UnityCompositorInterface")]
        public static extern bool QueueingHoloFrames();

        [DllImport("UnityCompositorInterface")]
        public static extern bool HardwareEncodeVideo();

        [DllImport("UnityCompositorInterface")]
        public static extern void StopFrameProvider();

        [DllImport("UnityCompositorInterface")]
        public static extern void TakePicture();

        [DllImport("UnityCompositorInterface", CharSet = CharSet.Unicode)]
        public static extern void TakeRawPicture(string path);

        [DllImport("UnityCompositorInterface")]
        public static extern void StartRecording();

        [DllImport("UnityCompositorInterface")]
        public static extern void StopRecording();
        
        [DllImport("UnityCompositorInterface")]
        public static extern bool InitializeFrameProviderOnDevice(int providerId);  //0 = blackmagic, 1 = elgato

        [DllImport("UnityCompositorInterface")]
        public static extern void Reset();

        [DllImport("UnityCompositorInterface")]
        public static extern long GetColorDuration();

        [DllImport("UnityCompositorInterface")]
        public static extern int GetNumQueuedOutputFrames();

        [DllImport("UnityCompositorInterface")]
        public static extern IntPtr GetRenderEventFunc();

        [DllImport("UnityCompositorInterface")]
        public static extern void SetAudioData(byte[] audioData, int dataLength, double audioTime);

        [DllImport("UnityCompositorInterface")]
        public static extern void UpdateCompositor();

        [DllImport("UnityCompositorInterface")]
        public static extern void UpdateSpectatorView();

        [DllImport("UnityCompositorInterface")]
        public static extern int GetCaptureFrameIndex();

        [DllImport("UnityCompositorInterface")]
        public static extern void SetCompositeFrameIndex(int index);
    }
}
#endif
