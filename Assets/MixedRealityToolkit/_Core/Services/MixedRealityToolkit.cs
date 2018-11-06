// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Definitions;
using Microsoft.MixedReality.Toolkit.Core.Extensions;
using Microsoft.MixedReality.Toolkit.Core.Interfaces;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.BoundarySystem;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.Diagnostics;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.InputSystem;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.SpatialAwarenessSystem;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.TeleportSystem;
using Microsoft.MixedReality.Toolkit.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Services
{
    /// <summary>
    /// This class is responsible for coordinating the operation of the Mixed Reality Toolkit. It is the only Singleton in the entire project.
    /// It provides a service registry for all active services that are used within a project as well as providing the active configuration profile for the project.
    /// The Profile can be swapped out at any time to meet the needs of your project.
    /// </summary>
    public class MixedRealityToolkit : MonoBehaviour
    {
        #region Mixed Reality Toolkit Profile configuration

        private const string MixedRealityPlayspaceName = "MixedRealityPlayspace";

        private bool isInitializing = false;

        /// <summary>
        /// Checks if there is a valid instance of the MixedRealityToolkit, then checks if there is there a valid Active Profile.
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
        /// The active profile of the Mixed Reality Toolkit which controls which components are active and their initial configuration.
        /// *Note configuration is used on project initialization or replacement, changes to properties while it is running has no effect.
        /// </summary>
        [SerializeField]
        [Tooltip("The current active configuration for the Mixed Reality project")]
        private MixedRealityToolkitConfigurationProfile activeProfile = null;

        /// <summary>
        /// The public property of the Active Profile, ensuring events are raised on the change of the configuration
        /// </summary>
        public MixedRealityToolkitConfigurationProfile ActiveProfile
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
        /// When a configuration Profile is replaced with a new configuration, force all services to reset and read the new values
        /// </summary>
        /// <param name="profile"></param>
        public void ResetConfiguration(MixedRealityToolkitConfigurationProfile profile)
        {
            if (activeProfile != null)
            {
                DisableAllServices();
                DestroyAllServices();
            }

            activeProfile = profile;

            if (profile != null)
            {
                DisableAllServices();
                DestroyAllServices();
            }

            Initialize();
        }

        #endregion Mixed Reality Toolkit Profile configuration

        #region Mixed Reality runtime component registry

        /// <summary>
        /// Local component registry for the Mixed Reality Toolkit, to allow runtime use of the <see cref="IMixedRealityExtensionService"/>.
        /// </summary>
        public List<Tuple<Type, IMixedRealityExtensionService>> MixedRealityComponents { get; } = new List<Tuple<Type, IMixedRealityExtensionService>>();

        private int mixedRealityComponentsCount = 0;

        #endregion Mixed Reality runtime component registry

        /// <summary>
        /// Function called when the instance is assigned.
        /// Once all services are registered and properties updated, the Mixed Reality Toolkit will initialize all active services.
        /// This ensures all services can reference each other once started.
        /// </summary>
        private void Initialize()
        {
            isInitializing = true;

            //If the Mixed Reality Toolkit is not configured, stop.
            if (ActiveProfile == null)
            {
                Debug.LogError("No Mixed Reality Configuration Profile found, cannot initialize the Mixed Reality Toolkit");
                return;
            }

#if UNITY_EDITOR
            if (ActiveProfile.ActiveServices.Count > 0)
            {
                if (!Application.isPlaying)
                {
                    DisableAllServices();
                    DestroyAllServices();
                }
                else
                {
                    mixedRealityComponentsCount = 0;
                    MixedRealityComponents.Clear();
                    ActiveProfile.ActiveServices.Clear();
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

            #region Services Registration

            // If the Input system has been selected for initialization in the Active profile, enable it in the project
            if (ActiveProfile.IsInputSystemEnabled)
            {
#if UNITY_EDITOR
                // Make sure unity axis mappings are set.
                Utilities.Editor.InputMappingAxisUtility.CheckUnityInputManagerMappings(Definitions.Devices.ControllerMappingLibrary.UnityInputManagerAxes);
#endif

                if (!RegisterService(typeof(IMixedRealityInputSystem), Activator.CreateInstance(ActiveProfile.InputSystemType) as IMixedRealityInputSystem) ||
                    InputSystem == null)
                {
                    Debug.LogError("Failed to start the Input System!");
                }
            }

            // If the Boundary system has been selected for initialization in the Active profile, enable it in the project
            if (ActiveProfile.IsBoundarySystemEnabled)
            {
                if (!RegisterService(typeof(IMixedRealityBoundarySystem), Activator.CreateInstance(ActiveProfile.BoundarySystemSystemType) as IMixedRealityBoundarySystem) ||
                    BoundarySystem == null)
                {
                    Debug.LogError("Failed to start the Boundary System!");
                }
            }


            // If the Spatial Awareness system has been selected for initialization in the Active profile, enable it in the project
            if (ActiveProfile.IsSpatialAwarenessSystemEnabled)
            {
                if (!RegisterService(typeof(IMixedRealitySpatialAwarenessSystem), Activator.CreateInstance(ActiveProfile.SpatialAwarenessSystemSystemType) as IMixedRealitySpatialAwarenessSystem) ||
                    SpatialAwarenessSystem == null)
                {
                    Debug.LogError("Failed to start the Spatial Awareness System!");
                }
            }

            // If the Teleport system has been selected for initialization in the Active profile, enable it in the project
            if (ActiveProfile.IsTeleportSystemEnabled)
            {
                if (!RegisterService(typeof(IMixedRealityTeleportSystem), Activator.CreateInstance(ActiveProfile.TeleportSystemSystemType) as IMixedRealityTeleportSystem) ||
                    TeleportSystem == null)
                {
                    Debug.LogError("Failed to start the Teleport System!");
                }
            }

            if (ActiveProfile.IsDiagnosticsSystemEnabled)
            {
                if (!RegisterService(typeof(IMixedRealityDiagnosticsSystem), Activator.CreateInstance(ActiveProfile.DiagnosticsSystemSystemType) as IMixedRealityDiagnosticsSystem) ||
                    DiagnosticsSystem == null)
                {
                    Debug.LogError("Failed to start the Diagnostics System!");
                }
            }

            if (ActiveProfile.RegisteredServiceProvidersProfile != null)
            {
                for (int i = 0; i < ActiveProfile.RegisteredServiceProvidersProfile.Configurations?.Length; i++)
                {
                    var configuration = ActiveProfile.RegisteredServiceProvidersProfile.Configurations[i];
#if UNITY_EDITOR
                    if (UnityEditor.EditorUserBuildSettings.activeBuildTarget.IsPlatformSupported(configuration.RuntimePlatform))
#else
                    if (Application.platform.IsPlatformSupported(configuration.RuntimePlatform))
#endif
                    {
                        if (configuration.ComponentType.Type != null)
                        {
                            if (!RegisterService(typeof(IMixedRealityExtensionService), Activator.CreateInstance(configuration.ComponentType, configuration.ComponentName, configuration.Priority) as IMixedRealityExtensionService))
                            {
                                Debug.LogError($"Failed to register the {configuration.ComponentType.Type} Extension Service!");
                            }
                        }
                    }
                }
            }

            #endregion Service Registration

            #region Services Initialization

            //TODO should this be optional?
            //Sort the services based on Priority
            var orderedServices = ActiveProfile.ActiveServices.OrderBy(m => m.Value.Priority).ToArray();
            ActiveProfile.ActiveServices.Clear();

            foreach (var service in orderedServices)
            {
                RegisterService(service.Key, service.Value);
            }

            InitializeAllServices();

            #endregion Services Initialization

            isInitializing = false;
        }

        private void EnsureMixedRealityRequirements()
        {
            // There's lots of documented cases that if the camera doesn't start at 0,0,0, things break with the WMR SDK specifically.
            // We'll enforce that here, then tracking can update it to the appropriate position later.
            CameraCache.Main.transform.position = Vector3.zero;
        }

        #region MonoBehaviour Implementation

        private static MixedRealityToolkit instance;

        /// <summary>
        /// Returns the Singleton instance of the classes type.
        /// If no instance is found, then we search for an instance in the scene.
        /// If more than one instance is found, we log an error and no instance is returned.
        /// </summary>
        public static MixedRealityToolkit Instance
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

                var objects = FindObjectsOfType<MixedRealityToolkit>();
                searchForInstance = false;

                switch (objects.Length)
                {
                    case 0:
                        instance = new GameObject(nameof(MixedRealityToolkit)).AddComponent<MixedRealityToolkit>();
                        break;
                    case 1:
                        instance = objects[0];
                        break;
                    default:
                        Debug.LogError($"Expected exactly 1 {nameof(MixedRealityToolkit)} but found {objects.Length}.");
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
        /// Expose an assertion whether the MixedRealityToolkit class is initialized.
        /// </summary>
        public static void AssertIsInitialized()
        {
            Debug.Assert(IsInitialized, "The MixedRealityToolkit has not been initialized.");
        }

        /// <summary>
        /// Returns whether the instance has been initialized or not.
        /// </summary>
        public static bool IsInitialized => instance != null;

        /// <summary>
        /// Static function to determine if the MixedRealityToolkit class has been initialized or not.
        /// </summary>
        /// <returns></returns>
        public static bool ConfirmInitialized()
        {
            // ReSharper disable once UnusedVariable
            // Assigning the Instance to access is used Implicitly.
            MixedRealityToolkit access = Instance;
            return IsInitialized;
        }

        /// <summary>
        /// Lock property for the Mixed Reality Toolkit to prevent reinitialization
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
            get
            {
                AssertIsInitialized();

                if (mixedRealityPlayspace)
                {
                    return mixedRealityPlayspace;
                }

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
                        Debug.LogWarning($"The Mixed Reality Toolkit expected the camera\'s parent to be named {MixedRealityPlayspaceName}. The existing parent will be renamed and used instead.");
                        // If we rename it, we make it clearer that why it's being teleported around at runtime.
                        CameraCache.Main.transform.parent.name = MixedRealityPlayspaceName;
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

        private void ApplicationOnQuitting()
        {
            DisableAllServices();
            DestroyAllServices();
        }

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

                Debug.LogWarning("Trying to instantiate a second instance of the Mixed Reality Toolkit. Additional Instance was destroyed");
            }
            else if (!IsInitialized)
            {
                InitializeInternal();
                searchForInstance = false;
            }
        }

        private void OnEnable()
        {
            EnableAllServices();
        }

        private void Update()
        {
            UpdateAllServices();
        }

        private void OnDisable()
        {
            DisableAllServices();
        }

        private void OnDestroy()
        {
            DestroyAllServices();

            if (instance == this)
            {
                instance = null;
                searchForInstance = true;
            }
        }

        #endregion MonoBehaviour Implementation

        #region Service Container Management

        #region Individual Service Management

        /// <summary>
        /// Add a new service to the Mixed Reality Toolkit active service registry.
        /// </summary>
        /// <param name="type">The interface type for the system to be managed.  E.G. InputSystem, BoundarySystem</param>
        /// <param name="service">The Instance of the service class to register</param>
        public bool RegisterService(Type type, IMixedRealityService service)
        {
            if (ActiveProfile == null)
            {
                Debug.LogError($"Unable to add a new {type.Name} Service as the Mixed Reality Toolkit has to Active Profile");
                return false;
            }

            if (type == null)
            {
                Debug.LogWarning("Unable to add a manager of type null.");
                return false;
            }

            if (service == null)
            {
                Debug.LogWarning("Unable to add a manager with a null instance.");
                return false;
            }

            if (IsCoreSystem(type))
            {
                IMixedRealityService preExistingService;

                ActiveProfile.ActiveServices.TryGetValue(type, out preExistingService);

                if (preExistingService == null)
                {
                    ActiveProfile.ActiveServices.Add(type, service);
                    return true;
                }

                Debug.LogError($"There's already a {type.Name} registered.");
                return false;
            }

            if (!typeof(IMixedRealityExtensionService).IsAssignableFrom(type))
            {
                Debug.LogError($"Unable to register {type}. Concrete type does not implement the IMixedRealityExtensionService implementation.");
                return false;
            }

            MixedRealityComponents.Add(new Tuple<Type, IMixedRealityExtensionService>(type, (IMixedRealityExtensionService)service));
            if (!isInitializing) { service.Initialize(); }
            mixedRealityComponentsCount = MixedRealityComponents.Count;
            return true;
        }

        /// <summary>
        /// Generic function used to retrieve a service from the Mixed Reality Toolkit active service registry
        /// </summary>
        /// <typeparam name="T">The interface type for the system to be retrieved.  E.G. InputSystem, BoundarySystem.
        /// *Note type should be the Interface of the system to be retrieved and not the class itself</typeparam>
        /// <returns>The instance of the service class that is registered with the selected Interface</returns>
        public T GetService<T>() where T : IMixedRealityService
        {
            return (T)GetService(typeof(T));
        }

        /// <summary>
        /// Retrieve a service from the Mixed Reality Toolkit active service registry
        /// </summary>
        /// <param name="type">The interface type for the system to be retrieved.  E.G. InputSystem, BoundarySystem</param>
        /// <returns>The Mixed Reality Toolkit of the specified type</returns>
        public IMixedRealityService GetService(Type type)
        {
            if (ActiveProfile == null)
            {
                Debug.LogError($"Unable to get {nameof(type)} Manager as the Mixed Reality Manager has no Active Profile.");
                return null;
            }

            if (!IsInitialized)
            {
                Debug.LogError($"Unable to get {nameof(type)} Manager as the Mixed Reality Manager has not been initialized!");
                return null;
            }

            if (type == null)
            {
                Debug.LogError("Unable to get null manager type.");
                return null;
            }

            IMixedRealityService service;
            if (IsCoreSystem(type))
            {
                ActiveProfile.ActiveServices.TryGetValue(type, out service);
            }
            else
            {
                GetService(type, out service);
            }

            if (service == null)
            {
                Debug.Log($"Unable to find {type.Name}.");
            }

            return service;
        }

        /// <summary>
        /// Retrieve a service from the Mixed Reality Toolkit active service registry
        /// </summary>
        /// <param name="type">The interface type for the system to be retrieved.  E.G. InputSystem, BoundarySystem</param>
        /// <param name="serviceName">Name of the specific service</param>
        /// <returns>The Mixed Reality Toolkit of the specified type</returns>
        public IMixedRealityService GetService(Type type, string serviceName)
        {
            if (ActiveProfile == null)
            {
                Debug.LogError($"Unable to get {serviceName} Manager as the Mixed Reality Manager has no Active Profile.");
                return null;
            }

            if (type == null)
            {
                Debug.LogError("Unable to get null manager type.");
                return null;
            }
            if (string.IsNullOrEmpty(serviceName))
            {
                Debug.LogError("Unable to get manager by name without the name being specified.");
                return null;
            }

            IMixedRealityService service;
            if (IsCoreSystem(type))
            {
                ActiveProfile.ActiveServices.TryGetValue(type, out service);
            }
            else
            {
                GetService(type, serviceName, out service);
            }

            if (service == null)
            {
                Debug.LogError($"Unable to find {serviceName} Manager.");
            }

            return service;
        }

        /// <summary>
        /// Remove all services from the Mixed Reality Toolkit active service registry for a given type
        /// </summary>
        /// <param name="type">The interface type for the system to be removed.  E.G. InputSystem, BoundarySystem</param>
        public void UnregisterService(Type type)
        {
            if (ActiveProfile == null)
            {
                Debug.LogError($"Unable to remove {nameof(type)} Manager as the Mixed Reality Manager has no Active Profile.");
                return;
            }

            if (type == null)
            {
                Debug.LogError("Unable to remove null manager type.");
                return;
            }

            if (IsCoreSystem(type))
            {
                ActiveProfile.ActiveServices.Remove(type);
            }
            else
            {
                IMixedRealityService service;
                GetService(type, out service);
                if (service != null)
                {
                    MixedRealityComponents.Remove(new Tuple<Type, IMixedRealityExtensionService>(type, (IMixedRealityExtensionService)service));
                }
            }
        }

        /// <summary>
        /// Remove services from the Mixed Reality Toolkit active service registry for a given type and name
        /// Name is only supported for Mixed Reality runtime components
        /// </summary>
        /// <param name="type">The interface type for the system to be removed.  E.G. InputSystem, BoundarySystem</param>
        /// <param name="serviceName">The name of the service to be removed. (Only for runtime components) </param>
        public void UnregisterService(Type type, string serviceName)
        {
            if (ActiveProfile == null)
            {
                Debug.LogError($"Unable to remove {serviceName} Manager as the Mixed Reality Manager has no Active Profile.");
                return;
            }

            if (type == null)
            {
                Debug.LogError("Unable to remove null manager type.");
                return;
            }

            if (string.IsNullOrEmpty(serviceName))
            {
                Debug.LogError("Unable to remove manager by name without the name being specified.");
                return;
            }

            if (IsCoreSystem(type))
            {
                ActiveProfile.ActiveServices.Remove(type);
            }
            else
            {
                IMixedRealityService service;

                if (GetService(type, serviceName, out service))
                {
                    MixedRealityComponents.Remove(new Tuple<Type, IMixedRealityExtensionService>(type, (IMixedRealityExtensionService)service));
                }
            }
        }

        /// <summary>
        /// Disable all services in the Mixed Reality Toolkit active service registry for a given type
        /// </summary>
        /// <param name="type">The interface type for the system to be removed.  E.G. InputSystem, BoundarySystem</param>
        public void DisableService(Type type)
        {
            if (type == null)
            {
                Debug.LogError("Unable to disable null manager type.");
                return;
            }

            if (IsCoreSystem(type))
            {
                GetService(type).Disable();
            }
            else
            {
                foreach (var service in GetActiveServices(type))
                {
                    service.Disable();
                }
            }
        }

        /// <summary>
        /// Disable a specific service from the Mixed Reality Toolkit active service registry
        /// </summary>
        /// <param name="type">The interface type for the system to be removed.  E.G. InputSystem, BoundarySystem</param>
        /// <param name="serviceName">Name of the specific service</param>
        public void DisableService(Type type, string serviceName)
        {
            if (type == null)
            {
                Debug.LogError("Unable to disable null manager type.");
                return;
            }
            if (string.IsNullOrEmpty(serviceName))
            {
                Debug.LogError("Unable to disable manager by name without the name being specified.");
                return;
            }

            if (IsCoreSystem(type))
            {
                GetService(type).Disable();
            }
            else
            {
                foreach (var service in GetActiveServices(type, serviceName))
                {
                    service.Disable();
                }
            }
        }

        /// <summary>
        /// Enable all services in the Mixed Reality Toolkit active service registry for a given type
        /// </summary>
        /// <param name="type">The interface type for the system to be removed.  E.G. InputSystem, BoundarySystem</param>
        public void EnableService(Type type)
        {
            if (type == null)
            {
                Debug.LogError("Unable to enable null manager type.");
                return;
            }

            if (IsCoreSystem(type))
            {
                GetService(type).Enable();
            }
            else
            {
                foreach (var service in GetActiveServices(type))
                {
                    service.Enable();
                }
            }
        }

        /// <summary>
        /// Enable a specific service from the Mixed Reality Toolkit active service registry
        /// </summary>
        /// <param name="type">The interface type for the system to be removed.  E.G. InputSystem, BoundarySystem</param>
        /// <param name="serviceName">Name of the specific service</param>
        public void EnableService(Type type, string serviceName)
        {
            if (type == null)
            {
                Debug.LogError("Unable to enable null manager type.");
                return;
            }
            if (string.IsNullOrEmpty(serviceName))
            {
                Debug.LogError("Unable to enable manager by name without the name being specified.");
                return;
            }

            if (IsCoreSystem(type))
            {
                GetService(type).Enable();
            }
            else
            {
                foreach (var service in GetActiveServices(type, serviceName))
                {
                    service.Enable();
                }
            }
        }

        #endregion Individual Service Management

        #region Multiple Service Management

        /// <summary>
        /// Retrieve all services from the Mixed Reality Toolkit active service registry for a given type and an optional name
        /// </summary>
        /// <param name="type">The interface type for the system to be retrieved.  E.G. InputSystem, BoundarySystem</param>
        /// <returns>An array of services that meet the search criteria</returns>
        public List<IMixedRealityService> GetActiveServices(Type type)
        {
            if (type == null)
            {
                Debug.LogWarning("Unable to get managers with a type of null.");
                return new List<IMixedRealityService>();
            }

            return GetActiveServices(type, string.Empty);
        }

        /// <summary>
        /// Retrieve all services from the Mixed Reality Toolkit active service registry for a given type and an optional name
        /// </summary>
        /// <param name="type">The interface type for the system to be retrieved.  E.G. InputSystem, BoundarySystem</param>
        /// <param name="serviceName">Name of the specific service</param>
        /// <returns>An array of services that meet the search criteria</returns>
        public List<IMixedRealityService> GetActiveServices(Type type, string serviceName)
        {
            if (ActiveProfile == null)
            {
                Debug.LogWarning($"Unable to get {nameof(type)} Manager as the Mixed Reality Manager has no Active Profile");
                return new List<IMixedRealityService>();
            }

            if (type == null)
            {
                Debug.LogWarning("Unable to get managers with a type of null.");
                return new List<IMixedRealityService>();
            }

            var services = new List<IMixedRealityService>();

            if (IsCoreSystem(type))
            {
                foreach (var service in ActiveProfile.ActiveServices)
                {
                    if (service.Key.Name == type.Name)
                    {
                        services.Add(service.Value);
                    }
                }
            }
            else
            {
                // If no name provided, return all components of the same type. Else return the type/name combination.
                if (string.IsNullOrWhiteSpace(serviceName))
                {
                    GetService(type, ref services);
                }
                else
                {
                    GetServices(type, serviceName, ref services);
                }
            }

            return services;
        }

        private void InitializeAllServices()
        {
            //If the Mixed Reality Toolkit is not configured, stop.
            if (activeProfile == null) { return; }

            //Initialize all services
            foreach (var service in activeProfile.ActiveServices)
            {
                service.Value.Initialize();
            }

            // Enable all registered runtime components
            foreach (var component in MixedRealityComponents)
            {
                component.Item2.Initialize();
            }
        }

        private void ResetAllServices()
        {
            //If the Mixed Reality Toolkit is not configured, stop.
            if (activeProfile == null) { return; }

            // Reset all active services in the registry
            foreach (var service in activeProfile.ActiveServices)
            {
                service.Value.Reset();
            }

            // Reset all registered runtime components
            foreach (var service in MixedRealityComponents)
            {
                service.Item2.Reset();
            }
        }

        private void EnableAllServices()
        {
            //If the Mixed Reality Toolkit is not configured, stop.
            if (activeProfile == null) { return; }

            // Enable all active services in the registry
            foreach (var service in activeProfile.ActiveServices)
            {
                service.Value.Enable();
            }

            // Enable all registered runtime components
            foreach (var component in MixedRealityComponents)
            {
                component.Item2.Enable();
            }
        }

        private void UpdateAllServices()
        {
            //If the Mixed Reality Toolkit is not configured, stop.
            if (activeProfile == null) { return; }

            // Update service registry
            foreach (var service in activeProfile.ActiveServices)
            {
                service.Value.Update();
            }

            //Update runtime component registry
            foreach (var component in MixedRealityComponents)
            {
                component.Item2.Update();
            }
        }

        private void DisableAllServices()
        {
            //If the Mixed Reality Toolkit is not configured, stop.
            if (activeProfile == null) { return; }

            // Disable all active services in the registry
            foreach (var service in activeProfile.ActiveServices)
            {
                service.Value.Disable();
            }

            // Disable all registered runtime components
            foreach (var component in MixedRealityComponents)
            {
                component.Item2.Disable();
            }
        }

        private void DestroyAllServices()
        {
            //If the Mixed Reality Toolkit is not configured, stop.
            if (activeProfile == null) { return; }

            // Destroy all active services in the registry
            foreach (var service in activeProfile.ActiveServices)
            {
                service.Value.Destroy();
            }

            activeProfile.ActiveServices.Clear();

            // Destroy all registered runtime components
            foreach (var component in MixedRealityComponents)
            {
                component.Item2.Destroy();
            }

            MixedRealityComponents.Clear();
        }

        #endregion Multiple Service Management

        #region Service Utilities

        /// <summary>
        /// Generic function used to interrogate the Mixed Reality Toolkit active service registry for the existence of a service
        /// </summary>
        /// <typeparam name="T">The interface type for the system to be retrieved.  E.G. InputSystem, BoundarySystem.
        /// *Note type should be the Interface of the system to be retrieved and not the class itself</typeparam>
        /// <returns>True, there is a service registered with the selected interface, False, no service found for that interface</returns>
        public bool IsServiceRegistered<T>() where T : class
        {
            IMixedRealityService service;
            ActiveProfile.ActiveServices.TryGetValue(typeof(T), out service);
            return service != null;
        }

        private bool IsCoreSystem(Type type)
        {
            if (type == null)
            {
                Debug.LogWarning($"Null cannot be a core manager.");
                return false;
            }

            return typeof(IMixedRealityInputSystem).IsAssignableFrom(type) ||
                   typeof(IMixedRealityTeleportSystem).IsAssignableFrom(type) ||
                   typeof(IMixedRealityBoundarySystem).IsAssignableFrom(type) ||
                   typeof(IMixedRealitySpatialAwarenessSystem).IsAssignableFrom(type) ||
                   typeof(IMixedRealityDiagnosticsSystem).IsAssignableFrom(type);
        }

        /// <summary>
        /// Retrieve the first component from the registry that meets the selected type
        /// </summary>
        /// <param name="type">Interface type of the component being requested</param>
        /// <param name="service">return parameter of the function</param>
        private void GetService(Type type, out IMixedRealityService service)
        {
            if (type == null)
            {
                Debug.LogWarning("Unable to get a component with a type of null.");
                service = null;
                return;
            }

            GetService(type, string.Empty, out service);
        }

        /// <summary>
        /// Retrieve the first component from the registry that meets the selected type and name
        /// </summary>
        /// <param name="type">Interface type of the component being requested</param>
        /// <param name="serviceName">Name of the specific service</param>
        /// <param name="service">return parameter of the function</param>
        private bool GetService(Type type, string serviceName, out IMixedRealityService service)
        {
            if (type == null)
            {
                Debug.LogWarning("Unable to get a component with a type of null.");
                service = null;
                return false;
            }

            service = null;

            if (isInitializing)
            {
                Debug.LogWarning("Unable to get a service while initializing!");
                return false;
            }

            if (mixedRealityComponentsCount != MixedRealityComponents.Count)
            {
                Initialize();
            }

            for (int i = 0; i < mixedRealityComponentsCount; i++)
            {
                if (CheckComponentMatch(type, serviceName, MixedRealityComponents[i]))
                {
                    service = MixedRealityComponents[i].Item2;
                    return true;
                }
            }

            return false;
        }

        private void GetService(Type type, ref List<IMixedRealityService> services)
        {
            if (type == null)
            {
                Debug.LogWarning("Unable to get components with a type of null.");
                return;
            }

            GetServices(type, string.Empty, ref services);
        }

        private void GetServices(Type type, string serviceName, ref List<IMixedRealityService> services)
        {
            if (type == null)
            {
                Debug.LogWarning("Unable to get components with a type of null.");
                return;
            }

            for (int i = 0; i < mixedRealityComponentsCount; i++)
            {
                if (CheckComponentMatch(type, serviceName, MixedRealityComponents[i]))
                {
                    services.Add(MixedRealityComponents[i].Item2);
                }
            }
        }

        private static bool CheckComponentMatch(Type type, string serviceName, Tuple<Type, IMixedRealityExtensionService> components)
        {
            bool isValid = string.IsNullOrEmpty(serviceName) || components.Item2.Name == serviceName;

            if ((components.Item1.Name == type.Name || components.Item2.GetType().Name == type.Name) && isValid)
            {
                return true;
            }

            var interfaces = components.Item2.GetType().GetInterfaces();

            for (int i = 0; i < interfaces.Length; i++)
            {
                if (interfaces[i].Name == type.Name && isValid)
                {
                    return true;
                }
            }

            return false;
        }

        #endregion Service Utilities

        #endregion Service Container Management

        #region Core System Accessors

        private static IMixedRealityInputSystem inputSystem = null;

        /// <summary>
        /// The current Input System registered with the Mixed Reality Toolkit.
        /// </summary>
        public static IMixedRealityInputSystem InputSystem => inputSystem ?? (inputSystem = Instance.GetService<IMixedRealityInputSystem>());

        private static IMixedRealityBoundarySystem boundarySystem = null;

        /// <summary>
        /// The current Boundary System registered with the Mixed Reality Toolkit.
        /// </summary>
        public static IMixedRealityBoundarySystem BoundarySystem => boundarySystem ?? (boundarySystem = Instance.GetService<IMixedRealityBoundarySystem>());

        private static IMixedRealitySpatialAwarenessSystem spatialAwarenessSystem = null;

        /// <summary>
        /// The current Spatial Awareness System registered with the Mixed Reality Manager.
        /// </summary>
        public static IMixedRealitySpatialAwarenessSystem SpatialAwarenessSystem => spatialAwarenessSystem ?? (spatialAwarenessSystem = Instance.GetService<IMixedRealitySpatialAwarenessSystem>());

        private static IMixedRealityTeleportSystem teleportSystem = null;

        /// <summary>
        /// The current Teleport System registered with the Mixed Reality Toolkit.
        /// </summary>
        public static IMixedRealityTeleportSystem TeleportSystem => teleportSystem ?? (teleportSystem = Instance.GetService<IMixedRealityTeleportSystem>());

        private static IMixedRealityDiagnosticsSystem diagnosticsSystem = null;

        /// <summary>
        /// The current Diagnostics System registered with the Mixed Reality Toolkit.
        /// </summary>
        public static IMixedRealityDiagnosticsSystem DiagnosticsSystem => diagnosticsSystem ?? (diagnosticsSystem = Instance.GetService<IMixedRealityDiagnosticsSystem>());

        #endregion Core System Accessors
    }
}