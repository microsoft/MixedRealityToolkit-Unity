// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License

using UnityEngine.EventSystems;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// A wrapper around <see cref="UnityEngine.EventSystems.PhysicsRaycaster"/>, which
    /// will automatically disable itself if it detects the application is running on an
    /// XR device (i.e., a <see cref="UnityEngine.XR.XRDisplaySubsystem"/> is present and running).
    /// </summary>
    /// <remarks>
    /// This is useful for automatically enabling UGUI-event-based UI with mouse/touchscreen
    /// input on flat/2D platforms, while saving performance on XR devices that don't need
    /// 2D input processing.
    /// </remarks>
    internal class PlatformAwarePhysicsRaycaster : PhysicsRaycaster
    {
        protected override void Awake()
        {
            base.Awake();

            // Are we on an XR device? If so, we don't want to
            // use camera raycasting at all.
            if (XRDisplaySubsystemHelpers.AreAnyActive())
            {
                enabled = false;
            }
        }
    }
}
