namespace Microsoft.MixedReality.Toolkit.Extensions.Sharing
{
    public struct ConnectConfig
    {
        public RoomConfig RoomConfig;
        public AppRole RequestedRole;
        public SubscriptionMode SubscriptionMode;
        public short[] SubscriptionTypes;
    }
}