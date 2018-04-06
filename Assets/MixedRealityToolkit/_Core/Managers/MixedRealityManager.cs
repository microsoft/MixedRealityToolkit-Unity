// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Definitions;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces;
using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Internal.Managers
{
    /// <summary>
    /// The Mixed Reality manager is responsible for coordinating the operation of the Mixed Reality Toolkit
    /// It provides a service registry for all active managers that are used within a project as well as providing the active configuration profile for the project
    /// The Profile can be swapped out at any time to meet the needs of your project.
    /// </summary>
    public class MixedRealityManager : Utilities.Singleton<MixedRealityManager>
    {
        #region Mixed Reality Manager Profile configuration

        /// <summary>
        /// The active profile of the Mixed Reality Manager which controls which components are active and their initial configuration.
        /// *Note configuration is used on project initialization or replacement, changes to properties while it is running has no effect.
        /// </summary>
        [SerializeField]
        [Tooltip("The current active configuration for the Mixed Reality project")]
        private MixedRealityConfigurationProfile activeProfile;

        /// <summary>
        /// The public property of the Active Profile, ensuring events are raised on the change of the configuration
        /// </summary>
        public MixedRealityConfigurationProfile ActiveProfile
        {
            get { return activeProfile; }
            set { activeProfile = value; if (ProfileUpdateEvent != null) ProfileUpdateEvent.Invoke(); }
        }

        #endregion

        #region Active SDK components

        /// <summary>
        /// The Active Controllers property lists all the controllers detected by the Mixed Reality manager on startup
        /// </summary>
        [SerializeField]
        [Tooltip("The collection of currently active / detected controllers")]
        private Controller[] activeControllers;

        /// <summary>
        /// The Active Headset property maintains the Headsets/SDK detected by the Mixed Reality manager on startup
        /// </summary>
        [SerializeField]
        [Tooltip("The currently active / detected Headset or SDK")]
        private Headset activeHeadset;

        #endregion

        #region Mixed Reality Manager Events

        /// <summary>
        /// Delegate for the Initialize event
        /// </summary>
        public delegate void InitializeHandler();
        /// <summary>
        /// Initialize event called after all managers have been registered. Informs all managers to initialize (Awake)
        /// </summary>
        public event InitializeHandler InitializeEvent;

        /// <summary>
        /// Delegate for the Update event
        /// </summary>
        public delegate void UpdateHandler();
        /// <summary>
        /// The Update event delegates the MonoBehaviour update message, informing all active managers to update (which is faster as managers are not bound to a MonoBehaviour)
        /// </summary>
        public event UpdateHandler UpdateEvent;

        /// <summary>
        /// Delegate for the Destroy event
        /// </summary>
        public delegate void DestroyHandler();
        /// <summary>
        /// The Destroy event is fired before the Mixed Reality Manager is destroyed in a scene and informs all active managers to clean up prior to close. Linked to the MonoBehaviour Destroy event.
        /// </summary>
        public event DestroyHandler DestroyEvent;

        /// <summary>
        /// Delegate for the Profile Update event
        /// </summary>
        public delegate void ProfileUpdateHandler();
        /// <summary>
        /// The ProfileUpdate event fires whenever the MixedRealityManager active profile is replaced.
        /// *Note Profile options cannot be changed once the project is running.
        /// </summary>
        public event ProfileUpdateHandler ProfileUpdateEvent;

        #endregion

        #region Mixed Reality Manager Methods

        /// <summary>
        /// The InitializeInternal method is called when the Singleton Manager is initialized (MonoBehaviour Awake)
        /// Once all managers are registered and properties updated, the Mixed Reality Manager will initialize all active managers.  This ensures all managers can reference each other once started.
        /// </summary>
        protected override void InitializeInternal()
        {
            base.Awake();

            #region ActiveSDK Discovery
            //Microsoft.MixedReality.Toolkit - Active SDK Discovery
            #endregion

            #region SDK Initialization
            //Microsoft.MixedReality.Toolkit - SDK Initialization
            #endregion

            #region Managers Initialization
            //Microsoft.MixedReality.Toolkit - Managers initialization

            //If the Input system has been selected for initialization in the Active profile, enable it in the project
            if (ActiveProfile.EnableInputSystem)
            {
                //Enable Input (example initializer)
                AddManager(typeof(IMixedRealityInputSystem), new InputSystem.MixedRealityInputManager());
            }

            //If the Boundary system has been selected for initialization in the Active profile, enable it in the project
            if (ActiveProfile.EnableBoundary)
            {
                //Enable Boundary (example initializer)
                AddManager(typeof(IMixedRealityBoundarySystem), new InputSystem.MixedRealityBoundaryManager());
            }

            if (ActiveProfile.EnableControllers)
            {
                //Enable Motion Controllers
            }

            if (ActiveProfile.EnableFocus)
            {
                //Enable Focus
            }
            #endregion

            if (InitializeEvent != null) InitializeEvent.Invoke();
        }

        /// <summary>
        /// The MonoBehaviour Update event, which is then circulated to all active managers
        /// </summary>
        void Update()
        {
            if (UpdateEvent != null) UpdateEvent.Invoke();
        }

        /// <summary>
        /// The MonoBehaviour Destroy event, which is then circulated to all active managers prior to the Mixed Reality Manager being destroyed
        /// </summary>
        protected override void OnDestroy()
        {
            if (DestroyEvent != null) DestroyEvent.Invoke();

            //Final destroy - needs to be last
            base.OnDestroy();
        }

        #endregion

        #region Manager Container Management

        /// <summary>
        /// Add a new manager to the Mixed Reality Manager active Manager registry.
        /// </summary>
        /// <param name="type">The interface type for the system to be managed.  E.G. InputSystem, BoundarySystem</param>
        /// <param name="manager">The Instance of the manager class to register</param>
        public void AddManager(Type type, IMixedRealityManager manager)
        {
            if (type == null)
                throw new ArgumentNullException("type");
            if (manager == null)
                throw new ArgumentNullException("manager");

            if (GetManager(type) == null)
            {
                ActiveProfile.ActiveManagers.Add(type, manager);
            }
        }

        /// <summary>
        /// Retrieve a manager from the Mixed Reality Manager active manager registry
        /// </summary>
        /// <param name="type">The interface type for the system to be retrieved.  E.G. InputSystem, BoundarySystem</param>
        /// <returns></returns>
        public IMixedRealityManager GetManager(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            IMixedRealityManager manager;
            if (ActiveProfile.ActiveManagers.TryGetValue(type, out manager))
                return manager;

            return null;
        }

        /// <summary>
        /// Remove a manager from the Mixed Reality Manager active manager registry
        /// </summary>
        /// <param name="type">The interface type for the system to be removed.  E.G. InputSystem, BoundarySystem<</param>
        public void RemoveManager(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            ActiveProfile.ActiveManagers.Remove(type);
        }

        /// <summary>
        /// Generic function used to retrieve a manager from the Mixed Reality Manager active manager registry
        /// </summary>
        /// <typeparam name="T">The interface type for the system to be retrieved.  E.G. InputSystem, BoundarySystem.
        /// *Note type should be the Interface of the system to be retrieved and not the class itself</typeparam>
        /// <returns>The instance of the manager class that is registered with the selected Interface</returns>
        public T GetManager<T>() where T : IMixedRealityManager
        {
            var manager = GetManager(typeof(T));

            if (manager == null)
                return default(T);

            return (T)manager;
        }

        /// <summary>
        /// Generic function used to interrogate the Mixed Reality Manager active manager registry for the existence of a manager
        /// </summary>
        /// <typeparam name="T">The interface type for the system to be retrieved.  E.G. InputSystem, BoundarySystem.
        /// *Note type should be the Interface of the system to be retrieved and not the class itself</typeparam>
        /// <returns>True, there is a manager registered with the selected interface, False, no manager found for that interface</returns>
        public bool ManagerExists<T>() where T : class
        {
            var manager = GetManager(typeof(T));

            if (manager == null)
                return true;

            return false;
        }

        #endregion
    }
}
