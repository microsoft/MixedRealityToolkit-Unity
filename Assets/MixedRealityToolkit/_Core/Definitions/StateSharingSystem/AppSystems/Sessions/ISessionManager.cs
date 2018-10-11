using Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.Core;
using System.Collections.Generic;

namespace Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.AppSystems.Sessions
{
    public interface ISessionManager : ISharingAppObject
    {
        ISessionStageBase CurrentStage { get; }
        IEnumerable<IExperienceMode> AvailableModes { get; }
        IExperienceMode ExperienceMode { get; }
        SessionStateEnum State { get; }
        byte StageNum { get; }
        
        void SetExperienceMode(short gameModeID);
        void SetLayoutSceneLoaded();
        void SetUsersReadyToStart();

        void StartSession();
        void ForceCompleteStage();
        void ResetSession();

        bool TryUpdateSession();
    }
}