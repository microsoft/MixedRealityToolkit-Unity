// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.MixedReality.Toolkit
{
    /// <summary>
    /// Interface providing properties that components can call to determine the current state of a mixed reality service.
    /// </summary>
    public interface IMixedRealityServiceState
    {
        /// <summary>
        /// Indicates whether or not the service has been initialized.
        /// </summary>
        /// <remarks>
        /// Calls to the service's Uninitialize method will reset this value to false.
        /// </remarks>
        bool IsInitialized { get; }

        /// <summary>
        /// Indicates whether or not the service is currently enabled.
        /// </summary>
        bool IsEnabled { get; }

        /// <summary>
        /// Indicates whether or not the Destroy method been called on this service.
        /// </summary>
        /// <remarks>
        /// There may be a short period of time between when an object i
        /// </remarks>
        bool IsMarkedDestroyed { get; }
    }
}