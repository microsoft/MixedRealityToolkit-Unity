// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Input;

/// <summary>
/// Interface defining a mouse input device manager.
/// </summary>
public interface IMixedRealityMouseDeviceManager : IMixedRealityInputDeviceManager
{
    /// <summary>
    /// Typed representation of the ConfigurationProfile property.
    /// </summary>
    MixedRealityMouseInputProfile MouseInputProfile { get; }
}
