namespace Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.AppSystems
{
    public interface ITimeSource
    {
        float Time { get; }
        float DeltaTime { get; }
        bool Paused { get; }
    }
}