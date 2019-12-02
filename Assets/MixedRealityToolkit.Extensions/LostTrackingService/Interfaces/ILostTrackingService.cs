// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;

namespace Microsoft.MixedReality.Toolkit.Extensions.Tracking
{
    /// <summary>
    /// A service that detects when tracking is lost on WSA devices. 
    /// When tracking is lost, the service displays a visual indicator and sets the main camera's culling mask to hide all other objects.
    /// When tracking is restored, the camera mask is restored and the visual indicator is hidden.
    /// </summary>
    public interface ILostTrackingService : IMixedRealityExtensionService
    {
        /// <summary>
        /// True if tracking is lost, false if tracking is present.
        /// </summary>
        bool TrackingLost { get; }

        /// <summary>
        /// Called when tracking is lost.
        /// (When UnityEngine.VR.WSA.PositionalLocatorState is Inhibited.)
        /// </summary>
        Action OnTrackingLost { get; set; }

        /// <summary>
        /// Called when tracking is stored
        /// (UnityEngine.VR.WSA.PositionalLocatorState is anything other than Inhibited.)
        /// </summary>
        Action OnTrackingRestored { get; set; }

#if UNITY_EDITOR
        /// <summary>
        /// Editor-only method to test lost tracking visual.
        /// </summary>
        /// <param name="trackingLost">If true, sets tracking to be lost. If false, sets tracking to be found.</param>
        void EditorSetTrackingLost(bool trackingLost);
#endif
    }
}