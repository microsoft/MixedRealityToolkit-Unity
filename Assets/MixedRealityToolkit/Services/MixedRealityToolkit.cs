// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Boundary;
using Microsoft.MixedReality.Toolkit.Diagnostics;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.SpatialAwareness;
using Microsoft.MixedReality.Toolkit.Teleport;
using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

#if UNITY_EDITOR
using Microsoft.MixedReality.Toolkit.Input.Editor;
#endif

namespace Microsoft.MixedReality.Toolkit
{
    /// <summary>
    /// This class is responsible for coordinating the operation of the Mixed Reality Toolkit. It is the only Singleton in the entire project.
    /// It provides a service registry for all active services that are used within a project as well as providing the active configuration profile for the project.
    /// The Profile can be swapped out at any time to meet the needs of your project.
    /// </summary>
    [DisallowMultipleComponent]
    public class MixedRealityToolkit : MonoBehaviour, IMixedRealityServiceRegistrar
    {
#region Mixed Reality Toolkit Profile configuration

        private const string MixedRealityPlayspaceName = "MixedRealityPlayspace";

        private static bool isInitializing = false;

        private static bool isApplicationQuitting = false;

        /// <summary>
        /// Checks if there is a valid instance of the MixedRealityToolkit, then checks if there is there a valid Active Profile.
        /// </summary>
        public bool HasActiveProfile
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

                return ActiveProfile != null;
            }
        }

        private bool HasProfileAndIsInitialized => activeProfile != null && IsInitialized;

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
                // Services are only enabled when playing.
                if (Application.IsPlaying(activeProfile))
                {
                    DisableAllServices();
                }
                DestroyAllServices();
            }

            activeProfile = profile;

            if (profile != null)
            {
                if (Application.IsPlaying(profile))
                {
                    DisableAllServices();
                }
                DestroyAllServices();
            }

            InitializeServiceLocator();

            if (profile != null && Application.IsPlaying(profile))
            {
                EnableAllServices();
            }
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
        public IReadOnlyDictionary<Type, IMixedRealityService> ActiveSystems => new Dictionary<Type, IMixedRealityService>(activeSystems) as IReadOnlyDictionary<Type, IMixedRealityService>;

        private static readonly List<Tuple<Type, IMixedRealityService>> registeredMixedRealityServices = new List<Tuple<Type, IMixedRealityService>>();

        /// <summary>
        /// Local service registry for the Mixed Reality Toolkit, to allow runtime use of the <see cref="Microsoft.MixedReality.Toolkit.IMixedRealityService"/>.
        /// </summary>
        public IReadOnlyList<Tuple<Type, IMixedRealityService>> RegisteredMixedRealityServices => new List<Tuple<Type, IMixedRealityService>>(registeredMixedRealityServices) as IReadOnlyList<Tuple<Type, IMixedRealityService>>;

#endregion Mixed Reality runtime service registry

#region IMixedRealityServiceRegistrar implementation

        /// <inheritdoc />
        public bool RegisterService<T>(T serviceInstance) where T : IMixedRealityService
        {
            return RegisterServiceInternal(serviceInstance);
        }

        /// <inheritdoc />
        public bool RegisterService<T>(
            Type concreteType, 
            SupportedPlatforms supportedPlatforms = (SupportedPlatforms)(-1), 
            params object[] args) where T : IMixedRealityService
        {
            if (isApplicationQuitting)
            {
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

            if (concreteType == null)
            {
                Debug.LogError("Unable to register a service with a null concrete type.");
                return false;
            }

            if (!typeof(IMixedRealityService).IsAssignableFrom(concreteType))
            {
                Debug.LogError($"Unable to register the {concreteType.Name} service. It does not implement {typeof(IMixedRealityService)}.");
                return false;
            }

            T serviceInstance;

            try
            {
                serviceInstance = (T)Activator.CreateInstance(concreteType, args);
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to register the {concreteType.Name} service: {e.GetType()} - {e.Message}");
                return false;
            }

            return RegisterServiceInternal<T>(serviceInstance);
        }

        /// <inheritdoc />
        public bool UnregisterService<T>(string name = null) where T : IMixedRealityService
        {
            T serviceInstance = GetServiceByName<T>(name);
            
            if (serviceInstance == null) { return false; }

            return UnregisterService<T>(serviceInstance);
        }

        /// <inheritdoc />
        public bool UnregisterService<T>(T serviceInstance) where T : IMixedRealityService
        {
            Type interfaceType = typeof(T);

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

            Tuple<Type, IMixedRealityService> registryInstance = new Tuple<Type, IMixedRealityService>(interfaceType, serviceInstance);

            if (registeredMixedRealityServices.Contains(registryInstance))
            {
                registeredMixedRealityServices.Remove(registryInstance);
                return true;
            }

            Debug.LogError($"Failed to find registry instance of {interfaceType.Name}.{serviceInstance.Name}!");

            return false;
        }

        /// <inheritdoc />
        public bool IsServiceRegistered<T>(string name = null) where T : IMixedRealityService
        {
            return GetService<T>(name) != null;
        }

        /// <inheritdoc />
        public T GetService<T>(string name = null, bool showLogs = true) where T : IMixedRealityService
        {
            Type interfaceType = typeof(T);
            T serviceInstance = GetServiceByName<T>(name);

            if ((serviceInstance == null) && showLogs)
            {
                Debug.LogError($"Unable to find {(string.IsNullOrWhiteSpace(name) ? interfaceType.Name : name)} service.");
            }

            return serviceInstance;
        }

        /// <inheritdoc />
        public IReadOnlyList<T> GetServices<T>(string name = null) where T : IMixedRealityService
        {
            return GetAllServicesByNameInternal<T>(typeof(T), name);
        }

        /// <inheritdoc />
        public bool RegisterDataProvider<T>(T dataProviderInstance) where T : IMixedRealityDataProvider
        {
            return RegisterService<T>(dataProviderInstance);
        }

        /// <inheritdoc />
        public bool RegisterDataProvider<T>(
            Type concreteType,
            SupportedPlatforms supportedPlatforms = (SupportedPlatforms)(-1),
            params object[] args) where T : IMixedRealityDataProvider
        {
            return RegisterService<T>(concreteType, supportedPlatforms, args);
        }

        /// <inheritdoc />
        public bool UnregisterDataProvider<T>(string name = null) where T : IMixedRealityDataProvider
        {
            return UnregisterService<T>(name);
        }

        /// <inheritdoc />
        public bool UnregisterDataProvider<T>(T dataProviderInstance) where T : IMixedRealityDataProvider
        {
            return UnregisterService<T>(dataProviderInstance);
        }

        /// <inheritdoc />
        public bool IsDataProviderRegistered<T>(string name = null) where T : IMixedRealityDataProvider
        {
            return IsServiceRegistered<T>(name);
        }

        /// <inheritdoc />
        public T GetDataProvider<T>(string name = null) where T : IMixedRealityDataProvider
        {
            return GetService<T>(name);
        }

        /// <inheritdoc />
        public IReadOnlyList<T> GetDataProviders<T>(string name = null) where T : IMixedRealityDataProvider
        {
            return GetServices<T>(name);
        }

        /// <inheritdoc />
        public IReadOnlyList<T> GetDataProviders<T>() where T : IMixedRealityDataProvider
        {
            throw new NotImplementedException();
        }

#endregion IMixedRealityServiceRegistrar implementation

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
                InputMappingAxisUtility.CheckUnityInputManagerMappings(ControllerMappingLibrary.UnityInputManagerAxes);
#endif

                object[] args = { this, ActiveProfile.InputSystemProfile, Instance.MixedRealityPlayspace };
                if (!RegisterService<IMixedRealityInputSystem>(ActiveProfile.InputSystemType, args: args) || InputSystem == null)
                {
                    Debug.LogError("Failed to start the Input System!");
                }
                
                args = new object[] { this, InputSystem, ActiveProfile.InputSystemProfile };
                if (!RegisterDataProvider<IMixedRealityFocusProvider>(ActiveProfile.InputSystemProfile.FocusProviderType, args: args))
                {
                    Debug.LogError("Failed to register the focus provider! The input system will not function without it.");
                    return;
                }
            }
            else
            {
#if UNITY_EDITOR
                InputMappingAxisUtility.RemoveMappings(ControllerMappingLibrary.UnityInputManagerAxes);
#endif
            }

            // If the Boundary system has been selected for initialization in the Active profile, enable it in the project
            if (ActiveProfile.IsBoundarySystemEnabled)
            {
                object[] args = { this, ActiveProfile.BoundaryVisualizationProfile, Instance.MixedRealityPlayspace, ActiveProfile.TargetExperienceScale };
                if (!RegisterService<IMixedRealityBoundarySystem>(ActiveProfile.BoundarySystemSystemType, args: args) || BoundarySystem == null)
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
                object[] args = { this, ActiveProfile.SpatialAwarenessSystemProfile };
                if (!RegisterService<IMixedRealitySpatialAwarenessSystem>(ActiveProfile.SpatialAwarenessSystemSystemType, args: args) && SpatialAwarenessSystem != null)
                {
                    Debug.LogError("Failed to start the Spatial Awareness System!");
                }
            }

            // If the Teleport system has been selected for initialization in the Active profile, enable it in the project
            if (ActiveProfile.IsTeleportSystemEnabled)
            {
                object[] args = { this, Instance.MixedRealityPlayspace };
                if (!RegisterService<IMixedRealityTeleportSystem>(ActiveProfile.TeleportSystemSystemType, args: args) || TeleportSystem == null)
                {
                    Debug.LogError("Failed to start the Teleport System!");
                }
            }

            if (ActiveProfile.IsDiagnosticsSystemEnabled)
            {
                object[] args = { this, ActiveProfile.DiagnosticsSystemProfile, Instance.MixedRealityPlayspace };
                if (!RegisterService<IMixedRealityDiagnosticsSystem>(ActiveProfile.DiagnosticsSystemSystemType, args: args) || DiagnosticsSystem == null)
                {
                    Debug.LogError("Failed to start the Diagnostics System!");
                }
            }

            if (ActiveProfile.RegisteredServiceProvidersProfile != null)
            {
                for (int i = 0; i < ActiveProfile.RegisteredServiceProvidersProfile?.Configurations?.Length; i++)
                {
                    var configuration = ActiveProfile.RegisteredServiceProvidersProfile.Configurations[i];

                    if (typeof(IMixedRealityExtensionService).IsAssignableFrom(configuration.ComponentType.Type))
                    {
                        object[] args = { this, configuration.ComponentName, configuration.Priority, configuration.ConfigurationProfile };
                        if (!RegisterService<IMixedRealityExtensionService>(configuration.ComponentType, configuration.RuntimePlatform, args))
                        {
                            Debug.LogError($"Failed to register {configuration.ComponentName}");
                        }
                    }
                }
            }

#endregion Service Registration

#region Services Initialization

            var orderedCoreSystems = activeSystems.OrderBy(m => m.Value.Priority).ToArray();
            activeSystems.Clear();

            foreach (var system in orderedCoreSystems)
            {
                RegisterServiceInternal(system.Key, system.Value);
            }

            var orderedServices = registeredMixedRealityServices.OrderBy(service => service.Item2.Priority).ToArray();
            registeredMixedRealityServices.Clear();

            foreach (var service in orderedServices)
            {
                RegisterServiceInternal(service.Item1, service.Item2);
            }

            InitializeAllServices();

#endregion Services Initialization

            isInitializing = false;
        }

        private void EnsureMixedRealityRequirements()
        {
            if (MixedRealityPlayspace == null)
            {
                Debug.LogError("Failed to generate a MixedRealityPlayspace.");
            }

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
        private static bool newInstanceBeingInitialized = false;

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
#if UNITY_EDITOR
                        // Generating a new instance during app shutdown in the editor allows instances to persist into edit mode, which may cause unwanted behavior
                        if (isApplicationQuitting)
                        {
                            Debug.LogWarning("Trying to find instance during application shutdown.");
                            return null;
                        }
#endif
                        Debug.Assert(!newInstanceBeingInitialized, "We shouldn't be initializing another MixedRealityToolkit unless we errored on the previous.");
                        newInstanceBeingInitialized = true;
                        newInstance = new GameObject(nameof(MixedRealityToolkit)).AddComponent<MixedRealityToolkit>();
                        break;
                    case 1:
                        newInstanceBeingInitialized = false;
                        newInstance = objects[0];
                        break;
                    default:
                        newInstanceBeingInitialized = false;
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
                    newInstanceBeingInitialized = false;
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
                if (IsInitialized)
                {
                    newInstanceBeingInitialized = false;
                    return;
                }

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

                newInstanceBeingInitialized = false;
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
            if (!newInstanceBeingInitialized && !IsInitialized && !UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode)
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

        private void LateUpdate()
        {
            LateUpdateAllServices();
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
        private bool RegisterServiceInternal(Type interfaceType, IMixedRealityService serviceInstance)
        {
            if (serviceInstance == null)
            {
                Debug.LogWarning($"Unable to add a {interfaceType.Name} service with a null instance.");
                return false;
            }

            if (!CanGetService(interfaceType)) { return false; }

            IMixedRealityService preExistingService = GetServiceByNameInternal(interfaceType, serviceInstance.Name);

            if (preExistingService != null)
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
        /// Internal service registration.
        /// </summary>
        /// <param name="interfaceType">The interface type for the system to be registered.</param>
        /// <param name="serviceInstance">Instance of the service.</param>
        /// <returns>True if registration is successful, false otherwise.</returns>
        private bool RegisterServiceInternal<T>(T serviceInstance) where T : IMixedRealityService
        {
            Type interfaceType = typeof(T);
            return RegisterServiceInternal(interfaceType, serviceInstance);
        }

#endregion Registration

#region Multiple Service Management

        /// <summary>
        /// Enable all services in the Mixed Reality Toolkit active service registry for a given type
        /// </summary>
        /// <param name="interfaceType">The interface type for the system to be enabled.  E.G. InputSystem, BoundarySystem</param>
        public void EnableAllServicesByType(Type interfaceType)
        {
            EnableAllServicesByTypeAndName(interfaceType, string.Empty);
        }

        /// <summary>
        /// Enable all services in the Mixed Reality Toolkit active service registry for a given type and name
        /// </summary>
        /// <param name="interfaceType">The interface type for the system to be enabled.  E.G. InputSystem, BoundarySystem</param>
        /// <param name="serviceName">Name of the specific service</param>
        public  void EnableAllServicesByTypeAndName(Type interfaceType, string serviceName)
        {
            if (interfaceType == null)
            {
                Debug.LogError("Unable to enable null service type.");
                return;
            }

            IReadOnlyList<IMixedRealityService> services = GetAllServicesByNameInternal<IMixedRealityService>(interfaceType, serviceName);
            for (int i = 0; i < services.Count; i++)
            {
                services[i].Enable();
            }
        }

        /// <summary>
        /// Disable all services in the Mixed Reality Toolkit active service registry for a given type
        /// </summary>
        /// <param name="interfaceType">The interface type for the system to be removed.  E.G. InputSystem, BoundarySystem</param>
        public void DisableAllServicesByType(Type interfaceType)
        {
            DisableAllServicesByTypeAndName(interfaceType, string.Empty);
        }

        /// <summary>
        /// Disable all services in the Mixed Reality Toolkit active service registry for a given type and name
        /// </summary>
        /// <param name="interfaceType">The interface type for the system to be disabled.  E.G. InputSystem, BoundarySystem</param>
        /// <param name="serviceName">Name of the specific service</param>
        public void DisableAllServicesByTypeAndName(Type interfaceType, string serviceName)
        {
            if (interfaceType == null)
            {
                Debug.LogError("Unable to disable null service type.");
                return;
            }

            IReadOnlyList<IMixedRealityService> services =  GetAllServicesByNameInternal<IMixedRealityService>(interfaceType, serviceName);
            for (int i = 0; i < services.Count; i++)
            {
                services[i].Disable();
            }
        }

        private void InitializeAllServices()
        {
            // Initialize all systems
            ExecuteOnAllServices(service => service.Initialize());
        }

        private void ResetAllServices()
        {
            // Reset all systems
            ExecuteOnAllServices(service => service.Reset());
        }

        private void EnableAllServices()
        {
            // Enable all systems
            ExecuteOnAllServices(service => service.Enable());
        }

        private void UpdateAllServices()
        {
            // Update all systems
            ExecuteOnAllServices(service => service.Update());
        }

        private void LateUpdateAllServices()
        {
            // If the Mixed Reality Toolkit is not configured, stop.
            if (activeProfile == null) { return; }

            // If the Mixed Reality Toolkit is not initialized, stop.
            if (!IsInitialized) { return; }

            // Update all systems
            foreach (var system in activeSystems)
            {
                system.Value.LateUpdate();
            }

            // Update all registered runtime services
            foreach (var service in registeredMixedRealityServices)
            {
                service.Item2.LateUpdate();
            }
        }

        private void DisableAllServices()
        {
            // Disable all systems
            ExecuteOnAllServices(service => service.Disable());
        }

        private void DestroyAllServices()
        {
            if (!ExecuteOnAllServices(service => service.Destroy())) { return; }

            activeSystems.Clear();
            registeredMixedRealityServices.Clear();
        }

        private bool ExecuteOnAllServices(Action<IMixedRealityService> execute)
        {
            // If the Mixed Reality Toolkit is not configured, stop.
            if (!HasProfileAndIsInitialized) { return false; }

            foreach (var system in activeSystems)
            {
                execute(system.Value);
            }

            foreach (var service in registeredMixedRealityServices)
            {
                execute(service.Item2);
            }

            return true;
        }

#endregion Multiple Service Management

#region Service Utilities

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

        private void ClearCoreSystemCache()
        {
            inputSystem = null;
            teleportSystem = null;
            boundarySystem = null;
            spatialAwarenessSystem = null;
            diagnosticsSystem = null;
        }

        private IMixedRealityService GetServiceByNameInternal(Type interfaceType, string serviceName)
        {
            if (!CanGetService(interfaceType)) { return null; }

            if (IsCoreSystem(interfaceType))
            {
                IMixedRealityService serviceInstance;
                if (activeSystems.TryGetValue(interfaceType, out serviceInstance))
                {
                    if (CheckServiceMatch(interfaceType, serviceName, interfaceType, serviceInstance))
                    {
                        return serviceInstance;
                    }
                }
            }
            else
            {
                for (int i = 0; i < registeredMixedRealityServices.Count; i++)
                {
                    if (CheckServiceMatch(interfaceType, serviceName, registeredMixedRealityServices[i].Item1, registeredMixedRealityServices[i].Item2))
                    {
                        return registeredMixedRealityServices[i].Item2;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Retrieve the first service from the registry that meets the selected type and name
        /// </summary>
        /// <param name="interfaceType">Interface type of the service being requested</param>
        /// <param name="serviceName">Name of the specific service</param>
        /// <param name="serviceInstance">return parameter of the function</param>
        private T GetServiceByName<T>(string serviceName) where T : IMixedRealityService
        {
            return (T)GetServiceByNameInternal(typeof(T), serviceName);
        }

        /// <summary>
        /// Gets all services by type and name.
        /// </summary>
        /// <param name="serviceName">The name of the service to search for. If the string is empty than any matching <see cref="interfaceType"/> will be added to the <see cref="services"/> list.</param>
        private IReadOnlyList<T> GetAllServicesByNameInternal<T>(Type interfaceType, string serviceName) where T : IMixedRealityService
        {
            List<T> services = new List<T>();

            if (!CanGetService(interfaceType)) { return new List<T>() as IReadOnlyList<T>; }

            if (IsCoreSystem(interfaceType))
            {
                IMixedRealityService serviceInstance = GetServiceByName<T>(serviceName);

                if ((serviceInstance != null) &&
                    CheckServiceMatch(interfaceType, serviceName, interfaceType, serviceInstance))
                {
                    services.Add((T)serviceInstance);
                }
            }
            else
            {
                for (int i = 0; i < registeredMixedRealityServices.Count; i++)
                {
                    if (CheckServiceMatch(interfaceType, serviceName, registeredMixedRealityServices[i].Item1, registeredMixedRealityServices[i].Item2))
                    {
                        services.Add((T)registeredMixedRealityServices[i].Item2);
                    }
                }
            }

            return services;
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
        /// <returns></returns>
        private static bool CanGetService(Type interfaceType)
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
                Debug.LogError($"Interface type is null.");
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

                inputSystem = Instance.GetService<IMixedRealityInputSystem>(showLogs: logInputSystem);
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

                boundarySystem = Instance.GetService<IMixedRealityBoundarySystem>(showLogs: logBoundarySystem);
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

                spatialAwarenessSystem = Instance.GetService<IMixedRealitySpatialAwarenessSystem>(showLogs: logSpatialAwarenessSystem);
                // If we found a valid system, then we turn logging back on for the next time we need to search.
                // If we didn't find a valid system, then we stop logging so we don't spam the debug window.
                logSpatialAwarenessSystem = spatialAwarenessSystem != null;
                return spatialAwarenessSystem;
            }
        }

        private static bool logSpatialAwarenessSystem = true;

        private static IMixedRealityTeleportSystem teleportSystem = null;

        /// <summary>
        /// Returns true if the MixedRealityToolkit exists and has an active profile that has Teleport system enabled.
        /// </summary>
        public static bool IsTeleportSystemEnabled => IsInitialized && Instance.HasActiveProfile && Instance.ActiveProfile.IsTeleportSystemEnabled;

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

                // Quiet warnings as we check for the service. If it's not available, it has probably been
                // disabled. We'll notify about that, in case it's an accident, but otherwise remain calm about it.
                teleportSystem = Instance.GetService<IMixedRealityTeleportSystem>(showLogs: false);
                if (logTeleportSystem && (teleportSystem == null))
                {
                    Debug.LogWarning("IMixedRealityTeleportSystem service is disabled. Teleport will not be available.\nCheck MRTK Configuration Profile settings if this is unexpected.");
                }
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

                diagnosticsSystem = Instance.GetService<IMixedRealityDiagnosticsSystem>(showLogs: logDiagnosticsSystem);
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
