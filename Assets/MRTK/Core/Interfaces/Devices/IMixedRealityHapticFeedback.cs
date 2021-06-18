// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Provides access to the haptic capabilities of a device.
    /// </summary>
    public interface IMixedRealityHapticFeedback
    {
        /// <summary>
        /// Start haptic feedback by the input device.
        /// </summary>
        /// <param name="intensity">The 0.0 to 1.0 strength of the haptic feedback based on the capability of the input device.</param>
        /// <param name="durationInSeconds">The duration in seconds or float.MaxValue for indefinite duration (if supported by the platform).</param>
        /// <returns>True if haptic feedback was successfully started.</returns>
        bool StartHapticImpulse(float intensity, float durationInSeconds = float.MaxValue);

        /// <summary>
        /// Terminates haptic feedback by the input device.
        /// </summary>
        void StopHapticFeedback();
    }
}
