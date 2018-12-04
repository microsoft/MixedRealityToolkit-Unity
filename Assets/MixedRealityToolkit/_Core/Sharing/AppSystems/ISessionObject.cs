using Pixie.Core;
using UnityEngine;

namespace Pixie.AppSystems
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