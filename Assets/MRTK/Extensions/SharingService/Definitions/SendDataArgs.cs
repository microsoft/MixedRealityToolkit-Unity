namespace Microsoft.MixedReality.Toolkit.Extensions.Sharing
{
    public struct SendDataArgs
    {
        /// <summary>
        /// Used by consumers of the service to idenfity the type of data being sent.
        /// </summary>
        public short Type;
        /// <summary>
        /// Binary data being sent. Null values are permitted.
        /// </summary>
        public byte[] Data;
        /// <summary>
        /// Reliability and sequencing modes.
        /// </summary>
        public DeliveryMode DeliveryMode;
        /// <summary>
        /// Who will receive the data.
        /// </summary>
        public TargetMode TargetMode;
        /// <summary>
        /// If not null, data will be sent to the specified device IDs.
        /// This overrides SkipSender.
        /// </summary>
        public short[] Targets;
    }
}