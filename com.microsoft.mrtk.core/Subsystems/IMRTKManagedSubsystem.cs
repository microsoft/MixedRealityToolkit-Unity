// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.MixedReality.Toolkit.Subsystems
{
    /// <summary>
    /// Interface defining a subsystem that supports being managed by the
    /// <see cref="MRTKLifecycleManager"/>.
    /// </summary>
    public interface IMRTKManagedSubsystem
    {
        /// <summary>
        /// Start the subsystem.
        /// </summary>
        public void Start();

        /// <summary>
        /// Stop the subsystem.
        /// </summary>
        public void Stop();

        /// <summary>
        /// Destroy the subsystem.
        /// </summary>
        public void Destroy();

        /// <summary>
        /// Called by the lifecycle manager in response to the Unity
        /// Update method being called.
        /// </summary>
        public void Update();

        /// <summary>
        /// Called by the lifecycle manager in response to the Unity
        /// LateUpdate method being called.
        /// </summary>
        public void LateUpdate();

        /// <summary>
        /// Called by the lifecycle manager in response to the Unity
        /// Fixed Update method being called.
        /// </summary>
        public void FixedUpdate();
    }
}