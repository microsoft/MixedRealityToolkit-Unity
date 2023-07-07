// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Represents the current state of the <see cref="ArticulatedHandController"/>.
    /// Contains extra state values extended from the base <see cref="XRControllerState"/>,
    /// including the left and right pinch/select progress.
    /// </summary>
    [Serializable]
    internal class ArticulatedHandControllerState : XRControllerState
    {
        /// <summary>
        /// Is the controller/hand ready to select via pinch?
        /// </summary>
        public bool PinchSelectReady;

        /// <summary>
        /// Constructs a new ArticulatedHandControllerState with default values.
        /// </summary>
        public ArticulatedHandControllerState() : base()
        {
            PinchSelectReady = false;
        }
    }
}
