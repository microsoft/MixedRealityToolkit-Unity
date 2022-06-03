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

        public override void Destroy() { }
        public override void Start() { }
        public override void Stop() { }
    }
}
