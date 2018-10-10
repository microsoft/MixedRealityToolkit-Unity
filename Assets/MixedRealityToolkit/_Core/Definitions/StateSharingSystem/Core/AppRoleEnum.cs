namespace Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.Core
{
    public enum AppRoleEnum : byte
    {
        Client = 1,
        Server = 2,
        Host = Client | Server
    }
}