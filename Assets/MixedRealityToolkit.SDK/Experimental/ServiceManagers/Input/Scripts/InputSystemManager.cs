// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;
using UnityEngine.EventSystems;
using Microsoft.MixedReality.Toolkit.Input;

#if UNITY_EDITOR
using Microsoft.MixedReality.Toolkit.Input.Editor;
#endif

namespace Microsoft.MixedReality.Toolkit.Experimental.Input
{
    /// <summary>
    /// Service manager supporting running the input system, without requiring the MixedRealityToolkit object.
    /// </summary>
    public class InputSystemManager : BaseServiceManager
    {
        [SerializeField]
        [Tooltip("The input system type that will be instantiated.")]
        [Implements(typeof(IMixedRealityInputSystem), TypeGrouping.ByNamespaceFlat)]
        private SystemType InputSystemType = null;

        [SerializeField]
        [Tooltip("The input system configuration profile.")]
        private MixedRealityInputSystemProfile profile = null;

        private void Awake()
        {
            InitializeManager();
        }

        protected override void OnDestroy()
        {
            UninitializeManager();
            base.OnDestroy();
        }

        /// <summary>
        /// Initialize the manager.
        /// </summary>
        private void InitializeManager()
        {
#if UNITY_EDITOR
            // Make sure unity axis mappings are set.
            InputMappingAxisUtility.CheckUnityInputManagerMappings(ControllerMappingLibrary.UnityInputManagerAxes);
#endif

            // The input system class takes arguments for:
            // * The input system profile
            object[] args = { profile };
            Initialize<IMixedRealityInputSystem>(InputSystemType.Type, args: args);

            // The input system uses the focus provider specified in the profile.
            // The args for the focus provider are:
            // * The input system profile
            args = new object[] { profile };
            Initialize<IMixedRealityFocusProvider>(profile.FocusProviderType.Type, args: args);

            // The input system uses the raycast provider specified in the profile.
            // The args for the focus provider are:
            // * The input system profile
            args = new object[] { profile };
            Initialize<IMixedRealityRaycastProvider>(profile.RaycastProviderType, args: args);


            EventSystem[] eventSystems = FindObjectsOfType<EventSystem>();

            if (eventSystems.Length == 0)
            {
                CameraCache.Main.gameObject.EnsureComponent<EventSystem>();
            }

        }

        /// <summary>
        /// Uninitialize the manager.
        /// </summary>
        private void UninitializeManager()
        {
            Uninitialize<IMixedRealityRaycastProvider>();
            Uninitialize<IMixedRealityFocusProvider>();
            Uninitialize<IMixedRealityInputSystem>();

#if UNITY_EDITOR
            InputMappingAxisUtility.RemoveMappings(ControllerMappingLibrary.UnityInputManagerAxes);
#endif
        }
    }
}