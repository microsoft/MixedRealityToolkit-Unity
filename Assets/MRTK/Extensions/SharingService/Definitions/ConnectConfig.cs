namespace Microsoft.MixedReality.Toolkit.Extensions.Sharing
{
    public struct ConnectConfig
    {
        public AppRoleEnum RequestedRole;
        public string LobbyName;
        public string RoomName;
        public SubscriptionModeEnum SubscriptionMode;
        public int[] SubscriptionTypes;
    }
}