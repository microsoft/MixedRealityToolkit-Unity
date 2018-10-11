using Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.Core;
using System;
using System.Collections.Generic;

namespace Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.StateControl
{
    /// <summary>
    /// Set of structs that represents the app's state.
    /// </summary>
    public interface IAppStateReadOnly
    {
        /// <summary>
        /// True when server considers states synchronized enough to proceed with app.
        /// </summary>
        bool Synchronized { get; }

        /// <summary>
        /// The types needed for this app to function.
        /// Will vary on an app-to-app basis.
        /// If state arrays of these types are not detected on startup an exception is thrown.
        /// </summary>
        IEnumerable<Type> RequiredStateTypes { get; }

        /// <summary>
        /// The state types available in this app state.
        /// </summary>
        IEnumerable<Type> ItemStateTypes { get; }

        /// <summary>
        /// Returns all states of type T in no particlar order.
        /// If no state array of type T exists exception is thrown.
        /// </summary>
        IEnumerable<T> GetStates<T>() where T : struct, IItemState<T>;

        /// <summary>
        /// Returns state of type T with item key.
        /// If no state array of type T exists exception is thrown.
        /// </summary>
        T GetState<T>(sbyte stateKey) where T : struct, IItemState<T>;

        /// <summary>
        /// Returns true if state array of type T has no elements.
        /// If no state array of type T exists exception is thrown.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        bool IsEmpty<T>() where T : struct, IItemState<T>;

        /// <summary>
        /// Returns number of items in state array of type T.
        /// If no state array of type T exists exception is thrown.
        /// </summary>
        int GetNumStates<T>() where T : struct, IItemState<T>;

        /// <summary>
        /// Returns true if state of type T with key stateKey exists
        /// Use sparingly
        /// </summary>
        bool StateExists<T>(sbyte stateKey);
    }
}