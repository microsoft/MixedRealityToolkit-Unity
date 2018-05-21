// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Interfaces.InputSystem;

namespace Microsoft.MixedReality.Toolkit.Internal.Definitions.InputSystem
{
    /// <summary>
    /// An Input Action for mapping an action to an Input Sources Button, Joystick, Sensor, etc.
    /// </summary>
    public struct InputAction : IMixedRealityInputAction
    {
        public InputAction(uint id, string description) : this()
        {
            Id = id;
            Description = description;
        }

        /// <inheritdoc />
        public uint Id { get; }

        /// <inheritdoc />
        public string Description { get; }
    }
}
