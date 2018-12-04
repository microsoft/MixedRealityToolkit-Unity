using Pixie.Core;
using System;
using System.Collections.Generic;

namespace Pixie.StateControl
{
    public interface IAppStateSource : IGameObject
    {
        IEnumerable<Type> StateTypes { get; }
        void GenerateRequiredStates(IAppStateReadWrite appState);
    }
}