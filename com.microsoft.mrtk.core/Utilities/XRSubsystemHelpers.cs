// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Subsystems;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.XR;

namespace Microsoft.MixedReality.Toolkit
{
    /// <summary>
    /// A helper class to provide easier access to XR subsystems.
    /// </summary>
    /// <remarks>These properties are only valid for the XR SDK pipeline.</remarks>
    public static class XRSubsystemHelpers
    {
        /// <summary>
        /// Get the first active subsystem of type T.
        /// </summary>
        /// <remarks>
        /// Caution: this method allocs a new list of subsystems.
        /// For performance critical (frame loop) applications,
        /// consider calling SubsystemManager.GetSubsystems directly
        /// with a pre-allocated list.
        /// </remarks>
        public static T GetFirstSubsystem<T>() where T : ISubsystem
        {
            List<T> subsystems = GetAllSubsystems<T>();

            // default returns null on reference type
            return subsystems.Count > 0 ? subsystems[0] : default;
        }

        /// <summary>
        /// Get the first active and running subsystem of type T.
        /// </summary>
        /// <remarks>
        /// Caution: this method allocs a new list of subsystems.
        /// For performance critical (frame loop) applications,
        /// consider calling SubsystemManager.GetSubsystems directly
        /// with a pre-allocated list.
        /// </remarks>
        public static T GetFirstRunningSubsystem<T>() where T : ISubsystem
        {
            List<T> runningSubsystems = GetAllRunningSubsystems<T>();

            // default returns null on reference type
            return runningSubsystems.Count > 0 ? runningSubsystems[0] : default;
        }

        /// <summary>
        /// Get all running subsystems of type T without allocating a new list.
        /// </summary>
        /// <param name="runningSubsystems">The list to fill with all running subsystems of the specified type.</param>
        public static void GetAllRunningSubsystemsNonAlloc<T>(List<T> runningSubsystems) where T : ISubsystem
        {
            SubsystemManager.GetSubsystems(runningSubsystems);
            runningSubsystems.RemoveAll(subsystem => !subsystem.running);
        }

        /// <summary>
        /// Get all running subsystems of type T.
        /// </summary>
        /// <remarks>
        /// Caution: this method allocs a new list of subsystems.
        /// For performance critical (frame loop) applications,
        /// consider calling SubsystemManager.GetSubsystems directly
        /// with a pre-allocated list.
        /// </remarks>
        public static List<T> GetAllRunningSubsystems<T>() where T : ISubsystem
        {
            List<T> filtered = new List<T>();

            foreach (T sys in GetAllSubsystems<T>())
            {
                if (sys.running)
                {
                    filtered.Add(sys);
                }
            }

            return filtered;
        }

        /// <summary>
        /// Get all subsystems (not necessarily running) of type T.
        /// </summary>
        /// <remarks>
        /// Caution: this method allocs a new list of subsystems.
        /// For performance critical (frame loop) applications,
        /// consider calling SubsystemManager.GetSubsystems directly
        /// with a pre-allocated list.
        /// </remarks>
        public static List<T> GetAllSubsystems<T>() where T : ISubsystem
        {
            List<T> subsystems = new List<T>();
            SubsystemManager.GetSubsystems(subsystems);
            return subsystems;
        }

        private static XRInputSubsystem inputSubsystem = null;

        /// <summary>
        /// The XR SDK input subsystem for the currently loaded XR plug-in.
        /// </summary>
        public static XRInputSubsystem InputSubsystem
        {
            get
            {
                if (inputSubsystem == null || !inputSubsystem.running)
                {
                    inputSubsystem = GetFirstRunningSubsystem<XRInputSubsystem>();
                }
                return inputSubsystem;
            }
        }

        private static XRMeshSubsystem meshSubsystem = null;

        /// <summary>
        /// The XR SDK mesh subsystem for the currently loaded XR plug-in.
        /// </summary>
        public static XRMeshSubsystem MeshSubsystem
        {
            get
            {
                if (meshSubsystem == null || !meshSubsystem.running)
                {
                    meshSubsystem = GetFirstRunningSubsystem<XRMeshSubsystem>();
                }
                return meshSubsystem;
            }
        }

        private static XRDisplaySubsystem displaySubsystem = null;

        /// <summary>
        /// The XR SDK display subsystem for the currently loaded XR plug-in.
        /// </summary>
        public static XRDisplaySubsystem DisplaySubsystem
        {
            get
            {
                if (displaySubsystem == null || !displaySubsystem.running)
                {
                    displaySubsystem = GetFirstRunningSubsystem<XRDisplaySubsystem>();
                }
                return displaySubsystem;
            }
        }

        #region Subsystem internal utilities

        /// <summary>
        /// Constructs a cInfo struct for the specified subsystem type.
        /// Used when registering a subsystem.
        /// </summary>
        public static CinfoT ConstructCinfo<SubsystemT, CinfoT>() where CinfoT : IMRTKSubsystemDescriptor, new()
        {
            var metadata = RetrieveMetadata<SubsystemT>();

            CinfoT cinfo = new CinfoT
            {
                Name = metadata.Name,
                DisplayName = metadata.DisplayName,
                Author = metadata.Author,
                ProviderType = metadata.ProviderType,
                SubsystemTypeOverride = metadata.SubsystemTypeOverride,
                ConfigType = metadata.ConfigType
            };

            return cinfo;
        }

        /// <summary>
        /// Retrieves metadata for a specified subsystem type.
        /// Used when registering a subsystem.
        /// </summary>
        /// <typeparam name="SubsystemT">
        /// The derived type of the subsystem for which the metadata will be retrieved.
        /// </typeparam>
        /// <returns>
        /// The metadata associated with the <typeparamref name="SubsystemT"/>,
        /// wrapped as an <see cref="IMRTKSubsystemDescriptor"/>.
        /// </returns>
        public static IMRTKSubsystemDescriptor RetrieveMetadata<SubsystemT>()
        {
            return typeof(SubsystemT).GetCustomAttribute<MRTKSubsystemAttribute>();
        }

        /// <summary>
        /// Retrieves metadata for a specified subsystem type.
        /// Used when registering a subsystem.
        /// </summary>
        /// <param name="type">
        /// The derived type of the subsystem for which the metadata will be retrieved.
        /// </param>
        public static IMRTKSubsystemDescriptor RetrieveMetadata(Type type)
        {
            return type.GetCustomAttribute<MRTKSubsystemAttribute>();
        }

        /// <summary>
        /// Gets the loaded configuration of type <typeparamref name="ConfigT"/>
        /// for the indicated subsystem type <typeparamref name="SubsystemT"/>
        /// </summary>
        /// <typeparam name="ConfigT">
        /// The derived type of the config the caller expects to receive.
        /// </typeparam>
        /// <typeparam name="SubsystemT">
        /// The derived type of the subsystem for which the config will be retrieved.
        /// </typeparam>
        public static ConfigT GetConfiguration<ConfigT, SubsystemT>()
            where ConfigT : BaseSubsystemConfig
            where SubsystemT : IMRTKManagedSubsystem
        {
            // Grab the currently loaded profile from the singleton instance.
            // In editor, this is set in an Editor-only OnEnable() function to the
            // profile associated with the Standalone build target group.
            MRTKProfile profile = MRTKProfile.Instance;
            Debug.Assert(profile != null, "MRTK Profile could not be retrieved");

            // Attempt to retrieve the config associated with the indicated subsystem.
            profile.TryGetConfigForSubsystem(typeof(SubsystemT), out BaseSubsystemConfig config);
            Debug.Assert(config != null, "Configuration could not be retrieved for " + typeof(SubsystemT));

            // Cast the config down to the requested config type for client's convenience.
            return config as ConfigT;
        }

        /// <summary>
        /// Validates the descriptor parameters against possible type errors that could arise at runtime.
        /// The base subsystem type and base provider type are passed as generic type params for validation.
        /// </summary>
        /// <param name='name'>The parameters required to initialize the descriptor.</param>
        /// <param name='subsystemTypeOverride'>The derived type of the subsystem.</param>
        /// <param name='providerType'>The derived type of the provider.</param>
        /// <returns>
        /// True if all validation checks pass.
        /// </returns>
        /// <exception cref="ArgumentException">Thrown when there are errors in the descriptor parameters.
        /// Typically, this will occur
        /// <list type="bullet">
        /// <item>
        /// <description>if <see cref="name"/> is <c>null</c> or empty</description>
        /// </item>
        /// <item>
        /// <description>if <see cref="subsystemTypeOverride"/> does not subclass the base subsystem type
        /// </description>
        /// </item>
        /// <item>
        /// <description>if <see cref="providerType"/> does not subclass the base provider type
        /// </description>
        /// </item>
        /// </list>
        /// </exception>
        public static bool CheckTypes<SubsystemT, ProviderT>(string name, Type subsystemTypeOverride, Type providerType)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Cannot create descriptor because name is invalid");
            }

            if (providerType == null
                || !providerType.IsSubclassOf(typeof(ProviderT)))
            {
                throw new ArgumentException("Cannot create descriptor because providerType doesn't inherit from base provider type.");
            }

            if (subsystemTypeOverride != null
                && !subsystemTypeOverride.IsSubclassOf(typeof(SubsystemT)))
            {
                throw new ArgumentException("Cannot create descriptor because subsystemTypeOverride (" + subsystemTypeOverride + ") is invalid");
            }

            return true;
        }

        #endregion Subsystem internal utilities
    }
}
