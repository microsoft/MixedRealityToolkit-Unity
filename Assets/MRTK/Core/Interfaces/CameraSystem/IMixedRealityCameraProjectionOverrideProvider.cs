// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.MixedReality.Toolkit.CameraSystem
{
    /// <summary>
    /// Provides a property for enabling and disabling projection override behavior. The actual implementation is dependent on the platform.
    /// </summary>
    public interface IMixedRealityCameraProjectionOverrideProvider : IMixedRealityCameraSettingsProvider
    {
        /// <summary>
        /// Whether the camera's projection matrices are being overridden or not.
        /// </summary>
        /// <remarks>
        /// Different platforms and devices may handle this differently, or not at all.
        /// Windows Mixed Reality refers to this as
        /// <see href="https://docs.microsoft.com/en-us/hololens/hololens2-display#what-improvements-are-coming-that-will-improve-hololens-2-image-quality">Reading Mode</see>.
        /// </remarks>
        bool IsProjectionOverrideEnabled { get; set; }
    }
}
