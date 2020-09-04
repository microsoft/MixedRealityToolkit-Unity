// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Provides eye tracking saccade events.
    /// </summary>
    public interface IMixedRealityEyeSaccadeProvider : IMixedRealityDataProvider
    {
        /// <summary>
        /// Triggered when user is saccading across the view (jumping quickly with their eye gaze above a certain threshold in visual angles).
        /// </summary>
        event Action OnSaccade;

        /// <summary>
        /// Triggered when user is saccading horizontally across the view (jumping quickly with their eye gaze above a certain threshold in visual angles).
        /// </summary>
        event Action OnSaccadeX;

        /// <summary>
        /// Triggered when user is saccading vertically across the view (jumping quickly with their eye gaze above a certain threshold in visual angles).
        /// </summary>
        event Action OnSaccadeY;
    }
}
