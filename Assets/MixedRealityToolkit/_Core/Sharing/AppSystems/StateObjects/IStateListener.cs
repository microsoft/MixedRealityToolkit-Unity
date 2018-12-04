using Pixie.Core;

namespace Pixie.AppSystems.StateObjects
{
    public interface IStateListener<T> where T : struct, IItemState
    {
        void OnStateInitialize(IStateObject<T> stateObj, T initState);
        void OnStateChange(IStateObject<T> stateObj, T oldState, T newState);
    }
}