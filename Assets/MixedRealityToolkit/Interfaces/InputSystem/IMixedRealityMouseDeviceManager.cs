// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Interface defining a mouse input device manager.
    /// </summary>
    public interface IMixedRealityMouseDeviceManager : IMixedRealityInputDeviceManager
    {
        /// <summary>
        /// Typed representation of the ConfigurationProfile property.
        /// </summary>
        [ObsoleteAttribute("The MouseInputProfile property has been deprecated and will be removed in a future version of MRTK.")]
        MixedRealityMouseInputProfile MouseInputProfile { get; }
        
        /// <summary>
        /// Gets or sets a multiplier value used to adjust the speed of the mouse cursor.
        /// </summary>
        float CursorSpeed { get; set; }

        /// <summary>
        /// Gets or sets a multiplier value used to adjust the speed of the mouse wheel.
        /// </summary>
        float WheelSpeed { get; set; }
    }
}
