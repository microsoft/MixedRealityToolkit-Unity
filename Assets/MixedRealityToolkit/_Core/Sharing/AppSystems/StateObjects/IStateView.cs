using Pixie.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Pixie.AppSystems.StateObjects
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
        /// Makes sure an object has been instantiated for all item states in appState
        /// Initialized / updates all existing IStateObjectBase objects
        /// </summary>
        void UpdateStateObjects();

        /// <summary>
        /// Adds an existing state object to the list.
        /// This can be used for StateObjects that have been pre-placed in the scene.
        /// </summary>
        void AddStateObject(short itemNum, GameObject gameObject, Type stateType);
                
        /// <summary>
        /// Retrieves a state object component of type T.
        /// </summary>
        bool GetStateObject<S, T>(short itemNum, out T stateObject, bool throwExceptionIfTypeNotFound = true) where S : IItemState, IItemStateComparer<S> where T : Component, IStateObject<S>;

        /// <summary>
        /// Retrieves all state object components of type T
        /// </summary>
        IEnumerable<T> GetStateObjects<S, T>() where S : IItemState, IItemStateComparer<S> where T : Component, IStateObject<S>;
    }
}