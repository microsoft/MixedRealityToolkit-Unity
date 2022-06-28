// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using Unity.Profiling;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Subsystems
{
    /// <summary>
    /// This class is responsible for managing and coordinating the lifecycle of
    /// Mixed Reality Toolkit subsystems.
    /// </summary>
    [AddComponentMenu("MRTK/Core/MRTK Lifecycle Manager")]
    [DisallowMultipleComponent]
    public class MRTKLifecycleManager :
        MonoBehaviour,
        IDisposable
    {
        protected List<IMRTKManagedSubsystem> managedSubsystems = new List<IMRTKManagedSubsystem>();

        #region MonoBehaviour

        private void Awake()
        {
            // Fetch all descriptors from subsystem management
            List<ISubsystemDescriptor> subsystemDescriptors = new List<ISubsystemDescriptor>();
            SubsystemManager.GetAllSubsystemDescriptors(subsystemDescriptors);

            // Get the currently loaded profile, with which we will
            // determine which subsystems to create and launch.
            MRTKProfile currentProfile = MRTKProfile.Instance;

            if (currentProfile == null)
            {
                Debug.LogError("No profile is available. Please ensure a profile is assigned in the MRTK settings or file a bug if one is and this error is still occurring.");
                return;
            }

            // Filter the list for subsystems that we care about
            foreach (ISubsystemDescriptor descriptor in subsystemDescriptors)
            {
                IMRTKSubsystemDescriptor mrtkDescriptor = descriptor as IMRTKSubsystemDescriptor;

                if (mrtkDescriptor == null)
                {
                    // The descriptor is not manageable by our lifecycle manager.
                    continue;
                }

                // If the current profile does not want the subsystem to be created/started,
                // we skip this. The subsystem can still be created + started manually later.
                if (!currentProfile.LoadedSubsystems.Contains(mrtkDescriptor.SubsystemTypeOverride))
                {
                    continue;
                }

                Debug.Log($"[MRTKLifecycleManager] Creating {mrtkDescriptor.SubsystemTypeOverride}");

                // Create the subsystem.
                ISubsystem subsystem = descriptor.Create();

                // Assert that the subsystem is, indeed, a managed subsystem.
                // This will ensure we can call lifecycle events on the subsystem.
                Debug.Assert(subsystem is IMRTKManagedSubsystem);

                managedSubsystems.Add(subsystem as IMRTKManagedSubsystem);
            }
        }

        private void OnDestroy()
        {
            Dispose();
        }

        private static readonly ProfilerMarker OnDisableProfilerMarker =
            new ProfilerMarker("[MRTK] MRTKLifecycleManager.OnDisable");

        private void OnDisable()
        {
            using (OnDisableProfilerMarker.Auto())
            {
                foreach (IMRTKManagedSubsystem subsystem in managedSubsystems)
                {
                    subsystem.Stop();
                }
            }
        }

        private static readonly ProfilerMarker OnEnableProfilerMarker =
            new ProfilerMarker("[MRTK] MRTKLifecycleManager.OnEnable");

        private void OnEnable()
        {
            using (OnEnableProfilerMarker.Auto())
            {
                foreach (IMRTKManagedSubsystem subsystem in managedSubsystems)
                {
                    // TODO Do we want to call start on all of these when we onEnable?
                    Debug.Log($"[MRTKLifecycleManager] Starting {subsystem}");
                    subsystem.Start();
                }
            }
        }

        private static readonly ProfilerMarker UpdateProfilerMarker =
            new ProfilerMarker("[MRTK] MRTKLifecycleManager.Update");

        private void Update()
        {
            using (UpdateProfilerMarker.Auto())
            {
                foreach (IMRTKManagedSubsystem subsystem in managedSubsystems)
                {
                    subsystem.Update();
                }
            }
        }

        private static readonly ProfilerMarker FixedUpdateProfilerMarker =
            new ProfilerMarker("[MRTK] MRTKLifecycleManager.FixedUpdate");

        private void FixedUpdate()
        {
            using (FixedUpdateProfilerMarker.Auto())
            {
                foreach (IMRTKManagedSubsystem subsystem in managedSubsystems)
                {
                    subsystem.FixedUpdate();
                }
            }
        }

        private static readonly ProfilerMarker LateUpdateProfilerMarker =
            new ProfilerMarker("[MRTK] MRTKLifecycleManager.LateUpdate");

        private void LateUpdate()
        {
            using (LateUpdateProfilerMarker.Auto())
            {
                foreach (IMRTKManagedSubsystem subsystem in managedSubsystems)
                {
                    subsystem.LateUpdate();
                }
            }
        }

        #endregion MonoBehaviour

        /// <summary>
        /// A manual override for creating the subsystem with the concrete
        /// type <paramref name="concreteType"/>. The subsystem will be created,
        /// started, and registered with the <see cref="MRTKLifecycleManager"/>,
        /// resulting in the appropriate lifecycle methods being called on the subsystem.
        /// </summary>
        /// <param name="concreteType">The concrete type of the subsystem to be added.</param>
        /// <remarks>
        /// If the user has selected the subsystem in their profile in Project Settings,
        /// and the subsystem is present in the project, it will already have been created
        /// and started at launch. Use this method to override the active profile and
        /// create the subsystem even if the user did not select it.
        /// </remarks>
        /// <returns> The created and started subsystem. </returns>s
        public IMRTKManagedSubsystem ForceAddSubsystem(Type concreteType)
        {
            Debug.Assert(typeof(IMRTKManagedSubsystem).IsAssignableFrom(concreteType),
                $"ForceAddSubsystem called with a non-MRTK subsystem ({concreteType}). Only subsystems " +
                 "that implement IMRTKSubsystem can be managed by the MRTKLifecycleManager.");

            // Check that we haven't already added this subsystem.
            foreach (var subsystem in managedSubsystems)
            {
                if (subsystem.GetType() == concreteType)
                {
                    Debug.LogWarning($"ForceAddSubsystem was called with a subsystem ({concreteType}) " +
                                      "that was already managed by the MRTKLifecycleManager.");
                    return null;
                }
            }

            // Fetch all descriptors from subsystem management
            List<ISubsystemDescriptor> subsystemDescriptors = new List<ISubsystemDescriptor>();
            SubsystemManager.GetAllSubsystemDescriptors(subsystemDescriptors);

            // Find the requested subsystem type in the retrieved list of descriptors.
            foreach (ISubsystemDescriptor descriptor in subsystemDescriptors)
            {
                IMRTKSubsystemDescriptor mrtkDescriptor = descriptor as IMRTKSubsystemDescriptor;

                if (mrtkDescriptor == null)
                {
                    // The descriptor is not manageable by our lifecycle manager.
                    continue;
                }

                // If the current profile does not want the subsystem to be created/started,
                // we skip this. The subsystem can still be created + started manually later.
                if (!(mrtkDescriptor.SubsystemTypeOverride == concreteType))
                {
                    continue;
                }

                Debug.Log($"[MRTKLifecycleManager] ForceAddSubsystem creating {mrtkDescriptor.SubsystemTypeOverride}");

                // Create the subsystem.
                ISubsystem subsystem = descriptor.Create();

                // Assert that the subsystem is, indeed, a managed subsystem.
                // This will ensure we can call lifecycle events on the subsystem.
                Debug.Assert(subsystem is IMRTKManagedSubsystem);

                Debug.Log($"[MRTKLifecycleManager] ForceAddSubsystem starting {mrtkDescriptor.SubsystemTypeOverride}");

                // Start the subsystem.
                subsystem.Start();

                managedSubsystems.Add(subsystem as IMRTKManagedSubsystem);

                return subsystem as IMRTKManagedSubsystem;
            }

            Debug.LogWarning($"AddSubsystem was called with {concreteType}, but we couldn't " +
                              "find this subsystem in your project. Are you missing a package?");
            return null;
        }

        #region IDisposable

        /// <summary>
        /// Value indicating if the object has completed disposal.
        /// </summary>
        /// <remarks>
        /// Set by derived classes to indicate that disposal has been completed.
        /// </remarks>
        protected bool disposed = false;

        /// <summary>
        /// Finalizer
        /// </summary>
        ~MRTKLifecycleManager()
        {
            Dispose();
        }

        /// <summary>
        /// Cleanup resources used by this object.
        /// </summary>
        public void Dispose()
        {
            // Clean up our resources (managed and unmanaged resources)
            Dispose(true);

            // Suppress finalization as the finalizer also calls our cleanup code.
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Implementation of Dispose.
        /// </summary>
        /// <param name="disposing">
        /// Are we fully disposing the object?
        /// True will release all managed resources, unmanaged resources are always released.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            foreach (IMRTKManagedSubsystem s in managedSubsystems)
            {
                // Debug.Log("Calling destroy on " + s);
                s.Destroy();
            }
            managedSubsystems.Clear();
        }

        #endregion IDisposable
    }
}
