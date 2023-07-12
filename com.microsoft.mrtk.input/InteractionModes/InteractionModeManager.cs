// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Used to manage interactors and ensure that each several interactors for a 'controller' aren't clashing and firing at the same time
    /// </summary>
    [AddComponentMenu("MRTK/Input/Interaction Mode Manager")]
    [RequireComponent(typeof(ControllerLookup))]
    public class InteractionModeManager : MonoBehaviour
    {
        /// <summary>
        /// Describes the types of interaction modes an interactor can belong to
        /// </summary>
        [Serializable]
        private class ManagedInteractorStatus
        {
            [SerializeField]
            [Tooltip("A value representing interactor mode or state that is being targeted by this Managed Interactor Status.")]
            private InteractionMode currentMode;

            /// <summary>
            /// Get or set the value representing interactor mode or state that is being targeted by the <see cref="ManagedInteractorStatus"/> instance.
            /// </summary>
            public InteractionMode CurrentMode
            {
                get => currentMode;
                set => currentMode = value;
            }

            [SerializeField]
            [Tooltip("The interactor mode or state that is being targeted by this Managed Interactor Status.")]
            private List<XRBaseInteractor> interactors = new List<XRBaseInteractor>();

            /// <summary>
            /// The interactor mode or state that is being targeted by the <see cref="ManagedInteractorStatus"/> instance.
            /// </summary>
            public List<XRBaseInteractor> Interactors => interactors;
        }

#if UNITY_EDITOR
        private static InteractionModeManager activeInstance;
        /// <summary>
        /// The current active instance of the Interaction Mode Manager
        /// Only one interaction mode manager should be present in a scene at any given time
        /// </summary>
        public static InteractionModeManager Instance
        {
            get
            {
                if (activeInstance == null)
                {
                    activeInstance = UnityEditor.SceneManagement.StageUtility.GetCurrentStageHandle().FindComponentOfType<InteractionModeManager>();
                }

                return activeInstance;
            }
        }

        /// <summary>
        /// Editor only function for initializing the Interaction Mode Manager with the existing XR controllers in the scene
        /// </summary>
        public void InitializeControllers()
        {
            controllerMapping.Clear();
            foreach (XRController xrController in FindObjectsOfType<XRController>())
            {
                if (!controllerMapping.ContainsKey(xrController.gameObject))
                {
                    controllerMapping.Add(xrController.gameObject, new ManagedInteractorStatus());
                }
            }
        }

        /// <summary>
        /// Expands this object's <see cref="PrioritizedInteractionModes"/> property with base and sub types associated with
        /// the current value stored in the <see cref="PrioritizedInteractionModes"/> property.
        /// </summary>
        /// <remarks>
        /// This function is only intended for use in Unity's inspector window. See 
        /// <see cref="Microsoft.MixedReality.Toolkit.Input.Editor.InteractionModeManagerEditor"> InteractionModeManagerEditor</see>
        /// documentation for more details.
        /// </remarks>
        public void PopulateModesWithSubtypes()
        {
            List<InteractionModeDefinition> newPrioritizedInteractionModes = new List<InteractionModeDefinition>();

            for (int i = 0; i < prioritizedInteractionModes.Count; i++)
            {
                InteractionModeDefinition mode = prioritizedInteractionModes[i];

                List<SystemType> subtypes = new List<SystemType>();
                foreach (SystemType baseType in mode.AssociatedTypes)
                {
                    subtypes.Add(baseType);
                    var allSubtypes = AppDomain.CurrentDomain.GetAssemblies()
                         .SelectMany(assembly => assembly.GetLoadableTypes())
                         .Where(type => type.IsSubclassOf(baseType))
                         .ToList();

                    foreach (SystemType type in allSubtypes)
                    {
                        subtypes.Add(type);
                    }
                }

                newPrioritizedInteractionModes.Add(new InteractionModeDefinition(mode.ModeName, subtypes));
            }
            prioritizedInteractionModes = newPrioritizedInteractionModes;
        }
#endif

        /// <summary>
        /// Initializes the Interaction Mode Manager with the existing Interaction Mode Detectors in the scene
        /// </summary>
        public void InitializeInteractionModeDetectors()
        {
            interactionModeDectectors.Clear();

            // PERFORMANCE FIXME: This is not great for performance. Find better way to register detectors?
            // We would query interactors and then add all interactors that happen to be a detector, but
            // detectors may not necessarily be interactors.
            foreach (IInteractionModeDetector detector in FindObjectsOfType<MonoBehaviour>().OfType<IInteractionModeDetector>())
            {
                interactionModeDectectors.Add(detector);
            }
        }

        /// <summary>
        /// The list of interaction mode detectors.
        /// </summary>
        private List<IInteractionModeDetector> interactionModeDectectors = new List<IInteractionModeDetector>();

        /// <summary>
        /// The MRTK Interaction Mode Manager will only mediate controller interactors and interactors which are designated as managed
        /// </summary>
        [SerializeField]
        [Tooltip("The MRTK Interaction Mode Manager will only mediate controller interactors and interactors which are designated as managed")]
        private SerializableDictionary<GameObject, ManagedInteractorStatus> controllerMapping = new SerializableDictionary<GameObject, ManagedInteractorStatus>();

        /// <summary>
        /// Private collection kept in lock-step with interactorMapping. Used to keep track of all registered interactors.
        /// Interactors are only registered once, when they are created. They are also unregistered once, when their reference becomes null.
        /// </summary>
        private HashSet<XRBaseInteractor> registeredControllerInteractors = new HashSet<XRBaseInteractor>();

        [SerializeField]
        [Tooltip("Describes the order of priority that interactor types have over each other.")]
        private List<InteractionModeDefinition> prioritizedInteractionModes = new List<InteractionModeDefinition>();

        /// <summary>
        /// Describes the order of priority that interactor types have over each other.
        /// </summary>
        public List<InteractionModeDefinition> PrioritizedInteractionModes => prioritizedInteractionModes;

        [SerializeField]
        [Tooltip("The default interaction mode when no other mode has been specified.")]
        private InteractionMode defaultMode;

        /// <summary>
        /// The default interaction mode when no other mode has been specified.
        /// </summary>
        public InteractionMode DefaultMode
        {
            get => defaultMode;
            set => defaultMode = value;
        }

        #region Internal protected properties

        private XRInteractionManager interactionManager;

        /// <summary>
        /// The interaction manager to use to query interactors and their registration events.
        /// Currently protected internal, may be exposed in a future update.
        /// </summary>
        internal protected XRInteractionManager InteractionManager
        {
            get
            {
                if (interactionManager == null)
                {
                    interactionManager = ComponentCache<XRInteractionManager>.FindFirstActiveInstance();
                }

                return interactionManager;
            }
            set => interactionManager = value;
        }

        #endregion Internal protected properties

        /// <summary>
        /// Registers an interactor to be managed by the interaction mode manager
        /// </summary>
        /// <param name="interactor">An XRBaseInteractor which needs to be managed based on interaction modes</param>
        public void RegisterInteractor(XRBaseInteractor interactor)
        {
            // Only register controllers which are governed by some kind of interaction mode
            if (!IsInteractorValid(interactor))
            {
                return;
            }

            GameObject controllerObject = null;
            if (interactor is XRBaseControllerInteractor controllerInteractor)
            {
                controllerObject = controllerInteractor.xrController.gameObject;
            }
            if (interactor is IModeManagedInteractor modeManagedInteractor)
            {
                controllerObject = modeManagedInteractor.GetModeManagedController();
            }

            if (!controllerMapping.ContainsKey(controllerObject))
            {
                controllerMapping.Add(controllerObject, new ManagedInteractorStatus());
            }

            if (!registeredControllerInteractors.Contains(interactor))
            {
                controllerMapping[controllerObject].Interactors.Add(interactor);
                registeredControllerInteractors.Add(interactor);
            }
        }

        /// <summary>
        /// Unregisters an interactor from this Interaction Mode Manager. Used when the interactor's game object is destroyed or when 
        /// it is no longer meant to be used in the scene.
        /// </summary>
        /// <remarks>
        /// Not called by the InteractionManager itself, because we would receive an unregistration
        /// every time we disabled an interactor. We only call this when we are removing an interactor from
        /// scene completely, e.g. when a controller is destroyed.
        /// </remarks>
        /// <param name="controllerInteractor">The XRBaseInteractor to be unregistered</param>
        public void UnregisterInteractor(XRBaseInteractor interactor)
        {
            GameObject controllerObject = null;
            if (interactor is XRBaseControllerInteractor controllerInteractor)
            {
                controllerObject = controllerInteractor.xrController.gameObject;
            }
            if (interactor is IModeManagedInteractor modeManagedInteractor)
            {
                controllerObject = modeManagedInteractor.GetModeManagedController();
            }

            if (controllerMapping.TryGetValue(controllerObject, out ManagedInteractorStatus controllerInteractorStatus))
            {
                controllerInteractorStatus.Interactors.Remove(interactor);
            }
            registeredControllerInteractors.Remove(interactor);
        }

        private void Awake()
        {
            // Sanity check making sure that there are no duplicate entries in the prioritized interaction mode list
            if (InteractionManager != null)
            {
                // We only listen to interactor registrations, not deregistrations,
                // because we are going to be in charge of deregistering interactors when
                // their mode is not active. We manually call our own deregistration function
                // when an interactor will be permanently removed from play, such as when
                // the controller is destroyed.
                InteractionManager.interactorRegistered += OnInteractorRegistered;

                List<IXRInteractor> interactors = new List<IXRInteractor>();
                InteractionManager.GetRegisteredInteractors(interactors);

                // Fire a registration event for all pre-existing interactors.
                foreach (IXRInteractor interactor in interactors)
                {
                    if (interactor is XRBaseInteractor controllerInteractor)
                    {
                        RegisterInteractor(controllerInteractor);
                    }
                }
            }

            // Validate that the list of Interaction Modes is valid
            OnValidate();

            // Go find all detectors.
            InitializeInteractionModeDetectors();
        }

        private void OnValidate()
        {
            ValidateInteractionModes();
            HashSet<string> duplicatedNames = GetDuplicateInteractionModes();
            if (duplicatedNames.Count > 0)
            {
                var duplicatedNameString = CompileDuplicatedNames(duplicatedNames);

                Debug.LogError($"Duplicate interaction mode definitions detected in the interaction mode manager on {gameObject.name}. " +
                                    $"Please check the following interaction modes: {duplicatedNameString}");
            }
        }

        /// <summary>
        /// This internal function ensures that the changes made in editor are reflected in the internal associatedTypes data structure.
        /// This allows for runtime editing of an interaction modes associated types in editor.
        /// </summary>
        internal void ValidateInteractionModes()
        {
            foreach (InteractionModeDefinition mode in PrioritizedInteractionModes)
            {
                mode.InitializeAssociatedTypes();
            }
        }

        internal HashSet<string> GetDuplicateInteractionModes()
        {
            // First check for any duplicated interaction modes
            HashSet<string> seenNames = new HashSet<string>();
            HashSet<string> duplicatedNames = new HashSet<string>();

            foreach (InteractionModeDefinition mode in PrioritizedInteractionModes)
            {
                string name = mode.ModeName;

                if (seenNames.Contains(name))
                {
                    duplicatedNames.Add(name);
                }
                else
                {
                    seenNames.Add(name);
                }
            }

            return duplicatedNames;
        }

        internal string CompileDuplicatedNames(HashSet<string> duplicatedNames)
        {
            string duplicatedNameString = "";
            int i = 0;
            foreach (var duplicatedName in duplicatedNames)
            {
                i++;
                if (i < duplicatedNames.Count)
                {
                    duplicatedNameString += duplicatedName + ", ";
                }
                else
                {
                    duplicatedNameString += duplicatedName;
                }
            }
            return duplicatedNameString;
        }

        /// <summary>
        /// Private callback fired when an interactor is registered with the
        /// <see cref="InteractionManager"/>.
        /// </summary>
        private void OnInteractorRegistered(InteractorRegisteredEventArgs args)
        {
            if (args.interactorObject is XRBaseInteractor controllerInteractor)
            {
                RegisterInteractor(controllerInteractor);
            }
        }

        /// <summary>
        /// Caches interactors which have been destroyed but not yet unregistered from the interactor mediator
        /// </summary>
        private List<XRBaseInteractor> destroyedInteractors = new List<XRBaseInteractor>();

        /// <summary>
        /// Caches controllers which have been destroyed but not yet unregistered from the interactor mediator
        /// </summary>
        private List<GameObject> destroyedControllers = new List<GameObject>();

        /// <summary>
        /// Marks controllers that have been modified by a detector, so other detectors
        /// don't overwrite their changes.
        /// </summary>
        private HashSet<GameObject> modifiedControllersThisFrame = new HashSet<GameObject>();

        private static readonly ProfilerMarker UpdatePerfMarker =
            new ProfilerMarker("[MRTK] InteractionModeManager.Update");

        private void Update()
        {
            using (UpdatePerfMarker.Auto())
            {
                modifiedControllersThisFrame.Clear();

                // Updating the status of all controllers based on their interaction mode
                foreach (IInteractionModeDetector detector in interactionModeDectectors)
                {
                    List<GameObject> controllers = detector.GetControllers();

                    foreach (GameObject controller in controllers)
                    {
                        if (detector.IsModeDetected())
                        {
                            SetInteractionMode(controller, detector.ModeOnDetection);

                            // Mark this controller as modified this frame.
                            modifiedControllersThisFrame.Add(controller);
                        }
                        // Reset mode, if and only if none of the other detectors
                        // have not modified it this frame.
                        else if (!modifiedControllersThisFrame.Contains(controller))
                        {
                            ResetToDefaultMode(controller);
                        }
                    }
                }

                destroyedControllers.Clear();
                destroyedInteractors.Clear();

                foreach (GameObject controller in controllerMapping.Keys)
                {
                    // If the controller has be destroyed, be sure to mark it and its interactors for unregistration
                    if (controller == null)
                    {
                        destroyedControllers.Add(controller);
                        foreach (XRBaseInteractor interactor in controllerMapping[controller].Interactors)
                        {
                            destroyedInteractors.Add(interactor);
                        }
                        continue;
                    }

                    // mediating all of the interactors to ensure the correct ones are active for their controller's given interaction mode
                    InteractionModeDefinition controllerCurrentMode = prioritizedInteractionModes[controllerMapping[controller].CurrentMode.priority];

                    foreach (XRBaseInteractor interactor in controllerMapping[controller].Interactors)
                    {
                        // If the interactor has be destroyed, be sure to mark it for unregistration
                        if (interactor == null)
                        {
                            destroyedInteractors.Add(interactor);
                            continue;
                        }

                        interactor.enabled = IsInteractorValidForMode(controllerCurrentMode, interactor);
                    }
                }

                foreach (GameObject controller in destroyedControllers)
                {
                    controllerMapping.Remove(controller);
                }

                foreach (XRBaseInteractor interactor in destroyedInteractors)
                {
                    UnregisterInteractor(interactor);
                }
            }
        }

        /// <summary>
        /// Sets the interaction mode for the target InteractionModeController.
        /// </summary>
        /// <param name="controller">The controller we need to toggle the mode of</param>
        /// <param name="interactionMode">The interaction mode that is currently being applied to this controller.</param>
        public void SetInteractionMode(GameObject controller, InteractionMode interactionMode)
        {
            if (controllerMapping.TryGetValue(controller, out ManagedInteractorStatus controllerInteractorStatus))
            {
                controllerInteractorStatus.CurrentMode = controllerInteractorStatus.CurrentMode.priority > interactionMode.priority ? controllerInteractorStatus.CurrentMode : interactionMode;
            }
        }

        /// <summary>
        /// Resets the controller's interaction mode to the default mode specified on the interaction mode manager
        /// </summary>
        /// <param name="controller">The controller we intend to reset to the default mode</param>
        public void ResetToDefaultMode(GameObject controller)
        {
            if (controllerMapping.TryGetValue(controller, out ManagedInteractorStatus controllerInteractorStatus))
            {
                controllerInteractorStatus.CurrentMode = defaultMode;
            }
        }

        private bool IsInteractorValidForMode(InteractionModeDefinition mode, XRBaseInteractor interactor)
        {
            return mode.AssociatedTypes.Contains(interactor.GetType());
        }

        /// <summary>
        /// Maps an interactor's system.type to a InteractorType enum, describing various modes of interaction.
        /// </summary>
        /// <param name="interactor">The interactor we wish to check</param>
        /// <returns>Returns whether or not the interactor is governed by any of the defined interaction modes</returns>
        private bool IsInteractorValid(XRBaseInteractor interactor)
        {
            for (int i = 0; i < prioritizedInteractionModes.Count; i++)
            {
                if (IsInteractorValidForMode(prioritizedInteractionModes[i], interactor))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
