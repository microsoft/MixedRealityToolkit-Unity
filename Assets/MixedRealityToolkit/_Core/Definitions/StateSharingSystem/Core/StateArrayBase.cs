using UnityEngine.Networking;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.Core
{
    // We use this class to get / set items in our sync list state in a standard way
    public abstract class StateArrayBase<T> : IStateArray<T> where T : struct, IItemState<T>
    {
        public Action<Type, sbyte> OnStateChangedExternal { get; set; }
        public Action<Type, sbyte> OnStateChangedInternal { get; set; }

        public Type StateType { get { return typeof(T); } }

        public bool IsEmpty { get { return states == null || states.Count == 0; } }

        public int Count { get { return (!IsEmpty) ? states.Count : 0; } }

        protected SyncListStruct<T> states;
        protected Dictionary<sbyte, T> modifiedStates = new Dictionary<sbyte, T>();

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

        public abstract void Create(SyncListStruct<T> states);

        public bool KeyExists(sbyte key)
        {
            // No need to check modified values since they don't change keys
            for (int i = 0; i < states.Count; i++)
            {
                if (states[i].Key == key)
                    return true;
            }
            return false;
        }

        public abstract void Flush();

        public T this[sbyte key]
        {
            get
            {
                if (key >= sbyte.MaxValue || key < 0)
                    throw new IndexOutOfRangeException("Index " + key + " is out of range");

                if (states == null)
                    throw new NullReferenceException("States was null in ObjectStateArray");

                if (modifiedStates != null && modifiedStates.ContainsKey(key))
                    return modifiedStates[key];

                for (int i = 0; i < states.Count; i++)
                {
                    if (states[i].Key == key)
                        return states[i];
                }

                throw new IndexOutOfRangeException("Couldn't find element " + key + " in " + typeof(T).ToString() + " ObjectStateArray");
            }
            set
            {
                if (states == null)
                    throw new NullReferenceException("States was null in ObjectStateArray");

                if (key >= sbyte.MaxValue || key <= sbyte.MinValue)
                    throw new IndexOutOfRangeException("Index " + key + " is out of range");

                if (modifiedStates == null)
                    modifiedStates = new Dictionary<sbyte, T>();

                if (!TrySetValue(key, value))
                    throw new IndexOutOfRangeException("Couldn't find element " + key + " in " + typeof(T).ToString() + " ObjectStateArray");
            }
        }

        protected virtual bool TrySetValue(sbyte key, T value)
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
            for (sbyte i = 0; i < states.Count; i++)
            {
                if (states[i].Key == key)
                {
                    // If there's no difference in state
                    // Do nothing
                    if (!states[i].IsDifferent(value))
                        return true;

                    // Store the value in our modified states.
                    modifiedStates.Add(key, value);

                    // Let subscribers know a value has changed
                    if (OnStateChangedExternal != null)
                        OnStateChangedExternal(StateType, key);

                    // We're done here
                    return true;
                }
            }

            return false;
        }

        private sbyte GetNextKey(sbyte currentKey)
        {
            if (IsEmpty)
                return -1;

            // If it's zero, we're at the start
            if (currentKey < 0)
            {
                return states[0].Key;
            }

            for (sbyte i = 0; i < states.Count; i++)
            {
                if (states[i].Key == currentKey)
                {
                    // If we're at the end of the array, there is no next index
                    if (i >= states.Count - 1)
                    {
                        return -1;
                    }

                    return states[i + 1].Key;
                }
            }

            return -1;
        }

        public sbyte GetNextAvailableKey()
        {
            if (IsEmpty)
                return 0;

            HashSet<int> availableKeys = new HashSet<int>(Enumerable.Range(0, sbyte.MaxValue));

            for (int i = 0; i < states.Count; i++)
            {
                availableKeys.Remove(states[i].Key);
            }

            return (sbyte)availableKeys.Min();
        }

        public object GetState(sbyte key)
        {
            if (key >= sbyte.MaxValue || key < 0)
                throw new IndexOutOfRangeException("Index " + key + " is out of range");

            if (states == null)
                throw new NullReferenceException("States was null in ObjectStateArray");

            if (modifiedStates != null && modifiedStates.ContainsKey(key))
                return modifiedStates[key];

            for (int i = 0; i < states.Count; i++)
            {
                if (states[i].Key == key)
                    return states[i];
            }

            throw new IndexOutOfRangeException("Couldn't find element " + key + " in " + typeof(T).ToString() + " ObjectStateArray");
        }

        public IEnumerable<object> GetStates()
        {
            if (modifiedStates == null || modifiedStates.Count == 0)
            {
                foreach (object state in states)
                    yield return state;
            }
            else
            {
                for (int i = 0; i < states.Count; i++)
                {
                    sbyte key = states[i].Key;
                    yield return modifiedStates.ContainsKey(key) ? modifiedStates[key] : states[i];
                }
            }
        }

        public struct StateArrayEnumerator<R> : IEnumerator<R> where R : struct, IItemState<R>
        {
            public void Initialize(StateArrayBase<R> states)
            {
                this.states = states;
                this.currentKey = -1;
            }

            public R Current
            {
                get
                {
                    return states[currentKey];
                }
            }

            object IEnumerator.Current
            {
                get
                {
                    return states[currentKey];
                }
            }

            public void Dispose()
            {
                states = null;
            }

            public bool MoveNext()
            {
                if (states == null)
                {
                    return false;
                }

                currentKey = states.GetNextKey(currentKey);

                if (currentKey < 0)
                {
                    return false;
                }

                return true;
            }

            public void Reset()
            {
                currentKey = -1;
            }

            private sbyte currentKey;
            private StateArrayBase<R> states;
        }
    }
}