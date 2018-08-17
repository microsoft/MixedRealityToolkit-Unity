// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Definitions;
using Microsoft.MixedReality.Toolkit.Internal.Devices.OpenVR;
using Microsoft.MixedReality.Toolkit.Internal.Devices.UnityInput;
using Microsoft.MixedReality.Toolkit.Internal.Devices.WindowsMixedReality;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces.BoundarySystem;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces.Devices;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces.InputSystem;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces.TeleportSystem;
using System;
using System.Collections.Generic;
using System.Linq;
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

        private bool isMixedRealityManagerInitializing = false;

        /// <summary>
        /// Checks if there is a valid instance of the MixedRealityManager, then checks if there
        /// is there a valid Active Profile on this manager.
        /// </summary>
        public static bool HasActiveProfile
        {
            get
            {
                if (!IsInitialized)
                {
                    return false;
                }

                if (!ConfirmInitialized())
                {
                    return false;
                }

                return Instance.ActiveProfile != null;
            }
        }

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
            get
            {
#if UNITY_EDITOR
                if (!Application.isPlaying && activeProfile == null)
                {
                    UnityEditor.Selection.activeObject = Instance;
                    UnityEditor.EditorGUIUtility.PingObject(Instance);
                }
#endif // UNITY_EDITOR
                return activeProfile;
            }
            set
            {
                ResetConfiguration(value);
            }
        }

        /// <summary>
        /// When a configuration Profile is replaced with a new configuration, force all managers to reset and read the new values
        /// </summary>
        /// <param name="profile"></param>
        public void ResetConfiguration(MixedRealityConfigurationProfile profile)
        {
            if (activeProfile != null)
            {
                DisableAllManagers();
                DestroyAllManagers();
            }

            activeProfile = profile;

            if (profile != null)
            {
                DisableAllManagers();
                DestroyAllManagers();
            }

            Initialize();
        }

        #endregion Mixed Reality Manager Profile configuration

        #region Mixed Reality runtime component registry

        /// <summary>
        /// Local component registry for the Mixed Reality Manager, to allow runtime use of the Manager.
        /// </summary>
        public List<Tuple<Type, IMixedRealityManager>> MixedRealityComponents { get; } = new List<Tuple<Type, IMixedRealityManager>>();

        private int mixedRealityComponentsCount = 0;

        #endregion Mixed Reality runtime component registry

        /// <summary>
        /// Function called when the instance is assigned.
        /// Once all managers are registered and properties updated, the Mixed Reality Manager will initialize all active managers.
        /// This ensures all managers can reference each other once started.
        /// </summary>
        private void Initialize()
        {
            isMixedRealityManagerInitializing = true;

            //If the Mixed Reality Manager is not configured, stop.
            if (ActiveProfile == null)
            {
                Debug.LogError("No Mixed Reality Configuration Profile found, cannot initialize the Mixed Reality Manager");
                return;
            }

#if UNITY_EDITOR
            if (ActiveProfile.ActiveManagers.Count > 0)
            {
                if (!Application.isPlaying)
                {
                    DisableAllManagers();
                    DestroyAllManagers();
                }
                else
                {
                    ActiveProfile.ActiveManagers.Clear();
                }
            }
#endif

            if (ActiveProfile.IsCameraProfileEnabled)
            {
                if (MixedRealityCameraProfile.IsOpaque)
                {
                    ActiveProfile.CameraProfile.ApplySettingsForOpaqueDisplay();
                }
                else
                {
                    ActiveProfile.CameraProfile.ApplySettingsForTransparentDisplay();
                }
            }

            #region  Managers Registration

            // If the Input system has been selected for initialization in the Active profile, enable it in the project
            if (ActiveProfile.IsInputSystemEnabled)
            {
#if UNITY_EDITOR
                // Make sure unity axis mappings are set.
                Utilities.Editor.InputMappingAxisUtility.CheckUnityInputManagerMappings(Definitions.Devices.ControllerMappingLibrary.UnityInputManagerAxes);
#endif

                //Enable Input (example initializer)
                AddManager(typeof(IMixedRealityInputSystem), Activator.CreateInstance(ActiveProfile.InputSystemType) as IMixedRealityInputSystem);
            }

            // If the Boundary system has been selected for initialization in the Active profile, enable it in the project
            if (ActiveProfile.IsBoundarySystemEnabled)
            {
                //Enable Boundary (example initializer)
                AddManager(typeof(IMixedRealityBoundarySystem), Activator.CreateInstance(ActiveProfile.BoundarySystemSystemType) as IMixedRealityBoundarySystem);
            }

            // If the Teleport system has been selected for initialization in the Active profile, enable it in the project
            if (ActiveProfile.IsTeleportSystemEnabled)
            {
                AddManager(typeof(IMixedRealityTeleportSystem), Activator.CreateInstance(ActiveProfile.TeleportSystemSystemType) as IMixedRealityTeleportSystem);
            }

            #region ActiveSDK Discovery

            // TODO Microsoft.MixedReality.Toolkit - Active SDK Discovery

            #endregion ActiveSDK Discovery

            #endregion Managers Registration

            #region SDK Initialization

#if UNITY_EDITOR
            AddManagersForTheCurrentPlatformEditor();
#else
            AddManagersForTheCurrentPlatform();
#endif

            #endregion SDK Initialization

            #region Managers Initialization

            //TODO should this be optional?
            //Sort the managers based on Priority
            var orderedManagers = ActiveProfile.ActiveManagers.OrderBy(m => m.Value.Priority).ToArray();
            ActiveProfile.ActiveManagers.Clear();

            foreach (var manager in orderedManagers)
            {
                AddManager(manager.Key, manager.Value);
            }

            InitializeAllManagers();

            #endregion Managers Initialization

            isMixedRealityManagerInitializing = false;
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

                if (Application.isPlaying && !searchForInstance)
                {
                    return null;
                }

                MixedRealityManager[] objects = FindObjectsOfType<MixedRealityManager>();
                searchForInstance = false;

                switch (objects.Length)
                {
                    case 0:
                        instance = new GameObject(nameof(MixedRealityManager)).AddComponent<MixedRealityManager>();
                        break;
                    case 1:
                        instance = objects[0];
                        break;
                    default:
                        Debug.LogError($"Expected exactly 1 {nameof(MixedRealityManager)} but found {objects.Length}.");
                        return null;
                }

                instance.InitializeInternal();
                return instance;
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

                if (Application.isPlaying)
                {
                    DontDestroyOnLoad(instance.transform.root);
                }

                Application.quitting += ApplicationOnQuitting;

#if UNITY_EDITOR
                UnityEditor.EditorApplication.playModeStateChanged += playModeState =>
                {
                    if (playModeState == UnityEditor.PlayModeStateChange.ExitingEditMode && activeProfile == null)
                    {
                        UnityEditor.EditorApplication.isPlaying = false;
                        UnityEditor.Selection.activeObject = Instance;
                        UnityEditor.EditorGUIUtility.PingObject(Instance);
                    }
                };
#endif // UNITY_EDITOR

                Initialize();
            }
        }

        private void ApplicationOnQuitting()
        {
            DisableAllManagers();
            DestroyAllManagers();
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

                Debug.LogWarning("Trying to instantiate a second instance of the Mixed Reality Manager. Additional Instance was destroyed");
            }
            else if (!IsInitialized)
            {
                InitializeInternal();
                searchForInstance = false;
            }
        }

        /// <summary>
        /// The MonoBehaviour OnEnable event, which is then circulated to all active managers
        /// </summary>
        private void OnEnable()
        {
            EnableAllManagers();
        }

        /// <summary>
        /// The MonoBehaviour Update event, which is then circulated to all active managers
        /// </summary>
        private void Update()
        {
            UpdateAllManagers();
        }

        /// <summary>
        /// The MonoBehaviour OnDisable event, which is then circulated to all active managers
        /// </summary>
        private void OnDisable()
        {
            DisableAllManagers();
        }

        /// <summary>
        /// The MonoBehaviour Destroy event, which is then circulated to all active managers prior to the Mixed Reality Manager being destroyed
        /// </summary>
        private void OnDestroy()
        {
            DestroyAllManagers();

            if (instance == this)
            {
                instance = null;
                searchForInstance = true;
            }
        }

        #endregion MonoBehaviour Implementation

        #region Manager Container Management

        #region Individual Manager Management

        /// <summary>
        /// Add a new manager to the Mixed Reality Manager active Manager registry.
        /// </summary>
        /// <param name="type">The interface type for the system to be managed.  E.G. InputSystem, BoundarySystem</param>
        /// <param name="manager">The Instance of the manager class to register</param>
        public void AddManager(Type type, IMixedRealityManager manager)
        {
            if (ActiveProfile == null)
            {
                Debug.LogError($"Unable to add a new {type.Name} Manager as the Mixed Reality manager has to Active Profile");
            }

            if (type == null) { throw new ArgumentNullException(nameof(type)); }
            if (manager == null) { throw new ArgumentNullException(nameof(manager)); }

            if (IsCoreManagerType(type))
            {
                IMixedRealityManager preexistingManager;
                if (IsCoreManagerType(type))
                {
                    ActiveProfile.ActiveManagers.TryGetValue(type, out preexistingManager);
                }
                else
                {
                    GetComponentByType(type, out preexistingManager);
                }

                if (preexistingManager == null)
                {
                    ActiveProfile.ActiveManagers.Add(type, manager);
                }
                else
                {
                    Debug.LogError($"There's already a {type.Name} registered.");
                }
            }
            else
            {
                MixedRealityComponents.Add(new Tuple<Type, IMixedRealityManager>(type, manager));
                if (!isMixedRealityManagerInitializing) { manager.Initialize(); }
                mixedRealityComponentsCount = MixedRealityComponents.Count;
            }
        }

        /// <summary>
        /// Generic function used to retrieve a manager from the Mixed Reality Manager active manager registry
        /// </summary>
        /// <typeparam name="T">The interface type for the system to be retrieved.  E.G. InputSystem, BoundarySystem.
        /// *Note type should be the Interface of the system to be retrieved and not the class itself</typeparam>
        /// <returns>The instance of the manager class that is registered with the selected Interface</returns>
        public T GetManager<T>() where T : IMixedRealityManager
        {
            return (T)GetManager(typeof(T));
        }

        /// <summary>
        /// Retrieve a manager from the Mixed Reality Manager active manager registry
        /// </summary>
        /// <param name="type">The interface type for the system to be retrieved.  E.G. InputSystem, BoundarySystem</param>
        /// <returns>The Mixed Reality manager of the specified type</returns>
        public IMixedRealityManager GetManager(Type type)
        {
            if (ActiveProfile == null)
            {
                throw new ArgumentNullException($"Unable to get {nameof(type)} Manager as the Mixed Reality Manager has no Active Profile.");
            }

            if (type == null) { throw new ArgumentNullException(nameof(type)); }

            IMixedRealityManager manager;
            if (IsCoreManagerType(type))
            {
                ActiveProfile.ActiveManagers.TryGetValue(type, out manager);
            }
            else
            {
                GetComponentByType(type, out manager);
            }

            if (manager == null)
            {
                throw new NullReferenceException($"Unable to find {type.Name}.");
            }

            return manager;
        }

        /// <summary>
        /// Retrieve a manager from the Mixed Reality Manager active manager registry
        /// </summary>
        /// <param name="type">The interface type for the system to be retrieved.  E.G. InputSystem, BoundarySystem</param>
        /// <param name="managerName">Name of the specific manager</param>
        /// <returns>The Mixed Reality manager of the specified type</returns>
        public IMixedRealityManager GetManager(Type type, string managerName)
        {
            if (ActiveProfile == null)
            {
                throw new ArgumentNullException($"Unable to get {managerName} Manager as the Mixed Reality Manager has no Active Profile");
            }

            if (type == null) { throw new ArgumentNullException(nameof(type)); }
            if (string.IsNullOrEmpty(managerName)) { throw new ArgumentNullException(nameof(managerName)); }

            IMixedRealityManager manager;
            if (IsCoreManagerType(type))
            {
                ActiveProfile.ActiveManagers.TryGetValue(type, out manager);
            }
            else
            {
                GetComponentByTypeAndName(type, managerName, out manager);
            }

            if (manager == null)
            {
                throw new NullReferenceException($"Unable to find {managerName} Manager.");
            }

            return manager;
        }

        /// <summary>
        /// Remove all managers from the Mixed Reality Manager active manager registry for a given type
        /// </summary>
        /// <param name="type">The interface type for the system to be removed.  E.G. InputSystem, BoundarySystem</param>
        public void RemoveManager(Type type)
        {
            if (ActiveProfile == null)
            {
                throw new ArgumentNullException($"Unable to remove {nameof(type)} Manager as the Mixed Reality Manager has no Active Profile");
            }

            if (type == null) { throw new ArgumentNullException(nameof(type)); }

            if (IsCoreManagerType(type))
            {
                ActiveProfile.ActiveManagers.Remove(type);
            }
            else
            {
                MixedRealityComponents.RemoveAll(tuple => tuple.Item1.Name == type.Name);
            }
        }

        /// <summary>
        /// Remove managers from the Mixed Reality Manager active manager registry for a given type and name
        /// Name is only supported for Mixed Reality runtime components
        /// </summary>
        /// <param name="type">The interface type for the system to be removed.  E.G. InputSystem, BoundarySystem</param>
        /// <param name="managerName">The name of the manager to be removed. (Only for runtime components) </param>
        public void RemoveManager(Type type, string managerName)
        {
            if (ActiveProfile == null)
            {
                throw new ArgumentNullException($"Unable to remove {nameof(type)} Manager as the Mixed Reality Manager has no Active Profile");
            }

            if (type == null) { throw new ArgumentNullException(nameof(type)); }
            if (string.IsNullOrEmpty(managerName)) { throw new ArgumentNullException(nameof(managerName)); }

            if (IsCoreManagerType(type))
            {
                ActiveProfile.ActiveManagers.Remove(type);
            }
            else
            {
                MixedRealityComponents.RemoveAll(tuple => tuple.Item1.Name == type.Name && tuple.Item2.Name == managerName);
            }
        }

        /// <summary>
        /// Disable all managers in the Mixed Reality Manager active manager registry for a given type
        /// </summary>
        /// <param name="type">The interface type for the system to be removed.  E.G. InputSystem, BoundarySystem</param>
        public void DisableManager(Type type)
        {
            if (type == null) { throw new ArgumentNullException(nameof(type)); }

            if (IsCoreManagerType(type))
            {
                GetManager(type).Disable();
            }
            else
            {
                foreach (var manager in GetManagers(type))
                {
                    manager.Disable();
                }
            }
        }

        /// <summary>
        /// Disable a specific manager from the Mixed Reality Manager active manager registry
        /// </summary>
        /// <param name="type">The interface type for the system to be removed.  E.G. InputSystem, BoundarySystem</param>
        /// <param name="managerName">Name of the specific manager</param>
        public void DisableManager(Type type, string managerName)
        {
            if (type == null) { throw new ArgumentNullException(nameof(type)); }
            if (string.IsNullOrEmpty(managerName)) { throw new ArgumentNullException(nameof(managerName)); }

            if (IsCoreManagerType(type))
            {
                GetManager(type).Disable();
            }
            else
            {
                foreach (var manager in GetManagers(type, managerName))
                {
                    manager.Disable();
                }
            }
        }

        /// <summary>
        /// Enable all managers in the Mixed Reality Manager active manager registry for a given type
        /// </summary>
        /// <param name="type">The interface type for the system to be removed.  E.G. InputSystem, BoundarySystem</param>
        public void EnableManager(Type type)
        {
            if (type == null) { throw new ArgumentNullException(nameof(type)); }

            if (IsCoreManagerType(type))
            {
                GetManager(type).Enable();
            }
            else
            {
                foreach (var manager in GetManagers(type))
                {
                    manager.Enable();
                }
            }
        }

        /// <summary>
        /// Enable a specific manager from the Mixed Reality Manager active manager registry
        /// </summary>
        /// <param name="type">The interface type for the system to be removed.  E.G. InputSystem, BoundarySystem</param>
        /// <param name="managerName">Name of the specific manager</param>
        public void EnableManager(Type type, string managerName)
        {
            if (type == null) { throw new ArgumentNullException(nameof(type)); }
            if (string.IsNullOrEmpty(managerName)) { throw new ArgumentNullException(nameof(managerName)); }

            if (IsCoreManagerType(type))
            {
                GetManager(type).Enable();
            }
            else
            {
                foreach (var manager in GetManagers(type, managerName))
                {
                    manager.Enable();
                }
            }
        }

        #endregion Individual Manager Management

        #region Multiple Managers Management

        /// <summary>
        /// Retrieve all managers from the Mixed Reality Manager active manager registry for a given type and an optional name
        /// </summary>
        /// <param name="type">The interface type for the system to be retrieved.  E.G. InputSystem, BoundarySystem</param>
        /// <returns>An array of Managers that meet the search criteria</returns>
        public IEnumerable<IMixedRealityManager> GetManagers(Type type)
        {
            if (type == null) { throw new ArgumentNullException(nameof(type)); }

            return GetManagers(type, string.Empty);
        }

        /// <summary>
        /// Retrieve all managers from the Mixed Reality Manager active manager registry for a given type and an optional name
        /// </summary>
        /// <param name="type">The interface type for the system to be retrieved.  E.G. InputSystem, BoundarySystem</param>
        /// <param name="managerName">Name of the specific manager</param>
        /// <returns>An array of Managers that meet the search criteria</returns>
        public List<IMixedRealityManager> GetManagers(Type type, string managerName)
        {
            if (ActiveProfile == null)
            {
                throw new ArgumentNullException($"Unable to get {nameof(type)} Manager as the Mixed Reality Manager has no Active Profile");
            }

            if (type == null) { throw new ArgumentNullException(nameof(type)); }

            var managers = new List<IMixedRealityManager>();

            if (IsCoreManagerType(type))
            {
                foreach (var manager in ActiveProfile.ActiveManagers)
                {
                    if (manager.Key.Name == type.Name)
                    {
                        managers.Add(manager.Value);
                    }
                }
            }
            else
            {
                //If no name provided, return all components of the same type. Else return the type/name combination.
                if (string.IsNullOrWhiteSpace(managerName))
                {
                    for (int i = 0; i < mixedRealityComponentsCount; i++)
                    {
                        if (MixedRealityComponents[i].Item1.Name == type.Name)
                        {
                            managers.Add(MixedRealityComponents[i].Item2);
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < mixedRealityComponentsCount; i++)
                    {
                        if (MixedRealityComponents[i].Item1.Name == type.Name && MixedRealityComponents[i].Item2.Name == managerName)
                        {
                            managers.Add(MixedRealityComponents[i].Item2);
                        }
                    }
                }
            }

            return managers;
        }

        private void InitializeAllManagers()
        {
            //If the Mixed Reality Manager is not configured, stop.
            if (activeProfile == null) { return; }

            //Initialize all managers
            foreach (var manager in activeProfile.ActiveManagers)
            {
                manager.Value.Initialize();
            }

            // Enable all registered runtime components
            foreach (var manager in MixedRealityComponents)
            {
                manager.Item2.Initialize();
            }
        }

        private void ResetAllManagers()
        {
            //If the Mixed Reality Manager is not configured, stop.
            if (activeProfile == null) { return; }

            // Reset all active managers in the registry
            foreach (var manager in activeProfile.ActiveManagers)
            {
                manager.Value.Reset();
            }

            // Reset all registered runtime components
            foreach (var manager in MixedRealityComponents)
            {
                manager.Item2.Reset();
            }
        }

        private void EnableAllManagers()
        {
            //If the Mixed Reality Manager is not configured, stop.
            if (activeProfile == null) { return; }

            // Enable all active managers in the registry
            foreach (var manager in activeProfile.ActiveManagers)
            {
                manager.Value.Enable();
            }

            // Enable all registered runtime components
            foreach (var manager in MixedRealityComponents)
            {
                manager.Item2.Enable();
            }
        }

        private void UpdateAllManagers()
        {
            //If the Mixed Reality Manager is not configured, stop.
            if (activeProfile == null) { return; }

            // Update manager registry
            foreach (var manager in activeProfile.ActiveManagers)
            {
                manager.Value.Update();
            }

            //Update runtime component registry
            foreach (var manager in MixedRealityComponents)
            {
                manager.Item2.Update();
            }
        }

        private void DisableAllManagers()
        {
            //If the Mixed Reality Manager is not configured, stop.
            if (activeProfile == null) { return; }

            // Disable all active managers in the registry
            foreach (var manager in activeProfile.ActiveManagers)
            {
                manager.Value.Disable();
            }

            // Disable all registered runtime components
            foreach (var manager in MixedRealityComponents)
            {
                manager.Item2.Disable();
            }
        }

        private void DestroyAllManagers()
        {
            //If the Mixed Reality Manager is not configured, stop.
            if (activeProfile == null) { return; }

            // Destroy all active managers in the registry
            foreach (var manager in activeProfile.ActiveManagers)
            {
                manager.Value.Destroy();
            }

            activeProfile.ActiveManagers.Clear();

            // Destroy all registered runtime components
            foreach (var manager in MixedRealityComponents)
            {
                manager.Item2.Destroy();
            }

            MixedRealityComponents.Clear();
        }

        #endregion Multiple Managers Management

        #region Manager Utilities

        /// <summary>
        /// Generic function used to interrogate the Mixed Reality Manager active manager registry for the existence of a manager
        /// </summary>
        /// <typeparam name="T">The interface type for the system to be retrieved.  E.G. InputSystem, BoundarySystem.
        /// *Note type should be the Interface of the system to be retrieved and not the class itself</typeparam>
        /// <returns>True, there is a manager registered with the selected interface, False, no manager found for that interface</returns>
        public bool ManagerExists<T>() where T : class
        {
            IMixedRealityManager manager;
            ActiveProfile.ActiveManagers.TryGetValue(typeof(T), out manager);
            return manager != null;
        }

        private bool IsCoreManagerType(Type type)
        {
            if (type == null) { throw new ArgumentNullException(nameof(type)); }

            return type == typeof(IMixedRealityInputSystem) ||
                   type == typeof(IMixedRealityBoundarySystem);
        }

        /// <summary>
        /// Retrieve the first component from the registry that meets the selected type
        /// </summary>
        /// <param name="type">Interface type of the component being requested</param>
        /// <param name="manager">return parameter of the function</param>
        private void GetComponentByType(Type type, out IMixedRealityManager manager)
        {
            if (type == null) { throw new ArgumentNullException(nameof(type)); }

            manager = null;

            for (int i = 0; i < mixedRealityComponentsCount; i++)
            {
                if (MixedRealityComponents[i].Item1.Name == type.Name || MixedRealityComponents[i].Item2.GetType().Name == type.Name)
                {
                    manager = MixedRealityComponents[i].Item2;
                    break;
                }
            }

            if (manager == null)
            {
                throw new NullReferenceException($"Unable to find {type.Name} Manager.");
            }
        }

        /// <summary>
        /// Retrieve the first component from the registry that meets the selected type and name
        /// </summary>
        /// <param name="type">Interface type of the component being requested</param>
        /// <param name="managerName">Name of the specific manager</param>
        /// <param name="manager">return parameter of the function</param>
        private void GetComponentByTypeAndName(Type type, string managerName, out IMixedRealityManager manager)
        {
            if (type == null) { throw new ArgumentNullException(nameof(type)); }
            if (string.IsNullOrEmpty(managerName)) { throw new ArgumentNullException(nameof(managerName)); }

            manager = null;

            for (int i = 0; i < mixedRealityComponentsCount; i++)
            {
                if (MixedRealityComponents[i].Item1.Name == type.Name && MixedRealityComponents[i].Item2.Name == managerName)
                {
                    manager = MixedRealityComponents[i].Item2;
                    break;
                }
            }
        }

        #endregion Manager Utilities

        #endregion Manager Container Management

        #region Platform Selectors

        private void AddManagersForTheCurrentPlatform()
        {
            switch (Application.platform)
            {
                case RuntimePlatform.WindowsPlayer:
                case RuntimePlatform.WindowsEditor:
                case RuntimePlatform.OSXPlayer:
                case RuntimePlatform.OSXEditor:
                    AddManager(typeof(IMixedRealityDeviceManager), new UnityDeviceManager("Unity Device Manager", 10));
                    AddManager(typeof(IMixedRealityDeviceManager), new OpenVRDeviceManager("Unity OpenVR Device Manager", 10));
                    break;
                case RuntimePlatform.IPhonePlayer:
                    break;
                case RuntimePlatform.Android:
                    break;
                case RuntimePlatform.WebGLPlayer:
                    break;
                case RuntimePlatform.WSAPlayerX86:
                case RuntimePlatform.WSAPlayerX64:
                case RuntimePlatform.WSAPlayerARM:
                    AddManager(typeof(IMixedRealityDeviceManager), new WindowsMixedRealityDeviceManager("Mixed Reality Device Manager", 10));
                    break;
            }
        }

#if UNITY_EDITOR

        private void AddManagersForTheCurrentPlatformEditor()
        {
            switch (UnityEditor.EditorUserBuildSettings.activeBuildTarget)
            {
                case UnityEditor.BuildTarget.StandaloneWindows:
                case UnityEditor.BuildTarget.StandaloneWindows64:
                case UnityEditor.BuildTarget.StandaloneOSX:
                    AddManager(typeof(IMixedRealityDeviceManager), new UnityDeviceManager("Unity Device Manager", 10));
                    AddManager(typeof(IMixedRealityDeviceManager), new OpenVRDeviceManager("Unity OpenVR Device Manager", 10));
                    break;
                case UnityEditor.BuildTarget.iOS:
                    break;
                case UnityEditor.BuildTarget.Android:
                    break;
                case UnityEditor.BuildTarget.WebGL:
                    break;
                case UnityEditor.BuildTarget.WSAPlayer:
                    AddManager(typeof(IMixedRealityDeviceManager), new WindowsMixedRealityDeviceManager("Mixed Reality Device Manager", 10));
                    break;
            }
        }

#endif

        #endregion Platform Selectors
    }
}