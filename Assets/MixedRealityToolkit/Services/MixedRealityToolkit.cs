// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Definitions;
using Microsoft.MixedReality.Toolkit.Core.Definitions.Utilities;
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
using UnityEngine.EventSystems;

namespace Microsoft.MixedReality.Toolkit.Core.Services
{
    /// <summary>
    /// This class is responsible for coordinating the operation of the Mixed Reality Toolkit. It is the only Singleton in the entire project.
    /// It provides a service registry for all active services that are used within a project as well as providing the active configuration profile for the project.
    /// The Profile can be swapped out at any time to meet the needs of your project.
    /// </summary>
    [DisallowMultipleComponent]
    public class MixedRealityToolkit : MonoBehaviour
    {
        #region Mixed Reality Toolkit Profile configuration

        private const string MixedRealityPlayspaceName = "MixedRealityPlayspace";

        private static bool isInitializing = false;

        private static bool isApplicationQuitting = false;

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
        /// The active profile of the Mixed Reality Toolkit which controls which services are active and their initial configuration.
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

            InitializeServiceLocator();
        }

        #endregion Mixed Reality Toolkit Profile configuration

        #region Mixed Reality runtime service registry

        private static readonly Dictionary<Type, IMixedRealityService> activeSystems = new Dictionary<Type, IMixedRealityService>();

        /// <summary>
        /// Current active systems registered with the MixedRealityToolkit.
        /// </summary>
        /// <remarks>
        /// Systems can only be registered once by <see cref="Type"/>
        /// </remarks>
        public static IReadOnlyDictionary<Type, IMixedRealityService> ActiveSystems => activeSystems;

        private static readonly List<Tuple<Type, IMixedRealityService>> registeredMixedRealityServices = new List<Tuple<Type, IMixedRealityService>>();

        /// <summary>
        /// Local service registry for the Mixed Reality Toolkit, to allow runtime use of the <see cref="IMixedRealityService"/>.
        /// </summary>
        public static IReadOnlyList<Tuple<Type, IMixedRealityService>> RegisteredMixedRealityServices => registeredMixedRealityServices;

        /// <summary>
        /// Local service registry for the Mixed Reality Toolkit, to allow runtime use of the <see cref="IMixedRealityService"/>.
        /// </summary>
        [Obsolete("Use RegisteredMixedRealityServices instead.")]
        public List<Tuple<Type, IMixedRealityExtensionService>> MixedRealityComponents => null;

        #endregion Mixed Reality runtime service registry

        /// <summary>
        /// Once all services are registered and properties updated, the Mixed Reality Toolkit will initialize all active services.
        /// This ensures all services can reference each other once started.
        /// </summary>
        private void InitializeServiceLocator()
        {
            isInitializing = true;

            //If the Mixed Reality Toolkit is not configured, stop.
            if (ActiveProfile == null)
            {
                Debug.LogError("No Mixed Reality Configuration Profile found, cannot initialize the Mixed Reality Toolkit");
                return;
            }

#if UNITY_EDITOR
            if (ActiveSystems.Count > 0)
            {
                activeSystems.Clear();
            }

            if (RegisteredMixedRealityServices.Count > 0)
            {
                registeredMixedRealityServices.Clear();
            }
#endif

            ClearCoreSystemCache();
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

                if (!RegisterService<IMixedRealityInputSystem>(ActiveProfile.InputSystemType) || InputSystem == null)
                {
                    Debug.LogError("Failed to start the Input System!");
                }

                if (!RegisterService<IMixedRealityFocusProvider>(ActiveProfile.InputSystemProfile.FocusProviderType))
                {
                    Debug.LogError("Failed to register the focus provider! The input system will not function without it.");
                    return;
                }
            }
            else
            {
#if UNITY_EDITOR
                Utilities.Editor.InputMappingAxisUtility.RemoveMappings(Definitions.Devices.ControllerMappingLibrary.UnityInputManagerAxes);
#endif
            }

            // If the Boundary system has been selected for initialization in the Active profile, enable it in the project
            if (ActiveProfile.IsBoundarySystemEnabled)
            {
                if (!RegisterService<IMixedRealityBoundarySystem>(ActiveProfile.BoundarySystemSystemType) || BoundarySystem == null)
                {
                    Debug.LogError("Failed to start the Boundary System!");
                }
            }

            // If the Spatial Awareness system has been selected for initialization in the Active profile, enable it in the project
            if (ActiveProfile.IsSpatialAwarenessSystemEnabled)
            {
#if UNITY_EDITOR                
                LayerExtensions.SetupLayer(31, "Spatial Awareness");
#endif
                if (!RegisterService<IMixedRealitySpatialAwarenessSystem>(ActiveProfile.SpatialAwarenessSystemSystemType) && SpatialAwarenessSystem != null)
                {
                    Debug.LogError("Failed to start the Spatial Awareness System!");
                }
            }

            // If the Teleport system has been selected for initialization in the Active profile, enable it in the project
            if (ActiveProfile.IsTeleportSystemEnabled)
            {
                if (!RegisterService<IMixedRealityTeleportSystem>(ActiveProfile.TeleportSystemSystemType) || TeleportSystem == null)
                {
                    Debug.LogError("Failed to start the Teleport System!");
                }
            }

            if (ActiveProfile.IsDiagnosticsSystemEnabled)
            {
                if (!RegisterService<IMixedRealityDiagnosticsSystem>(ActiveProfile.DiagnosticsSystemSystemType) || DiagnosticsSystem == null)
                {
                    Debug.LogError("Failed to start the Diagnostics System!");
                }
            }

            if (ActiveProfile.RegisteredServiceProvidersProfile != null)
            {
                for (int i = 0; i < ActiveProfile.RegisteredServiceProvidersProfile.Configurations?.Length; i++)
                {
                    var configuration = ActiveProfile.RegisteredServiceProvidersProfile.Configurations[i];
                    RegisterService<IMixedRealityExtensionService>(configuration.ComponentType, configuration.RuntimePlatform, configuration.ComponentName, configuration.Priority, configuration.ConfigurationProfile);
                }
            }

            #endregion Service Registration

            #region Services Initialization

            var orderedCoreSystems = activeSystems.OrderBy(m => m.Value.Priority).ToArray();
            activeSystems.Clear();

            foreach (var system in orderedCoreSystems)
            {
                RegisterService(system.Key, system.Value);
            }

            var orderedServices = registeredMixedRealityServices.OrderBy(service => service.Item2.Priority).ToArray();
            registeredMixedRealityServices.Clear();

            foreach (var service in orderedServices)
            {
                RegisterService(service.Item1, service.Item2);
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

            bool addedComponents = false;
            if (!Application.isPlaying)
            {
                var eventSystems = FindObjectsOfType<EventSystem>();

                if (eventSystems.Length == 0)
                {
                    CameraCache.Main.gameObject.EnsureComponent<EventSystem>();
                    addedComponents = true;
                }
                else
                {
                    bool raiseWarning;

                    if (eventSystems.Length == 1)
                    {
                        raiseWarning = eventSystems[0].gameObject != CameraCache.Main.gameObject;
                    }
                    else
                    {
                        raiseWarning = true;
                    }

                    if (raiseWarning)
                    {
                        Debug.LogWarning("Found an existing event system in your scene. The Mixed Reality Toolkit requires only one, and must be found on the main camera.");
                    }
                }
            }

            if (!addedComponents)
            {
                CameraCache.Main.gameObject.EnsureComponent<EventSystem>();
            }
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
                MixedRealityToolkit newInstance;

                switch (objects.Length)
                {
                    case 0:
                        newInstance = new GameObject(nameof(MixedRealityToolkit)).AddComponent<MixedRealityToolkit>();
                        break;
                    case 1:
                        newInstance = objects[0];
                        break;
                    default:
                        Debug.LogError($"Expected exactly 1 {nameof(MixedRealityToolkit)} but found {objects.Length}.");
                        return null;
                }

                Debug.Assert(newInstance != null);

                if (!isApplicationQuitting)
                {
                    // Setup any additional things the instance needs.
                    newInstance.InitializeInstance();
                }
                else
                {
                    // Don't do any additional setup because the app is quitting.
                    instance = newInstance;
                }

                Debug.Assert(instance != null);

                return instance;
            }
        }

        /// <summary>
        /// Lock property for the Mixed Reality Toolkit to prevent reinitialization
        /// </summary>
        private static readonly object initializedLock = new object();

        private void InitializeInstance()
        {
            lock (initializedLock)
            {
                if (IsInitialized) { return; }

                instance = this;

                if (Application.isPlaying)
                {
                    DontDestroyOnLoad(instance.transform.root);
                }

                Application.quitting += () =>
                {
                    isApplicationQuitting = true;
                };

#if UNITY_EDITOR
                UnityEditor.EditorApplication.playModeStateChanged += playModeState =>
                {
                    if (playModeState == UnityEditor.PlayModeStateChange.ExitingEditMode ||
                        playModeState == UnityEditor.PlayModeStateChange.EnteredEditMode)
                    {
                        isApplicationQuitting = false;
                    }

                    if (playModeState == UnityEditor.PlayModeStateChange.ExitingEditMode && activeProfile == null)
                    {
                        UnityEditor.EditorApplication.isPlaying = false;
                        UnityEditor.Selection.activeObject = Instance;
                        UnityEditor.EditorGUIUtility.PingObject(Instance);
                    }
                };

                UnityEditor.EditorApplication.hierarchyChanged += () =>
                {
                    if (instance != null)
                    {
                        Debug.Assert(instance.transform.parent == null, "The MixedRealityToolkit should not be parented under any other GameObject!");
                        Debug.Assert(instance.transform.childCount == 0, "The MixedRealityToolkit should not have GameObject children!");
                    }
                };
#endif // UNITY_EDITOR

                if (HasActiveProfile)
                {
                    InitializeServiceLocator();
                }
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

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (!IsInitialized && !UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode)
            {
                ConfirmInitialized();
            }
        }
#endif // UNITY_EDITOR

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
                InitializeInstance();
            }
            else
            {
                Debug.LogError("Failed to properly initialize the MixedRealityToolkit");
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
            ClearCoreSystemCache();

            if (instance == this)
            {
                instance = null;
                searchForInstance = true;
            }
        }

        #endregion MonoBehaviour Implementation

        #region Service Container Management

        #region Registration

        /// <summary>
        /// Add a new service to the Mixed Reality Toolkit active service registry.
        /// </summary>
        /// <param name="interfaceType">The interface type for the system to be registered.  E.G. InputSystem, BoundarySystem</param>
        /// <param name="serviceInstance">The Instance of the service class to register</param>
        public bool RegisterService(Type interfaceType, IMixedRealityService serviceInstance)
        {
            return RegisterServiceInternal(interfaceType, serviceInstance);
        }

        /// <summary>
        /// Create and register a new service to the Mixed Reality Toolkit service registry.
        /// </summary>
        /// <typeparam name="T">The interface type for the system to be registered.</typeparam>
        /// <param name="interfaceType">The concrete type to instantiate.</param>
        /// <param name="supportedPlatforms">The runtime platform to check against when registering.</param>
        /// <param name="args">Optional arguments used when instantiating the concrete type.</param>
        /// <returns>True, if the service was successfully registered.</returns>
        public bool RegisterService<T>(Type interfaceType, SupportedPlatforms supportedPlatforms = (SupportedPlatforms)(-1), params object[] args)
        {
            if (isApplicationQuitting)
            {
                return false;
            }

            if (interfaceType == null)
            {
                Debug.LogError("Unable to register a service with a null concrete type.");
                return false;
            }

            if (!typeof(IMixedRealityService).IsAssignableFrom(interfaceType))
            {
                Debug.LogError($"Unable to register the {interfaceType.Name} service. It does not implement {typeof(IMixedRealityService)}.");
                return false;
            }

#if !UNITY_EDITOR
            if (!Application.platform.IsPlatformSupported(supportedPlatforms))
#else
            if (!UnityEditor.EditorUserBuildSettings.activeBuildTarget.IsPlatformSupported(supportedPlatforms))
#endif
            {
                return false;
            }

            T serviceInstance;

            try
            {
                serviceInstance = (T)Activator.CreateInstance(interfaceType, args);
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to register the {interfaceType.Name} service: {e.GetType()} - {e.Message}");
                return false;
            }

            return RegisterServiceInternal(typeof(T), serviceInstance as IMixedRealityService);
        }

        /// <summary>
        /// Internal service registration.
        /// </summary>
        /// <param name="interfaceType">The interface type for the system to be registered.</param>
        /// <param name="serviceInstance">Instance of the service.</param>
        /// <returns>True if registration is successful, false otherwise.</returns>
        private static bool RegisterServiceInternal(Type interfaceType, IMixedRealityService serviceInstance)
        {
            if (serviceInstance == null)
            {
                Debug.LogWarning($"Unable to add a {interfaceType.Name} service with a null instance.");
                return false;
            }

            if (!CanGetService(interfaceType, serviceInstance.Name)) { return false; }

            IMixedRealityService preExistingService;

            if (GetServiceByNameInternal(interfaceType, serviceInstance.Name, out preExistingService))
            {
                Debug.LogError($"There's already a {interfaceType.Name}.{preExistingService.Name} registered!");
                return false;
            }

            if (IsCoreSystem(interfaceType))
            {
                activeSystems.Add(interfaceType, serviceInstance);
            }
            else if (typeof(IMixedRealityDataProvider).IsAssignableFrom(interfaceType) ||
                     typeof(IMixedRealityExtensionService).IsAssignableFrom(interfaceType))
            {
                registeredMixedRealityServices.Add(new Tuple<Type, IMixedRealityService>(interfaceType, serviceInstance));
            }
            else
            {
                Debug.LogError($"Unable to register {interfaceType.Name}. Concrete type does not implement {typeof(IMixedRealityExtensionService).Name} or {typeof(IMixedRealityDataProvider).Name}.");
                return false;
            }

            if (!isInitializing)
            {
                serviceInstance.Initialize();
                serviceInstance.Enable();
            }

            return true;
        }

        /// <summary>
        /// Remove all services from the Mixed Reality Toolkit active service registry for a given type
        /// </summary>
        /// <param name="interfaceType">The interface type for the system to be removed.  E.G. InputSystem, BoundarySystem</param>
        public bool UnregisterService(Type interfaceType)
        {
            return UnregisterService(interfaceType, string.Empty);
        }

        /// <summary>
        /// Remove services from the Mixed Reality Toolkit active service registry for a given type and name
        /// Name is only supported for Mixed Reality runtime services
        /// </summary>
        /// <param name="interfaceType">The interface type for the system to be removed.  E.G. InputSystem, BoundarySystem</param>
        /// <param name="serviceName">The name of the service to be removed. (Only for runtime services) </param>
        public static bool UnregisterService(Type interfaceType, string serviceName)
        {
            if (interfaceType == null)
            {
                Debug.LogError("Unable to remove null service type.");
                return false;
            }

            IMixedRealityService serviceInstance;

            if (GetServiceByNameInternal(interfaceType, serviceName, out serviceInstance))
            {
                if (IsInitialized)
                {
                    serviceInstance.Disable();
                    serviceInstance.Destroy();
                }

                if (IsCoreSystem(interfaceType))
                {
                    activeSystems.Remove(interfaceType);
                    return true;
                }

                var registryInstance = new Tuple<Type, IMixedRealityService>(interfaceType, serviceInstance);

                if (registeredMixedRealityServices.Contains(registryInstance))
                {
                    registeredMixedRealityServices.Remove(registryInstance);
                    return true;
                }

                Debug.LogError($"Failed to find registry instance of {interfaceType.Name}.{serviceInstance.Name}!");
            }

            return false;
        }

        #endregion Registration

        #region Multiple Service Management

        /// <summary>
        /// Enable all services in the Mixed Reality Toolkit active service registry for a given type
        /// </summary>
        /// <param name="interfaceType">The interface type for the system to be enabled.  E.G. InputSystem, BoundarySystem</param>
        public static void EnableAllServicesByType(Type interfaceType)
        {
            EnableAllServicesByTypeAndName(interfaceType, string.Empty);
        }

        [Obsolete("Use EnableAllServicesByType instead.")]
        public void EnableService(Type interfaceType, string serviceName)
        {
            EnableAllServicesByTypeAndName(interfaceType, serviceName);
        }

        /// <summary>
        /// Enable all services in the Mixed Reality Toolkit active service registry for a given type and name
        /// </summary>
        /// <param name="interfaceType">The interface type for the system to be enabled.  E.G. InputSystem, BoundarySystem</param>
        /// <param name="serviceName">Name of the specific service</param>
        public static void EnableAllServicesByTypeAndName(Type interfaceType, string serviceName)
        {
            if (interfaceType == null)
            {
                Debug.LogError("Unable to enable null service type.");
                return;
            }

            var services = new List<IMixedRealityService>();
            GetAllServicesByNameInternal(interfaceType, serviceName, ref services);

            for (int i = 0; i < services?.Count; i++)
            {
                services[i].Enable();
            }
        }

        /// <summary>
        /// Disable all services in the Mixed Reality Toolkit active service registry for a given type
        /// </summary>
        /// <param name="interfaceType">The interface type for the system to be removed.  E.G. InputSystem, BoundarySystem</param>
        public static void DisableAllServicesByType(Type interfaceType)
        {
            DisableAllServicesByTypeAndName(interfaceType, string.Empty);
        }


        [Obsolete("Use DisableAllServicesByType instead.")]
        public void DisableService(Type interfaceType, string serviceName)
        {
            DisableAllServicesByTypeAndName(interfaceType, serviceName);
        }

        /// <summary>
        /// Disable all services in the Mixed Reality Toolkit active service registry for a given type and name
        /// </summary>
        /// <param name="interfaceType">The interface type for the system to be disabled.  E.G. InputSystem, BoundarySystem</param>
        /// <param name="serviceName">Name of the specific service</param>
        public static void DisableAllServicesByTypeAndName(Type interfaceType, string serviceName)
        {
            if (interfaceType == null)
            {
                Debug.LogError("Unable to disable null service type.");
                return;
            }

            var services = new List<IMixedRealityService>();
            GetAllServicesByNameInternal(interfaceType, serviceName, ref services);

            for (int i = 0; i < services?.Count; i++)
            {
                services[i].Disable();
            }
        }

        /// <summary>
        /// Retrieve all services from the Mixed Reality Toolkit active service registry for a given type and an optional name
        /// </summary>
        /// <param name="interfaceType">The interface type for the system to be retrieved.  E.G. InputSystem, BoundarySystem</param>
        /// <returns>An array of services that meet the search criteria</returns>
        public List<IMixedRealityService> GetActiveServices(Type interfaceType)
        {
            return GetActiveServices(interfaceType, string.Empty);
        }

        /// <summary>
        /// Retrieve all services from the Mixed Reality Toolkit active service registry for a given type and name
        /// </summary>
        /// <param name="interfaceType">The interface type for the system to be retrieved.  E.G. InputSystem, BoundarySystem</param>
        /// <param name="serviceName">Name of the specific service</param>
        /// <returns>An array of services that meet the search criteria</returns>
        public List<IMixedRealityService> GetActiveServices(Type interfaceType, string serviceName)
        {
            var services = new List<IMixedRealityService>();

            if (interfaceType == null)
            {
                Debug.LogWarning("Unable to get services with a type of null.");
                return services;
            }

            if (IsCoreSystem(interfaceType))
            {
                foreach (var system in activeSystems)
                {
                    if (system.Key.Name == interfaceType.Name)
                    {
                        services.Add(system.Value);
                    }
                }
            }
            else
            {
                // If no name provided, return all services of the same type. Else return the type/name combination.
                if (string.IsNullOrWhiteSpace(serviceName))
                {
                    GetAllServicesInternal(interfaceType, ref services);
                }
                else
                {
                    GetAllServicesByNameInternal(interfaceType, serviceName, ref services);
                }
            }

            return services;
        }

        private void InitializeAllServices()
        {
            // If the Mixed Reality Toolkit is not configured, stop.
            if (activeProfile == null) { return; }

            // Initialize all systems
            foreach (var system in activeSystems)
            {
                system.Value.Initialize();
            }

            // Initialize all registered runtime services
            foreach (var service in registeredMixedRealityServices)
            {
                service.Item2.Initialize();
            }
        }

        private void ResetAllServices()
        {
            // If the Mixed Reality Toolkit is not configured, stop.
            if (activeProfile == null) { return; }

            // If the Mixed Reality Toolkit is not initialized, stop.
            if (!IsInitialized) { return; }

            // Reset all systems
            foreach (var system in activeSystems)
            {
                system.Value.Reset();
            }

            // Reset all registered runtime services
            foreach (var service in registeredMixedRealityServices)
            {
                service.Item2.Reset();
            }
        }

        private void EnableAllServices()
        {
            // If the Mixed Reality Toolkit is not configured, stop.
            if (activeProfile == null) { return; }

            // If the Mixed Reality Toolkit is not initialized, stop.
            if (!IsInitialized) { return; }

            // Enable all systems
            foreach (var system in activeSystems)
            {
                system.Value.Enable();
            }

            // Reset all registered runtime services
            foreach (var service in registeredMixedRealityServices)
            {
                service.Item2.Enable();
            }
        }

        private void UpdateAllServices()
        {
            // If the Mixed Reality Toolkit is not configured, stop.
            if (activeProfile == null) { return; }

            // If the Mixed Reality Toolkit is not initialized, stop.
            if (!IsInitialized) { return; }

            // Update all systems
            foreach (var system in activeSystems)
            {
                system.Value.Update();
            }

            // Update all registered runtime services
            foreach (var service in registeredMixedRealityServices)
            {
                service.Item2.Update();
            }
        }

        private void DisableAllServices()
        {
            // If the Mixed Reality Toolkit is not configured, stop.
            if (activeProfile == null) { return; }

            // If the Mixed Reality Toolkit is not initialized, stop.
            if (!IsInitialized) { return; }

            // Disable all systems
            foreach (var system in activeSystems)
            {
                system.Value.Disable();
            }

            // Disable all registered runtime services
            foreach (var service in registeredMixedRealityServices)
            {
                service.Item2.Disable();
            }
        }

        private void DestroyAllServices()
        {
            // If the Mixed Reality Toolkit is not configured, stop.
            if (activeProfile == null) { return; }

            // If the Mixed Reality Toolkit is not initialized, stop.
            if (!IsInitialized) { return; }

            // Destroy all systems
            foreach (var system in activeSystems)
            {
                system.Value.Destroy();
            }

            activeSystems.Clear();

            // Destroy all registered runtime services
            foreach (var service in registeredMixedRealityServices)
            {
                service.Item2.Destroy();
            }

            registeredMixedRealityServices.Clear();
        }

        #endregion Multiple Service Management

        #region Service Utilities

        /// <summary>
        /// Generic function used to interrogate the Mixed Reality Toolkit registered services registry for the existence of a service.
        /// </summary>
        /// <typeparam name="T">The interface type for the service to be retrieved.</typeparam>
        /// <remarks>
        /// Note: type should be the Interface of the system to be retrieved and not the concrete class itself.
        /// </remarks>
        /// <returns>True, there is a service registered with the selected interface, False, no service found for that interface</returns>
        public bool IsServiceRegistered<T>() where T : class
        {
            return GetService(typeof(T)) != null;
        }

        /// <summary>
        /// Generic function used to interrogate the Mixed Reality Toolkit active system registry for the existence of a core system.
        /// </summary>
        /// <typeparam name="T">The interface type for the system to be retrieved.  E.G. InputSystem, BoundarySystem.</typeparam>
        /// <remarks>
        /// Note: type should be the Interface of the system to be retrieved and not the concrete class itself.
        /// </remarks>
        /// <returns>True, there is a system registered with the selected interface, False, no system found for that interface</returns>
        public bool IsSystemRegistered<T>() where T : class
        {
            IMixedRealityService service;
            activeSystems.TryGetValue(typeof(T), out service);
            return service != null;
        }

        private static bool IsCoreSystem(Type type)
        {
            if (type == null)
            {
                Debug.LogWarning("Null cannot be a core system.");
                return false;
            }

            return typeof(IMixedRealityInputSystem).IsAssignableFrom(type) ||
                   typeof(IMixedRealityFocusProvider).IsAssignableFrom(type) ||
                   typeof(IMixedRealityTeleportSystem).IsAssignableFrom(type) ||
                   typeof(IMixedRealityBoundarySystem).IsAssignableFrom(type) ||
                   typeof(IMixedRealitySpatialAwarenessSystem).IsAssignableFrom(type) ||
                   typeof(IMixedRealityDiagnosticsSystem).IsAssignableFrom(type);
        }

        private static void ClearCoreSystemCache()
        {
            inputSystem = null;
            teleportSystem = null;
            boundarySystem = null;
            spatialAwarenessSystem = null;
            diagnosticsSystem = null;
        }

        /// <summary>
        /// Generic function used to retrieve a service from the Mixed Reality Toolkit active service registry
        /// </summary>
        /// <param name="showLogs">Should the logs show when services cannot be found?</param>
        /// <typeparam name="T">The interface type for the system to be retrieved.  E.G. InputSystem, BoundarySystem.
        /// *Note type should be the Interface of the system to be retrieved and not the class itself</typeparam>
        /// <returns>The instance of the service class that is registered with the selected Interface</returns>
        public T GetService<T>(bool showLogs = true) where T : IMixedRealityService
        {
            return (T)GetService(typeof(T), showLogs);
        }

        /// <summary>
        /// Retrieve a service from the Mixed Reality Toolkit active service registry
        /// </summary>
        /// <param name="interfaceType">The interface type for the system to be retrieved.  E.G. InputSystem, BoundarySystem</param>
        /// <param name="showLogs">Should the logs show when services cannot be found?</param>
        /// <returns>The Mixed Reality Toolkit of the specified type</returns>
        public IMixedRealityService GetService(Type interfaceType, bool showLogs = true)
        {
            return GetService(interfaceType, string.Empty, showLogs);
        }

        /// <summary>
        /// Retrieve a service from the Mixed Reality Toolkit active service registry
        /// </summary>
        /// <param name="interfaceType">The interface type for the system to be retrieved.  E.G. InputSystem, BoundarySystem</param>
        /// <param name="serviceName">Name of the specific service</param>
        /// <param name="showLogs">Should the logs show when services cannot be found?</param>
        /// <returns>The Mixed Reality Toolkit of the specified type</returns>
        public IMixedRealityService GetService(Type interfaceType, string serviceName, bool showLogs = true)
        {
            IMixedRealityService serviceInstance;

            if (!GetServiceByNameInternal(interfaceType, serviceName, out serviceInstance) && showLogs)
            {
                Debug.LogError($"Unable to find {(string.IsNullOrWhiteSpace(serviceName) ? interfaceType.Name : serviceName)} service.");
            }

            return serviceInstance;
        }

        /// <summary>
        /// Retrieve the first service from the registry that meets the selected type and name
        /// </summary>
        /// <param name="interfaceType">Interface type of the service being requested</param>
        /// <param name="serviceName">Name of the specific service</param>
        /// <param name="serviceInstance">return parameter of the function</param>
        private static bool GetServiceByNameInternal(Type interfaceType, string serviceName, out IMixedRealityService serviceInstance)
        {
            serviceInstance = null;

            if (!CanGetService(interfaceType, serviceName)) { return false; }

            if (IsCoreSystem(interfaceType))
            {
                if (activeSystems.TryGetValue(interfaceType, out serviceInstance))
                {
                    if (CheckServiceMatch(interfaceType, serviceName, interfaceType, serviceInstance))
                    {
                        return true;
                    }

                    serviceInstance = null;
                }
            }
            else
            {
                for (int i = 0; i < registeredMixedRealityServices.Count; i++)
                {
                    if (CheckServiceMatch(interfaceType, serviceName, registeredMixedRealityServices[i].Item1, registeredMixedRealityServices[i].Item2))
                    {
                        serviceInstance = registeredMixedRealityServices[i].Item2;
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Gets all services by type.
        /// </summary>
        /// <param name="interfaceType">The interface type to search for.</param>
        /// <param name="services">Memory reference value of the service list to update.</param>
        private static void GetAllServicesInternal(Type interfaceType, ref List<IMixedRealityService> services)
        {
            GetAllServicesByNameInternal(interfaceType, string.Empty, ref services);
        }

        /// <summary>
        /// Gets all services by type and name.
        /// </summary>
        /// <param name="interfaceType">The interface type to search for.</param>
        /// <param name="serviceName">The name of the service to search for. If the string is empty than any matching <see cref="interfaceType"/> will be added to the <see cref="services"/> list.</param>
        /// <param name="services">Memory reference value of the service list to update.</param>
        private static void GetAllServicesByNameInternal(Type interfaceType, string serviceName, ref List<IMixedRealityService> services)
        {
            if (!CanGetService(interfaceType, serviceName)) { return; }

            if (IsCoreSystem(interfaceType))
            {
                IMixedRealityService serviceInstance;

                if (GetServiceByNameInternal(interfaceType, serviceName, out serviceInstance) &&
                    CheckServiceMatch(interfaceType, serviceName, interfaceType, serviceInstance))
                {
                    services.Add(serviceInstance);
                }
            }
            else
            {
                for (int i = 0; i < registeredMixedRealityServices.Count; i++)
                {
                    if (CheckServiceMatch(interfaceType, serviceName, registeredMixedRealityServices[i].Item1, registeredMixedRealityServices[i].Item2))
                    {
                        services.Add(registeredMixedRealityServices[i].Item2);
                    }
                }
            }
        }

        /// <summary>
        /// Check if the interface type and name matches the registered interface type and service instance found.
        /// </summary>
        /// <param name="interfaceType">The interface type of the service to check.</param>
        /// <param name="serviceName">The name of the service to check.</param>
        /// <param name="registeredInterfaceType">The registered interface type.</param>
        /// <param name="serviceInstance">The instance of the registered service.</param>
        /// <returns>True, if the registered service contains the interface type and name.</returns>
        private static bool CheckServiceMatch(Type interfaceType, string serviceName, Type registeredInterfaceType, IMixedRealityService serviceInstance)
        {
            bool isValid = string.IsNullOrEmpty(serviceName) || serviceInstance.Name == serviceName;

            if ((registeredInterfaceType.Name == interfaceType.Name || serviceInstance.GetType().Name == interfaceType.Name) && isValid)
            {
                return true;
            }

            var interfaces = serviceInstance.GetType().GetInterfaces();

            for (int i = 0; i < interfaces.Length; i++)
            {
                if (interfaces[i].Name == interfaceType.Name && isValid)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Checks if the system is ready to get a service.
        /// </summary>
        /// <param name="interfaceType"></param>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        private static bool CanGetService(Type interfaceType, string serviceName)
        {
            if (isApplicationQuitting)
            {
                return false;
            }

            if (!IsInitialized)
            {
                Debug.LogError("The Mixed Reality Toolkit has not been initialized!");
                return false;
            }

            if (interfaceType == null)
            {
                Debug.LogError($"{serviceName} interface type is null.");
                return false;
            }

            if (!typeof(IMixedRealityService).IsAssignableFrom(interfaceType))
            {
                Debug.LogError($"{interfaceType.Name} does not implement {typeof(IMixedRealityService).Name}.");
                return false;
            }

            return true;
        }

        #endregion Service Utilities

        #endregion Service Container Management

        #region Core System Accessors

        private static IMixedRealityInputSystem inputSystem = null;

        /// <summary>
        /// The current Input System registered with the Mixed Reality Toolkit.
        /// </summary>
        public static IMixedRealityInputSystem InputSystem
        {
            get
            {
                if (isApplicationQuitting)
                {
                    return null;
                }

                if (inputSystem != null)
                {
                    return inputSystem;
                }

                inputSystem = Instance.GetService<IMixedRealityInputSystem>(logInputSystem);
                // If we found a valid system, then we turn logging back on for the next time we need to search.
                // If we didn't find a valid system, then we stop logging so we don't spam the debug window.
                logInputSystem = inputSystem != null;
                return inputSystem;
            }
        }

        private static bool logInputSystem = true;

        private static IMixedRealityBoundarySystem boundarySystem = null;

        /// <summary>
        /// The current Boundary System registered with the Mixed Reality Toolkit.
        /// </summary>
        public static IMixedRealityBoundarySystem BoundarySystem
        {
            get
            {
                if (isApplicationQuitting)
                {
                    return null;
                }

                if (boundarySystem != null)
                {
                    return boundarySystem;
                }

                boundarySystem = Instance.GetService<IMixedRealityBoundarySystem>(logBoundarySystem);
                // If we found a valid system, then we turn logging back on for the next time we need to search.
                // If we didn't find a valid system, then we stop logging so we don't spam the debug window.
                logBoundarySystem = boundarySystem != null;
                return boundarySystem;
            }
        }

        private static bool logBoundarySystem = true;

        private static IMixedRealitySpatialAwarenessSystem spatialAwarenessSystem = null;

        /// <summary>
        /// The current Spatial Awareness System registered with the Mixed Reality Toolkit.
        /// </summary>
        public static IMixedRealitySpatialAwarenessSystem SpatialAwarenessSystem
        {
            get
            {
                if (isApplicationQuitting)
                {
                    return null;
                }

                if (spatialAwarenessSystem != null)
                {
                    return spatialAwarenessSystem;
                }

                spatialAwarenessSystem = Instance.GetService<IMixedRealitySpatialAwarenessSystem>(logSpatialAwarenessSystem);
                // If we found a valid system, then we turn logging back on for the next time we need to search.
                // If we didn't find a valid system, then we stop logging so we don't spam the debug window.
                logSpatialAwarenessSystem = spatialAwarenessSystem != null;
                return spatialAwarenessSystem;
            }
        }

        private static bool logSpatialAwarenessSystem = true;

        private static IMixedRealityTeleportSystem teleportSystem = null;

        /// <summary>
        /// The current Teleport System registered with the Mixed Reality Toolkit.
        /// </summary>
        public static IMixedRealityTeleportSystem TeleportSystem
        {
            get
            {
                if (isApplicationQuitting)
                {
                    return null;
                }

                if (teleportSystem != null)
                {
                    return teleportSystem;
                }

                teleportSystem = Instance.GetService<IMixedRealityTeleportSystem>(logTeleportSystem);
                // If we found a valid system, then we turn logging back on for the next time we need to search.
                // If we didn't find a valid system, then we stop logging so we don't spam the debug window.
                logTeleportSystem = teleportSystem != null;
                return teleportSystem;
            }
        }

        private static bool logTeleportSystem = true;

        private static IMixedRealityDiagnosticsSystem diagnosticsSystem = null;

        /// <summary>
        /// The current Diagnostics System registered with the Mixed Reality Toolkit.
        /// </summary>
        public static IMixedRealityDiagnosticsSystem DiagnosticsSystem
        {
            get
            {
                if (isApplicationQuitting)
                {
                    return null;
                }

                if (diagnosticsSystem != null)
                {
                    return diagnosticsSystem;
                }

                diagnosticsSystem = Instance.GetService<IMixedRealityDiagnosticsSystem>(logDiagnosticsSystem);
                // If we found a valid system, then we turn logging back on for the next time we need to search.
                // If we didn't find a valid system, then we stop logging so we don't spam the debug window.
                logDiagnosticsSystem = diagnosticsSystem != null;
                return diagnosticsSystem;
            }
        }

        private static bool logDiagnosticsSystem = true;

        #endregion Core System Accessors
    }
}
