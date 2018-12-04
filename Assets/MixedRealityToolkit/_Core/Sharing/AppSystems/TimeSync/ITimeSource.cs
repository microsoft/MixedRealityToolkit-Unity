namespace Pixie.AppSystems.TimeSync
{
    public interface ITimeSource
    {
        float Time { get; }
        float DeltaTime { get; }
        bool Paused { get; }
    }
}