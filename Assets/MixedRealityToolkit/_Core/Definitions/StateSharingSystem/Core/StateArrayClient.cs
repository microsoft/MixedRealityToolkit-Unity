using UnityEngine.Networking;

namespace Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.Core
{
    public class StateArrayClient<T> : StateArrayBase<T> where T : struct, IItemState<T>
    {
        public override void Create(SyncListStruct<T> states)
        {
            this.states = states;
            states.Callback += StatesCallback;
        }

        private void StatesCallback(SyncList<T>.Operation op, int itemIndex)
        {
            switch (op)
            {
                case SyncList<T>.Operation.OP_ADD:
                case SyncList<T>.Operation.OP_CLEAR:
                case SyncList<T>.Operation.OP_DIRTY:
                case SyncList<T>.Operation.OP_INSERT:
                case SyncList<T>.Operation.OP_REMOVE:
                case SyncList<T>.Operation.OP_REMOVEAT:
                    // We don't care about these operations
                    // They don't modify existing set values
                    break;

                case SyncList<T>.Operation.OP_SET:
                    // If our modified states has an entry for this item index, clear that modified state
                    T serverSideValue = states[itemIndex];
                    if (modifiedStates.ContainsKey(serverSideValue.Key))
                    {
                        // Do a comparison before removing the item.
                        // If it's different than our 'predicted' value then the states will need to be merged.
                        T clientSideValue = modifiedStates[serverSideValue.Key];
                        if (clientSideValue.IsDifferent(serverSideValue))
                        {
                            // Get the merged state
                            T mergedState = clientSideValue.Merge(clientSideValue, serverSideValue);                            
                            if (mergedState.IsDifferent(serverSideValue))
                            {
                                /*Debug.LogWarning("Local value of type " + typeof(T).ToString() + " in sync list is different from newly aquired server side value after merge. \n"
                                    + "Client:\n"
                                    + mergedState.ToString()
                                    + "\nServer:\n"
                                    + serverSideValue.ToString());*/
                                // If they're STILL different, store the merged state
                                modifiedStates[serverSideValue.Key] = mergedState;
                                // Let subscribers know that the state has changed
                                if (OnStateChangedInternal != null)
                                    OnStateChangedInternal(StateType, serverSideValue.Key);
                            }
                            else
                            {
                                // If the merged state is now the same as the server state, just discard the modified state
                                // (State changed action was already fired when the modified state was added)
                                modifiedStates.Remove(serverSideValue.Key);
                            }
                        }
                        else
                        {
                            // If there's no difference, just remove the modified state
                            // (State changed action was already fired when the modified state was added)
                            modifiedStates.Remove(serverSideValue.Key);
                        }
                    }
                    break;
            }
        }

        public override void Flush()
        {
            // Does nothing
            // Modified states are retained until they are over-written by a server change
        }
    }
}