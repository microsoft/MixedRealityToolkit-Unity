namespace Microsoft.MixedReality.Toolkit.Extensions.Sharing
{
    public struct ConnectConfig
    {
        public AppRole RequestedRole;
        public string LobbyName;
        public string RoomName;
        public SubscriptionModeEnum SubscriptionMode;
        public short[] SubscriptionTypes;
    }
}