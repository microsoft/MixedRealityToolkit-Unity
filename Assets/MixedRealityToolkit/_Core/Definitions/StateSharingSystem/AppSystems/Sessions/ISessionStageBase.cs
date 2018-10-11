using Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.Core;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.AppSystems.Sessions
{
    public interface ISessionStageBase : IGameObject
    {
        StageStateEnum State { get; }
        StageProgressionTypeEnum ProgressionType { get; }
        float MaxTime { get; }
        float TimeStarted { get; }
        float TimeElapsed { get; }
        AudioClip MusicClip { get; }
        AudioClip OneShotClip { get; }
        AudioClip AmbientClip { get; }
        float MusicVolume { get; }
        float OneShotVolume { get; }
        float AmbientVolume { get; }
    }
}