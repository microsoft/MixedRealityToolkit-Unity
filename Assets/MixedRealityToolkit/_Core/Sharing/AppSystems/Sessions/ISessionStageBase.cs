using Pixie.Core;

namespace Pixie.AppSystems.Sessions
{
    public interface ISessionStageBase : IGameObject
    {
        StageStateEnum State { get; }
        StageProgressionTypeEnum ProgressionType { get; }
        float MaxTime { get; }
        float TimeStarted { get; }
        float TimeElapsed { get; }
    }
}