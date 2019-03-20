namespace Microsoft.MixedReality.Toolkit.Extensions.Webrtc.Marshalling
{
    /// <summary>
    /// A readonly frame queue stat
    /// </summary>
    public interface IReadonlyFrameQueueStat
    {
        // <summary>
        /// The average value calculated for the stat
        /// </summary>
        float Value
        {
            get;
        }
    }
}
