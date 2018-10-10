using Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.Core;

namespace Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.AppSystems.StateObjects
{
    public interface IStateListener<T> where T : struct, IItemState<T>
    {
        void OnStateInitialize(IStateObject<T> stateObj, T initState);
        void OnStateChange(IStateObject<T> stateObj, T oldState, T newState);
    }
}