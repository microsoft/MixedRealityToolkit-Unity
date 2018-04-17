// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Definitions;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces;
using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Internal.Managers
{
    /// <summary>
    /// The Mixed Reality manager is responsible for coordinating the operation of the Mixed Reality Toolkit.
    /// It provides a service registry for all active managers that are used within a project as well as providing the active configuration profile for the project.
    /// The Profile can be swapped out at any time to meet the needs of your project.
    /// </summary>
    public class MixedRealityManager : MonoBehaviour
    {
        #region Mixed Reality Manager Profile configuration

        /// <summary>
        /// The active profile of the Mixed Reality Manager which controls which components are active and their initial configuration.
        /// *Note configuration is used on project initialization or replacement, changes to properties while it is running has no effect.
        /// </summary>
        [SerializeField]
        [Tooltip("The current active configuration for the Mixed Reality project")]
        private MixedRealityConfigurationProfile activeProfile = null;

        /// <summary>
        /// The public property of the Active Profile, ensuring events are raised on the change of the configuration
        /// </summary>
        public MixedRealityConfigurationProfile ActiveProfile
        {
            get { return activeProfile; }
            set { activeProfile = value; ResetConfiguration(); }
        }

        private void ResetConfiguration()
        {
            foreach (var manager in ActiveProfile.ActiveManagers)
            {
                manager.Value.Reset();
            }
        }

        #endregion Mixed Reality Manager Profile configuration

        #region Active SDK components

        /// <summary>
        /// The Active Controllers property lists all the controllers detected by the Mixed Reality manager on startup
        /// </summary>
        //[SerializeField]
        //[Tooltip("The collection of currently active / detected controllers")]
        //private Controller[] activeControllers = null;

        ///// <summary>
        ///// The Active Headset property maintains the Headsets/SDK detected by the Mixed Reality manager on startup
        ///// </summary>
        //[SerializeField]
        //[Tooltip("The currently active / detected Headset or SDK")]
        //private Headset activeHeadset = default(Headset);

        #endregion Active SDK components

        /// <summary>
        /// Function called when the instance is assigned.
        /// Once all managers are registered and properties updated, the Mixed Reality Manager will initialize all active managers.
        /// This ensures all managers can reference each other once started.
        /// </summary>
        private void Initialize()
        {
            #region ActiveSDK Discovery
            // TODO Microsoft.MixedReality.Toolkit - Active SDK Discovery
            #endregion ActiveSDK Discovery

            #region SDK Initialization
            // TODO Microsoft.MixedReality.Toolkit - SDK Initialization
            #endregion SDK Initialization

            #region Managers Initialization

            //If the Input system has been selected for initialization in the Active profile, enable it in the project
            if (ActiveProfile.EnableInputSystem)
            {
                //Enable Input (example initializer)
                AddManager(typeof(IMixedRealityInputSystem), new InputSystem.MixedRealityInputManager());
            }

            //If the Boundary system has been selected for initialization in the Active profile, enable it in the project
            if (ActiveProfile.EnableBoundarySystem)
            {
                //Enable Boundary (example initializer)
                AddManager(typeof(IMixedRealityBoundarySystem), new InputSystem.MixedRealityBoundaryManager());
            }
            foreach (var manager in ActiveProfile.ActiveManagers)
            {
                manager.Value.Initialize();
            }

            #endregion Managers Initialization
        }

        #region MonoBehaviour Implementation

        private static MixedRealityManager instance;

        /// <summary>
        /// Returns the Singleton instance of the classes type.
        /// If no instance is found, then we search for an instance in the scene.
        /// If more than one instance is found, we throw an error and no instance is returned.
        /// </summary>
        public static MixedRealityManager Instance
        {
            get
            {
                if (IsInitialized)
                {
                    return instance;
                }

                if (!searchForInstance)
                {
                    return null;
                }

                MixedRealityManager[] objects = FindObjectsOfType<MixedRealityManager>();
                searchForInstance = false;

                if (objects.Length == 1)
                {
                    objects[0].InitializeInternal();
                    return instance;
                }

                Debug.LogError($"Expected exactly 1 MixedRealityManager but found {objects.Length}.");
                return null;
            }
        }

        /// <summary>
        /// Flag to search for instance the first time Instance property is called.
        /// Subsequent attempts will generally switch this flag false, unless the instance was destroyed.
        /// </summary>
        // ReSharper disable once StaticMemberInGenericType
        private static bool searchForInstance = true;

        /// <summary>
        /// Expose an assertion whether the MixedRealityManager class is initialized.
        /// </summary>
        public static void AssertIsInitialized()
        {
             Debug.Assert(IsInitialized, "The MixedRealityManager has not been initialized.");
        }

        /// <summary>
        /// Returns whether the instance has been initialized or not.
        /// </summary>
        public static bool IsInitialized => instance != null;

        /// <summary>
        /// Static function to determine if the MixedRealityManager class has been initialized or not.
        /// </summary>
        /// <returns></returns>
        public static bool ConfirmInitialized()
        {
            // ReSharper disable once UnusedVariable
            // Assigning the Instance to access is used Implicitly.
            MixedRealityManager access = Instance;
            return IsInitialized;
        }

        /// <summary>
        /// Lock property for the Mixed Reality Manager to prevent reinitialization
        /// </summary>
        private readonly object initializedLock = new object();

        private void InitializeInternal()
        {
            lock (initializedLock)
            {
                if (IsInitialized) { return; }

                instance = this;
                DontDestroyOnLoad(instance.transform.root);
                Initialize();
            }
        }

        /// <summary>
        /// Base Awake method that sets the Singleton's unique instance.
        /// Called by Unity when initializing a MonoBehaviour.
        /// Scripts that extend Singleton should be sure to call base.Awake() unless they want
        /// lazy initialization
        /// </summary>
        private void Awake()
        {
            if (IsInitialized && instance != this)
            {
                if (Application.isEditor)
                {
                    DestroyImmediate(this);
                }
                else
                {
                    Destroy(this);
                }

                Debug.LogWarning($"Trying to instantiate a second instance of the Mixed Reality Manager. Additional Instance was destroyed");
            }
            else if (!IsInitialized)
            {
                InitializeInternal();
                searchForInstance = false;
            }
        }

        /// <summary>
        /// The MonoBehaviour Update event, which is then circulated to all active managers
        /// </summary>
        private void Update()
        {
            foreach (var manager in ActiveProfile.ActiveManagers)
            {
                manager.Value.Update();
            }
        }

        /// <summary>
        /// The MonoBehaviour Destroy event, which is then circulated to all active managers prior to the Mixed Reality Manager being destroyed
        /// </summary>
        private void OnDestroy()
        {
            foreach (var manager in ActiveProfile.ActiveManagers)
            {
                manager.Value.Destroy();
            }

            if (instance == this)
            {
                instance = null;
                searchForInstance = true;
            }
        }

        #endregion MonoBehaviour Implementation

        #region Manager Container Management

        /// <summary>
        /// Add a new manager to the Mixed Reality Manager active Manager registry.
        /// </summary>
        /// <param name="type">The interface type for the system to be managed.  E.G. InputSystem, BoundarySystem</param>
        /// <param name="manager">The Instance of the manager class to register</param>
        public void AddManager(Type type, IMixedRealityManager manager)
        {
            if (type == null) { throw new ArgumentNullException(nameof(type)); }
            if (manager == null) { throw new ArgumentNullException(nameof(manager)); }

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
            if (type == null) { throw new ArgumentNullException(nameof(type)); }

            IMixedRealityManager manager;
            return ActiveProfile.ActiveManagers.TryGetValue(type, out manager) ? manager : null;
        }

        /// <summary>
        /// Remove a manager from the Mixed Reality Manager active manager registry
        /// </summary>
        /// <param name="type">The interface type for the system to be removed.  E.G. InputSystem, BoundarySystem<</param>
        public void RemoveManager(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

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
            {
                return default(T);
            }

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
            return GetManager(typeof(T)) == null;
        }

        #endregion Manager Container Management
    }
}
