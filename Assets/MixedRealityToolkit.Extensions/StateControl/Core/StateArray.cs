using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.MixedReality.Toolkit.Extensions.StateControl.Core
{
    // We use this class to get / set items in our sync list state in a standard way
    public class StateArray<T> : IStateArray<T> where T : struct, IItemState, IItemStateComparer<T>
    {
        public StateArray(IStatePipe statePipe)
        {
            this.statePipe = statePipe;
        }

        public Action<Type, short> OnStateChangedExternal { get; set; }
        public Action<Type, short> OnStateChangedInternal { get; set; }

        public Type StateType { get { return typeof(T); } }

        public bool IsEmpty { get { return states.Count == 0 && modifiedStates.Count == 0; } }

        public int Count { get { return states.Count; } }

        public bool IsReadOnly { get { return false; } }

        // Synchronized states
        private Dictionary<short, T> states = new Dictionary<short, T>();
        // States that have been modified locally (including added to)
        private Dictionary<short, T> modifiedStates = new Dictionary<short, T>();
        // A list of keys that is maintained for performant iteration by our enumerator
        // Contains keys for base states as well as modified states
        private HashSet<short> keys = new HashSet<short>();
        // A list of objects used for serialization when flushing
        private List<object> flushedStates = new List<object>();

        private IStatePipe statePipe;

        public IEnumerator<T> GetEnumerator()
        {
            StateArrayEnumerator<T> enumerator = new StateArrayEnumerator<T>();
            enumerator.Initialize(this);
            return enumerator;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            StateArrayEnumerator<T> enumerator = new StateArrayEnumerator<T>();
            enumerator.Initialize(this);
            return enumerator;
        }

        public bool KeyExists(short key)
        {
            // No need to check modified values since they don't change keys
            return (modifiedStates.ContainsKey(key) || states.ContainsKey(key));
        }

        public void ReceiveSynchronizedStates(IEnumerable<object> remoteStates)
        {
            UnityEngine.Debug.Log("Receiving synced states in state array " + StateType.Name);

            foreach (T remoteValue in remoteStates)
            {
                if (!states.ContainsKey(remoteValue.Key))
                {
                    states.Add(remoteValue.Key, remoteValue);
                    keys.Add(remoteValue.Key);
                    continue;
                }

                // Don't bother with modified states when synchronizing
                // Just dump the states into the main array
                states[remoteValue.Key] = remoteValue;
            }
        }

        public void ReceiveFlushedStates(IEnumerable<object> remoteStates)
        {
            foreach (T remoteValue in remoteStates)
            {
                // If our local states don't contain this key, add it to states
                // NOTE that this will not affect states added to modified states
                // Those values will be dealt with when this state array is flushed
                if (!states.ContainsKey(remoteValue.Key))
                {
                    states.Add(remoteValue.Key, remoteValue);
                    keys.Add(remoteValue.Key);
                    continue;
                }

                // If our modified states has an entry for this item index, clear that modified state
                T localValue = states[remoteValue.Key];
                if (modifiedStates.ContainsKey(localValue.Key))
                {
                    // Do a comparison before removing the item.
                    // If it's different than our 'predicted' value then the states will need to be merged.
                    T localModifiedValue = modifiedStates[localValue.Key];
                    if (localModifiedValue.IsDifferent(localValue))
                    {
                        // Get the merged state
                        T mergedState = localModifiedValue.Merge(localModifiedValue, remoteValue);
                        if (mergedState.IsDifferent(localValue))
                        {
                            // If they're STILL different, store the merged state
                            modifiedStates[localValue.Key] = mergedState;
                            // Let subscribers know that the state has changed
                            if (OnStateChangedInternal != null)
                                OnStateChangedInternal(StateType, localValue.Key);
                        }
                        else
                        {
                            // If the merged state is now the same as the server state, just discard the modified state
                            // (State changed action was already fired when the modified state was added)
                            modifiedStates.Remove(localValue.Key);
                        }
                    }
                    else
                    {
                        // If there's no difference, just remove the modified state
                        // (State changed action was already fired when the modified state was added)
                        modifiedStates.Remove(localValue.Key);
                    }
                }
                else
                {
                    // If our modified states doesn't have an entry, just set the local state
                    states[remoteValue.Key] = remoteValue;

                    if (OnStateChangedInternal != null)
                        OnStateChangedInternal(StateType, localValue.Key);
                }
            }
        }

        public void Flush()
        {
            // On the server we copy our modified states into our states array
            // If any are still different, these changes are sent to clients via rpc call
            if (modifiedStates.Count > 0)
            {
                flushedStates.Clear();
                foreach (KeyValuePair<short, T> modifiedState in modifiedStates)
                {
                    if (!states.ContainsKey(modifiedState.Key))
                    {
                        // If states doesn't contain the key, add it
                        flushedStates.Add(modifiedState.Value);
                    }
                    else if (states[modifiedState.Key].IsDifferent(modifiedState.Value))
                    {
                        // Otherwise, if it's different from modified value, change it
                        flushedStates.Add(modifiedState.Value);
                    }
                }
                modifiedStates.Clear();

                // Did any states survive this process?
                if (flushedStates.Count > 0)
                    statePipe.SendFlushedStates(StateType, flushedStates);
            }
        }

        public void Flush(short key)
        {
            T modifiedState;
            if (modifiedStates.TryGetValue(key, out modifiedState))
            {
                flushedStates.Clear();
                if (!states.ContainsKey(modifiedState.Key))
                {
                    // If states doesn't contain the key, add it
                    flushedStates.Add(modifiedState);
                }
                else if (states[modifiedState.Key].IsDifferent(modifiedState))
                {
                    // Otherwise, if it's different from modified value, change it
                    flushedStates.Add(modifiedState);
                }

                if (flushedStates.Count > 0)
                    statePipe.SendFlushedStates(StateType, flushedStates);
            }
        }

        public void Flush(IEnumerable<short> keys)
        {
            if (modifiedStates.Count > 0)
            {
                flushedStates.Clear();
                foreach (short key in keys)
                {
                    T modifiedState;
                    if (modifiedStates.TryGetValue(key, out modifiedState))
                    {
                        if (!states.ContainsKey(modifiedState.Key))
                        {
                            // If states doesn't contain the key, add it
                            flushedStates.Add(modifiedState);
                        }
                        else if (states[modifiedState.Key].IsDifferent(modifiedState))
                        {
                            // Otherwise, if it's different from modified value, change it
                            flushedStates.Add(modifiedState);
                        }
                    }
                }

                if (flushedStates.Count > 0)
                    statePipe.SendFlushedStates(StateType, flushedStates);
            }
        }

        public T this[short key]
        {
            get
            {
                if (key >= short.MaxValue || key < 0)
                    throw new IndexOutOfRangeException("Index " + key + " is out of range");

                if (states == null)
                    throw new NullReferenceException("States was null in ObjectStateArray");
                
                if (modifiedStates.ContainsKey(key))
                    return modifiedStates[key];

                if (states.ContainsKey(key))
                    return states[key];

                throw new IndexOutOfRangeException("Couldn't find element " + key + " in " + StateType.Name + " ObjectStateArray");
            }
            set
            {
                if (states == null)
                    throw new NullReferenceException("States was null in ObjectStateArray");

                if (key >= short.MaxValue || key <= short.MinValue)
                    throw new IndexOutOfRangeException("Index " + key + " is out of range");

                if (!TrySetValue(key, value))
                    throw new IndexOutOfRangeException("Couldn't set element " + key + " in " + StateType.Name + " ObjectStateArray");
            }
        }

        protected bool TrySetValue(short key, T value)
        {
            // If we've set a modified value for this key
            // Check against the modified states
            if (modifiedStates.ContainsKey(key))
            {
                // If the modified state is the same as the new value, do nothing
                T state = modifiedStates[key];
                if (!state.IsDifferent(value))
                    return true;

                // Set the new modified state
                modifiedStates[key] = value;

                // Let subscribers know a value has changed
                if (OnStateChangedExternal != null)
                    OnStateChangedExternal(StateType, key);

                // We're done here
                return true;
            }

            // If we haven't set a modified value yet
            // Check against the unmodified states
            if (states.ContainsKey(key))
            {
                // If there's no difference in state
                // Do nothing
                T state = states[key];
                if (!state.IsDifferent(value))
                    return true;

                // Otherwise store the value in our modified states
                modifiedStates.Add(key, value);

                // Let subscribers know a value has changed
                if (OnStateChangedExternal != null)
                    OnStateChangedExternal(StateType, key);

                // We're done here
                return true;
            }

            return false;
        }

        private short GetNextKey(short currentKey)
        {
            if (IsEmpty)
                return -1;
            
            IEnumerator<short> keyEnumerator = keys.GetEnumerator();
            while (keyEnumerator.MoveNext())
            {
                if (keyEnumerator.Current == currentKey)
                {
                    if (keyEnumerator.MoveNext())
                        return keyEnumerator.Current;

                    break;
                }
            }

            return -1;
        }

        public short GetNextAvailableKey()
        {
            if (IsEmpty)
                return 0;

            short maxKey = 0;
            foreach (short key in keys)
            {
                if (key > maxKey)
                    maxKey = key;
            }

            maxKey++;
            return maxKey;
        }

        public object GetState(short key)
        {
            if (key >= sbyte.MaxValue || key < 0)
                throw new IndexOutOfRangeException("Index " + key + " is out of range");

            if (states == null)
                throw new NullReferenceException("States was null in ObjectStateArray");

            if (modifiedStates.ContainsKey(key))
                return modifiedStates[key];

            if (states.ContainsKey(key))
                return states[key];

            throw new IndexOutOfRangeException("Couldn't find element " + key + " in " + typeof(T).ToString() + " ObjectStateArray");
        }

        public void AddState(T state)
        {
            if (states.ContainsKey(state.Key))
                throw new System.Exception("Collision with existing state key " + state.Key + "\n" + state.ToString());

            if (modifiedStates.ContainsKey(state.Key))
                throw new System.Exception("Collision with existing state key " + state.Key + "\n" + state.ToString());

            modifiedStates.Add(state.Key, state);
            keys.Add(state.Key);
        }

        public void AddState(IItemState state)
        {
            if (states.ContainsKey(state.Key))
                throw new System.Exception("Collision with existing state key " + state.Key + "\n" + state.ToString());

            if (modifiedStates.ContainsKey(state.Key))
                throw new System.Exception("Collision with existing state key " + state.Key + "\n" + state.ToString());

            modifiedStates.Add(state.Key, (T)state);
            keys.Add(state.Key);
        }

        public IEnumerable<object> GetStates()
        {
            if (modifiedStates.Count == 0)
            {
                foreach (object state in states.Values)
                    yield return state;
            }
            else
            {
                foreach (short key in keys)
                    yield return modifiedStates.ContainsKey(key) ? modifiedStates[key] : states[key];
            }
        }

        public struct StateArrayEnumerator<R> : IEnumerator<R> where R : struct, IItemState, IItemStateComparer<R>
        {
            public void Initialize(StateArray<R> stateArray)
            {
                this.stateArray = stateArray;
                this.keyEnumerator = stateArray.keys.GetEnumerator();
            }

            public R Current
            {
                get
                {
                    return stateArray[keyEnumerator.Current];
                }
            }

            object IEnumerator.Current
            {
                get
                {
                    return stateArray[keyEnumerator.Current];
                }
            }

            public void Dispose()
            {
                stateArray = null;
                keyEnumerator.Dispose();
            }

            public bool MoveNext()
            {
                return keyEnumerator.MoveNext();
            }

            public void Reset()
            {

            }

            StateArray<R> stateArray;
            private IEnumerator<short> keyEnumerator;
        }
    }
}