// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;

namespace Microsoft.MixedReality.Toolkit.Core.Interfaces.InputSystem
{
    /// <summary>
    /// Provides eye tracking information.
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