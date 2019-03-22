using MRTK.Core;
using System;
using System.Collections.Generic;

namespace MRTK.StateControl
{
    public interface IAppStateData : IEnumerable<IStateArrayBase>, ISharingAppObject
    {
        Action<Type, List<object>> OnReceiveChangedStates { get; set; }
        bool Synchronized { get; }

        void OnServiceInitialized(AppStateProfile profile, List<ISharingAppObject> sharingAppObjects);
        void CreateStateArray(Type type);
        bool ContainsStateType(Type stateType);
        bool TryGetData(Type type, out IStateArrayBase stateArray);
        bool TryGetData<T>(out IStateArray<T> stateArray) where T : struct, IItemState, IItemStateComparer<T>;
    }
}