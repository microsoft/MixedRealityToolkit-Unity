namespace Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.AppSystems
{
    public enum SessionStateEnum : byte
    {
        ChoosingExperience,
        WaitingForUsers,
        WaitingForDevices,
        LoadingLayoutScene,
        UsersReadyToStart,
        InProgress,
        Completed,
    }
}