// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Boundary;
using Microsoft.MixedReality.Toolkit.CameraSystem;
using Microsoft.MixedReality.Toolkit.Diagnostics;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Rendering;
using Microsoft.MixedReality.Toolkit.SceneSystem;
using Microsoft.MixedReality.Toolkit.SpatialAwareness;
using Microsoft.MixedReality.Toolkit.Teleport;
using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.EventSystems;

#if UNITY_EDITOR
using Microsoft.MixedReality.Toolkit.Input.Editor;
using UnityEditor;
#endif

namespace Microsoft.MixedReality.Toolkit
{
    /// <summary>
    /// This class is responsible for coordinating the operation of the Mixed Reality Toolkit. It is the only Singleton in the entire project.
    /// It provides a service registry for all active services that are used within a project as well as providing the active configuration profile for the project.
    /// The Profile can be swapped out at any time to meet the needs of your project.
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu("Scripts/MRTK/Core/MixedRealityToolkit")]
    public class MixedRealityToolkit : MonoBehaviour, IMixedRealityServiceRegistrar
    {
        private static bool isInitializing = false;
        private static bool isApplicationQuitting = false;
        private static bool internalShutdown = false;
        private const string NoMRTKProfileErrorMessage = "No Mixed Reality Configuration Profile found, cannot initialize the Mixed Reality Toolkit";

        /// <summary>
        /// Whether an active profile switching is currently in progress
        /// </summary>
        public bool IsProfileSwitching { get; private set; }

        #region Mixed Reality Toolkit Profile configuration

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

                return ActiveProfile != null;
            }
        }

        /// <summary>
        /// Returns true if this is the active instance.
        /// </summary>
        public bool IsActiveInstance
        {
            get
            {
                return activeInstance == this;
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
        /// <remarks>
        /// <para>If changing the Active profile prior to the initialization (i.e. Awake()) of <see cref="MixedRealityToolkit"/> is desired, 
        /// call the static function <see cref="SetProfileBeforeInitialization(MixedRealityToolkitConfigurationProfile)"/> instead.</para>
        /// <para>When setting the ActiveProfile during runtime, the destroy of the currently running services will happen after the last LateUpdate()
        /// of all services, and the instantiation and initialization of the services associated with the new profile will happen before the
        /// first Update() of all services.</para>
        /// <para>A noticeable application hesitation may occur during this process. Also any script with higher priority than this can enter its Update
        /// before the new profile is properly setup.</para>
        /// <para>You are strongly recommended to see 
        /// <see href="https://docs.microsoft.com/windows/mixed-reality/mrtk-unity/configuration/mixed-reality-configuration-guide#changing-profiles-at-runtime">here</see> 
        /// for more information on profile switching.</para>
        /// </remarks>
        public MixedRealityToolkitConfigurationProfile ActiveProfile
        {
            get
            {
                return activeProfile;
            }
            set
            {
                // Behavior during a valid runtime profile switch
                if (Application.isPlaying && activeProfile != null && value != null)
                {
                    newProfile = value;
                }
                // Behavior in other scenarios (e.g. when profile switch is being requested by editor code)
                else
                {
                    ResetConfiguration(value);
                }

            }
        }

        /// <summary>
        /// Set the active profile prior to the initialization (i.e. Awake()) of <see cref="MixedRealityToolkit"/>
        /// </summary>
        /// <remarks>
        /// <para>If changing the Active profile after <see cref="MixedRealityToolkit"/> has been initialized, modify <see cref="ActiveProfile"/> of the active instance directly.</para>
        /// <para>This function requires the caller script to be executed earlier than the <see cref="MixedRealityToolkit"/> script, which can be achieved by setting 
        /// <see href="https://docs.unity3d.com/Manual/class-MonoManager.html">Script Execution Order settings</see>.</para>
        /// <para>You are strongly recommended to see 
        /// <see href="https://docs.microsoft.com/windows/mixed-reality/mrtk-unity/configuration/mixed-reality-configuration-guide#changing-profiles-at-runtime">here</see> 
        /// for more information on profile switching.</para>
        /// </remarks>
        public static void SetProfileBeforeInitialization(MixedRealityToolkitConfigurationProfile profile)
        {
            MixedRealityToolkit toolkit = FindObjectOfType<MixedRealityToolkit>();
            toolkit.activeProfile = profile;
        }

        /// <summary>
        /// When a configuration Profile is replaced with a new configuration, force all services to reset and read the new values
        /// </summary>
        /// <remarks>
        /// <para>This function should only be used by editor code in most cases.</para>
        /// <para>Do not call this function if resetting profile at runtime.
        /// Instead see 
        /// <see href="https://docs.microsoft.com/windows/mixed-reality/mrtk-unity/configuration/mixed-reality-configuration-guide#changing-profiles-at-runtime">here</see> 
        /// for more information on profile switching at runtime.</para>
        /// </remarks>
        public void ResetConfiguration(MixedRealityToolkitConfigurationProfile profile)
        {
            RemoveCurrentProfile(profile);
            InitializeNewProfile(profile);
        }

        private void InitializeNewProfile(MixedRealityToolkitConfigurationProfile profile)
        {
            InitializeServiceLocator();

            if (profile != null && Application.IsPlaying(profile))
            {
                EnableAllServices();
            }
        }

        private void RemoveCurrentProfile(MixedRealityToolkitConfigurationProfile profile)
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
        }

        private MixedRealityToolkitConfigurationProfile newProfile;

        #endregion Mixed Reality Toolkit Profile configuration

        #region Mixed Reality runtime service registry

        private static readonly Dictionary<Type, IMixedRealityService> activeSystems = new Dictionary<Type, IMixedRealityService>();

        /// <summary>
        /// Current active systems registered with the MixedRealityToolkit.
        /// </summary>
        /// <remarks>
        /// Systems can only be registered once by <see cref="System.Type"/>
        /// </remarks>
        [Obsolete("Use CoreService, MixedRealityServiceRegistry, or GetService<T> instead")]
        public IReadOnlyDictionary<Type, IMixedRealityService> ActiveSystems => new Dictionary<Type, IMixedRealityService>(activeSystems) as IReadOnlyDictionary<Type, IMixedRealityService>;

        private static readonly List<Tuple<Type, IMixedRealityService>> registeredMixedRealityServices = new List<Tuple<Type, IMixedRealityService>>();

        /// <summary>
        /// Local service registry for the Mixed Reality Toolkit, to allow runtime use of the <see cref="Microsoft.MixedReality.Toolkit.IMixedRealityService"/>.
        /// </summary>
        [Obsolete("Use GetDataProvider<T> of MixedRealityService registering the desired IMixedRealityDataProvider")]
        public IReadOnlyList<Tuple<Type, IMixedRealityService>> RegisteredMixedRealityServices => new List<Tuple<Type, IMixedRealityService>>(registeredMixedRealityServices) as IReadOnlyList<Tuple<Type, IMixedRealityService>>;

        #endregion Mixed Reality runtime service registry

        #region IMixedRealityServiceRegistrar implementation

        /// <inheritdoc />
        public bool RegisterService<T>(T serviceInstance) where T : IMixedRealityService
        {
            return RegisterServiceInternal<T>(serviceInstance);
        }

        /// <inheritdoc />
        public bool RegisterService<T>(
            Type concreteType,
            SupportedPlatforms supportedPlatforms = (SupportedPlatforms)(-1),
            params object[] args) where T : IMixedRealityService
        {
            return RegisterServiceInternal<T>(
                true, // Retry with an added IMixedRealityService parameter
                concreteType,
                supportedPlatforms,
                args);
        }

        /// <summary>
        /// Internal method that creates an instance of the specified concrete type and registers the service.
        /// </summary>
        private bool RegisterServiceInternal<T>(
            bool retryWithRegistrar,
            Type concreteType,
            SupportedPlatforms supportedPlatforms = (SupportedPlatforms)(-1),
            params object[] args) where T : IMixedRealityService
        {
            DebugUtilities.LogVerboseFormat("Attempting to register service of type: {0}", concreteType);

            if (isApplicationQuitting)
            {
                return false;
            }

            if (!PlatformUtility.IsPlatformSupported(supportedPlatforms))
            {
                DebugUtilities.LogVerboseFormat("Service of type: {0} does not support the current platform given supported platforms {1}", concreteType, supportedPlatforms);
                return false;
            }

            if (concreteType == null)
            {
                Debug.LogError($"Unable to register {typeof(T).Name} service because the value of concreteType is null.\n" +
                    "This may be caused by code being stripped during linking. The link.xml file in the MixedRealityToolkit.Generated folder is used to control code preservation.\n" +
                    "More information can be found at https://docs.unity3d.com/Manual/ManagedCodeStripping.html.");
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
                if (retryWithRegistrar && (e is MissingMethodException))
                {
                    Debug.LogWarning($"Failed to find an appropriate constructor for the {concreteType.Name} service. Adding the Registrar instance and re-attempting registration.");
                    List<object> updatedArgs = new List<object>();
                    updatedArgs.Add(this);
                    if (args != null)
                    {
                        updatedArgs.AddRange(args);
                    }
                    return RegisterServiceInternal<T>(
                        false, // Do NOT retry, we have already added the configured IMIxedRealityServiceRegistrar
                        concreteType,
                        supportedPlatforms,
                        updatedArgs.ToArray());
                }

                Debug.LogError($"Failed to register the {concreteType.Name} service: {e.GetType()} - {e.Message}");

                // Failures to create the concrete type generally surface as nested exceptions - just logging
                // the top level exception itself may not be helpful. If there is a nested exception (for example,
                // null reference in the constructor of the object itself), it's helpful to also surface those here.
                if (e.InnerException != null)
                {
                    Debug.LogError("Underlying exception information: " + e.InnerException);
                }
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
            DebugUtilities.LogVerboseFormat("Unregistering service of type: {0}", typeof(T));

            Type interfaceType = typeof(T);

            if (IsInitialized)
            {
                DebugUtilities.LogVerboseFormat("Unregistered service of type {0} was an initialized service, disabling and destroying it", typeof(T));
                serviceInstance.Disable();
                serviceInstance.Destroy();
            }

            if (IsCoreSystem(interfaceType))
            {
                DebugUtilities.LogVerboseFormat("Unregistered service of type {0} was a core system", typeof(T));
                activeSystems.Remove(interfaceType);

                CoreServices.ResetCacheReference(interfaceType);
                return true;
            }

            return MixedRealityServiceRegistry.RemoveService<T>(serviceInstance, this);
        }

        /// <inheritdoc />
        public bool IsServiceRegistered<T>(string name = null) where T : IMixedRealityService
        {
            Type interfaceType = typeof(T);
            if (typeof(IMixedRealityDataProvider).IsAssignableFrom(interfaceType))
            {
                Debug.LogWarning($"Unable to check a service of type {typeof(IMixedRealityDataProvider).Name}. Inquire with the MixedRealityService that registered the DataProvider type in question");
                return false;
            }

            T service;
            MixedRealityServiceRegistry.TryGetService<T>(out service, name);
            return service != null;
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

        #endregion IMixedRealityServiceRegistrar implementation

        /// <summary>
        /// Once all services are registered and properties updated, the Mixed Reality Toolkit will initialize all active services.
        /// This ensures all services can reference each other once started.
        /// </summary>
        private void InitializeServiceLocator()
        {
            isInitializing = true;

            // If the Mixed Reality Toolkit is not configured, stop.
            if (ActiveProfile == null)
            {
                if (!Application.isPlaying)
                {
                    // Log as warning if in edit mode. Likely user is making changes etc.
                    Debug.LogWarning(NoMRTKProfileErrorMessage);
                }
                else
                {
                    Debug.LogError(NoMRTKProfileErrorMessage);
                }

                return;
            }

            // If verbose logging is to be enabled, this should be done as early in service
            // initialization as possible to allow for other services to use verbose logging
            // as they initialize.
            DebugUtilities.LogLevel = activeProfile.EnableVerboseLogging ? DebugUtilities.LoggingLevel.Verbose : DebugUtilities.LoggingLevel.Information;

#if UNITY_EDITOR
            if (activeSystems.Count > 0)
            {
                activeSystems.Clear();
            }

            if (registeredMixedRealityServices.Count > 0)
            {
                registeredMixedRealityServices.Clear();
            }

            EnsureEditorSetup();
#endif

            CoreServices.ResetCacheReferences();
            EnsureMixedRealityRequirements();

            #region Services Registration

            // If the Input system has been selected for initialization in the Active profile, enable it in the project
            if (ActiveProfile.IsInputSystemEnabled)
            {
                DebugUtilities.LogVerbose("Begin registration of the input system");

#if UNITY_EDITOR
                // Make sure unity axis mappings are set.	
                InputMappingAxisUtility.CheckUnityInputManagerMappings(ControllerMappingLibrary.UnityInputManagerAxes);
#endif

                object[] args = { ActiveProfile.InputSystemProfile };
                if (!RegisterService<IMixedRealityInputSystem>(ActiveProfile.InputSystemType, args: args) || CoreServices.InputSystem == null)
                {
                    Debug.LogError("Failed to start the Input System!");
                }

                args = new object[] { ActiveProfile.InputSystemProfile };
                if (!RegisterService<IMixedRealityFocusProvider>(ActiveProfile.InputSystemProfile.FocusProviderType, args: args))
                {
                    Debug.LogError("Failed to register the focus provider! The input system will not function without it.");
                    return;
                }

                args = new object[] { ActiveProfile.InputSystemProfile };
                if (!RegisterService<IMixedRealityRaycastProvider>(ActiveProfile.InputSystemProfile.RaycastProviderType, args: args))
                {
                    Debug.LogError("Failed to register the raycast provider! The input system will not function without it.");
                    return;
                }

                DebugUtilities.LogVerbose("End registration of the input system");
            }
            else
            {
#if UNITY_EDITOR
                InputMappingAxisUtility.RemoveMappings(ControllerMappingLibrary.UnityInputManagerAxes);
#endif
            }

            // If the Boundary system has been selected for initialization in the Active profile, enable it in the project
            if (ActiveProfile.IsBoundarySystemEnabled && ActiveProfile.ExperienceSettingsProfile != null)
            {
                DebugUtilities.LogVerbose("Begin registration of the boundary system");
                object[] args = { ActiveProfile.BoundaryVisualizationProfile, ActiveProfile.ExperienceSettingsProfile.TargetExperienceScale };
                if (!RegisterService<IMixedRealityBoundarySystem>(ActiveProfile.BoundarySystemSystemType, args: args) || CoreServices.BoundarySystem == null)
                {
                    Debug.LogError("Failed to start the Boundary System!");
                }
                DebugUtilities.LogVerbose("End registration of the boundary system");
            }

            // If the Camera system has been selected for initialization in the Active profile, enable it in the project
            if (ActiveProfile.IsCameraSystemEnabled)
            {
                DebugUtilities.LogVerbose("Begin registration of the camera system");
                object[] args = { ActiveProfile.CameraProfile };
                if (!RegisterService<IMixedRealityCameraSystem>(ActiveProfile.CameraSystemType, args: args) || CoreServices.CameraSystem == null)
                {
                    Debug.LogError("Failed to start the Camera System!");
                }
                DebugUtilities.LogVerbose("End registration of the camera system");
            }

            // If the Spatial Awareness system has been selected for initialization in the Active profile, enable it in the project
            if (ActiveProfile.IsSpatialAwarenessSystemEnabled)
            {
                DebugUtilities.LogVerbose("Begin registration of the spatial awareness system");
                object[] args = { ActiveProfile.SpatialAwarenessSystemProfile };
                if (!RegisterService<IMixedRealitySpatialAwarenessSystem>(ActiveProfile.SpatialAwarenessSystemSystemType, args: args) && CoreServices.SpatialAwarenessSystem != null)
                {
                    Debug.LogError("Failed to start the Spatial Awareness System!");
                }
                DebugUtilities.LogVerbose("End registration of the spatial awareness system");
            }

            // If the Teleport system has been selected for initialization in the Active profile, enable it in the project
            if (ActiveProfile.IsTeleportSystemEnabled)
            {
                DebugUtilities.LogVerbose("Begin registration of the teleport system");
                if (!RegisterService<IMixedRealityTeleportSystem>(ActiveProfile.TeleportSystemSystemType) || CoreServices.TeleportSystem == null)
                {
                    Debug.LogError("Failed to start the Teleport System!");
                }
                DebugUtilities.LogVerbose("End registration of the teleport system");
            }

            if (ActiveProfile.IsDiagnosticsSystemEnabled)
            {
                DebugUtilities.LogVerbose("Begin registration of the diagnostic system");
                object[] args = { ActiveProfile.DiagnosticsSystemProfile };
                if (!RegisterService<IMixedRealityDiagnosticsSystem>(ActiveProfile.DiagnosticsSystemSystemType, args: args) || CoreServices.DiagnosticsSystem == null)
                {
                    Debug.LogError("Failed to start the Diagnostics System!");
                }
                DebugUtilities.LogVerbose("End registration of the diagnostic system");
            }

            if (ActiveProfile.IsSceneSystemEnabled)
            {
                DebugUtilities.LogVerbose("Begin registration of the scene system");
                object[] args = { ActiveProfile.SceneSystemProfile };
                if (!RegisterService<IMixedRealitySceneSystem>(ActiveProfile.SceneSystemSystemType, args: args) || CoreServices.SceneSystem == null)
                {
                    Debug.LogError("Failed to start the Scene System!");
                }
                DebugUtilities.LogVerbose("End registration of the scene system");
            }

            if (ActiveProfile.RegisteredServiceProvidersProfile != null)
            {
                for (int i = 0; i < ActiveProfile.RegisteredServiceProvidersProfile.Configurations?.Length; i++)
                {
                    var configuration = ActiveProfile.RegisteredServiceProvidersProfile.Configurations[i];

                    if (typeof(IMixedRealityExtensionService).IsAssignableFrom(configuration.ComponentType.Type))
                    {
                        object[] args = { configuration.ComponentName, configuration.Priority, configuration.Profile };
                        RegisterService<IMixedRealityExtensionService>(configuration.ComponentType, configuration.RuntimePlatform, args);
                    }
                }
            }

            #endregion Service Registration

            InitializeAllServices();

            isInitializing = false;
        }

        private void EnsureEditorSetup()
        {
            if (ActiveProfile.RenderDepthBuffer)
            {
                CameraCache.Main.gameObject.EnsureComponent<DepthBufferRenderer>();
                DebugUtilities.LogVerbose("Added a DepthBufferRenderer to the main camera");
            }
        }

        private void EnsureMixedRealityRequirements()
        {
            // There's lots of documented cases that if the camera doesn't start at 0,0,0, things break with the WMR SDK specifically.
            // We'll enforce that here, then tracking can update it to the appropriate position later.
            CameraCache.Main.transform.position = Vector3.zero;
            CameraCache.Main.transform.rotation = Quaternion.identity;

            // This will create the playspace
            _ = MixedRealityPlayspace.Transform;

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
                DebugUtilities.LogVerbose("Added an EventSystem to the main camera");
            }
        }

        #region MonoBehaviour Implementation

        private static MixedRealityToolkit activeInstance;
        private static bool newInstanceBeingInitialized = false;

#if UNITY_EDITOR
        /// <summary>
        /// Returns the Singleton instance of the classes type.
        /// </summary>
        public static MixedRealityToolkit Instance
        {
            get
            {
                if (activeInstance != null)
                {
                    return activeInstance;
                }

                // It's possible for MRTK to exist in the scene but for activeInstance to be
                // null when a custom editor component accesses Instance before the MRTK
                // object has clicked on in object hierarchy (see https://github.com/microsoft/MixedRealityToolkit-Unity/pull/4618)
                //
                // To avoid returning null in this case, make sure to search the scene for MRTK.
                // We do this only when in editor to avoid any performance cost at runtime.
                List<MixedRealityToolkit> mrtks = new List<MixedRealityToolkit>(FindObjectsOfType<MixedRealityToolkit>());
                // Sort the list by instance ID so we get deterministic results when selecting our next active instance
                mrtks.Sort(delegate (MixedRealityToolkit i1, MixedRealityToolkit i2) { return i1.GetInstanceID().CompareTo(i2.GetInstanceID()); });

                for (int i = 0; i < mrtks.Count; i++)
                {
                    RegisterInstance(mrtks[i]);
                }
                return activeInstance;
            }
        }
#else
        /// <summary>
        /// Returns the Singleton instance of the classes type.
        /// </summary>
        public static MixedRealityToolkit Instance => activeInstance;
#endif

        private void InitializeInstance()
        {
            if (newInstanceBeingInitialized)
            {
                return;
            }

            newInstanceBeingInitialized = true;

            gameObject.SetActive(true);

            if (HasActiveProfile)
            {
                InitializeServiceLocator();
            }

            newInstanceBeingInitialized = false;
        }

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
        public static bool IsInitialized => activeInstance != null;

        /// <summary>
        /// Static function to determine if the MixedRealityToolkit class has been initialized or not.
        /// </summary>
        public static bool ConfirmInitialized()
        {
            // Calling the Instance property performs important initialization work.
            _ = Instance;
            return IsInitialized;
        }

        private void Awake()
        {
            RegisterInstance(this);
        }

        private void OnEnable()
        {
            if (IsActiveInstance)
            {
                EnableAllServices();
            }
        }

        private void Update()
        {
            if (IsActiveInstance)
            {
                // Before any Update() of a service is performed check to see if we need to switch profile
                // If so we instantiate and initialize the services associated with the new profile.
                if (newProfile != null && IsProfileSwitching)
                {
                    InitializeNewProfile(newProfile);
                    newProfile = null;
                    IsProfileSwitching = false;
                }
                UpdateAllServices();
            }
        }

        private void LateUpdate()
        {
            if (IsActiveInstance)
            {
                LateUpdateAllServices();
                // After LateUpdate()s of all services are finished check to see if we need to switch profile
                // If so we destroy currently running services.
                if (newProfile != null)
                {
                    IsProfileSwitching = true;
                    RemoveCurrentProfile(newProfile);
                }
            }
        }

        private void OnDisable()
        {
            if (IsActiveInstance)
            {
                DisableAllServices();
            }
        }

        private void OnDestroy()
        {
            UnregisterInstance(this);
        }

        #endregion MonoBehaviour Implementation

        #region Instance Registration

        private const string activeInstanceGameObjectName = "MixedRealityToolkit";
        private const string inactiveInstanceGameObjectName = "MixedRealityToolkit (Inactive)";
        private static List<MixedRealityToolkit> toolkitInstances = new List<MixedRealityToolkit>();

        public static void SetActiveInstance(MixedRealityToolkit toolkitInstance)
        {
            if (MixedRealityToolkit.isApplicationQuitting)
            {   // Don't register instances while application is quitting
                return;
            }

            if (toolkitInstance == activeInstance)
            {   // Don't do anything
                return;
            }

            // Disable the old instance
            SetInstanceInactive(activeInstance);

            // Immediately register the new instance
            RegisterInstance(toolkitInstance, true);
        }

        private static void RegisterInstance(MixedRealityToolkit toolkitInstance, bool setAsActiveInstance = false)
        {
            if (MixedRealityToolkit.isApplicationQuitting || toolkitInstance == null)
            {   // Don't register instances while application is quitting
                return;
            }

            internalShutdown = false;

            if (!toolkitInstances.Contains(toolkitInstance))
            {   // If we're already registered, no need to proceed
                // Add to list
                toolkitInstances.Add(toolkitInstance);
                // Sort the list by instance ID so we get deterministic results when selecting our next active instance
                toolkitInstances.Sort(delegate (MixedRealityToolkit i1, MixedRealityToolkit i2) { return i1.GetInstanceID().CompareTo(i2.GetInstanceID()); });
            }

            if (activeInstance == null)
            {
                // If we don't have an active instance, either set this instance
                // to be the active instance if requested, or get the first valid remaining instance
                // in the list.
                if (setAsActiveInstance)
                {
                    activeInstance = toolkitInstance;
                }
                else
                {
                    for (int i = 0; i < toolkitInstances.Count; i++)
                    {
                        if (toolkitInstances[i] != null)
                        {
                            activeInstance = toolkitInstances[i];
                            break;
                        }
                    }
                }

                activeInstance.DestroyAllServices();
                activeInstance.InitializeInstance();
            }

            // Update instance's Name so it's clear who is the active instance
            for (int i = toolkitInstances.Count - 1; i >= 0; i--)
            {
                if (toolkitInstances[i] == null)
                {
                    toolkitInstances.RemoveAt(i);
                }
                else
                {
                    toolkitInstances[i].name = toolkitInstances[i].IsActiveInstance ? activeInstanceGameObjectName : inactiveInstanceGameObjectName;
                }
            }
        }

        private static void UnregisterInstance(MixedRealityToolkit toolkitInstance)
        {
            // We are shutting an instance down.
            internalShutdown = true;

            toolkitInstances.Remove(toolkitInstance);
            // Sort the list by instance ID so we get deterministic results when selecting our next active instance
            toolkitInstances.Sort(delegate (MixedRealityToolkit i1, MixedRealityToolkit i2) { return i1.GetInstanceID().CompareTo(i2.GetInstanceID()); });

            if (MixedRealityToolkit.activeInstance == toolkitInstance)
            {   // If this is the active instance, we need to break it down
                toolkitInstance.DestroyAllServices();
                CoreServices.ResetCacheReferences();

                // If this was the active instance, unregister the active instance
                MixedRealityToolkit.activeInstance = null;
                if (MixedRealityToolkit.isApplicationQuitting)
                {   // Don't search for additional instances if we're quitting
                    return;
                }

                for (int i = 0; i < toolkitInstances.Count; i++)
                {
                    if (toolkitInstances[i] == null)
                    {   // This may have been a mass-deletion - be wary of soon-to-be-unregistered instances
                        continue;
                    }
                    // Select the first available instance and register it immediately
                    RegisterInstance(toolkitInstances[i]);
                    break;
                }
            }
        }

        public static void SetInstanceInactive(MixedRealityToolkit toolkitInstance)
        {
            if (toolkitInstance == null)
            {   // Don't do anything.
                return;
            }

            if (toolkitInstance == activeInstance)
            {   // If this was the active instance, un-register the active instance
                // Break down all services
                if (Application.isPlaying)
                {
                    toolkitInstance.DisableAllServices();
                }

                toolkitInstance.DestroyAllServices();

                CoreServices.ResetCacheReferences();

                // If this was the active instance, unregister the active instance
                MixedRealityToolkit.activeInstance = null;
            }
        }

        #endregion Instance Registration

        #region Service Container Management

        #region Registration
        // NOTE: This method intentionally does not add to the registry. This is actually mostly a helper function for RegisterServiceInternal<T>.
        private bool RegisterServiceInternal(Type interfaceType, IMixedRealityService serviceInstance)
        {
            if (serviceInstance == null)
            {
                Debug.LogWarning($"Unable to register a {interfaceType.Name} service with a null instance.");
                return false;
            }

            if (typeof(IMixedRealityDataProvider).IsAssignableFrom(interfaceType))
            {
                Debug.LogWarning($"Unable to register a service of type {typeof(IMixedRealityDataProvider).Name}. Register this DataProvider with the MixedRealityService that depends on it.");
                return false;
            }

            if (!CanGetService(interfaceType))
            {
                return false;
            }

            IMixedRealityService preExistingService = GetServiceByNameInternal(interfaceType, serviceInstance.Name);

            if (preExistingService != null)
            {
                Debug.LogError($"There's already a {interfaceType.Name}.{preExistingService.Name} registered!");
                return false;
            }

            if (IsCoreSystem(interfaceType))
            {
                DebugUtilities.LogVerboseFormat("Added core service of type {0}", interfaceType);
                activeSystems.Add(interfaceType, serviceInstance);
            }

            if (!isInitializing)
            {
                serviceInstance.Initialize();
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
            if (RegisterServiceInternal(interfaceType, serviceInstance))
            {
                MixedRealityServiceRegistry.AddService<T>(serviceInstance, this);
                return true;
            }

            return false;
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
        public void EnableAllServicesByTypeAndName(Type interfaceType, string serviceName)
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

            IReadOnlyList<IMixedRealityService> services = GetAllServicesByNameInternal<IMixedRealityService>(interfaceType, serviceName);
            for (int i = 0; i < services.Count; i++)
            {
                services[i].Disable();
            }
        }

        private void InitializeAllServices()
        {
            // Initialize all systems
            DebugUtilities.LogVerboseFormat("Calling Initialize() on all services");
            ExecuteOnAllServicesInOrder(service => service.Initialize());
        }

        private void ResetAllServices()
        {
            // Reset all systems
            DebugUtilities.LogVerboseFormat("Calling Reset() on all services");
            ExecuteOnAllServicesInOrder(service => service.Reset());
        }

        private void EnableAllServices()
        {
            // Enable all systems
            DebugUtilities.LogVerboseFormat("Calling Enable() on all services");
            ExecuteOnAllServicesInOrder(service => service.Enable());
        }

        private static readonly ProfilerMarker UpdateAllServicesPerfMarker = new ProfilerMarker("[MRTK] MixedRealityToolkit.UpdateAllServices");

        private void UpdateAllServices()
        {
            using (UpdateAllServicesPerfMarker.Auto())
            {
                // Update all systems
                ExecuteOnAllServicesInOrder(service => service.Update());
            }
        }

        private static readonly ProfilerMarker LateUpdateAllServicesPerfMarker = new ProfilerMarker("[MRTK] MixedRealityToolkit.LateUpdateAllServices");

        private void LateUpdateAllServices()
        {
            using (LateUpdateAllServicesPerfMarker.Auto())
            {
                // If the Mixed Reality Toolkit is not configured, stop.
                if (activeProfile == null) { return; }

                // If the Mixed Reality Toolkit is not initialized, stop.
                if (!IsInitialized) { return; }

                // Update all systems
                ExecuteOnAllServicesInOrder(service => service.LateUpdate());
            }
        }

        private void DisableAllServices()
        {
            // Disable all systems
            DebugUtilities.LogVerboseFormat("Calling Disable() on all services");
            ExecuteOnAllServicesReverseOrder(service => service.Disable());
        }

        private void DestroyAllServices()
        {
            DebugUtilities.LogVerboseFormat("Destroying all services");

            // Unregister core services (active systems)
            // We need to destroy services in backwards order as those which are initialized 
            // later may rely on those which are initialized first.
            var orderedActiveSystems = activeSystems.OrderByDescending(m => m.Value.Priority);

            foreach (var service in orderedActiveSystems)
            {
                Type type = service.Key;

                if (typeof(IMixedRealityBoundarySystem).IsAssignableFrom(type))
                {
                    UnregisterService<IMixedRealityBoundarySystem>();
                }
                else if (typeof(IMixedRealityCameraSystem).IsAssignableFrom(type))
                {
                    UnregisterService<IMixedRealityCameraSystem>();
                }
                else if (typeof(IMixedRealityDiagnosticsSystem).IsAssignableFrom(type))
                {
                    UnregisterService<IMixedRealityDiagnosticsSystem>();
                }
                else if (typeof(IMixedRealityFocusProvider).IsAssignableFrom(type))
                {
                    UnregisterService<IMixedRealityFocusProvider>();
                }
                else if (typeof(IMixedRealityInputSystem).IsAssignableFrom(type))
                {
                    UnregisterService<IMixedRealityInputSystem>();
                }
                else if (typeof(IMixedRealitySpatialAwarenessSystem).IsAssignableFrom(type))
                {
                    UnregisterService<IMixedRealitySpatialAwarenessSystem>();
                }
                else if (typeof(IMixedRealityTeleportSystem).IsAssignableFrom(type))
                {
                    UnregisterService<IMixedRealityTeleportSystem>();
                }
            }

            activeSystems.Clear();
            CoreServices.ResetCacheReferences();
            MixedRealityServiceRegistry.ClearAllServices();
        }

        private static readonly ProfilerMarker ExecuteOnAllServicesInOrderPerfMarker = new ProfilerMarker("[MRTK] MixedRealityToolkit.ExecuteOnAllServicesInOrder");

        private bool ExecuteOnAllServicesInOrder(Action<IMixedRealityService> execute)
        {
            using (ExecuteOnAllServicesInOrderPerfMarker.Auto())
            {
                if (!HasProfileAndIsInitialized)
                {
                    return false;
                }

                var services = MixedRealityServiceRegistry.GetAllServices();
                int length = services.Count;
                for (int i = 0; i < length; i++)
                {
                    execute(services[i]);
                }

                return true;
            }
        }

        private static readonly ProfilerMarker ExecuteOnAllServicesReverseOrderPerfMarker = new ProfilerMarker("[MRTK] MixedRealityToolkit.ExecuteOnAllServicesReverseOrder");

        private bool ExecuteOnAllServicesReverseOrder(Action<IMixedRealityService> execute)
        {
            using (ExecuteOnAllServicesReverseOrderPerfMarker.Auto())
            {
                if (!HasProfileAndIsInitialized)
                {
                    return false;
                }

                var services = MixedRealityServiceRegistry.GetAllServices();
                int length = services.Count;

                for (int i = length - 1; i >= 0; i--)
                {
                    execute(services[i]);
                }

                return true;
            }
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
        [Obsolete("Use IsServiceRegistered instead")]
        public bool IsSystemRegistered<T>() where T : IMixedRealityService
        {
            if (!IsCoreSystem(typeof(T))) return false;

            T service;
            MixedRealityServiceRegistry.TryGetService<T>(out service);

            if (service == null)
            {
                IMixedRealityService activeSerivce;
                activeSystems.TryGetValue(typeof(T), out activeSerivce);
                return activeSerivce != null;
            }

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
                   typeof(IMixedRealityCameraSystem).IsAssignableFrom(type) ||
                   typeof(IMixedRealityFocusProvider).IsAssignableFrom(type) ||
                   typeof(IMixedRealityRaycastProvider).IsAssignableFrom(type) ||
                   typeof(IMixedRealityTeleportSystem).IsAssignableFrom(type) ||
                   typeof(IMixedRealityBoundarySystem).IsAssignableFrom(type) ||
                   typeof(IMixedRealitySpatialAwarenessSystem).IsAssignableFrom(type) ||
                   typeof(IMixedRealityDiagnosticsSystem).IsAssignableFrom(type) ||
                   typeof(IMixedRealitySceneSystem).IsAssignableFrom(type);
        }

        private IMixedRealityService GetServiceByNameInternal(Type interfaceType, string serviceName)
        {
            if (typeof(IMixedRealityDataProvider).IsAssignableFrom(interfaceType))
            {
                Debug.LogWarning($"Unable to get a service of type {typeof(IMixedRealityDataProvider).Name}.");
                return null;
            }

            if (!CanGetService(interfaceType)) { return null; }

            IMixedRealityService service;
            MixedRealityServiceRegistry.TryGetService(interfaceType, out service, out _, serviceName);
            if (service != null)
            {
                return service;
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

            bool isNullServiceName = string.IsNullOrEmpty(serviceName);
            var systems = MixedRealityServiceRegistry.GetAllServices();
            int length = systems.Count;
            for (int i = 0; i < length; i++)
            {
                IMixedRealityService service = systems[i];
                if (service is T serviceT && (isNullServiceName || service.Name == serviceName))
                {
                    services.Add(serviceT);
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
        /// <param name="interfaceType">The interface type of the service being checked.</param>
        private static bool CanGetService(Type interfaceType)
        {
            if (isApplicationQuitting && !internalShutdown)
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

        /// <summary>
        /// The current Input System registered with the Mixed Reality Toolkit.
        /// </summary>
        [Obsolete("Utilize CoreServices.InputSystem instead")]
        public static IMixedRealityInputSystem InputSystem
        {
            get
            {
                if (isApplicationQuitting)
                {
                    return null;
                }

                return CoreServices.InputSystem;
            }
        }

        /// <summary>
        /// The current Boundary System registered with the Mixed Reality Toolkit.
        /// </summary>
        [Obsolete("Utilize CoreServices.BoundarySystem instead")]
        public static IMixedRealityBoundarySystem BoundarySystem
        {
            get
            {
                if (isApplicationQuitting)
                {
                    return null;
                }

                return CoreServices.BoundarySystem;
            }
        }

        /// <summary>
        /// The current Camera System registered with the Mixed Reality Toolkit.
        /// </summary>
        [Obsolete("Utilize CoreServices.CameraSystem instead")]
        public static IMixedRealityCameraSystem CameraSystem
        {
            get
            {
                if (isApplicationQuitting)
                {
                    return null;
                }

                return CoreServices.CameraSystem;
            }
        }

        /// <summary>
        /// The current Spatial Awareness System registered with the Mixed Reality Toolkit.
        /// </summary>
        [Obsolete("Utilize CoreServices.SpatialAwarenessSystem instead")]
        public static IMixedRealitySpatialAwarenessSystem SpatialAwarenessSystem
        {
            get
            {
                if (isApplicationQuitting)
                {
                    return null;
                }

                return CoreServices.SpatialAwarenessSystem;
            }
        }

        /// <summary>
        /// Returns true if the MixedRealityToolkit exists and has an active profile that has Teleport system enabled.
        /// </summary>
        public static bool IsTeleportSystemEnabled => IsInitialized && Instance.HasActiveProfile && Instance.ActiveProfile.IsTeleportSystemEnabled;

        /// <summary>
        /// The current Teleport System registered with the Mixed Reality Toolkit.
        /// </summary>
        [Obsolete("Utilize CoreServices.TeleportSystem instead")]
        public static IMixedRealityTeleportSystem TeleportSystem
        {
            get
            {
                if (isApplicationQuitting)
                {
                    return null;
                }

                return CoreServices.TeleportSystem;
            }
        }

        /// <summary>
        /// The current Diagnostics System registered with the Mixed Reality Toolkit.
        /// </summary>
        [Obsolete("Utilize CoreServices.DiagnosticsSystem instead")]
        public static IMixedRealityDiagnosticsSystem DiagnosticsSystem
        {
            get
            {
                if (isApplicationQuitting)
                {
                    return null;
                }

                return CoreServices.DiagnosticsSystem;
            }
        }

        /// <summary>
        /// Returns true if the MixedRealityToolkit exists and has an active profile that has Scene system enabled.
        /// </summary>
        public static bool IsSceneSystemEnabled => IsInitialized && Instance.HasActiveProfile && Instance.ActiveProfile.IsSceneSystemEnabled;

        /// <summary>
        /// The current Scene System registered with the Mixed Reality Toolkit.
        /// </summary>
        [Obsolete("Utilize CoreServices.SceneSystem instead")]
        public static IMixedRealitySceneSystem SceneSystem
        {
            get
            {
                if (isApplicationQuitting)
                {
                    return null;
                }

                return CoreServices.SceneSystem;
            }
        }

        #endregion Core System Accessors

        #region Application Event Listeners
        /// <summary>
        /// Registers once on startup and sets isApplicationQuitting to true when quit event is detected.
        /// </summary>
        [RuntimeInitializeOnLoadMethod]
        private static void RegisterRuntimePlayModeListener()
        {
            Application.quitting += () =>
            {
                isApplicationQuitting = true;
            };
        }

#if UNITY_EDITOR
        /// <summary>
        /// Static class whose constructor is called once on startup. Listens for editor events.
        /// Removes the need for individual instances to listen for events.
        /// </summary>
        [InitializeOnLoad]
        private static class EditorEventListener
        {
            private const string WarnUser_EmptyActiveProfile = "WarnUser_EmptyActiveProfile";

            static EditorEventListener()
            {
                // Detect when we enter edit mode so we can reset isApplicationQuitting
                EditorApplication.playModeStateChanged += playModeState =>
                {
                    switch (playModeState)
                    {
                        case PlayModeStateChange.EnteredEditMode:
                            isApplicationQuitting = false;
                            break;
                        case PlayModeStateChange.ExitingEditMode:
                            isApplicationQuitting = false;

                            if (activeInstance != null && activeInstance.activeProfile == null)
                            {
                                // If we have an active instance, and its profile is null,
                                // Alert the user that we don't have an active profile
                                // Keep track though whether user has instructed to ignore this warning
                                if (SessionState.GetBool(WarnUser_EmptyActiveProfile, true))
                                {
                                    if (EditorUtility.DisplayDialog("Warning!", "Mixed Reality Toolkit cannot initialize because no Active Profile has been assigned.", "OK", "Ignore"))
                                    {
                                        // Stop play mode as changes done in play mode will be lost
                                        EditorApplication.isPlaying = false;
                                        Selection.activeObject = Instance;
                                        EditorGUIUtility.PingObject(Instance);
                                    }
                                    else
                                    {
                                        SessionState.SetBool(WarnUser_EmptyActiveProfile, false);
                                    }
                                }
                            }
                            break;
                        default:
                            break;
                    }
                };

                EditorApplication.hierarchyChanged += () =>
                {
                    // These checks are only necessary in edit mode
                    if (!Application.isPlaying)
                    {
                        // Clean the toolkit instances hierarchy in case instances were deleted.
                        for (int i = toolkitInstances.Count - 1; i >= 0; i--)
                        {
                            if (toolkitInstances[i] == null)
                            {
                                // If it has been destroyed, remove it
                                toolkitInstances.RemoveAt(i);
                            }
                        }

                        // If the active instance is null, it may not have been set, or it may have been deleted.
                        if (activeInstance == null)
                        {
                            // Do a search for a new active instance
                            MixedRealityToolkit instanceCheck = Instance;
                        }
                    }

                    for (int i = toolkitInstances.Count - 1; i >= 0; i--)
                    {
                        // Make sure MRTK is not parented under anything
                        Debug.Assert(toolkitInstances[i].transform.parent == null, "MixedRealityToolkit instances should not be parented under any other GameObject.");
                    }
                };
            }
        }

        private void OnValidate()
        {
            EditorApplication.delayCall += DelayOnValidate; // This is a workaround for a known unity issue when calling refresh assetdatabase from inside a on validate scope.
        }

        /// <summary>
        /// Used to register newly created instances in edit mode.
        /// Initially handled by using ExecuteAlways, but this attribute causes the instance to be destroyed as we enter play mode, which is disruptive to services.
        /// </summary>
        private void DelayOnValidate()
        {
            EditorApplication.delayCall -= DelayOnValidate;

            // This check is only necessary in edit mode. This can also get called during player builds as well,
            // and shouldn't be run during that time.
            if (EditorApplication.isPlayingOrWillChangePlaymode ||
                EditorApplication.isCompiling ||
                BuildPipeline.isBuildingPlayer)
            {
                return;
            }

            RegisterInstance(this);
        }
#endif // UNITY_EDITOR

        #endregion
    }
}
