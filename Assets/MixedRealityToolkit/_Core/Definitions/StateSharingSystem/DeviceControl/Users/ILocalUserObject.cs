using Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.StateControl;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.DeviceControl.Users
{
    /// <summary>
    /// Local player object - direct control over transforms and position values as well as specific player actions.
    /// </summary>
    public interface ILocalUserObject : IUserObject
    {
        Transform HeadTransform { get; }
        Transform RightHandTransform { get; }
        Transform LeftHandTransform { get; }
        Transform SceneOffset { get; }
        Transform CameraParent { get; }

        IStatePipeInput StatePipeInput { get; }
    }
}