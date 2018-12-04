namespace Pixie.StateControl.Logging
{
    public interface IAppStatePlayback
    {
        PlaybackStateEnum State { get; }
        float TotalDuration { get; }
        float CurrentTime { get; }

        void StartPlayback(string logFilePath);
        void SetTime(float currentTime);
        void StopPlayback();
    }
}