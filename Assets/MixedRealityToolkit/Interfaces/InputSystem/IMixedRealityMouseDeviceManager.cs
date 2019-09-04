// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Input;

/// <summary>
/// Interface defining a mouse input device manager.
/// </summary>
public interface IMixedRealityMouseDeviceManager : IMixedRealityInputDeviceManager
{
    // todo: figure out how to remove.... this is only "needed" by the profile inspector...
    /// <summary>
    /// Typed representation of the ConfigurationProfile property.
    /// </summary>
    MixedRealityMouseInputProfile MouseInputProfile { get; }

    /// <summary>
    /// Gets or sets a multiplier value used to adjust the speed of the mouse cursor.
    /// </summary>
    /// <remarks>
    /// The user's cursor speed, as configured in the operating system, is considered the unity value.
    /// </remarks>
    float CursorSpeed { get; set; }
}
