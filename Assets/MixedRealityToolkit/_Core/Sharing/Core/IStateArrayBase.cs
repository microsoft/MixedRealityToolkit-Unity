using System;
using System.Collections.Generic;

namespace MRTK.Core
{
    public interface IStateArrayBase
    {
        /// <summary>
        /// Called when an element has been changed from the outside
        /// Sends the type of the state and the key of the changed element
        /// </summary>
        Action<Type, short> OnStateChangedExternal { get; set; }
        /// <summary>
        /// Called when an element has been changed internally, typically by a server call
        /// Sends the type of the state and the key of the changed element
        /// </summary>
        Action<Type, short> OnStateChangedInternal { get; set; }

        Type StateType { get; }

        /// <summary>
        /// Safety check for early initialization
        /// </summary>
        bool IsEmpty { get; }

        /// <summary>
        /// The count of states in this array
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Copies local cache into internal sync list array.
        /// Local changes will not be propagated to clients until this is called.
        /// </summary>
        void Flush();

        /// <summary>
        /// Copies local cache into internal sync list array for specific key.
        /// If key is not found flush will be ignored.
        /// </summary>
        void Flush(short key);

        /// <summary>
        /// Copies local cache into internal sync list array for all provided keys.
        /// Keys that are not found will be ignored.
        /// </summary>
        void Flush(IEnumerable<short> keys);

        /// <summary>
        /// Returns the next available key in the array.
        /// </summary>
        /// <returns></returns>
        short GetNextAvailableKey();

        /// <summary>
        /// Enumerates through states
        /// </summary>
        /// <returns></returns>
        IEnumerable<object> GetStates();

        /// <summary>
        /// Returns single state object for key
        /// </summary>
        object GetState(short key);

        /// <summary>
        /// Returns true if state key is found in this state array
        /// </summary>
        bool KeyExists(short key);

        /// <summary>
        /// Adds remotely changed states to array and raises events
        /// </summary>
        /// <param name="flushedStates"></param>
        void ReceiveFlushedStates(IEnumerable<object> flushedStates);

        /// <summary>
        /// Adds remote states to array without raising any events
        /// </summary>
        /// <param name="synchronizedStates"></param>
        void ReceiveSynchronizedStates(IEnumerable<object> synchronizedStates);

        void AddState(IItemState newState);
    }
}