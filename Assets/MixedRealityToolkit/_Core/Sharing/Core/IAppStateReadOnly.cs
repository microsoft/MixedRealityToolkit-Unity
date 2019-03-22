using MRTK.Core;
using System;
using System.Collections.Generic;

namespace MRTK.StateControl
{
    /// <summary>
    /// Set of structs that represents the app's state.
    /// </summary>
    public interface IAppStateReadOnly
    {
        /// <summary>
        /// True when app role has been set
        /// </summary>
        bool Initialized { get; }
        
        /// <summary>
        /// True when server considers states synchronized enough to proceed with app.
        /// </summary>
        bool Synchronized { get; }

        /// <summary>
        /// The state types available in this app state.
        /// </summary>
        IEnumerable<Type> ItemStateTypes { get; }

        /// <summary>
        /// Returns all states of type T in no particlar order.
        /// If no state array of type T exists exception is thrown.
        /// </summary>
        IEnumerable<T> GetStates<T>() where T : struct, IItemState, IItemStateComparer<T>;

        /// <summary>
        /// Returns all states of type in no particlar order.
        /// If no state array of type exists exception is thrown.
        /// </summary>
        IEnumerable<object> GetStates(Type type);

        /// <summary>
        /// Returns state of type T with item key.
        /// If no state array of type T exists exception is thrown.
        /// </summary>
        T GetState<T>(short key) where T : struct, IItemState, IItemStateComparer<T>;

        /// <summary>
        /// Returns true if state array of type T has no elements.
        /// If no state array of type T exists exception is thrown.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        bool IsEmpty<T>() where T : struct, IItemState, IItemStateComparer<T>;

        /// <summary>
        /// Returns number of items in state array of type T.
        /// If no state array of type T exists exception is thrown.
        /// </summary>
        int GetNumStates<T>() where T : struct, IItemState, IItemStateComparer<T>;

        /// <summary>
        /// Returns true if state of type T with key stateKey exists
        /// Use sparingly
        /// </summary>
        bool StateExists<T>(short key);
    }
}