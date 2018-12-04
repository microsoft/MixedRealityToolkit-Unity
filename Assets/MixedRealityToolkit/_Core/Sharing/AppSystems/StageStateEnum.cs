namespace Pixie.AppSystems
{
    public enum StageStateEnum : byte
    {
        NotStarted,
        TransitionIn,
        Running,
        TransitionOut,
        Completed,
    }
}