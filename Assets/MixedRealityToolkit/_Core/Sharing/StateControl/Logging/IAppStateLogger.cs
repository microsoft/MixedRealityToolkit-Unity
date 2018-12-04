namespace Pixie.StateControl.Logging
{
    public interface IAppStateLogger
    {
        LogStateEnum State { get; }
        int NumLoggedStates { get; }
        int NumQueuedSnapshots { get; }

        void StartLogging(string logFilePath);
        void StopLogging();
    }
}