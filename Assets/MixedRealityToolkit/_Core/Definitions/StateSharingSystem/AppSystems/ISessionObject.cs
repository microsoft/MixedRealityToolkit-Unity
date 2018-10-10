using Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.Core;

namespace Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.AppSystems
{
    public interface ISessionObject : IGameObject
    {
        void OnSessionStart();
        void OnSessionStageBegin();
        void OnSessionUpdate(SessionState sessionState);
        void OnSessionStageEnd();
        void OnSessionEnd();
    }
}