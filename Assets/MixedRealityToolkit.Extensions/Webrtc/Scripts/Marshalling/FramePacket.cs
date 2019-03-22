namespace Microsoft.MixedReality.Toolkit.Extensions.WebRTC.Marshalling
{
    ///
    /// Based on https://unitylist.com/p/cxl/Web-Rtc-Unity-Plugin-Sample
    public class FramePacket
    {
        public FramePacket(int bufsize)
        {
            _buffer = new byte[bufsize];
        }

        public int width;
        public int height;
        private byte[] _buffer;
        public byte[] Buffer
        {
            get { return _buffer; }
        }

        public override string ToString()
        {
            return "FramePacket width, height=(" + width + "," + height + ") buffer size:" + _buffer.Length;
        }
    }
}
