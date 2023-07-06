// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Subsystems;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace Microsoft.MixedReality.Toolkit
{
    /// <summary>
    /// A build-target-specific profile that determines which subsystems are launched,
    /// and which configurations are bound to them.
    /// </summary>
    [System.Serializable]
    [CreateAssetMenu(fileName = "MRTKProfile.asset", menuName = "MRTK/MRTKProfile")]
    public class MRTKProfile : BaseMRTKProfile
    {
        /// <summary>
        /// Static instance that will hold the runtime asset instance we created in our build process.
        /// </summary>
        /// <see cref="SampleBuildProcessor"/>
        private static MRTKProfile instance = null;

        public static MRTKProfile Instance
        {
            get => instance;
#if UNITY_EDITOR
            set => instance = value;
#endif
        }

        [SerializeField]
        [Implements(typeof(IMRTKManagedSubsystem), TypeGrouping.ByNamespaceFlat)]
        protected List<SystemType> loadedSubsystems = new List<SystemType>();

        /// <summary>
        /// The list of subsystems intended to be started at runtime.
        /// </summary>
        /// <remarks>
        /// Subsystems not on this list may still be started at a later point, manually.
        /// </remarks>
        public List<SystemType> LoadedSubsystems => loadedSubsystems;

        [SerializeField]
        protected SerializableDictionary<SystemType, BaseSubsystemConfig> subsystemConfigs = new SerializableDictionary<SystemType, BaseSubsystemConfig>();

        /// <summary>
        /// Attempts to retrieve the specified <see cref="BaseSubsystemConfig"/> for a given subsystem type.
        /// </summary>
        /// <returns>
        /// True if there is a registered configuration for the specified subsystem. False otherwise.
        /// </returns>
        public bool TryGetConfigForSubsystem(SystemType subsystemType, out BaseSubsystemConfig config)
        {
            if (subsystemConfigs.ContainsKey(subsystemType))
            {
                config = subsystemConfigs[subsystemType];
                return true;
            }
            else
            {
                config = null;
                return false;
            }
        }

#if !UNITY_EDITOR
        private void Awake()
        {
            instance = this;
            DontDestroyOnLoad(instance);
        }
#endif
    }
}
