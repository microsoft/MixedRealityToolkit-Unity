// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

namespace Microsoft.MixedReality.Toolkit.Utilities
{
    /// <summary>
    /// Defines the display order of the Assets > Create > Mixed Reality Toolkit > Profiles menu items.
    /// </summary>
    public enum CreateProfileMenuItemIndices
    {
        Configuration = 0,
        Camera,
        Input,
        Pointer,
        ControllerMapping,
        InputActions,
        InputActionRules,
        Speech,
        BoundaryVisualization,
        ControllerVisualization,
        SpatialAwareness,   // todo: remove
        SpatialAwarenessMeshObserver,
        SpatialAwarenessSurfaceObserver,
        Gestures,
        Diagnostics,
        RegisteredServiceProviders,
        InputSimulation,
        HandTracking,
        EyeTracking,
        MouseInput,
        SceneSystem,

        Assembly = 99,
    }
}
