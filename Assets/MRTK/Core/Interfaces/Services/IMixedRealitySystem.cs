// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;

namespace Microsoft.MixedReality.Toolkit
{
    /// <summary>
    /// Base interface for all Mixed Reality systems
    /// </summary>
    public interface IMixedRealitySystem : IDisposable
    {
        /// <summary>
        /// Optional Priority attribute if multiple services of the same type are required, enables targeting a service for action.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Optional Priority to reorder registered managers based on their respective priority, reduces the risk of race conditions by prioritizing the order in which managers are evaluated.
        /// </summary>
        uint Priority { get; }

        /// <summary>
        /// The configuration profile for the service.
        /// </summary>
        /// <remarks>
        /// Many services may wish to provide a typed version (ex: MixedRealityInputSystemProfile) that casts this value for ease of use in calling code.
        /// </remarks>
        BaseMixedRealityProfile ConfigurationProfile { get; }

        #region State properties

        /// <summary>
        /// Indicates whether or not the service has been initialized.
        /// </summary>
        bool IsInitialized { get; }

        /// <summary>
        /// Indicates whether or not the service is currently enabled.
        /// </summary>
        bool IsEnabled { get; }

        /// <summary>
        /// Indicates whether or not the Destroy method been called on this service.
        /// </summary>
        bool IsMarkedDestroyed { get; }

        #endregion State properties

        #region Lifecycle methods

        /// <summary>
        /// The initialize function is used to setup the service once created.
        /// This method is called once all services have been registered in the Mixed Reality Toolkit.
        /// </summary>
        void Initialize();

        /// <summary>
        /// Optional Reset function to perform that will Reset the service, for example, whenever there is a profile change.
        /// </summary>
        void Reset();

        /// <summary>
        /// Optional Enable function to enable / re-enable the service.
        /// </summary>
        void Enable();

        /// <summary>
        /// Optional Update function to perform per-frame updates of the service.
        /// </summary>
        void Update();

        /// <summary>
        /// Optional LateUpdate function to that is called after Update has been called on all services.
        /// </summary>
        void LateUpdate();

        /// <summary>
        /// Optional Disable function to pause the service.
        /// </summary>
        void Disable();

        /// <summary>
        /// Optional Destroy function to perform cleanup of the service before the Mixed Reality Toolkit is destroyed.
        /// </summary>
        void Destroy();

        #endregion Lifecycle methods
    }
}
