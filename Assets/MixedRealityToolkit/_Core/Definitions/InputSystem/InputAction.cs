// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Definitions.Utilities;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces.InputSystem;
using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Internal.Definitions.InputSystem
{
    /// <summary>
    /// An Input Action for mapping an action to an Input Sources Button, Joystick, Sensor, etc.
    /// </summary>
    [Serializable]
    public class InputAction : IMixedRealityInputAction
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="description"></param>
        /// <param name="axisConstraint"></param>
        public InputAction(uint id, string description, AxisType axisConstraint = AxisType.None)
        {
            this.id = id;
            this.description = description;
            this.axisConstraint = axisConstraint;
        }

        /// <inheritdoc />
        public uint Id => id;

        [SerializeField]
        private uint id;

        /// <inheritdoc />
        public string Description => description;

        [SerializeField]
        private string description;

        /// <inheritdoc />
        public AxisType AxisConstraint => axisConstraint;

        [SerializeField]
        private AxisType axisConstraint;
    }
}
