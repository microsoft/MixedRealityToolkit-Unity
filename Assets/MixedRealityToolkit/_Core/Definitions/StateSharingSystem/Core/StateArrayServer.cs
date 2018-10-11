using UnityEngine.Networking;

namespace Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.Core
{
    public class StateArrayServer<T> : StateArrayBase<T> where T : struct, IItemState<T>
    {
        public override void Create(SyncListStruct<T> states)
        {
            this.states = states;
        }

        public override void Flush()
        {
            // On the server we copy our modified states into our states array
            // This change will then be propagated to all clients
            if (modifiedStates.Count > 0)
            {
                T modifiedState = default(T);
                for (int i = 0; i < states.Count; i++)
                {
                    if (modifiedStates.TryGetValue(states[i].Key, out modifiedState))
                    {
                        if (states[i].IsDifferent(modifiedState))
                            states[i] = modifiedState;
                    }
                }
                modifiedStates.Clear();
            }
        }

        public void AddState(T state)
        {
            for (int i = 0; i < states.Count; i++)
            {
                if (states[i].Key == state.Key)
                {
                    throw new System.Exception("Collision with existing state at index " + i);
                }
            }
            states.Add(state);
        }
    }
}