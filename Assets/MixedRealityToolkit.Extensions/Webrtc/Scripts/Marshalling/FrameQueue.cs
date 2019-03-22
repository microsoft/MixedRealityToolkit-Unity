using System.Collections.Generic;

namespace Microsoft.MixedReality.Toolkit.Extensions.WebRTC.Marshalling
{
    // FramePacket のキュー。
    // 指定サイズ以上Pushすると、指定サイズ以下になるよう末尾のデータから削除される。
    // スレッドセーフ。
    ///
    /// Based on https://unitylist.com/p/cxl/Web-Rtc-Unity-Plugin-Sample
    public class FrameQueue
    {
        public int Count
        {
            get
            {
                lock (this)
                {
                    return frames.Count;
                }
            }
        }

        public FramePacketPool FramePacketPool
        {
            get
            {
                return bufferPool;
            }
        }

        private FrameQueueStat frameLoad = new FrameQueueStat(100);
        public IReadonlyFrameQueueStat FrameLoad
        {
            get
            {
                return frameLoad;
            }
        }

        private FrameQueueStat framePresent = new FrameQueueStat(100);
        public IReadonlyFrameQueueStat FramePresent
        {
            get
            {
                return framePresent;
            }
        }

        private FrameQueueStat frameSkip = new FrameQueueStat(100);
        public IReadonlyFrameQueueStat FrameSkip
        {
            get
            {
                return frameSkip;
            }
        }

        private Deque<FramePacket> frames = new Deque<FramePacket>();
        private FramePacketPool bufferPool = new FramePacketPool();
        private int maxQueueCount;
        
        public FrameQueue(int _maxQueueCount)
        {
            maxQueueCount = _maxQueueCount;
        }

        public void Push(FramePacket frame)
        {
            frameLoad.Track();
            FramePacket trashBuf = null;
            lock (this)
            {
                frames.AddFront(frame);
                if (frames.Count >= maxQueueCount)
                {
                    frameSkip.Track();
                    trashBuf = frames.RemoveBack();
                }
            }
            // lock内でPushしないのは、thisとbufferPoolの両方のlockを同時にとらないようにする配慮。
            if (trashBuf != null)
            {
                bufferPool.Push(trashBuf);
            }
        }

        public FramePacket Pop()
        {
            lock (this)
            {
                if (frames.IsEmpty)
                {
                    return null;
                }
                framePresent.Track();
                return frames.RemoveBack();
            }
        }

        public FramePacket GetDataBufferWithContents(int width, int height, byte[] src, int size)
        {
            return bufferPool.GetDataBufferWithContents(width, height, src, size);
        }

        public FramePacket GetDataBufferWithoutContents(int size)
        {
            return bufferPool.GetDataBuffer(size);
        }

        public void Pool(FramePacket buf)
        {
            bufferPool.Push(buf);
        }
    }
}
