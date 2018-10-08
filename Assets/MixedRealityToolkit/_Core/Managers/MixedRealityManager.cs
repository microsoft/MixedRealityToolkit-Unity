// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Definitions;
using Microsoft.MixedReality.Toolkit.Core.Extensions;
using Microsoft.MixedReality.Toolkit.Core.Interfaces;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.BoundarySystem;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.Diagnostics;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.InputSystem;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.TeleportSystem;
using Microsoft.MixedReality.Toolkit.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Managers
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
                    mixedRealityComponentsCount = 0;
                    MixedRealityComponents.Clear();
                    ActiveProfile.ActiveManagers.Clear();
                }
            }
#endif
            EnsureMixedRealityRequirements();

            if (ActiveProfile.IsCameraProfileEnabled)
            {
                if (ActiveProfile.CameraProfile.IsCameraPersistent)
                {
                    CameraCache.Main.transform.root.DontDestroyOnLoad();
                }

                if (ActiveProfile.CameraProfile.IsOpaque)
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

                AddManager(typeof(IMixedRealityInputSystem), Activator.CreateInstance(ActiveProfile.InputSystemType) as IMixedRealityInputSystem);

                if (InputSystem == null)
                {
                    Debug.LogError("Failed to start the Input System!");
                }
            }

            // If the Boundary system has been selected for initialization in the Active profile, enable it in the project
            if (ActiveProfile.IsBoundarySystemEnabled)
            {
                AddManager(typeof(IMixedRealityBoundarySystem), Activator.CreateInstance(ActiveProfile.BoundarySystemSystemType) as IMixedRealityBoundarySystem);

                if (BoundarySystem == null)
                {
                    Debug.LogError("Failed to start the Boundary System!");
                }
            }


            // If the Teleport system has been selected for initialization in the Active profile, enable it in the project
            if (ActiveProfile.IsTeleportSystemEnabled)
            {
                AddManager(typeof(IMixedRealityTeleportSystem), Activator.CreateInstance(ActiveProfile.TeleportSystemSystemType) as IMixedRealityTeleportSystem);

                if (TeleportSystem == null)
                {
                    Debug.LogError("Failed to start the Teleport System!");
                }
            }

            if (ActiveProfile.IsDiagnosticsSystemEnabled)
            {
                AddManager(typeof(IMixedRealityDiagnosticsSystem), Activator.CreateInstance(ActiveProfile.DiagnosticsSystemSystemType) as IMixedRealityDiagnosticsSystem);

                if (DiagnosticsSystem == null)
                {
                    Debug.LogError("Failed to start the Diagnostics System!");
                }
            }

            if (ActiveProfile.RegisteredComponentsProfile != null)
            {
                for (int i = 0; i < ActiveProfile.RegisteredComponentsProfile.Configurations?.Length; i++)
                {
                    var configuration = ActiveProfile.RegisteredComponentsProfile.Configurations[i];
#if UNITY_EDITOR
                    if (UnityEditor.EditorUserBuildSettings.activeBuildTarget.IsPlatformSupported(configuration.RuntimePlatform))
#else
                    if (Application.platform.IsPlatformSupported(configuration.RuntimePlatform))
#endif
                    {
                        if (configuration.ComponentType.Type != null)
                        {
                            AddManager(typeof(IMixedRealityComponent), Activator.CreateInstance(configuration.ComponentType, configuration.ComponentName, configuration.Priority) as IMixedRealityComponent);
                        }
                    }
                }
            }

            #endregion Manager Registration

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

        private void EnsureMixedRealityRequirements()
        {
            // There's lots of documented cases that if the camera doesn't start at 0,0,0, things break with the WMR SDK specifically.
            // We'll enforce that here, then tracking can update it to the appropriate position later.
           CameraCache.Main.transform.position = Vector3.zero;
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

        private Transform mixedRealityPlayspace;

        /// <summary>
        /// Returns the MixedRealityPlayspace for the local player
        /// </summary>
        public Transform MixedRealityPlayspace
        {
            get {
                AssertIsInitialized();
                if (mixedRealityPlayspace)
                {
                    return mixedRealityPlayspace;
                }
                else
                {
                    string MixedRealityPlayspaceName = "MixedRealityPlayspace";
                    if (CameraCache.Main.transform.parent == null)
                    {
                        mixedRealityPlayspace = new GameObject(MixedRealityPlayspaceName).transform;
                        CameraCache.Main.transform.SetParent(mixedRealityPlayspace);
                    }
                    else
                    {
                        if (CameraCache.Main.transform.parent.name != MixedRealityPlayspaceName)
                        {
                            // Since the scene is set up with a different camera parent, its likely
                            // that there's an expectation that that parent is going to be used for
                            // something else. We print a warning to call out the fact that we're 
                            // co-opting this object for use with teleporting and such, since that
                            // might cause conflicts with the parent's intended purpose.
                            Debug.LogWarning("The Mixed Reality Manager expected the camera's parent to be named " + MixedRealityPlayspaceName + ". The existing parent will be renamed and used instead.");
                            CameraCache.Main.transform.parent.name = MixedRealityPlayspaceName; // If we rename it, we make it clearer that why it's being teleported around at runtime.
                        }
                        mixedRealityPlayspace = CameraCache.Main.transform.parent;
                    }

                    // It's very important that the MixedRealityPlayspace align with the tracked space,
                    // otherwise reality-locked things like playspace boundaries won't be aligned properly.
                    // For now, we'll just assume that when the playspace is first initialized, the
                    // tracked space origin overlaps with the world space origin. If a platform ever does
                    // something else (i.e, placing the lower left hand corner of the tracked space at world 
                    // space 0,0,0), we should compensate for that here.

                    return mixedRealityPlayspace;
                }
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

            if (!IsInitialized)
            {
                throw new ArgumentNullException($"Unable to get {nameof(type)} Manager as the Mixed Reality Manager has not been initialized!");
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
                IMixedRealityManager manager;
                GetComponentByType(type, out manager);
                if (manager != null)
                {
                    MixedRealityComponents.Remove(new Tuple<Type, IMixedRealityManager>(type, manager));
                }
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
                IMixedRealityManager manager;
                GetComponentByTypeAndName(type, managerName, out manager);
                if (manager != null)
                {
                    MixedRealityComponents.Remove(new Tuple<Type, IMixedRealityManager>(type, manager));
                }
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
                // If no name provided, return all components of the same type. Else return the type/name combination.
                if (string.IsNullOrWhiteSpace(managerName))
                {
                    GetComponentsByType(type, ref managers);
                }
                else
                {
                    GetComponentsByTypeAndName(type, managerName, ref managers);
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
                   type == typeof(IMixedRealityTeleportSystem) ||
                   type == typeof(IMixedRealityBoundarySystem) ||
                   type == typeof(IMixedRealityDiagnosticsSystem);
        }

        /// <summary>
        /// Retrieve the first component from the registry that meets the selected type
        /// </summary>
        /// <param name="type">Interface type of the component being requested</param>
        /// <param name="manager">return parameter of the function</param>
        private void GetComponentByType(Type type, out IMixedRealityManager manager)
        {
            if (type == null) { throw new ArgumentNullException(nameof(type)); }

            GetComponentByTypeAndName(type, string.Empty, out manager);
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

            manager = null;

            if (isMixedRealityManagerInitializing)
            {
                Debug.LogWarning("Unable to get a manager while initializing!");
                return;
            }

            if (mixedRealityComponentsCount != MixedRealityComponents.Count)
            {
                Initialize();
            }

            for (int i = 0; i < mixedRealityComponentsCount; i++)
            {
                if (CheckComponentMatch(type, managerName, MixedRealityComponents[i]))
                {
                    manager = MixedRealityComponents[i].Item2;
                    break;
                }
            }
        }

        private void GetComponentsByType(Type type, ref List<IMixedRealityManager> managers)
        {
            if (type == null) { throw new ArgumentNullException(nameof(type)); }

            GetComponentsByTypeAndName(type, string.Empty, ref managers);
        }

        private void GetComponentsByTypeAndName(Type type, string managerName, ref List<IMixedRealityManager> managers)
        {
            if (type == null) { throw new ArgumentNullException(nameof(type)); }

            for (int i = 0; i < mixedRealityComponentsCount; i++)
            {
                if (CheckComponentMatch(type, managerName, MixedRealityComponents[i]))
                {
                    managers.Add(MixedRealityComponents[i].Item2);
                }
            }
        }

        private static bool CheckComponentMatch(Type type, string managerName, Tuple<Type, IMixedRealityManager> managerTuple)
        {
            if ((managerTuple.Item1.Name == type.Name ||
                managerTuple.Item2.GetType().Name == type.Name) &&
                (string.IsNullOrEmpty(managerName) || managerTuple.Item2.Name == managerName))
            {
                return true;
            }

            var interfaces = managerTuple.Item2.GetType().GetInterfaces();
            for (int i = 0; i < interfaces.Length; i++)
            {
                if (interfaces[i].Name == type.Name &&
                    (string.IsNullOrEmpty(managerName) || managerTuple.Item2.Name == managerName))
                {
                    return true;
                }
            }

            return false;
        }

        #endregion Manager Utilities

        #endregion Manager Container Management

        #region Manager Accessors

        private static IMixedRealityInputSystem inputSystem = null;

        /// <summary>
        /// The current Input System registered with the Mixed Reality Manager.
        /// </summary>
        public static IMixedRealityInputSystem InputSystem => inputSystem ?? (inputSystem = Instance.GetManager<IMixedRealityInputSystem>());

        private static IMixedRealityBoundarySystem boundarySystem = null;

        /// <summary>
        /// The current Boundary System registered with the Mixed Reality Manager.
        /// </summary>
        public static IMixedRealityBoundarySystem BoundarySystem => boundarySystem ?? (boundarySystem = Instance.GetManager<IMixedRealityBoundarySystem>());

        private static IMixedRealityTeleportSystem teleportSystem = null;

        /// <summary>
        /// The current Teleport System registered with the Mixed Reality Manager.
        /// </summary>
        public static IMixedRealityTeleportSystem TeleportSystem => teleportSystem ?? (teleportSystem = Instance.GetManager<IMixedRealityTeleportSystem>());

        private static IMixedRealityDiagnosticsSystem diagnosticsSystem = null;

        /// <summary>
        /// The current Diagnostics System registered with the Mixed Reality Manager.
        /// </summary>
        public static IMixedRealityDiagnosticsSystem DiagnosticsSystem => diagnosticsSystem ?? (diagnosticsSystem = Instance.GetManager<IMixedRealityDiagnosticsSystem>());

        #endregion Manager Accessors
    }
}