namespace Pixie.AppSystems
{
    public enum SessionStateEnum : byte
    {
        Uninitialized,              // App hasn't initialized manager yet
        ChoosingExperience,         // The experience mode hasn't been chosen
        WaitingForApp,              // Experience mode is chosen, but app hasn't requested session start
        LoadingLayoutScene,         // App has requested session start and layout scene is loading
        ReadyToStart,               // Session is loaded and ready to start
        InProgress,                 // Session has begun
        Completed,                  // Session has completed
    }
}