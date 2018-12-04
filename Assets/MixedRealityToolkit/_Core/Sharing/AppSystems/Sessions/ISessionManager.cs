using Pixie.Core;
using System.Collections.Generic;

namespace Pixie.AppSystems.Sessions
{
    public interface ISessionManager : ISharingAppObject
    {
        ISessionStageBase CurrentStage { get; }
        IEnumerable<IExperienceMode> AvailableModes { get; }
        IExperienceMode ExperienceMode { get; }
        SessionStateEnum State { get; }
        byte StageNum { get; }
        bool Paused { get; }
        
        void SetExperienceMode(short gameModeID);
        void SetLayoutSceneLoaded();

        void StartSession();
        bool TryUpdateSession();
        void SetPaused(bool paused);
        void ForceCompleteStage();
        void ResetSession();
    }
}