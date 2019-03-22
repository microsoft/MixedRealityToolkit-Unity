using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.WebRTC.Marshalling
{
    // 返却されたバッファをできるだけ再利用するバッファプール。
    // スレッドセーフ。
    ///
    /// Based on https://unitylist.com/p/cxl/Web-Rtc-Unity-Plugin-Sample
    public class FramePacketPool
    {
        private Deque<FramePacket> pool = new Deque<FramePacket>();

        // リクエストされたサイズ以上のバッファを返す。
        public FramePacket GetDataBuffer(int size)
        {
            lock (this)
            {
                // TODO: １回だけでいいの？
                if (pool.Count > 0)
                {
                    FramePacket candidate = pool.RemoveFront();
                    if (candidate == null)
                    {
                        Debug.LogError("candidate is null! returns new buffer.");
                        return GetNewBuffer(size);
                    }
                    else
                    {
                        if (candidate.Buffer == null)
                        {
                            Debug.LogError("candidate.Buffer is null!");
                        }
                    }
                    if (candidate.Buffer.Length > size)
                    {
                        return candidate;
                    }
                }
            }
            return GetNewBuffer(size);
        }

        private FramePacket GetNewBuffer(int neededSize)
        {
            FramePacket packet = new FramePacket((int)(neededSize * 1.2));
            return packet;
        }

        // バッファをプールから取り出し、さらにデータをコピーして渡す
        public FramePacket GetDataBufferWithContents(int width, int height, byte[] src, int size)
        {
            FramePacket dest = GetDataBuffer(size);
            System.Array.Copy(src, 0, dest.Buffer, 0, size);
            dest.width = width;
            dest.height = height;
            return dest;
        }

        // 返却
        public void Push(FramePacket packet)
        {
            lock (this)
            {
                pool.AddFront(packet);
            }
        }
    }
}
