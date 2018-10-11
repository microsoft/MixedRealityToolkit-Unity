using System;
using System.Collections.Generic;

namespace Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.Core
{
    public interface IStateArrayBase
    {
        /// <summary>
        /// Called when an element has been changed from the outside
        /// Sends the type of the state and the key of the changed element
        /// </summary>
        Action<Type,sbyte> OnStateChangedExternal { get; set; }
        /// <summary>
        /// Called when an element has been changed internally, typically by a server call
        /// Sends the type of the state and the key of the changed element
        /// </summary>
        Action<Type,sbyte> OnStateChangedInternal { get; set; }

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
        /// Returns the next available key in the array.
        /// </summary>
        /// <returns></returns>
        sbyte GetNextAvailableKey();

        /// <summary>
        /// Enumerates through states
        /// </summary>
        /// <returns></returns>
        IEnumerable<object> GetStates();

        /// <summary>
        /// Returns single state object for key
        /// </summary>
        object GetState(sbyte key);

        /// <summary>
        /// Returns true if state key is found in this state array
        /// </summary>
        bool KeyExists(sbyte stateKey);
    }
}