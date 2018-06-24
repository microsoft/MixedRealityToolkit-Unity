// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Definitions.Devices;
using Microsoft.MixedReality.Toolkit.Internal.Definitions.Utilities;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces.Devices;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces.InputSystem;
using System.Collections.Generic;

namespace Microsoft.MixedReality.Toolkit.Internal.Interfaces
{
    /// <summary>
    /// Mixed Reality Toolkit controller definition, used to manage a specific controller type
    /// </summary>
    public interface IMixedRealityController
    {
        /// <summary>
        /// Outputs the current state of the Input Source, whether it is tracked or not. As defined by the SDK / Unity.
        /// </summary>
        ControllerState ControllerState { get; }

        /// <summary>
        /// The designated hand that the Input Source is managing, as defined by the SDK / Unity.
        /// </summary>
        Handedness ControllerHandedness { get; }

        /// <summary>
        /// The registered Input Source for this controller
        /// </summary>
        IMixedRealityInputSource InputSource { get; }

        /// <summary>
        /// Mapping definition for this controller, linking the Physical inputs to logical Input System Actions
        /// </summary>
        Dictionary<DeviceInputType, IMixedRealityInteractionMapping> Interactions { get; }

        /// <summary>
        /// Inform the controller to setup and be ready when asked by it's registered device
        /// </summary>
        /// <typeparam name="T">Type of setup parameter (unique to certain SDK's)</typeparam>
        /// <param name="state">The optional input state provided by the SDK/Platform that is required for setup</param>
        void SetupInputSource<T>(T state);

        /// <summary>
        /// Update the controller with new state information / values
        /// </summary>
        /// <typeparam name="T">Type of setup parameter (unique to certain SDK's)</typeparam>
        /// <param name="state">The optional input state provided by the SDK/Platform that is required for update</param>
        void UpdateInputSource<T>(T state);
    }
}