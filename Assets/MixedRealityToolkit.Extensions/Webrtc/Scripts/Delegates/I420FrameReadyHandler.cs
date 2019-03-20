using System;

namespace Microsoft.MixedReality.Toolkit.Extensions.Webrtc.Delegates
{
    /// <summary>
    /// I420 frame callback
    /// </summary>
    /// <param name="dataY">Y data pointer</param>
    /// <param name="dataU">U data pointer</param>
    /// <param name="dataV">V data pointer</param>
    /// <param name="dataA">A data pointer</param>
    /// <param name="strideY">Y stride</param>
    /// <param name="strideU">U stride</param>
    /// <param name="strideV">V stride</param>
    /// <param name="strideA">A stride</param>
    /// <param name="width">frame width</param>
    /// <param name="height">frame height</param>
    public delegate void I420FrameReadyHandler(IntPtr dataY,
            IntPtr dataU,
            IntPtr dataV,
            IntPtr dataA,
            int strideY,
            int strideU,
            int strideV,
            int strideA,
            uint width,
            uint height);
}
