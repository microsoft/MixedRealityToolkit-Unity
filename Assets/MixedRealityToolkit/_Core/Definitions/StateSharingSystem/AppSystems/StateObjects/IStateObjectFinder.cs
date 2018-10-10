using Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.Core;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.AppSystems.StateObjects
{
    /// <summary>
    /// Used to find state objects in scene
    /// When a state view is present in the scene, the finder uses that
    /// Otherwise, the finder uses FindObjectsOfType.
    /// Useful when you want to test a scene without multiplayer.
    /// </summary>
    public interface IStateObjectFinder
    {
        IEnumerable<T> FindStateObjectsOfType<S, T>(bool requireActive = true) where S : IItemState<S> where T : Component, IStateObject<S>;
    }
}