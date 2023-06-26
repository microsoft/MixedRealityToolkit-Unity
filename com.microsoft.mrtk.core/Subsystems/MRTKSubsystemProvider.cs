// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine.SubsystemsImplementation;

namespace Microsoft.MixedReality.Toolkit.Subsystems
{
    /// <summary>
    /// A provider that supports MRTK-specific lifecycle events.
    /// </summary>
    public abstract class MRTKSubsystemProvider<TSubsystem> :
        SubsystemProvider<TSubsystem>, IMRTKManagedSubsystem
        where TSubsystem : SubsystemWithProvider, new()
    {
        /// <summary>
        /// Called by the subsystem in response to the lifecycle manager's
        /// Update method being called.
        /// </summary>
        public virtual void Update() { }

        /// <summary>
        /// Called by the subsystem in response to the lifecycle manager's
        /// LateUpdate method being called.
        /// </summary>
        public virtual void LateUpdate() { }

        /// <summary>
        /// Called by the subsystem in response to the lifecycle manager's
        /// FixedUpdate method being called.
        /// </summary>
        public virtual void FixedUpdate() { }

        /// <summary>
        /// Destroys this instance of a Unity subsystem.
        /// </summary>
        public override void Destroy() { }

        /// <summary>
        /// Starts an instance of a Unity subsystem.
        /// </summary>
        public override void Start() { }

        /// <summary>
        /// Stops an instance of a Unity subsystem.
        /// </summary>
        public override void Stop() { }
    }
}
