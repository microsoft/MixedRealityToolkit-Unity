// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Tests
{
    /// <summary>
    /// Useful utilities for testing subsystems.
    /// </summary>
    public static class SubsystemTestUtilities
    {
        /// <summary>
        /// Creates all subsystems that use the provided descriptor type.
        /// </summary>
        /// <typeparam name="TDescriptor">
        /// The type of the descriptor used when creating the subsystem.
        /// </typeparam>
        /// <returns>
        /// A list of subsystems that were created.
        /// </returns>
        public static List<ISubsystem> CreateSubsystemForTest<TDescriptor>() where TDescriptor : ISubsystemDescriptor
        {
            List<TDescriptor> descriptors = new List<TDescriptor>();
            SubsystemManager.GetSubsystemDescriptors<TDescriptor>(descriptors);

            List<ISubsystem> subsystems = new List<ISubsystem>();

            foreach (ISubsystemDescriptor descriptor in descriptors)
            {
                subsystems.Add(descriptor.Create());
            }

            return subsystems;
        }

        /// <summary>
        /// Creates the subsystem specified by <typeparamref name="TSubsystem">
        /// and <typeparamref name="TDescriptor">, and ensures that it can be queried
        /// from <see cref="XRSubsystemHelpers">.
        /// </summary>
        /// <typeparam name="TSubsystem">
        /// The type of the subsystem to be created.
        /// </typeparam>
        /// <typeparam name="TDescriptor">
        /// The type of the descriptor used when creating the subsystem.
        /// </typeparam>
        /// <returns>
        /// The first subsystem retrieved after creation.
        /// </returns>
        public static TSubsystem CreateAndEnsureExists<TSubsystem, TDescriptor>()
            where TSubsystem : ISubsystem
            where TDescriptor : ISubsystemDescriptor
        {
            var subsystemsCreated = SubsystemTestUtilities.CreateSubsystemForTest<TDescriptor>();
            Debug.Assert(subsystemsCreated.Count > 0, "No subsystems were created from " + typeof(TDescriptor) + ".");

            var system = XRSubsystemHelpers.GetFirstSubsystem<TSubsystem>();
            Debug.Assert(system != null, "Couldn't find " + typeof(TSubsystem) + " after trying to create it.");

            return system;
        }

        /// <summary>
        /// Tests whether the specified subsystem exists, can be Start()ed, and
        /// whether it is running after being Start()ed.
        /// </summary>
        /// <typeparam name="TSubsystem">
        /// The type of the subsystem under test.
        /// </typeparam>
        /// <returns>
        /// The retrieved subsystem.
        /// </returns>
        public static TSubsystem TestStart<TSubsystem>()
            where TSubsystem : ISubsystem
        {
            var system = XRSubsystemHelpers.GetFirstSubsystem<TSubsystem>();
            Debug.Assert(system != null, "Couldn't find " + typeof(TSubsystem) + " when trying to test if we could start it.");

            system.Start();
            Debug.Assert(system.running, typeof(TSubsystem) + " was not running after we Start()ed it.");

            return system;
        }
    }
}