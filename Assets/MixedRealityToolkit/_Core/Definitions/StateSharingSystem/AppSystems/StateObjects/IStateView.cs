using Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Definitions.StateSharingSystem.AppSystems.StateObjects
{
    /// <summary>
    /// Keeps track of dynamically instantiated objects generated from game states.
    /// </summary>
    public interface IStateView : ISharingAppObject, ISessionObject
    {
        /// <summary>
        /// All active GameObjects associated with state objects.
        /// (Each GameObject may have more than one IStateObject component.)
        /// </summary>
        IEnumerable<GameObject> GetActiveObjects();

        /// <summary>
        /// All active StateObjects.
        /// </summary>
        IEnumerable<IStateObjectBase> GetActiveStateObjects();

        /// <summary>
        /// Adds an existing state object to the list.
        /// This can be used for StateObjects that have been pre-placed in the scene.
        /// </summary>
        void AddStateObject(sbyte itemNum, GameObject gameObject, Type stateType);
                
        /// <summary>
        /// Retrieves a state object component of type T.
        /// </summary>
        bool GetStateObject<S, T>(sbyte itemNum, out T stateObject, bool throwExceptionIfTypeNotFound = true) where S : IItemState<S> where T : Component, IStateObject<S>;

        /// <summary>
        /// Retrieves all state object components of type T
        /// </summary>
        IEnumerable<T> GetStateObjects<S, T>() where S : IItemState<S> where T : Component, IStateObject<S>;
    }
}